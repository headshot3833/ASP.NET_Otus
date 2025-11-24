using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Pcf.ReceivingFromPartner.Core.Abstractions.Redis;
using Pcf.ReceivingFromPartner.Core.Abstractions.Repositories;
using Pcf.ReceivingFromPartner.Core.Domain;
using Pcf.ReceivingFromPartner.WebHost.Models;

namespace Pcf.ReceivingFromPartner.WebHost.Controllers
{
    /// <summary>
    /// Предпочтения клиентов
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class PreferencesController
        : ControllerBase
    {
        private readonly IPreferenceCacheService _cache;
        public PreferencesController(IPreferenceCacheService cache)
        {
            _cache = cache;
        }
        /// <summary>
        /// Получить предпочтение по ключу
        /// </summary>
        [HttpGet("{key}")]
        public async Task<ActionResult<Preference>> GetPreferenceAsync(string key)
        {
            var preference = await _cache.GetPreferenceAsync(key);

            if (preference == null)
                return NotFound();

            return Ok(preference);
        }

        /// <summary>
        /// Добавить или обновить предпочтение
        /// </summary>
        [HttpPost("{key}")]
        public async Task<IActionResult> SetPreferenceAsync(string key, [FromBody] Preference value)
        {
            await _cache.SetAsync(key, value);
            return Ok();
        }
    }
}