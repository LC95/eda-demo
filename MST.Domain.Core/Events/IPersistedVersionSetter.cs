namespace MST.Domain.Abstraction.Events
{
    public interface IPersistedVersionSetter
    {
        long PersistedVersion { set; }
    }
}