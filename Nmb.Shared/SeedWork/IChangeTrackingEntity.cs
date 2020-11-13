using MediatR;
using System.Collections.Generic;

namespace Nmb.Shared.SeedWork
{
    public interface IChangeTrackingEntity
    {
        IReadOnlyCollection<INotification> DomainEvents { get; }
        void ClearDomainEvents();
        void AddDomainEvent(INotification eventItem);
    }
}