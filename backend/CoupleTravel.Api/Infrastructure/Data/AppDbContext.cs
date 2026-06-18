using CoupleTravel.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CoupleTravel.Api.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Couple> Couples => Set<Couple>();
    public DbSet<CoupleMember> CoupleMembers => Set<CoupleMember>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<Trip> Trips => Set<Trip>();
    public DbSet<Photo> Photos => Set<Photo>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.HasPostgresExtension("postgis");

        model.Entity<User>(e =>
        {
            e.ToTable("users");
            e.HasKey(u => u.Id);
            e.Property(u => u.Id).HasDefaultValueSql("gen_random_uuid()");
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.CreatedAt).HasDefaultValueSql("now()");
        });

        model.Entity<Couple>(e =>
        {
            e.ToTable("couples");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
        });

        model.Entity<CoupleMember>(e =>
        {
            e.ToTable("couple_members");
            e.HasKey(cm => new { cm.CoupleId, cm.UserId });
            e.HasOne(cm => cm.Couple).WithMany(c => c.Members).HasForeignKey(cm => cm.CoupleId);
            e.HasOne(cm => cm.User).WithMany(u => u.CoupleMembers).HasForeignKey(cm => cm.UserId);
        });

        model.Entity<City>(e =>
        {
            e.ToTable("cities");
            e.HasKey(c => c.Id);
            e.Property(c => c.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(c => c.CreatedAt).HasDefaultValueSql("now()");
            e.HasIndex(c => c.GeocodePlaceId).IsUnique().HasFilter("\"GeocodePlaceId\" IS NOT NULL");
            e.HasIndex(c => c.Location).HasMethod("GIST");
        });

        model.Entity<Trip>(e =>
        {
            e.ToTable("trips");
            e.HasKey(t => t.Id);
            e.Property(t => t.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(t => t.CreatedAt).HasDefaultValueSql("now()");
            e.Property(t => t.UpdatedAt).HasDefaultValueSql("now()");
            e.HasIndex(t => t.ClientUuid).IsUnique();
            e.HasIndex(t => new { t.CoupleId, t.StartDate });
            e.HasOne(t => t.Couple).WithMany(c => c.Trips).HasForeignKey(t => t.CoupleId);
            e.HasOne(t => t.City).WithMany(c => c.Trips).HasForeignKey(t => t.CityId);
            e.HasOne(t => t.CoverPhoto).WithMany().HasForeignKey(t => t.CoverPhotoId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        model.Entity<Photo>(e =>
        {
            e.ToTable("photos");
            e.HasKey(p => p.Id);
            e.Property(p => p.Id).HasDefaultValueSql("gen_random_uuid()");
            e.Property(p => p.CreatedAt).HasDefaultValueSql("now()");
            e.HasOne(p => p.Trip).WithMany(t => t.Photos).HasForeignKey(p => p.TripId);
        });
    }
}
