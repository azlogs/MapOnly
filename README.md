
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
// destination is an instance of DestinationClass
// source is an instance of SourceClass
var destination = source.Map(new DestinationClass());
```

#### Example
```csharp
var user = userViewModel.Map(new User());
var viewModel = user.Map(new UserDetailsViewModel());
var userListing = users
    .Select(user => user.Map(new UserDetailsViewModel()))
    .ToList();
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

### Web Applications
```csharp
// Create a MapOnlySetting class in App_Start/MapOnlySetting.cs 
public static class MapOnlySetting
{
    public static void Register()
    {
        MapExtension.Create<A, A_Display>()
            .Ignore(x => x.DisplayName)
            .Add(source => source.Status, destination => destination.ProgressStatus);
    }
}

// Call Register method in Application_Start
protected void Application_Start() 
{  
    MapOnlySetting.Register(); 
}
```

### Console/Desktop Applications
```csharp
// Add configuration in the Main method in Program.cs
static class Program
{
    [STAThread]
    static void Main()
    {
        // Configure mappings here
        MapOnlyConfig.Register();

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new fmMain());
    } 
}
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
