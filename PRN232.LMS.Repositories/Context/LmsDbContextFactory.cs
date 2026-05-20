using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace PRN232.LMS.Repositories.Context;

/// <summary>
/// Design-time factory used by EF Core CLI tools (migrations, scaffolding).
/// Not used at runtime.
/// </summary>
public class LmsDbContextFactory : IDesignTimeDbContextFactory<LmsDbContext>
{
    public LmsDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<LmsDbContext>();
        optionsBuilder.UseSqlServer(
            "Server=LAPTOP-IOT1D9JU;Database=LMS;User Id=sa;Password=1234567890;TrustServerCertificate=True;",
            sql => sql.MigrationsAssembly("PRN232.LMS.Repositories"));
        return new LmsDbContext(optionsBuilder.Options);
    }
}
