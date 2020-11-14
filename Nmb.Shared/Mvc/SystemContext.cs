using System;

namespace Nmb.Shared.Mvc
{
    public class SystemContext: IScopeContext
    {
        public string UserId => "system";
        public string UserDisplayName => "system";
        public string UserEmail => "system";

        public void Impersonate(string userId, string userDisplayName = null, string userEmail = null)
        {
            throw new System.NotImplementedException();
        }
    }
}
