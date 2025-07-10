# RobinS.Analyzers.EntityFrameworkCore

A Roslyn analyzer that detects synchronous Entity Framework Core calls in async contexts and suggests async alternatives to prevent thread pool starvation and deadlocks.

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

## Examples

The analyzer will flag these patterns when they appear in async methods:

```csharp
// In an async method:
async Task GetUsersAsync()
{
    // ❌ Will trigger EFASYNC001
    var users = context.Users.ToList();
    var user = context.Users.First();
    var count = context.Users.Count();
    context.SaveChanges();

    // ✅ Recommended alternatives
    var users = await context.Users.ToListAsync();
    var user = await context.Users.FirstAsync();
    var count = await context.Users.CountAsync();
    await context.SaveChangesAsync();
}
```

But will ignore these (no false positives):

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

Contributions are welcome! If you have ideas for improvements or new features:
- Open an issue to discuss your proposal
- Submit a PR with your changes
- Ensure tests are included for any new functionality

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
