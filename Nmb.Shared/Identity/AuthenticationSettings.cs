using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Identity
{
    public class AuthenticationSettings
    {
        public string PublicOrigin { get; set; }
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public bool RequireHttpsMetadata { get; set; }
    }
}
