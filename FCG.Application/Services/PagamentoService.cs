using System;
using AutoMapper;
using FCG.API.Models;
using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Domain.Entities;
using FCG.Domain.EventSourcing;
using FCG.Domain.EventSourcing.Events;
using FCG.Domain.Interfaces;
using FCG.Domain.Notifications;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace FCG.Application.Services
{
    public class PagamentoResponse
    {
        public string Mensagem { get; set; }
        public DateTime Data { get; set; }
    }

    public class PagamentoService : IPagamentoService
    {
        private readonly IPagamentoRepository _pagamentoRepository;
        private readonly ILogger<PagamentoService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IOptions<AzureFunctionsOptions> _azureOptions;
        private readonly IEventPublisher _eventPublisher;
        private readonly HttpClient _httpClient;

        public PagamentoService(IPagamentoRepository pagamentoRepository, ILogger<PagamentoService> logger,
            IUnitOfWork unitOfWork, IMapper mapper, IOptions<AzureFunctionsOptions> azureOptions, HttpClient httpClient,
            IEventPublisher eventPublisher)
        {
            _pagamentoRepository = pagamentoRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpClient = httpClient;
            _azureOptions = azureOptions;
            _eventPublisher = eventPublisher;
        }

        public async Task<DomainNotificationsResult<PagamentoViewModel>> Efetuar(PagamentoDTO pagamentoDTO)
        {
            var resultNotifications = new DomainNotificationsResult<PagamentoViewModel>();

            var correlationId = Guid.NewGuid();

            _logger.LogInformation("Iniciando efetuação de pagamento: UsuarioId={UsuarioId}, JogoId={JogoId}, Valor={Valor}, Quantidade={Quantidade}",
                pagamentoDTO.UsuarioId, pagamentoDTO.JogoId, pagamentoDTO.Valor, pagamentoDTO.Quantidade);

            try
            {
                var pagamento = _mapper.Map<Pagamento>(pagamentoDTO);

                await _pagamentoRepository.Efetuar(pagamento);
                await _unitOfWork.Commit();

                var pagamentoViewModel = _mapper.Map<PagamentoViewModel>(pagamento);

                await _eventPublisher.PublishAsync(new PagamentoIniciadoEvent(
                    correlationId,
                    pagamento.Id,
                    pagamento.UsuarioId,
                    pagamento.JogoId,
                    pagamento.FormaPagamentoId,
                    pagamento.Valor,
                    pagamento.Quantidade));

                var detalhesPagamento = await _pagamentoRepository.ObterDetalhesPagamento(pagamento.Id);

                var destinoNotificacao = detalhesPagamento?.Email ?? pagamentoViewModel.EmailDestino;

                await _eventPublisher.PublishAsync(new PagamentoProcessandoEvent(
                    correlationId,
                    pagamento.Id,
                    "NotificandoServicoExterno",
                    destinoNotificacao,
                    pagamento.Valor));

                resultNotifications.Result = pagamentoViewModel;

                var url = _azureOptions.Value.EnviarEmailUrl;

                var notificacaoRealizada = false;
                var mensagemConclusao = "Pagamento registrado com sucesso.";

                if (detalhesPagamento == null)
                {
                    mensagemConclusao = "Detalhes do pagamento indisponíveis para notificação externa.";
                    resultNotifications.Notifications.Add("Aviso: Detalhes do pagamento não disponíveis para a notificação externa.");
                    _logger.LogWarning("Detalhes do pagamento não encontrados para o pagamento Id={PagamentoId}", pagamento.Id);
                }
                else if (string.IsNullOrWhiteSpace(url))
                {
                    mensagemConclusao = "URL da Azure Function não configurada.";
                    resultNotifications.Notifications.Add("Aviso: URL da Azure Function não configurada.");
                    _logger.LogWarning("URL da Azure Function não configurada para envio de e-mail.");
                }
                else
                {
                    try
                    {
                        var response = await _httpClient.PostAsJsonAsync(url, detalhesPagamento);

                        if (response.IsSuccessStatusCode)
                        {
                            var pagamentoResponse = await response.Content.ReadFromJsonAsync<PagamentoResponse>();
                            notificacaoRealizada = true;
                            mensagemConclusao = pagamentoResponse?.Mensagem ?? "Azure Function processou o pagamento.";
                            _logger.LogInformation("Azure Function retornou: {Mensagem} em {Data}", pagamentoResponse?.Mensagem, pagamentoResponse?.Data);
                        }
                        else
                        {
                            mensagemConclusao = $"Azure Function retornou erro: {response.StatusCode}";
                            _logger.LogWarning("Azure Function retornou erro: {StatusCode}", response.StatusCode);
                            resultNotifications.Notifications.Add("Aviso: Azure Function não processou corretamente.");
                        }
                    }
                    catch (Exception ex)
                    {
                        mensagemConclusao = "Falha ao comunicar com a Azure Function.";
                        _logger.LogError(ex, "Erro ao chamar Azure Function para o pagamento Id={PagamentoId}", pagamento.Id);
                        resultNotifications.Notifications.Add("Aviso: Não foi possível notificar a Azure Function.");
                    }
                }

                await _eventPublisher.PublishAsync(new PagamentoConcluidoEvent(
                    correlationId,
                    pagamento.Id,
                    notificacaoRealizada,
                    mensagemConclusao,
                    pagamento.Valor));

                _logger.LogInformation("Pagamento efetuado com sucesso: Id={Id}, UsuarioId={UsuarioId}, JogoId={JogoId}",
                    pagamento.Id, pagamento.UsuarioId, pagamento.JogoId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao efetuar pagamento: UsuarioId={UsuarioId}, JogoId={JogoId}", pagamentoDTO.UsuarioId, pagamentoDTO.JogoId);
                resultNotifications.Notifications.Add("Erro ao efetuar pagamento.");
            }

            return resultNotifications;
        }

    }
}
