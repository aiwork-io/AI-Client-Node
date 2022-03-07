using System.Collections.Generic;

namespace Domain.Extensions
{
    public static class CollectionExtensions
    {
        public static void TryAddIfNotNull<T>(this ICollection<T> collection, T item)
        {
            if (item != null)
            {
                collection.Add(item);
            }
        }
    }
}