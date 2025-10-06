using FCG.API.Models;
using FCG.Application.DTOs;
using FCG.Domain.Notifications;

namespace FCG.Application.Interfaces
{
    public interface IPagamentoService
    {
        Task<DomainNotificationsResult<PagamentoViewModel>> Efetuar(PagamentoDTO pagamentoDTO);
    }
}
