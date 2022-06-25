using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Data.Common;
using EpubWebLibraryServer.Areas.Library.Extensions;
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
            services.AddCors(options =>
            {
                options.AddPolicy(name: "CorsAllowAll",
                                policy  =>
                                {
                                    policy.AllowAnyOrigin()
                                        .AllowAnyHeader()
                                        .AllowAnyMethod();
                                });
            });

            services.AddControllers();

            Action<DbContextOptionsBuilder> dbContextOptionsAction = ChooseUserDbContextOptionsAction();

            services.AddJwtAuthentication(dbContextOptionsAction, options => Configuration.Bind("JwtAuthenticationSettings", options));

            RegisterDbProviderFactories();

            services.AddEpubMetadataStorage(dbContextOptionsAction);
            services.AddEpubBinaryDataDbStorage(dbContextOptionsAction, ChooseEpubStorageDbProviderFactory(), ChooseEpubStorageConnectionString());
            services.AddEpubManager();
            services.AddEpubParser();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseCors("CorsAllowAll");

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{area}/{controller}/{action}");
            });
        }

        private Action<DbContextOptionsBuilder> ChooseUserDbContextOptionsAction()
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

        private void RegisterDbProviderFactories()
        {
            DbProviderFactories.RegisterFactory("Postgresql", NpgsqlFactory.Instance);
            DbProviderFactories.RegisterFactory("Sqlite", SqliteFactory.Instance);
        }

        private DbProviderFactory ChooseEpubStorageDbProviderFactory()
        {
            DbProviderFactory dbProviderFactory;
            if (!String.IsNullOrEmpty(Configuration.GetConnectionString("Postgresql")))
            {
                dbProviderFactory = DbProviderFactories.GetFactory("Postgresql");
            }
            else
            {
                dbProviderFactory = DbProviderFactories.GetFactory("Sqlite");
            }
            return dbProviderFactory;
        }

        private string ChooseEpubStorageConnectionString()
        {
            string connectionString;
            if (!String.IsNullOrEmpty(Configuration.GetConnectionString("Postgresql")))
            {
                connectionString = Configuration.GetConnectionString("Postgresql");
            }
            else
            {
                connectionString = Configuration.GetConnectionString("Sqlite");
            }
            return connectionString;
        }
    }
}
