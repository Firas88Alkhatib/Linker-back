using Linker.Models;
using Microsoft.EntityFrameworkCore;

namespace Linker.Data;

public class ApplicationDb: DbContext
{
    private readonly IConfiguration _config;
    public ApplicationDb(DbContextOptions<ApplicationDb> options, IConfiguration config) : base(options)
    {
        _config = config;
    }

    public DbSet<ShortLink> ShortLinks { get; set; }
    public DbSet<Click> Clicks { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_config.GetConnectionString("DefaultConnection"));

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ShortLink>()
            .HasMany(l => l.Clicks)
            .WithOne(c => c.ShortLink)
            .HasForeignKey(c=>c.ShortLinkId);

        modelBuilder.Entity<Click>()
            .HasOne(c => c.ShortLink)
            .WithMany(l => l.Clicks)
            .HasForeignKey(c => c.ShortLinkId);
    }
}
