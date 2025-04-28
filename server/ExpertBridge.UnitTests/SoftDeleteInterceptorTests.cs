using ExpertBridge.Core.Entities;
using ExpertBridge.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace ExpertBridge.UnitTests;

public class SoftDeleteInterceptorTests
{
    [Fact]
    public async Task SavingChanges_WhenEntityIsDeleted_ShouldSoftDelete()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .AddInterceptors(new SoftDeleteInterceptor())
            .Options;

        using var context = new TestDbContext(options);
        var entity = new TestEntity { Id = 1 };

        context.TestEntities.Add(entity);
        await context.SaveChangesAsync();

        // Act
        context.TestEntities.Remove(entity);
        await context.SaveChangesAsync();

        // Assert
        var softDeletedEntity = await context.TestEntities.FirstOrDefaultAsync();
        Assert.NotNull(softDeletedEntity);
        Assert.True(softDeletedEntity.IsDeleted);
        Assert.NotNull(softDeletedEntity.DeletedAt);
        Assert.True(softDeletedEntity.DeletedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void SavingChanges_WhenEntityIsDeleted_ShouldSoftDelete_Sync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase("TestDb_" + Guid.NewGuid())
            .AddInterceptors(new SoftDeleteInterceptor())
            .Options;

        using var context = new TestDbContext(options);
        var entity = new TestEntity { Id = 1 };

        context.TestEntities.Add(entity);
        context.SaveChanges();

        // Act
        context.TestEntities.Remove(entity);
        context.SaveChanges();

        // Assert
        var softDeletedEntity = context.TestEntities.FirstOrDefault();
        Assert.NotNull(softDeletedEntity);
        Assert.True(softDeletedEntity.IsDeleted);
        Assert.NotNull(softDeletedEntity.DeletedAt);
        Assert.True(softDeletedEntity.DeletedAt <= DateTime.UtcNow);
    }

    private class TestEntity : ISoftDeletable
    {
        public int Id { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
    }

    private class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> options)
            : base(options)
        {
        }

        public DbSet<TestEntity> TestEntities { get; } = null!;
    }
}
