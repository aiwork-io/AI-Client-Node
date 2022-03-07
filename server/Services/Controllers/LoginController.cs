using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Services.Controllers
{
    public class LoginController : ApiController
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<JobController> _logger;

        public LoginController(IDistributedCache distributedCache,
            ILogger<JobController> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var handler = new JwtSecurityTokenHandler();

            JwtSecurityToken jwt;

            try
            {
                jwt = handler.ReadJwtToken(model.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                return BadRequest("Invalid jwt token");
            }

            await _distributedCache.SetStringAsync("token", model.Token, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = jwt.ValidTo
            });

            return Ok(jwt.ValidTo);
        }

        public class LoginModel
        {
            public string Token { get; set; }
        }
    }
}