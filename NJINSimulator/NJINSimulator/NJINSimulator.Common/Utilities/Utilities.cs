using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NJINSimulator.Common.Utilities
{
    public static class Utilities
    {
        public static bool IsNullOrEmpty(this string toCheck)
        {
            return string.IsNullOrEmpty(toCheck);
        }
        public static void AddOrUpdate<K,V>(this Dictionary<K, V> dictionary, K key, V value)
        {
            if (dictionary == null) { return; }

            if (dictionary.ContainsKey(key))
            {
                dictionary[key] = value;
            }
            else
            {
                dictionary.Add(key, value);
            }
        }
        public static V SafeRetrieve<K, V>(this Dictionary<K, V> dictionary, K key)
        {
            if (dictionary == null) { return default; }

            if (dictionary.ContainsKey(key))
            {
                return dictionary[key];
            }
            else
            {
                return default;
            }
        }
    }
}
