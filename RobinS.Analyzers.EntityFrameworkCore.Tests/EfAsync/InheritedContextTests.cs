using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;
using Xunit;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests.EfAsync
{
    public class InheritedContextTests
    {
        [Fact]
        public async Task DetectsInheritedDbContextInAsyncContext()
        {
            // Arrange
            const string source = @"
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

class BaseContext : DbContext
{
    public DbSet<Entity> Entities { get; set; }
}

class DerivedContext : BaseContext { }
class Entity { public int Id { get; set; } }

class Program
{
    static async Task Main()
    {
        using var context = new DerivedContext();
        var entity = context.Entities.First();
        context.SaveChanges();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);

            // Assert
            Assert.Equal(2, diagnostics.Length);
            Assert.All(diagnostics, d => Assert.Equal(DiagnosticRules.EfSync.Id, d.Id));
        }
    }
}
