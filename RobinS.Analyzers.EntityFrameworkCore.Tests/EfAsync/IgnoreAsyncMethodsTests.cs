using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;
using Xunit;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests.EfAsync
{
    public class IgnoreAsyncMethodsTests
    {
        [Fact]
        public async Task IgnoresAsyncMethods()
        {
            // Arrange
            const string source = @"
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

class TestContext : DbContext
{
    public DbSet<Entity> Entities { get; set; }
}

class Entity { public int Id { get; set; } }

class Program
{
    static async Task Main()
    {
        using var context = new TestContext();
        var list = await context.Entities.ToListAsync();
        await context.SaveChangesAsync();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);
            
            // Assert
            Assert.Empty(diagnostics);
        }
    }
}
