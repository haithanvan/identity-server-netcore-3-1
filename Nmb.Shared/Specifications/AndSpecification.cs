using Nmb.Shared.Extensions;

namespace Nmb.Shared.Specifications
{
    public class AndSpecification<T> : Specification<T>
    {
        public AndSpecification(ISpecification<T> left, ISpecification<T> right) : base(left.Predicate.AndAlso(right.Predicate))
        {
        }
    }
}
