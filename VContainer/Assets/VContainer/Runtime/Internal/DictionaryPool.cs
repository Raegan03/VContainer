using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace VContainer.Internal
{
    internal static class DictionaryPool<TKey, TValue>
    {
        private static readonly Stack<Dictionary<TKey, TValue>> _pool = new Stack<Dictionary<TKey, TValue>>(4);
        
        /// <summary>
        /// Get a buffer from the pool.
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Dictionary<TKey, TValue> Get()
        {
            lock (_pool)
            {
                if (_pool.Count == 0)
                {
                    return new Dictionary<TKey, TValue>();
                }

                return _pool.Pop();
            }
        }

        /// <summary>
        /// Declare a buffer won't be used anymore and put it back to the pool.  
        /// </summary>
        /// <param name="buffer"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Release(Dictionary<TKey, TValue> buffer)
        {
            lock (_pool)
            {
                buffer.Clear();
                _pool.Push(buffer);
            }
        }
    }
    
    internal static class DictionaryPool
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, 
            Func<TKey, TValue> valueFactory)
        {
            if (!dict.TryGetValue(key, out var val))
            {
                val = valueFactory(key);
                dict.Add(key, val);
            }

            return val;
        }
    }
}