using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;
using Xunit;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests.EfAsync
{
    public class ContextSetTests
    {
        [Fact]
        public async Task DetectsContextSetMethodInAsyncContext()
        {
            // Arrange
            const string source = @"
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

class TestContext : DbContext { }
class Entity { public int Id { get; set; } }

class Program
{
    static async Task Main()
    {
        using var context = new TestContext();
        var list = context.Set<Entity>().ToList();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);
            
            // Assert
            Assert.Single(diagnostics);
            Assert.Contains("ToList", diagnostics[0].GetMessage());
        }
    }
}
