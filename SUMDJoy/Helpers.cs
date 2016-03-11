using System.Collections.Generic;
using System.Linq;

namespace SUMDJoy
{
    public static class Helpers
    {
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key, TValue newValue)
        {
            TValue value = default(TValue);
            if (dict.TryGetValue(key, out value))
                dict[key] = newValue;
            else
                dict.Add(key, newValue);

        }

        public static IEnumerable<T> FindDuplicates<T>(this IEnumerable<T> value)
        {
            return value.GroupBy(x => x)
                     .Where(g => g.Count() > 1)
                     .Select(g => g.Key);
        }
    }
}
