using System;

namespace MST.Domain.Core
{
    public interface IEventSubscriber : IDisposable {
        void Subscribe();
    }
}