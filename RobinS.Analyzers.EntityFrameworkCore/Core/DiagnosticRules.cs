using Microsoft.CodeAnalysis;

namespace RobinS.Analyzers.EntityFrameworkCore.Core;

/// <summary>
/// Contains all diagnostic rule definitions for the analyzers.
/// </summary>
public static class DiagnosticRules
{
    public static readonly DiagnosticDescriptor EfSync = new(
        id: "EFASYNC001",
        title: "Use async EF Core method",
        messageFormat: "Use '{0}Async' instead of '{0}' to avoid blocking",
        category: AnalyzerConstants.Categories.Usage,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Synchronous EF Core calls can cause thread pool starvation and deadlocks.",
        helpLinkUri: AnalyzerConstants.HelpUrls.EfCoreEfficientQuerying);
}
