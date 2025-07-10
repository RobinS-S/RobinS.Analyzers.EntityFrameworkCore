using Basic.Reference.Assemblies;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace RobinS.Analyzers.EntityFrameworkCore.Tests
{
    public static class TestHelper
    {
        private static readonly MetadataReference[] References =
        [
            .. Net80.References.All,
            MetadataReference.CreateFromFile(typeof(Microsoft.EntityFrameworkCore.DbContext).Assembly.Location),
            MetadataReference.CreateFromFile(typeof(Microsoft.EntityFrameworkCore.DbSet<>).Assembly.Location)
        ];

        public static async Task<Diagnostic[]> AnalyzeAsync(string source, DiagnosticAnalyzer analyzer, params string[] diagnosticIds)
        {
            var compilation = CSharpCompilation.Create(
                "TestAssembly",
                [CSharpSyntaxTree.ParseText(source)],
                References,
                new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var errors = compilation.GetDiagnostics().Where(d => d.Severity == DiagnosticSeverity.Error);
            if (errors.Any())
                throw new System.Exception($"Compilation failed: {string.Join(", ", errors)}");

            var analyzers = compilation.WithAnalyzers([analyzer]);
            var diagnostics = await analyzers.GetAnalyzerDiagnosticsAsync();

            return diagnosticIds.Length > 0
                ? [.. diagnostics.Where(d => diagnosticIds.Contains(d.Id))]
                : [.. diagnostics];
        }
    }
}
