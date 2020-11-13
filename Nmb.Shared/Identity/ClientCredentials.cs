using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Identity
{
    public class ClientCredentials
    {
        public string Authority { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string[] Scopes { get; set; } = { };
    }
}
