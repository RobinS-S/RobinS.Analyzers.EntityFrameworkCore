using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;
using Xunit;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests.EfAsync
{
    public class IgnoreNonEfCodeTests
    {
        [Fact]
        public async Task IgnoresCustomOrmInAsyncContext()
        {
            // Arrange
            const string source = @"
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomORM
{
    class DbContext { public void SaveChanges() { } }
    class DbSet<T> : List<T> { }
}

class TestContext : CustomORM.DbContext
{
    public CustomORM.DbSet<Entity> Entities { get; set; } = new();
}

class Entity { public int Id { get; set; } }

class Program
{
    static async Task Main()
    {
        var context = new TestContext();
        var list = context.Entities.ToList();
        context.SaveChanges();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);
            
            // Assert
            Assert.Empty(diagnostics);
        }

        [Fact]
        public async Task IgnoresSimpleCustomContextInAsyncContext()
        {
            // Arrange
            const string source = @"
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class CustomContext
{
    public List<Entity> Entities { get; set; } = new();
    public void SaveChanges() { }
}

class Entity { public int Id { get; set; } }

class Program
{
    static async Task Main()
    {
        var context = new CustomContext();
        var list = context.Entities.ToList();
        context.SaveChanges();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);
            
            // Assert
            Assert.Empty(diagnostics);
        }
    }
}
