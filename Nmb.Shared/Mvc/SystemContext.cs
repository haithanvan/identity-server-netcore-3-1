using System;

namespace Nmb.Shared.Mvc
{
    public class SystemContext: IScopeContext
    {
        public Guid? UserId => Guid.NewGuid();
        public string UserDisplayName => "system";
        public string UserEmail => "system";

        public void Impersonate(Guid? userId, string userDisplayName = null, string userEmail = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
