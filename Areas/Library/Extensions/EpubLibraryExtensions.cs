using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Data.Common;
using EpubWebLibraryServer.Areas.Library.Data;
using EpubWebLibraryServer.Areas.Library.Services;

namespace EpubWebLibraryServer.Areas.Library.Extensions
{
    public static class EpubLibraryExtensions
    {
        public static IServiceCollection AddEpubMetadataStorage(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsAction)
        {
            services.AddDbContext<EpubMetadataDbContext>(dbContextOptionsAction);

            return services;
        }

        public static IServiceCollection AddEpubBinaryDataDbStorage(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextOptionsAction, DbProviderFactory dbProviderFactory, string connectionString)
        {
            services.AddDbContext<EpubFileDbContext>(dbContextOptionsAction);

            services.AddDbContext<EpubCoverDbContext>(dbContextOptionsAction);

            DbStreamFactory dbStreamFactory = new DbStreamFactory(dbProviderFactory, connectionString);
            services.AddSingleton<DbStreamFactory>(dbStreamFactory);

            IEpubBinaryDataStorage epubBinaryDataStorage = new EpubBinaryDataDbStorage(dbProviderFactory, connectionString, dbStreamFactory);
            services.AddSingleton<IEpubBinaryDataStorage>(epubBinaryDataStorage);

            return services;
        }

        public static IServiceCollection AddEpubManager(this IServiceCollection services)
        {
            services.AddScoped<EpubManager>();

            return services;
        }
    }
}