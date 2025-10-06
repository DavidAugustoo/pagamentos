using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces
{
    public interface IPagamentoRepository
    {
        Task<Pagamento> Efetuar(Pagamento usuario);
        Task<PagamentoDetalhe?> ObterDetalhesPagamento(int pagamentoId);
    }
}
