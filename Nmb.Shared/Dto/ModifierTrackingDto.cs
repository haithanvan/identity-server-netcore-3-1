using Newtonsoft.Json;
using System;

namespace Nmb.Shared.Dto
{
    public class ModifierTrackingDto: ModifierTrackingDto<Guid>
    {
    }

    public class ModifierTrackingDto<T>: DateTrackingDto<T>
    {
        [JsonProperty("created_by_id")]
        public Guid CreatedById { get; set; }
        [JsonProperty("modified_by_id")]
        public Guid ModifiedById { get; set; }
    }
}
