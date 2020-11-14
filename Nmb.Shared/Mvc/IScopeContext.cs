using System;

namespace Nmb.Shared.Mvc
{
    public interface IScopeContext
    {
        string UserId { get; }
        string UserDisplayName { get; }
        string UserEmail { get; }
    }
}