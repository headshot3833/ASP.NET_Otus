using Pcf.ReceivingFromPartner.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pcf.ReceivingFromPartner.Core.Abstractions.Redis
{
    public interface IPreferenceCacheService
    {
        Task<Preference> GetPreferenceAsync(string key);
        Task SetAsync(string key, Preference value);
    }
}
