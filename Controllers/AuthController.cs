using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Google.Apis.Auth;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using YunInspector.CIIP;


namespace YunInspector.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly CIIPContext _context;
        private readonly ILogger _logger;
        private readonly IConfiguration _config;

        public AuthController(CIIPContext context, ILogger<AuthController> logger, IConfiguration config)
        {
            _context = context;
            _logger = logger;
            _config = config;
        }

        [HttpPost]
        [Route("authcallback")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> authCallback([FromBody]AuthCodeRequest code)
        {
            var id_token = code.Code;
            GoogleJsonWebSignature.Payload payload;

            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(id_token, new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _config.GetValue<string>("OAuth:ClientID") },
                    HostedDomain = "gemail.yuntech.edu.tw"
                });
            }
            catch (Google.Apis.Auth.InvalidJwtException ex)
            {
                _logger.LogError(ex.Message);
                return BadRequest();
            }


            var googldID = payload.Subject;
            //if not exist do register
            //TODO:redirect to tern of use first?
            if (!await _context.User.AnyAsync(x => x.GoogleId == googldID))
            {
                await _context.User.AddAsync(new User() { GoogleId = googldID, IsBlocked = false });
                await _context.SaveChangesAsync();
            }

            int userID = _context.User.Where(x => x.GoogleId == googldID).Select(x => x.UserId).FirstOrDefault();
            //login jwt token
            var token = new JwtBuilder().WithAlgorithm(new HMACSHA256Algorithm())
                                        .WithSecret(_config.GetValue<string>("JWT:secrets"))
                                        .Issuer(_config.GetValue<string>("JWT:iss"))
                                        .Subject(userID.ToString())
                                        .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(30).ToUnixTimeSeconds())
                                        .AddClaim("iat", DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                                        .Build();

            return Ok(token);
        }
        public class AuthCodeRequest
        {
            public string Code { get; set; }
        }
    }
}