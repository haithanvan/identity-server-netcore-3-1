using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nmb.Shared.Linq
{
    internal interface IExpressionCollection : IEnumerable<Expression>
    {
        void Fill();
    }
}