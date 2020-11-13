using Newtonsoft.Json;
using System;

namespace Nmb.Shared.Dto
{
    public abstract class DateTrackingDto : DateTrackingDto<Guid>
    {
    }

    public abstract class DateTrackingDto<T> : EntityDto<T>
    {
        [JsonProperty("created_date")]
        public DateTimeOffset CreatedDate { get; set; }
        [JsonProperty("modified_date")]
        public DateTimeOffset ModifiedDate { get; set; }
    }
}
