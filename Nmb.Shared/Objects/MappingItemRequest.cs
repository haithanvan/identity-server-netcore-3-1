using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Objects
{
    public class MappingItemRequest
    {
        public Guid EntityId { get; set; }
        public FolderResourseType Type { get; set; }
    }
}
