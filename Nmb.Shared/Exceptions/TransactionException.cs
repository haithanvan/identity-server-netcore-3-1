using System;

namespace Nmb.Shared.Exceptions
{
    public class TransactionException : DomainException
    {
        public TransactionException(string message): base(message)
        {

        }
    }
}
