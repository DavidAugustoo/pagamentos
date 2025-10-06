using FCG.Domain.Interfaces;
using FCG.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.Transactions
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Commit()
        {
            try{
                return await _context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex){
                throw new ApplicationException("Erro ao salvar alterações no banco de dados", ex);
            }
            catch (Exception ex){
                throw new ApplicationException("Erro inesperado durante a confirmação da transação", ex);
            }
        }

        public async Task Rollback()
        {
            await _context.DisposeAsync();
        }
    }
}
