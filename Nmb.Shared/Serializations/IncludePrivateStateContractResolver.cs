using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Nmb.Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Nmb.Shared.Serializations
{
    public class IncludePrivateStateContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var properties = objectType.GetProperties(flags);//.Where(p => p.HasSetter() && p.HasGetter());
            var fields = objectType.GetFields(flags);

            var allMembers = properties.Cast<MemberInfo>().Union(fields);
            return allMembers.ToList();
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var prop = base.CreateProperty(member, memberSerialization);

            if (!prop.Writable)
            {
                var property = member as PropertyInfo;
                if (property != null)
                {
                    prop.Writable = property.HasSetter();
                }
                else
                {
                    var field = member as FieldInfo;
                    if (field != null)
                    {
                        prop.Writable = true;
                    }
                }
            }

            if (!prop.Readable)
            {
                var field = member as FieldInfo;
                if (field != null)
                {
                    prop.Readable = true;
                }
            }

            return prop;
        }
    }
}
