using System;
using System.Linq.Expressions;

namespace Nmb.Shared.Specifications
{
    public class Specification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Predicate { get; }

        public Specification(Expression<Func<T, bool>> predicate)
        {
            Predicate = predicate ?? throw new ArgumentNullException();
        }

        public virtual bool IsSatisfiedBy(T o)
        {
            return Predicate.Compile()(o);
        }

        public ISpecification<T> And(ISpecification<T> specification)
        {
            return new AndSpecification<T>(this, specification);
        }

        public ISpecification<T> Or(ISpecification<T> specification)
        {
            return new OrSpecification<T>(this, specification);
        }

        public static ISpecification<T> Not(ISpecification<T> specification)
        {
            return new NotSpecification<T>(specification);
        }
    }

    public class EverythingSpecification<T> : Specification<T>
    {
        public EverythingSpecification() : base(t => true)
        {
        }
    }
}
