using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace RobinS.Analyzers.EntityFrameworkCore.Core;

/// <summary>
/// Base class for all analyzers in the library.
/// </summary>
public abstract class BaseAnalyzer : DiagnosticAnalyzer
{
    /// <summary>
    /// Gets the descriptor for this analyzer's diagnostic rule.
    /// </summary>
    protected abstract DiagnosticDescriptor Rule { get; }

    /// <summary>
    /// Gets the kinds of operations this analyzer should analyze.
    /// </summary>
    protected abstract OperationKind[] OperationKinds { get; }

    /// <summary>
    /// Returns a collection containing this analyzer's diagnostic descriptor.
    /// </summary>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(Rule);

    /// <summary>
    /// Initializes this analyzer with the specified analysis context.
    /// </summary>
    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        if (OperationKinds.Length > 0)
        {
            context.RegisterOperationAction(AnalyzeOperation, OperationKinds);
        }

        InitializeInternal(context);
    }

    /// <summary>
    /// Analyzes the specified operation.
    /// </summary>
    protected abstract void AnalyzeOperation(OperationAnalysisContext context);

    /// <summary>
    /// Provides additional initialization for derived analyzers.
    /// </summary>
    protected virtual void InitializeInternal(AnalysisContext context)
    {
    }
}
