using System;

namespace Nmb.Shared.Mvc
{
    public interface IScopeContext
    {
        Guid? UserId { get; }
        string UserDisplayName { get; }
        string UserEmail { get; }

        void Impersonate(Guid? userId, string userDisplayName = null, string userEmail = null);
    }
}