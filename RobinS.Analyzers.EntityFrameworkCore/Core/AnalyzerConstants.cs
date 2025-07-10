namespace RobinS.Analyzers.EntityFrameworkCore.Core;

/// <summary>
/// Constants used across the analyzers.
/// </summary>
public static class AnalyzerConstants
{
    /// <summary>
    /// Category names for various analyzer rules.
    /// </summary>
    public static class Categories
    {
        /// <summary>
        /// Rules related to usage patterns.
        /// </summary>
        public const string Usage = "Usage";

        /// <summary>
        /// Rules related to performance concerns.
        /// </summary>
        public const string Performance = "Performance";

        /// <summary>
        /// Rules related to code quality.
        /// </summary>
        public const string CodeQuality = "Code Quality";
    }

    /// <summary>
    /// Constants related to EF Core.
    /// </summary>
    public static class EfCore
    {
        public const string AssemblyPrefix = "Microsoft.EntityFrameworkCore";
        public const string DbContextTypeName = "Microsoft.EntityFrameworkCore.DbContext";
        public const string DbSetTypeName = "Microsoft.EntityFrameworkCore.DbSet`1";
    }

    /// <summary>
    /// URLs for help documentation.
    /// </summary>
    public static class HelpUrls
    {
        public const string EfCoreEfficientQuerying =
            "https://learn.microsoft.com/en-us/ef/core/performance/efficient-querying#asynchronous-programming";

        public const string EfCoreEfficientUpdating =
            "https://learn.microsoft.com/en-us/ef/core/performance/efficient-updating";
    }
}
