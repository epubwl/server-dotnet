using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EpubWebLibraryServer.Areas.User.Models;

namespace EpubWebLibraryServer.Areas.User.Services
{
    public class JwtGenerator : ITokenGenerator
    {
        private readonly IOptionsMonitor<JwtAuthenticationOptions> _jwtAuthenticationOptionsMonitor;

        private readonly IOptionsMonitor<JwtBearerOptions> _jwtBearerOptionsMonitor;

        public JwtGenerator(IOptionsMonitor<JwtAuthenticationOptions> jwtAuthenticationOptionsMonitor, IOptionsMonitor<JwtBearerOptions> jwtBearerOptionsMonitor)
        {
            this._jwtAuthenticationOptionsMonitor = jwtAuthenticationOptionsMonitor;
            this._jwtBearerOptionsMonitor = jwtBearerOptionsMonitor;
        }

        public string GenerateToken(ApplicationUser user)
        {
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            JwtAuthenticationOptions jwtAuthenticationOptions = _jwtAuthenticationOptionsMonitor.Get(Options.DefaultName);
            JwtBearerOptions jwtBearerOptions = _jwtBearerOptionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);
            var securityTokenDescriptor = new SecurityTokenDescriptor()
            {
                Audience = jwtAuthenticationOptions.Audience,
                EncryptingCredentials = new EncryptingCredentials(jwtBearerOptions.TokenValidationParameters.TokenDecryptionKey, SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256),
                Expires = DateTime.UtcNow.AddMinutes(jwtAuthenticationOptions.LifetimeInMinutes),
                Issuer = jwtAuthenticationOptions.Issuer,
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