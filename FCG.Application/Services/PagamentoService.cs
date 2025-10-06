using AutoMapper;
using FCG.API.Models;
using FCG.Application.DTOs;
using FCG.Application.Interfaces;
using FCG.Domain.Entities;
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

        private readonly HttpClient _httpClient;

        public PagamentoService(IPagamentoRepository pagamentoRepository, ILogger<PagamentoService> logger,
            IUnitOfWork unitOfWork, IMapper mapper, IOptions<AzureFunctionsOptions> azureOptions, HttpClient httpClient)
        {
            _pagamentoRepository = pagamentoRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpClient = httpClient;
            _azureOptions = azureOptions;
        }

        public async Task<DomainNotificationsResult<PagamentoViewModel>> Efetuar(PagamentoDTO pagamentoDTO)
        {
            var resultNotifications = new DomainNotificationsResult<PagamentoViewModel>();

            _logger.LogInformation("Iniciando efetuação de pagamento: UsuarioId={UsuarioId}, JogoId={JogoId}, Valor={Valor}, Quantidade={Quantidade}",
                pagamentoDTO.UsuarioId, pagamentoDTO.JogoId, pagamentoDTO.Valor, pagamentoDTO.Quantidade);

            try
            {
                var pagamento = _mapper.Map<Pagamento>(pagamentoDTO);

                await _pagamentoRepository.Efetuar(pagamento);
                await _unitOfWork.Commit();

                var pagamentoViewModel = _mapper.Map<PagamentoViewModel>(pagamento);

                var detalhesPagamento = await _pagamentoRepository.ObterDetalhesPagamento(pagamento.Id);

                resultNotifications.Result = pagamentoViewModel;

                var url = _azureOptions.Value.EnviarEmailUrl;


                var response = await _httpClient.PostAsJsonAsync(url, detalhesPagamento);

                if (response.IsSuccessStatusCode)
                {
                    var pagamentoResponse = await response.Content.ReadFromJsonAsync<PagamentoResponse>();
                    _logger.LogInformation("Azure Function retornou: {Mensagem} em {Data}", pagamentoResponse?.Mensagem, pagamentoResponse?.Data);
                }
                else
                {
                    _logger.LogWarning("Azure Function retornou erro: {StatusCode}", response.StatusCode);
                    resultNotifications.Notifications.Add("Aviso: Azure Function não processou corretamente.");
                }
                

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
