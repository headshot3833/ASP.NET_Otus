using Microsoft.Extensions.Caching.Distributed;
using Pcf.ReceivingFromPartner.Core.Abstractions.Redis;
using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Services.Redis
{
    public class PreferenceCacheService : IPreferenceCacheService
    {
        private readonly IDistributedCache _cache;

        public PreferenceCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<Preference> GetPreferenceAsync(string key)
        {
            var json = await _cache.GetAsync(key);
            if (json == null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<Preference>(json);
        }

        public async Task SetAsync(string key, Preference value)
        {
            var json = JsonSerializer.Serialize(value);
            await _cache.SetStringAsync(key, json);
        }
    }
}
