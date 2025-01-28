using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.Api.Database;

internal class ExpertBridgeDbContext(DbContextOptions<ExpertBridgeDbContext> options) : DbContext(options)
{
    /// <summary>
    ///     The OnModelCreating method that is called when the model is being created.
    /// </summary>
    /// <param name="modelBuilder">
    ///     The model builder that is used to build the model.
    /// </param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
