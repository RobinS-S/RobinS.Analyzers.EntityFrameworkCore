using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;
using Xunit;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests.EfAsync
{
    public class InterfacedDbContextTests
    {
        [Fact]
        public async Task DetectsContextWithInterfaceInAsyncContext()
        {
            // Arrange
            const string source = @"
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

interface ITestContext : IDisposable
{
    DbSet<Entity> Entities { get; set; }
    int SaveChanges();
}

class TestContext : DbContext, ITestContext
{
    public DbSet<Entity> Entities { get; set; }
}

class Entity { public int Id { get; set; } }

class Program
{
    static async Task Main()
    {
        using ITestContext context = new TestContext();
        var list = context.Entities.ToList();
        context.SaveChanges();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);

            // Assert
            Assert.Equal(2, diagnostics.Length);
            Assert.All(diagnostics, d => Assert.Equal(DiagnosticRules.EfSync.Id, d.Id));
            Assert.Contains(diagnostics, d => d.GetMessage().Contains("ToList"));
            Assert.Contains(diagnostics, d => d.GetMessage().Contains("SaveChanges"));
        }
    }
}
