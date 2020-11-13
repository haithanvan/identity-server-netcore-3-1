using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Nmb.Shared.Utils
{
    public static class ObjectUtils
    {
        public static string GetEnumDescription(this object target)
        {
            return target.GetType().GetMember(target.ToString()).FirstOrDefault()?.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        public static bool HasAny<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                return false;
            }
            return source.Any(predicate);
        }
    }
}
