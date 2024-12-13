using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using PhotoChef.Infrastructure.Persistence;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        // Cambia la cadena de conexión según sea necesario
        optionsBuilder.UseSqlite("Data Source=PhotoChef.db");

        return new AppDbContext(optionsBuilder.Options);
    }
}
