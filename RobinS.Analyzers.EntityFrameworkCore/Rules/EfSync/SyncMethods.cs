using System.Collections.Immutable;

namespace RobinS.Analyzers.EntityFrameworkCore.Rules.EfSync;

/// <summary>
/// Contains the list of synchronous methods that should be avoided in EF Core.
/// </summary>
public static class SyncMethods
{
    /// <summary>
    /// The collection of method names that have async alternatives and should be avoided in EF Core.
    /// </summary>
    public static readonly ImmutableHashSet<string> Names = ImmutableHashSet.Create(
        "ToList", "ToArray", "First", "FirstOrDefault", "Single", "SingleOrDefault",
        "Any", "Count", "All", "Last", "LastOrDefault", "ElementAt", "ElementAtOrDefault",
        "Min", "Max", "Sum", "Average", "Aggregate",
        "Contains", "SequenceEqual", "LongCount",
        "Load", "SaveChanges");

    /// <summary>
    /// Determines if a method name is one that should use an async alternative.
    /// </summary>
    public static bool IsSyncMethod(string methodName) => Names.Contains(methodName);
}
