using System;

namespace FCG.Domain.EventSourcing
{
    public abstract class Event
    {
        protected Event(Guid correlationId, int aggregateId)
        {
            CorrelationId = correlationId == Guid.Empty ? Guid.NewGuid() : correlationId;
            AggregateId = aggregateId;
            Timestamp = DateTime.UtcNow;
        }

        public Guid CorrelationId { get; }
        public int AggregateId { get; }
        public DateTime Timestamp { get; }
        public string EventType => GetType().Name;
    }
}
