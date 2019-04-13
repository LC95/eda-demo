using System;

namespace MST.Domain.Abstraction.Events.DomainEvents
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class HandlesInlineAttribute : Attribute
    {
    }
}