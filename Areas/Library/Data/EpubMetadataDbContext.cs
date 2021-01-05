using Microsoft.EntityFrameworkCore;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Data
{
    public class EpubMetadataDbContext : DbContext
    {
        public DbSet<EpubMetadata> EpubMetadata { get; set; }

        public EpubMetadataDbContext(DbContextOptions<EpubMetadataDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {   
            modelBuilder.Entity<EpubMetadata>()
                .HasKey(e => e.EpubId);

            modelBuilder.Entity<EpubMetadata>()
                .Property(e => e.EpubId)
                .ValueGeneratedOnAdd();
        }
    }
}