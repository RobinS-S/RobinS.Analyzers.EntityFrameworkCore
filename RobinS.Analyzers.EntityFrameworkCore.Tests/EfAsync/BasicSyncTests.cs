using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;
using Xunit;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests.EfAsync
{
    public class BasicSyncTests
    {
        [Fact]
        public async Task IgnoresSyncMethodsInSyncContext()
        {
            // Arrange
            const string source = @"
using Microsoft.EntityFrameworkCore;
using System.Linq;

class TestContext : DbContext
{
    public DbSet<Entity> Entities { get; set; }
}

class Entity { public int Id { get; set; } }

class Program
{
    static void Main()
    {
        using var context = new TestContext();
        var list = context.Entities.ToList();
        var first = context.Entities.First();
        var count = context.Entities.Count();
        context.SaveChanges();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);

            // Assert
            Assert.Empty(diagnostics);
        }

        [Fact]
        public async Task DetectsBasicSyncMethodsInAsyncContext()
        {
            // Arrange
            const string source = @"
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        var list = context.Entities.ToList();
        var first = context.Entities.First();
        var count = context.Entities.Count();
        context.SaveChanges();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);

            // Assert
            Assert.Equal(4, diagnostics.Length);
            Assert.All(diagnostics, d => Assert.Equal(DiagnosticRules.EfSync.Id, d.Id));
        }
    }
}
