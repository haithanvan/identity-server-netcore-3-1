using Nmb.Shared.Extensions;

namespace Nmb.Shared.Specifications
{
    public class NotSpecification<T> : Specification<T>
    {
        public NotSpecification(ISpecification<T> spec) : base(spec.Predicate.Not())
        {
        }

    }
}
