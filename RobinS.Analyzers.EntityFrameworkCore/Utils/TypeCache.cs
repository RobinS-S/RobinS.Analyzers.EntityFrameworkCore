using Microsoft.CodeAnalysis;
using RobinS.Analyzers.EntityFrameworkCore.Core;
using System.Linq;

namespace RobinS.Analyzers.EntityFrameworkCore.Utils;

/// <summary>
/// Caches type symbols used in the analyzers.
/// </summary>
public sealed class TypeCache
{
    /// <summary>
    /// Gets the DbContext type symbol.
    /// </summary>
    public INamedTypeSymbol? DbContextType { get; }

    /// <summary>
    /// Gets the DbSet type symbol.
    /// </summary>
    public INamedTypeSymbol? DbSetType { get; }

    /// <summary>
    /// Gets a value indicating whether EF Core is available in the compilation.
    /// </summary>
    public bool IsEfCoreAvailable { get; }

    public TypeCache(Compilation compilation)
    {
        DbContextType = compilation.GetTypeByMetadataName(AnalyzerConstants.EfCore.DbContextTypeName);
        DbSetType = compilation.GetTypeByMetadataName(AnalyzerConstants.EfCore.DbSetTypeName);

        IsEfCoreAvailable = DbContextType != null ||
            compilation.References.Any(r => r.Display?.Contains(AnalyzerConstants.EfCore.AssemblyPrefix) == true);
    }
}
