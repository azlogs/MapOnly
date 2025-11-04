
# Welcome to MapOnly Documentation

## Introduction

**MapOnly** is a simple and lightweight .NET library for object-to-object mapping. If you need a straightforward library focused solely on mapping without extra complexity, MapOnly is the perfect choice for you.

## Requirements

- .NET Standard 2.0+
- .NET Standard 2.1+
- .NET 6.0+
- .NET 8.0+
- .NET Framework 4.6.1+

## Installation

```bash
# Using .NET CLI
dotnet add package MapOnly

# Using Package Manager Console
Install-Package MapOnly

# Or search "MapOnly" in NuGet Package Manager
```

## Features

- ✨ Simple and intuitive API
- 🚀 Lightweight with minimal dependencies
- 🎯 Automatic property mapping by name
- 🔧 Custom property mappings
- 🚫 Property ignore functionality
- 💎 Support for constant values
- 🏷️ Attribute-based configuration
- ✅ Fully tested and documented

## Quick Start

### Basic Usage (No Configuration Required)

For simple object mapping where properties have the same names:

```csharp
using MapOnly;

// Map using the new instance creation overload (recommended)
var userDto = user.Map<User, UserDto>();

// Or if you need to map to an existing instance
var existingDto = new UserDto();
user.Map(existingDto);
```

### With Configuration

For custom mappings, configure once at application startup:

```csharp
// At application startup
MapExtension.Create<User, UserDto>()
    .Add(u => u.FullName, dto => dto.DisplayName)
    .Ignore(dto => dto.InternalField);

// Then use throughout your application
var userDto = user.Map<User, UserDto>();
```

## Methods

### 1. Create

This method creates a mapping configuration between two classes. If the classes have the same property names, automatic mapping will work without configuration.

#### Syntax
```csharp
using MapOnly;

MapExtension.Create<SourceClass, DestinationClass>()
```

#### Example
```csharp
MapExtension.Create<UserViewModel, User>()
```

### 2. Add

For properties with different names or when you need to set constant values, use the Add method.

#### Syntax
```csharp
using MapOnly;

// Map properties with different names
MapExtension.Create<SourceClass, DestinationClass>()
    .Add(source => source.Property1, dest => dest.Property2);

// Set a constant value
MapExtension.Create<SourceClass, DestinationClass>()
    .Add(dest => dest.Property1, "ConstantValue");
```

#### Example
```csharp
MapExtension.Create<UserViewModel, User>()
    .Add(v => v.DisplayName, u => u.FullName)
    .Add(u => u.CreatedBy, "Admin");
```

### 3. Ignore

Use this method to exclude properties from mapping.

#### Syntax
```csharp
using MapOnly;

MapExtension.Create<SourceClass, DestinationClass>()
    .Ignore(dest => dest.Property1);
```

#### Example
```csharp
MapExtension.Create<UserViewModel, User>()
    .Add(x => x.Birthday, a => a.UpdatedDate)
    .Ignore(u => u.CreatedDate)
    .Ignore(u => u.CreatedUser)
    .Ignore(u => u.IsActive)
    .Ignore(u => u.UpdatedDate)
    .Ignore(u => u.UpdatedUser);
```

### 4. Map

Converts one object to another following configured or automatic mappings.

#### Syntax
```csharp
// Recommended: Automatically create destination instance
var destination = source.Map<SourceClass, DestinationClass>();

// Alternative: Map to existing instance (useful when updating objects)
var destination = new DestinationClass();
source.Map(destination);
```

#### Example
```csharp
// Simple mapping - automatically creates UserViewModel instance
var viewModel = user.Map<User, UserViewModel>();

// Map collections
var userViewModels = users
    .Select(u => u.Map<User, UserViewModel>())
    .ToList();

// Map to existing instance (e.g., updating an entity)
var existingUser = GetUserFromDatabase(id);
userViewModel.Map(existingUser);
SaveChanges();
```

## Advanced: Attribute-Based Mapping

You can also use attributes to control mapping behavior:

```csharp
public class MyClass
{
    public int Id { get; set; }
    
    [MapAttribute(Ignored = true)]
    public string InternalProperty { get; set; }
}
```

## Configuration

Configure your mappings once at application startup. MapOnly uses a static configuration that persists throughout your application's lifetime.

### ASP.NET Core / Modern .NET

**Program.cs or Startup.cs:**
```csharp
// Program.cs (.NET 6+)
var builder = WebApplication.CreateBuilder(args);

// Configure MapOnly mappings
ConfigureMappings();

var app = builder.Build();
app.Run();

static void ConfigureMappings()
{
    MapExtension.Create<User, UserDto>()
        .Add(u => u.FullName, dto => dto.DisplayName)
        .Ignore(dto => dto.CreatedDate);

    MapExtension.Create<Product, ProductDto>()
        .Ignore(dto => dto.InternalId);
}
```

**Or create a dedicated configuration class:**
```csharp
// MappingConfiguration.cs
public static class MappingConfiguration
{
    public static void Configure()
    {
        ConfigureUserMappings();
        ConfigureProductMappings();
    }

    private static void ConfigureUserMappings()
    {
        MapExtension.Create<User, UserDto>()
            .Add(u => u.FullName, dto => dto.DisplayName)
            .Ignore(dto => dto.CreatedDate);
    }

    private static void ConfigureProductMappings()
    {
        MapExtension.Create<Product, ProductDto>()
            .Ignore(dto => dto.InternalId);
    }
}

// Program.cs
MappingConfiguration.Configure();
```

### ASP.NET MVC / Web API (.NET Framework)

**Global.asax.cs:**
```csharp
protected void Application_Start()
{
    // Other configurations...
    AreaRegistration.RegisterAllAreas();
    RouteConfig.RegisterRoutes(RouteTable.Routes);
    
    // Configure MapOnly
    MappingConfiguration.Configure();
}
```

### Console / Desktop Applications

**Program.cs:**
```csharp
static class Program
{
    [STAThread]
    static void Main()
    {
        // Configure mappings at startup
        MappingConfiguration.Configure();

        // Rest of your application
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MainForm());
    }
}
```

### Best Practices

1. **Configure Once, Use Everywhere**: Set up all mappings at application startup
2. **Organize by Domain**: Group related mappings together in separate methods
3. **Keep It Simple**: Only configure mappings for properties that differ between source and destination
4. **Use the Automatic Creation Overload**: Prefer `source.Map<TSource, TDest>()` over `source.Map(new TDest())`

```csharp
// ✅ Recommended
var dto = user.Map<User, UserDto>();

// ❌ Less efficient (creates instance before calling Map)
var dto = user.Map(new UserDto());

// ✅ Good - when you need to update an existing instance
var existingEntity = repository.Get(id);
dto.Map(existingEntity);
repository.Update(existingEntity);
```

## Change Log

### Version 0.3.0 (Latest)
- 🎯 Multi-target support: .NET Standard 2.0/2.1, .NET 6.0/8.0
- ✨ Nullable reference types enabled
- 🐛 Bug fixes: Better null handling, skip missing properties
- 📚 Improved documentation and error messages
- ✅ Comprehensive unit test coverage
- 🔒 Security improvements

### Version 0.2.1
- Initial stable release

## License

MIT License - See LICENSE.txt for details

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.
