using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nmb.Shared.Serializations
{
    public class PrivateFieldJsonConverter<T> : CustomCreationConverter<T> where T : class
    {
        public override T Create(Type objectType)
        {
            return Activator.CreateInstance<T>();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            serializer.ContractResolver = new IncludePrivateStateContractResolver();
            if (reader.TokenType == JsonToken.Null)
                return base.ReadJson(reader, objectType, existingValue, serializer);

            var target = existingValue ?? Activator.CreateInstance<T>();
            serializer.Populate(reader, target);
            return target;
        }
    }
}
