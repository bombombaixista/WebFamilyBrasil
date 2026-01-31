using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Usa a MESMA connection string do appsettings.json
        optionsBuilder.UseMySql(
            "Server=interchange.proxy.rlwy.net;Port=51120;Database=railway;User=root;Password=engjpriJMSgnwpdFAtrQVyIxSHeZVUBe;",
            new MySqlServerVersion(new Version(8, 0, 36))
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}
