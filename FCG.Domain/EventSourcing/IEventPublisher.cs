using System.Threading.Tasks;

namespace FCG.Domain.EventSourcing
{
    public interface IEventPublisher
    {
        Task PublishAsync(Event @event);
    }
}
