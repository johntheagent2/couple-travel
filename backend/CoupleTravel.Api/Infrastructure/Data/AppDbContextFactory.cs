using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CoupleTravel.Api.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var opt = new DbContextOptionsBuilder<AppDbContext>();
        opt.UseNpgsql(
            "Host=localhost;Database=coupletravel;Username=coupletravel;Password=coupletravel",
            o => o.UseNetTopologySuite()
        );
        return new AppDbContext(opt.Options);
    }
}
