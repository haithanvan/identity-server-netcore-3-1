using MediatR;
using System;

namespace Nmb.Shared.DomainEvents
{
    public class EntityDeletedDomainEvent : INotification
    {
        public EntityDeletedDomainEvent(Guid id, Type entityType)
        {
            Id = id;
            EntityType = entityType;
        }

        public Guid Id { get; private set; }
        public Type EntityType { get; private set; }
    }
}
