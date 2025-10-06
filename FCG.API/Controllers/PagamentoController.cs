using FCG.Application.DTOs;
using Microsoft.AspNetCore.Mvc;
using FCG.Application.Interfaces;

namespace FCG.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PagamentoController : BaseController
    {
        private readonly IPagamentoService _pagamentoService;
        private readonly ILogger<PagamentoController> _logger;

        public PagamentoController(IPagamentoService pagamentoService, ILogger<PagamentoController> logger)
        {
            _pagamentoService = pagamentoService;
            _logger = logger;
        }

        [HttpPost("Efetuar")]
        public async Task<IActionResult> Efetuar(PagamentoDTO pagamentoDTO)
        {
            _logger.LogInformation("Iniciando efetuação de pagamento. UsuarioId: {UsuarioId}, JogoId: {JogoId}, Valor: {Valor}, Quantidade: {Quantidade}, FormaPagamentoId: {FormaPagamentoId}",
                pagamentoDTO.UsuarioId, pagamentoDTO.JogoId, pagamentoDTO.Valor, pagamentoDTO.Quantidade, pagamentoDTO.FormaPagamentoId);
            try
            {
                var result = await _pagamentoService.Efetuar(pagamentoDTO);
                _logger.LogInformation("Pagamento efetuado com sucesso. UsuarioId: {UsuarioId}, JogoId: {JogoId}, Id: {Id}",
                    pagamentoDTO.UsuarioId, pagamentoDTO.JogoId, result.Result?.Id);
                return CreateIActionResult(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao efetuar pagamento. UsuarioId: {UsuarioId}, JogoId: {JogoId}",
                    pagamentoDTO.UsuarioId, pagamentoDTO.JogoId);
                throw;
            }
        }
    }
}
