using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Data
{
    public class EpubMetadataDbContext : DbContext
    {
        public EpubMetadataDbContext(DbContextOptions<EpubMetadataDbContext> options)
            : base(options)
        {
        }

        public DbSet<EpubMetadata> EpubMetadata { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            modelBuilder.Entity<EpubMetadata>()
                .HasKey(e => e.EpubId);

            modelBuilder.Entity<EpubMetadata>()
                .Property(e => e.EpubId)
                .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<EpubMetadata>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null),
                    new ValueComparer<ICollection<string>>(
                        (c1, c2) => c1.SequenceEqual(c2),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => (ICollection<string>)c.ToList()));
        }
    }
}