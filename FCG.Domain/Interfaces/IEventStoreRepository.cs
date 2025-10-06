using System.Threading.Tasks;
using FCG.Domain.Entities;

namespace FCG.Domain.Interfaces
{
    public interface IEventStoreRepository
    {
        Task StoreEventAsync(StoredEvent storedEvent);
        Task<int> GetNextVersionAsync(int aggregateId);
    }
}
