namespace MST.Domain.Abstraction.Events.DomainEvents
{
    public interface IPersistedVersionSetter
    {
        long PersistedVersion { set; }
    }
}