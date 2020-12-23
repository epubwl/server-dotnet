using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using EpubWebLibraryServer.Areas.User.Data;
using EpubWebLibraryServer.Areas.User.Services;

namespace EpubWebLibraryServer.Areas.User.Extensions
{
    public static class JwtAuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsAction, JwtAuthenticationOptions jwtAuthenticationOptions)
        {
            services.Configure<JwtAuthenticationOptions>(options => 
            {
                options.Audience = jwtAuthenticationOptions.Audience;
                options.Issuer = jwtAuthenticationOptions.Issuer;
                options.EncryptingSecret = jwtAuthenticationOptions.EncryptingSecret;
                options.SigningSecret = jwtAuthenticationOptions.SigningSecret;
                options.LifetimeInMinutes = jwtAuthenticationOptions.LifetimeInMinutes;
            });

            services.AddDbContext<UserDbContext>(dbContextOptionsAction);

            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
            }).AddEntityFrameworkStores<UserDbContext>();

            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.Audience = jwtAuthenticationOptions.Audience;
                options.ClaimsIssuer = jwtAuthenticationOptions.Issuer;
                options.TokenValidationParameters.ValidAudience = jwtAuthenticationOptions.Audience;
                options.TokenValidationParameters.ValidIssuer = jwtAuthenticationOptions.Issuer;
                options.TokenValidationParameters.TokenDecryptionKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtAuthenticationOptions.EncryptingSecret));
                options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(jwtAuthenticationOptions.SigningSecret));
            });
            
            services.AddSingleton<ITokenGenerator, JwtGenerator>();

            return services;
        }
    }
}