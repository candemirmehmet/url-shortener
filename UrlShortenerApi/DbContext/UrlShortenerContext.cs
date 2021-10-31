using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UrlShortener.Application.Domain;

namespace UrlShortener.Application.DbContext
{
    public class UrlShortenerContext : Microsoft.EntityFrameworkCore.DbContext
    {
        private readonly IConfiguration _configuration;

        public UrlShortenerContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public DbSet<ShortUrl> ShortUrls { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_configuration.GetConnectionString("UrlDatabase"), options =>
            {
                options.MigrationsAssembly(Assembly.GetExecutingAssembly().FullName);
            });

            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map table names
            modelBuilder.Entity<ShortUrl>().ToTable("ShortUrls", "UrlShortenerService");

            modelBuilder.Entity<ShortUrl>(entity =>
            {
                entity.HasKey(e => e.UrlId);

                entity.HasIndex(e => e.EncodedUrl).IsUnique(); // creates unique index

                entity.HasIndex(e => e.CustomAlias);
                entity.Property(e => e.CreationDateTime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
