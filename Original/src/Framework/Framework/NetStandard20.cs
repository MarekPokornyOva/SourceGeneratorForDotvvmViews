#if NETSTANDARD2_0
using System;
using System.Collections.Generic;

namespace DotVVM.Framework
{
    internal static class NetStandard20
    {
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
            => source.ToHashSet(null);

        internal static HashSet<TSource> ToHashSet<TSource>(this IEnumerable<TSource> source, IEqualityComparer<TSource>? comparer)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return new HashSet<TSource>(source, comparer);
        }

        internal static bool SetEquals<T>(this HashSet<T> self, IEnumerable<T> other)
        {
            bool ContainsAllElements(IEnumerable< T> other)
            {
                foreach (T item in other)
                    if (!self.Contains(item))
                        return false;
                return true;
            }

            if (other == null)
                throw new ArgumentNullException(nameof(other));
            if (other == self)
                return true;

            HashSet<T> hashSet = new HashSet<T>(other, self.Comparer);
            return (self.Count == hashSet.Count) && ContainsAllElements(hashSet);
        }
    }
}
#endif
