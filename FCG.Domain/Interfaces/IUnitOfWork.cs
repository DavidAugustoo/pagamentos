namespace FCG.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
        public Task Rollback();
    }
}
