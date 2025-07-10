using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace RobinS.Analyzers.EntityFrameworkCore.Utils;

/// <summary>
/// Helper methods for working with types and operations.
/// </summary>
public static class TypeHelpers
{
    /// <summary>
    /// Gets a cached set of type symbols for the compilation.
    /// </summary>
    public static TypeCache GetTypeCache(Compilation compilation) => new(compilation);

    /// <summary>
    /// Checks if a type inherits from or implements a base type or interface.
    /// </summary>
    public static bool InheritsFrom(ITypeSymbol type, ITypeSymbol baseType)
    {
        for (var current = type; current != null; current = current.BaseType)
        {
            if (SymbolEqualityComparer.Default.Equals(current, baseType))
                return true;
        }

        if (baseType.TypeKind == TypeKind.Interface)
        {
            foreach (var iface in type.AllInterfaces)
            {
                if (SymbolEqualityComparer.Default.Equals(iface, baseType))
                    return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Determines if an operation is related to a DbSet.
    /// </summary>
    public static bool IsDbSetOperation(IOperation? op, Compilation compilation)
    {
        var cache = GetTypeCache(compilation);
        var dbSetType = cache.DbSetType;

        if (dbSetType == null)
            return false;

        while (op != null)
        {
            if (IsDirectDbSetType(op, dbSetType))
                return true;

            if (IsDbContextSetMethod(op, cache.DbContextType))
                return true;

            op = GetParentOperation(op);
        }

        return false;
    }

    /// <summary>
    /// Checks if the operation has a DbSet type.
    /// </summary>
    public static bool IsDirectDbSetType(IOperation op, INamedTypeSymbol dbSetType)
    {
        return op.Type is INamedTypeSymbol { IsGenericType: true } namedType &&
               SymbolEqualityComparer.Default.Equals(namedType.ConstructedFrom, dbSetType);
    }

    /// <summary>
    /// Checks if the operation is a DbContext.Set call.
    /// </summary>
    public static bool IsDbContextSetMethod(IOperation op, INamedTypeSymbol? dbContextType)
    {
        if (dbContextType == null)
            return false;

        return op is IInvocationOperation { TargetMethod: { Name: "Set", IsGenericMethod: true } } invOp &&
               invOp.Instance?.Type != null &&
               InheritsFrom(invOp.Instance.Type, dbContextType);
    }

    /// <summary>
    /// Gets the parent operation in the operation tree.
    /// </summary>
    public static IOperation? GetParentOperation(IOperation op)
    {
        return op switch
        {
            IConversionOperation conversion => conversion.Operand,
            IMemberReferenceOperation member => member.Instance,
            IInvocationOperation invocation => invocation.Instance ??
                (invocation.Arguments.Length > 0 ? invocation.Arguments[0].Value : null),
            _ => null
        };
    }
}
