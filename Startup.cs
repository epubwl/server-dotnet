using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;
using EpubWebLibraryServer.Areas.User.Data;

namespace EpubWebLibraryServer
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            if (!String.IsNullOrEmpty(Configuration.GetConnectionString("Postgresql")))
            {
                services.AddDbContext<UserDbContext>(options =>
                    options.UseNpgsql(Configuration.GetConnectionString("Postgresql")));
            }
            else
            {
                services.AddDbContext<UserDbContext>(options =>
                    options.UseSqlite(Configuration.GetConnectionString("Sqlite")));
            }

            services.AddIdentity<ApplicationUser, ApplicationRole>(options => Configuration.Bind("IdentityOptions", options))
                .AddEntityFrameworkStores<UserDbContext>();

            services.AddAuthentication(options => 
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    Configuration.Bind("JwtBearerOptions", options);
                    options.TokenValidationParameters.TokenDecryptionKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Configuration.GetValue<string>("JwtSettings:EncryptingSecret")));
                    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.Default.GetBytes(Configuration.GetValue<string>("JwtSettings:SigningSecret")));
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area}/{controller}/{action}");
            });
        }
    }
}
