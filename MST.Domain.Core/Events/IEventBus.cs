namespace MST.Domain.Abstraction.Events
{
    public interface IEventBus : IEventPublisher, IEventSubscriber
    {
    }
}