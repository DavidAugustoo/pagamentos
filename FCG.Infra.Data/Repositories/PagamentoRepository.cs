using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.Repositories
{
    public class PagamentoRepository : IPagamentoRepository
    {
        private readonly ApplicationDbContext _context;

        public PagamentoRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Pagamento> Efetuar(Pagamento pagamento)
        {
            await _context.Pagamento.AddAsync(pagamento);
            return pagamento;
        }
        public async Task<PagamentoDetalhe?> ObterDetalhesPagamento(int pagamentoId)
        {
            return await _context.Set<PagamentoDetalhe>()
                .FromSqlInterpolated($@"
                    SELECT 
                        p.Id          AS PagamentoId, 
                        p.Valor       AS TotalPagamento, 
                        fp.Descricao  AS FormaPagamento,
                        u.Nome        AS Nome,
                        j.Nome        AS NomeJogo,
                        u.Email       AS Email
                    FROM Pagamento p
                    INNER JOIN Jogo j            ON j.Id = p.JogoId
                    INNER JOIN Usuario u         ON u.Id = p.UsuarioId
                    INNER JOIN FormaPagamento fp ON fp.Id = p.FormaPagamentoId
                    WHERE p.Id = {pagamentoId}")
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

    }
}
