using System;

namespace FCG.Domain.Entities
{
    public class StoredEvent
    {
        public Guid Id { get; private set; }
        public string EventType { get; private set; }
        public int AggregateId { get; private set; }
        public string Data { get; private set; }
        public DateTime Timestamp { get; private set; }
        public int Version { get; private set; }

        protected StoredEvent()
        {
        }

        public StoredEvent(string eventType, int aggregateId, string data, int version, DateTime timestamp)
        {
            Id = Guid.NewGuid();
            EventType = eventType;
            AggregateId = aggregateId;
            Data = data;
            Version = version;
            Timestamp = timestamp;
        }
    }
}
