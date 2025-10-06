using System.Threading.Tasks;
using FCG.Domain.Entities;
using FCG.Domain.EventSourcing;
using FCG.Domain.Interfaces;
using Newtonsoft.Json;

namespace FCG.Infra.Data.EventSourcing
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IEventStoreRepository _eventStoreRepository;

        public EventPublisher(IEventStoreRepository eventStoreRepository)
        {
            _eventStoreRepository = eventStoreRepository;
        }

        public async Task PublishAsync(Event @event)
        {
            var payload = JsonConvert.SerializeObject(@event);
            var version = await _eventStoreRepository.GetNextVersionAsync(@event.AggregateId);

            var storedEvent = new StoredEvent(@event.EventType, @event.AggregateId, payload, version, @event.Timestamp);

            await _eventStoreRepository.StoreEventAsync(storedEvent);
        }
    }
}
