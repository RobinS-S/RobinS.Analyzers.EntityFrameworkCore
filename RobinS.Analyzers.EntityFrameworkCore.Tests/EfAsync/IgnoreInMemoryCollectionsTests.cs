using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;
using Xunit;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests.EfAsync
{
    public class IgnoreInMemoryCollectionsTests
    {
        [Fact]
        public async Task IgnoresInMemoryCollectionsInAsyncContext()
        {
            // Arrange
            const string source = @"
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main()
    {
        var list = new List<int> { 1, 2, 3 };
        var result = list.ToList();
        var queryable = list.AsQueryable();
        var first = queryable.First();
    }
}";
            // Act
            var diagnostics = await TestHelper.AnalyzeAsync(source, new EfSyncInvocationAnalyzer(), DiagnosticRules.EfSync.Id);
            
            // Assert
            Assert.Empty(diagnostics);
        }
    }
}
