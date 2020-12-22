using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EpubWebLibraryServer.Areas.User.Data;

namespace EpubWebLibraryServer.Areas.User.Services
{
    public class JwtGenerator : ITokenGenerator
    {
        private readonly IConfiguration _configuration;

        private readonly IOptionsMonitor<JwtBearerOptions> _optionsMonitor;

        public JwtGenerator(IConfiguration configuration, IOptionsMonitor<JwtBearerOptions> optionsMonitor)
        {
            this._configuration = configuration;
            this._optionsMonitor = optionsMonitor;
        }

        public string GenerateToken(ApplicationUser user)
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