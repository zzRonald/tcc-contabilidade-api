using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using TCC.Contabilidade.Application.Interfaces;

namespace TCC.Contabilidade.Infrastructure.Services
{
    public class RateLimitService : IRateLimitService
    {
        private readonly IMemoryCache _cache;

        public RateLimitService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public Task<bool> IsRequestAllowedAsync(string key, int limit, TimeSpan window)
        {
            var count = _cache.GetOrCreate(key, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = window;
                return 0;
            });

            if (count >= limit)
                return Task.FromResult(false);

            _cache.Set(key, count + 1, window);

            return Task.FromResult(true);
        }
    }
}


