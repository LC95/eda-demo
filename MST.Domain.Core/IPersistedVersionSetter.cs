namespace MST.Domain.Abstraction
{
    public interface IPersistedVersionSetter
    {
        long PersistedVersion { set; }
    }
}