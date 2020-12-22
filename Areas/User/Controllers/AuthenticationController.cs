using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using EpubWebLibraryServer.Areas.User.Data;
using EpubWebLibraryServer.Areas.User.Models;

namespace EpubWebLibraryServer.Areas.User.Controllers
{
    [ApiController]
    [Area("User")]
    [Route("api/[area]/[controller]/[action]")]
    public class AuthenticationController : Controller
    {
        private readonly IConfiguration _configuration;

        private readonly IOptionsMonitor<JwtBearerOptions> _optionsMonitor;

        private readonly UserManager<ApplicationUser> _userManager;
        
        public AuthenticationController(IConfiguration configuration, IOptionsMonitor<JwtBearerOptions> optionsMonitor, UserManager<ApplicationUser> userManager)
        {
            this._configuration = configuration;
            this._optionsMonitor = optionsMonitor;
            this._userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserCredentials userCredentials)
        {
            var user = new ApplicationUser()
            {
                UserName = userCredentials.Username
            };
            IdentityResult result = await _userManager.CreateAsync(user, userCredentials.Password);
            if (!result.Succeeded)
            {
                return BadRequest();
            }
            return Ok(new { token = GenerateToken(user) });
        }

        [HttpPost]
        public async Task<IActionResult> Login([FromBody] UserCredentials userCredentials)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(userCredentials.Username);
            foreach (IPasswordValidator<ApplicationUser> passwordValidator in _userManager.PasswordValidators)
            {
                IdentityResult result = await passwordValidator.ValidateAsync(_userManager, user, userCredentials.Password);
                if (!result.Succeeded)
                {
                    return BadRequest();
                }
            }
            return Ok(new { token = GenerateToken(user) });
        }

        private string GenerateToken(ApplicationUser user)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtBearerOptions jwtBearerOptions = _optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = jwtBearerOptions.Audience,
                EncryptingCredentials = new EncryptingCredentials(jwtBearerOptions.TokenValidationParameters.TokenDecryptionKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256),
                Expires = DateTime.UtcNow.AddMinutes(_configuration.GetValue<Int32>("JwtSettings:LifetimeInMinutes")),
                Issuer = jwtBearerOptions.ClaimsIssuer,
                SigningCredentials = new SigningCredentials(jwtBearerOptions.TokenValidationParameters.IssuerSigningKey, SecurityAlgorithms.HmacSha256),
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName)
                })
            };
            JwtSecurityToken jwtSecurityToken = jwtSecurityTokenHandler.CreateJwtSecurityToken(securityTokenDescriptor);
            return jwtSecurityTokenHandler.WriteToken(jwtSecurityToken);
        }
    }
}