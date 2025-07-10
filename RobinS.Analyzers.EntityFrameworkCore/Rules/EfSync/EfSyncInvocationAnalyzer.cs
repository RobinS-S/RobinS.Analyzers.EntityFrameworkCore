using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;
using RobinS.Analyzers.EntityFrameworkCore.Core;
using RobinS.Analyzers.EntityFrameworkCore.Utils;
using System.Linq;

namespace RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EfSyncInvocationAnalyzer : BaseAnalyzer
{
    protected override DiagnosticDescriptor Rule => DiagnosticRules.EfSync;

    protected override OperationKind[] OperationKinds => [OperationKind.Invocation];

    protected override void AnalyzeOperation(OperationAnalysisContext ctx)
    {
        if (ctx.Operation is not IInvocationOperation invocation)
            return;

        if (!SyncMethods.IsSyncMethod(invocation.TargetMethod.Name))
            return;

        var typeCache = TypeHelpers.GetTypeCache(ctx.Compilation);
        if (!typeCache.IsEfCoreAvailable)
            return;

        if (!IsInAsyncContext(invocation))
            return;

        var method = invocation.TargetMethod;

        if (IsSynchronousEfCoreCall(invocation, method, typeCache, ctx.Compilation))
        {
            ReportDiagnostic(ctx, invocation, method.Name);
        }
    }

    private static bool IsSynchronousEfCoreCall(
        IInvocationOperation invocation,
        IMethodSymbol method,
        TypeCache typeCache,
        Compilation compilation)
    {
        if (method.ContainingAssembly?.Name.StartsWith(AnalyzerConstants.EfCore.AssemblyPrefix) == true)
            return true;

        if (typeCache.DbContextType != null && invocation.Instance?.Type != null)
        {
            if (TypeHelpers.InheritsFrom(invocation.Instance.Type, typeCache.DbContextType))
                return SyncMethods.IsSyncMethod(method.Name);

            if (invocation.Instance.Type.TypeKind == TypeKind.Interface && SyncMethods.IsSyncMethod(method.Name))
            {
                return typeCache.DbContextType.GetMembers(method.Name)
                    .OfType<IMethodSymbol>()
                    .Any(m => m.Parameters.Length == method.Parameters.Length);
            }
        }

        if (method.IsExtensionMethod &&
            invocation.Arguments.Length > 0 &&
            TypeHelpers.IsDbSetOperation(invocation.Arguments[0].Value, compilation))
        {
            return SyncMethods.IsSyncMethod(method.Name);
        }

        return false;
    }

    private static void ReportDiagnostic(
        OperationAnalysisContext context,
        IOperation invocation,
        string methodName)
    {
        var diagnostic = Diagnostic.Create(
            DiagnosticRules.EfSync,
            invocation.Syntax.GetLocation(),
            methodName);

        context.ReportDiagnostic(diagnostic);
    }

    private static bool IsInAsyncContext(IOperation invocation)
    {
        var node = invocation.Syntax;

        var methodDecl = node.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
        if (methodDecl != null)
        {
            if (methodDecl.Modifiers.Any(m => m.ValueText == "async"))
                return true;

            var returnTypeName = methodDecl.ReturnType.ToString();
            if (returnTypeName.Contains("Task") || returnTypeName.Contains("ValueTask"))
                return true;
        }

        var localFunction = node.AncestorsAndSelf().OfType<LocalFunctionStatementSyntax>().FirstOrDefault();
        if (localFunction != null)
        {
            if (localFunction.Modifiers.Any(m => m.ValueText == "async"))
                return true;

            var returnTypeName = localFunction.ReturnType.ToString();
            if (returnTypeName.Contains("Task") || returnTypeName.Contains("ValueTask"))
                return true;
        }

        var lambda = node.AncestorsAndSelf().OfType<ParenthesizedLambdaExpressionSyntax>().FirstOrDefault();
        if (lambda?.AsyncKeyword.ValueText == "async")
            return true;

        var simpleLambda = node.AncestorsAndSelf().OfType<SimpleLambdaExpressionSyntax>().FirstOrDefault();
        if (simpleLambda?.AsyncKeyword.ValueText == "async")
            return true;

        var anonymousMethod = node.AncestorsAndSelf().OfType<AnonymousMethodExpressionSyntax>().FirstOrDefault();
        return anonymousMethod?.AsyncKeyword.ValueText == "async";
    }
}
