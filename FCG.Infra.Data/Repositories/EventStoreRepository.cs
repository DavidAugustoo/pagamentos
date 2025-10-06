using System.Linq;
using System.Threading.Tasks;
using FCG.Domain.Entities;
using FCG.Domain.Interfaces;
using FCG.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FCG.Infra.Data.Repositories
{
    public class EventStoreRepository : IEventStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public EventStoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetNextVersionAsync(int aggregateId)
        {
            var lastVersion = await _context.StoredEvent
                .Where(e => e.AggregateId == aggregateId)
                .OrderByDescending(e => e.Version)
                .Select(e => e.Version)
                .FirstOrDefaultAsync();

            return lastVersion + 1;
        }

        public async Task StoreEventAsync(StoredEvent storedEvent)
        {
            await _context.StoredEvent.AddAsync(storedEvent);
            await _context.SaveChangesAsync();
        }
    }
}
