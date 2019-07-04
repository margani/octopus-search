using System;
using System.Collections.Generic;
using System.Linq;

namespace OctopusSearch.Core
{
    public static class Extensions
    {
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            var elements = collection as T[] ?? collection.ToArray();

            foreach (var element in elements)
            {
                action(element);
            }

            return elements;
        }
    }
}
