using System;

namespace Nmb.Shared.Dto
{
    public abstract class EntityDto: EntityDto<Guid>
    {
    }

    public abstract class EntityDto<T>
    {
        public T Id { get; set; }
    }
}
