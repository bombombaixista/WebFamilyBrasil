using Kanban.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

public class KanbanContextFactory : IDesignTimeDbContextFactory<KanbanContext>
{
    public KanbanContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>();

        optionsBuilder.UseNpgsql(
            "Host=turntable.proxy.rlwy.net;Port=13567;Database=railway;Username=postgres;Password=DgKcTSVsyNWWQMXXnDfCrFsuFrtZfKNY;SSL Mode=Require;Trust Server Certificate=true"
        );

        return new KanbanContext(optionsBuilder.Options);
    }
}
