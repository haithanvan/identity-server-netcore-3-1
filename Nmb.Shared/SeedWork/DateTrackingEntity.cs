using System;

namespace Nmb.Shared.SeedWork
{
    public interface IDateTracking : IEntity
    {
        DateTimeOffset CreatedDate { get; }
        DateTimeOffset ModifiedDate { get; }
    }

    public abstract class DateTrackingEntity : Entity, IDateTracking
    {
        public DateTimeOffset CreatedDate { get; private set; }
        public DateTimeOffset ModifiedDate { get; private set; }
    }
}
