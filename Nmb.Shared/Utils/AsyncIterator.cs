using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Nmb.Shared.Utils
{
    public class AsyncIterator<T>
    {
        private readonly Func<int, int, Task<IEnumerable<T>>> _query;
        private int _skip = 0;
        private int _take;
        private List<T> _value;

        public AsyncIterator(Func<int, int, Task<IEnumerable<T>>> query, int chunkSize = 10)
        {
            _query = query;
            _take = chunkSize;
        }

        private async Task<bool> Next(CancellationToken cancellationToken = default(CancellationToken))
        {
            var list = await _query(_skip, _take);
            if (list == null || !list.Any())
            {
                return false;
            }

            _skip += _take;
            _value = list.ToList();
            return true;
        }

        public async Task ForEach(Func<T, Task> selector,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (await Next(cancellationToken))
            {
                foreach (var item in _value)
                {
                    await selector(item);
                }
            }
        }

        public async Task ForEach(Action<T> selector, CancellationToken cancellationToken = default(CancellationToken))
        {
            while (await Next(cancellationToken))
            {
                foreach (var item in _value)
                {
                    selector(item);
                }
            }
        }

        public async Task ForChunk(Func<IEnumerable<T>, Task> selector,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (await Next(cancellationToken))
            {
                await selector(_value);
            }
        }

        public async Task ForChunk(Action<IEnumerable<T>> selector,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            while (await Next(cancellationToken))
            {
                selector(_value);
            }
        }

        public static AsyncIterator<T> From(Func<int, int, Task<IEnumerable<T>>> query)
        {
            return new AsyncIterator<T>(query);
        }

        public static AsyncIterator<T> From(IQueryable<T> queryable)
        {
            return new AsyncIterator<T>(async (skip, take) => await queryable.Skip(skip).Take(take).ToListAsync());
        }

        public AsyncIterator<T> WithChunkSize(int chunkSize)
        {
            _take = chunkSize;
            return this;
        }
    }

    public static class QueryableHelper
    {
        public static AsyncIterator<T> IterateAsync<T>(this IQueryable<T> @this, int chunkSize = 10)
        {
            return AsyncIterator<T>.From(@this).WithChunkSize(chunkSize);
        }

        public static IQueryable<T> GetIncludedAllQuery<T>(this DbContext context) where T: class
        {
            return context.GetIncludePaths<T>().Aggregate(context.Set<T>().AsQueryable(), (q, p) => q.Include(p));
        }

        public static IEnumerable<string> GetIncludePaths<T>(this DbContext context)
        {
            var entityType = context.Model.FindEntityType(typeof(T));
            var includedNavigations = new HashSet<INavigation>();

            var stack = new Stack<IEnumerator<INavigation>>();
            while (true)
            {
                var entityNavigations = new List<INavigation>();
                foreach (var navigation in entityType.GetNavigations())
                {
                    if (includedNavigations.Add(navigation))
                        entityNavigations.Add(navigation);
                }

                if (entityNavigations.Count == 0)
                {
                    if (stack.Count > 0)
                        yield return string.Join(".", stack.Reverse().Select(e => e.Current.Name));
                }
                else
                {
                    foreach (var navigation in entityNavigations)
                    {
                        var inverseNavigation = navigation.FindInverse();
                        if (inverseNavigation != null)
                            includedNavigations.Add(inverseNavigation);
                    }

                    stack.Push(entityNavigations.GetEnumerator());
                }

                while (stack.Count > 0 && !stack.Peek().MoveNext())
                    stack.Pop();
                if (stack.Count == 0) break;
                entityType = stack.Peek().Current.GetTargetType();
            }
        }
    }

}
