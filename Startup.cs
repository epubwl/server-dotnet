using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using EpubWebLibraryServer.Areas.User.Extensions;

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

            Action<DbContextOptionsBuilder> dbContextOptionsAction = ChooseDatabaseProvider();

            services.AddJwtAuthentication(dbContextOptionsAction, options => Configuration.Bind("JwtAuthenticationSettings", options));
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

        private Action<DbContextOptionsBuilder> ChooseDatabaseProvider()
        {
            Action<DbContextOptionsBuilder> dbContextOptionsAction;
            if (!String.IsNullOrEmpty(Configuration.GetConnectionString("Postgresql")))
            {
                dbContextOptionsAction = options =>
                    options.UseNpgsql(Configuration.GetConnectionString("Postgresql"));
            }
            else
            {
                dbContextOptionsAction = options =>
                    options.UseSqlite(Configuration.GetConnectionString("Sqlite"));
            }
            return dbContextOptionsAction;
        }
    }
}
