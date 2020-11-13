﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Nmb.Shared.Extensions
{
    public static class ListExtentions
    {
        public static void AddRange<T>(this ISet<T> set, IEnumerable<T> items)
        {
            set.AddRange(items.ToList());
        }

        public static void AddRange<T>(this ISet<T> set, IList<T> items)
        {
            foreach (var item in items)
            {
                set.Add(item);
            }
        }

        public static void AddRange<T>(this IList<T> list, IEnumerable<T> items)
        {
            list.AddRange(items.ToList());
        }

        public static void AddRange<T>(this IList<T> list, IList<T> items)
        {
            foreach (var item in items)
            {
                list.Add(item);
            }
        }

        public static void UpdateRange<T>(this IList<T> list, IList<T> items, Action<T> updateFn)
        {
            foreach (var item in items)
            {
                updateFn(item);
            }
        }

        public static void RemoveRange<T>(this IList<T> list, IList<T> items)
        {
            foreach (var item in items)
            {
                list.Remove(item);
            }
        }

        public static void RemoveRange<T>(this ISet<T> set, IEnumerable<T> items)
        {
            set.RemoveRange(items.ToList());
        }

        public static void RemoveRange<T>(this ISet<T> set, IList<T> items)
        {
            foreach (var item in items)
            {
                set.Remove(item);
            }
        }

        public static void Set<T>(this ISet<T> set, IEnumerable<T> items)
        {
            set.Set(items.ToList());
        }

        public static void Set<T>(this ISet<T> set, IList<T> items)
        {
            set.AddRange(items);
            var deletedItems = set.Except(items).ToList();
            set.RemoveRange(deletedItems);
        }

        public static void Set<T>(this IList<T> list, IList<T> items)
        {
            if (items == null)
            {
                foreach (var t in list.ToList())
                {
                    list.Remove(t);
                }

                return;
            }

            foreach (var t in items.ToList())
            {
                if (!list.Contains(t))
                {
                    list.Add(t);
                }
            }

            foreach (var t in list.ToList())
            {
                if (!items.Contains(t))
                {
                    list.Remove(t);
                }
            }
        }
    }
}
