using Microsoft.EntityFrameworkCore;
using EpubWebLibraryServer.Areas.Library.Models;

namespace EpubWebLibraryServer.Areas.Library.Data
{
    public class EpubCoverDbContext : DbContext
    {
        public EpubCoverDbContext(DbContextOptions<EpubCoverDbContext> options)
            : base(options)
        {
        }

        public DbSet<EpubCover> EpubCovers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<EpubCover>()
                .HasKey(e => e.EpubId);
        }
    }
}