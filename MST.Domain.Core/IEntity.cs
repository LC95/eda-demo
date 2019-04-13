using System;

namespace MST.Domain.Abstraction
{
    public interface IEntity
    {
        Guid Id { get; }
    }
}