# RobinS.Analyzers.EntityFrameworkCore

A Roslyn analyzer package for Entity Framework Core that helps developers write efficient, maintainable, and robust data access code.

## Overview

This analyzer package provides a growing set of rules to improve Entity Framework Core code quality and performance. By integrating seamlessly with the development process, it offers real-time guidance and suggestions as you write code, helping to enforce best practices and avoid common pitfalls in EF Core applications.

## Features

- Detects synchronous EF Core materializers like `ToList()`, `ToArray()`, `First()`, `FirstOrDefault()`, `Single()`, `SingleOrDefault()`, `Any()`, `Count()`, `All()`, `Last()`, `LastOrDefault()`, `ElementAt()`, `ElementAtOrDefault()`, `Min()`, `Max()`, `Sum()`, `Average()`, `Aggregate()`, `Contains()`, `SequenceEqual()`, `LongCount()`, `Load()`
- Detects synchronous `DbContext.SaveChanges()` calls
- Works with interface-based DbContext patterns
- Only flags calls on `IQueryable<T>` types (from EF DbSets) to avoid false positives on in-memory collections
- Only flags issues in async contexts (methods marked with async keyword or returning Task/ValueTask)
- Provides clear warning messages suggesting async alternatives

## Usage

Add the analyzer to your project:

```xml
<PackageReference Include="RobinS.Analyzers.EntityFrameworkCore" Version="1.0.0" PrivateAssets="all" />
```

Or build from source and reference the output DLL.

## Diagnostic Rules

### EFASYNC001: Use async EF Core method

**Severity:** Warning

**Description:** Detects synchronous EF Core calls in asynchronous contexts that should use their async alternatives.

**Problem:** Synchronous database operations in async methods can lead to thread pool starvation and potential deadlocks, especially in ASP.NET applications.

**Solution:** Replace synchronous calls with their asynchronous counterparts.

#### Examples

The analyzer will flag these patterns when they appear in async methods:

```csharp
// In an async method:
async Task GetUsersAsync()
{
    // ❌ Problematic: Will trigger EFASYNC001 warnings
    var users = context.Users.ToList();      // Blocks the thread while querying
    var user = context.Users.First();        // Blocks the thread while querying
    var count = context.Users.Count();       // Blocks the thread while querying
    context.SaveChanges();                  // Blocks the thread while saving
}
```

##### Recommended Fixes

```csharp
// In an async method:
async Task GetUsersAsync()
{
    // ✅ Correct: Using proper async alternatives
    var users = await context.Users.ToListAsync();
    var user = await context.Users.FirstAsync();
    var count = await context.Users.CountAsync();
    await context.SaveChangesAsync();
}
```

The rule is designed to minimize false positives and will ignore these scenarios:

```csharp
// ✅ Sync methods in non-async contexts are allowed
void GetUsers()
{
    var users = context.Users.ToList(); // No warning in sync method
}

// ✅ In-memory collections won't trigger the analyzer (even in async methods)
async Task GetListAsync()
{
    var list = new List<int> { 1, 2, 3 };
    var result = list.ToList(); // In-memory collection, not EF
}

// ✅ Works with interfaces and inherited DbContexts
interface IMyContext : IDisposable
{
    DbSet<User> Users { get; }
    int SaveChanges();
}

// Will still detect sync calls through interfaces
async Task GetUsersFromInterfaceAsync()
{
    using IMyContext context = new MyDbContext();
    var users = context.Users.ToList(); // Will be detected
}
```

#### Detected Methods

The analyzer detects these synchronous methods when used in async contexts:
- Materializers: `ToList()`, `ToArray()`, `First()`, `FirstOrDefault()`, etc.
- Aggregators: `Count()`, `Sum()`, `Min()`, `Max()`, `Average()`, etc.
- Context operations: `SaveChanges()`, `Load()`

## Diagnostic Codes (so far)

- `EFASYNC001`: Use async EF Core method (Warning)

## Project Structure

- `RobinS.Analyzers.EntityFrameworkCore/` - The main analyzer project
  - `Core/` - Core analyzer functionality and constants
  - `Rules/` - Rule implementations
  - `Utils/` - Helper utilities
- `RobinS.Analyzers.EntityFrameworkCore.Tests/` - Unit tests for the analyzer

## Building

```bash
dotnet build
dotnet test
```

## Roadmap and Contributing

More rules are planned for future releases.

Contributions are welcome! If you have ideas for improvements or new rules:
- Open an issue to discuss your idea or improvement or just straightaway submit a PR with your changes
- Ensure tests are included for any new functionality

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
