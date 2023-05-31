using System;
using System.Collections.Concurrent;
using System.Numerics;
using System.Threading;
using Fibonacci.Logic.Services.Interfaces;

namespace Fibonacci.Logic.Services
{
    public class CacheService: ICacheService
    {
        private readonly ConcurrentDictionary<int, BigInteger> _cache = new();

        public CacheService(int lifetime)
        {
            if (lifetime < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lifetime));
            }

            if (lifetime == 0)
            {
                return;
            }

            var timer = new Timer(_ => { Invalidate(); });
            timer.Change(lifetime, lifetime);
        }

        public bool Enabled { get; set; }

        public void Set(int index, BigInteger value)
        {
            if (Enabled)
            {
                _cache.TryAdd(index, value);
            }
        }

        public bool TryGet(int index, out BigInteger value)
        {
            if (Enabled && _cache.ContainsKey(index))
            {
                value = _cache[index];
                return true;
            }

            value = -1;
            return false;
        }

        private void Invalidate()
        {
            _cache.Clear();
        }
    }
}
