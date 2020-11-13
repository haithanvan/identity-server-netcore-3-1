using System;

namespace Nmb.Shared.SeedWork
{
    public interface IModifierTrackingEntity
    {
        Guid CreatedById { get; }
        Guid ModifiedById { get; }
    }

    public abstract class ModifierTrackingEntity : DateTrackingEntity, IModifierTrackingEntity
    {
        public string CreatedBy { get; private set; }
        public Guid CreatedById { get;  set; }

        public string ModifiedBy { get; private set; }
        public Guid ModifiedById { get;  set; }

        public void MarkCreated(Guid authorId, string authorName)
        {
            CreatedBy = authorName;
            ModifiedBy = authorName;
            CreatedById = authorId;
            ModifiedById = authorId;
        }

        public void MarkModified(Guid authorId, string authorName)
        {
            ModifiedBy = authorName;
            ModifiedById = authorId;
        }
    }
}
