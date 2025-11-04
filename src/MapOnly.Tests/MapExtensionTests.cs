using MapOnly.Interface;

namespace MapOnly.Tests;

public class MapExtensionTests : IDisposable
{
    // Track mappings created in tests for cleanup
    private readonly List<Action> _cleanupActions = new();

    public void Dispose()
    {
        // Clean up all mappings created during tests
        foreach (var cleanup in _cleanupActions)
        {
            cleanup();
        }
    }

    private IMapObject<TSource, TDest> CreateMapping<TSource, TDest>()
    {
        var mapping = MapExtension.Create<TSource, TDest>();
        _cleanupActions.Add(() =>
        {
            try
            {
                MapExtension.Create<TSource, TDest>().Remove();
            }
            catch
            {
                // Ignore if already removed
            }
        });
        return mapping;
    }

    [Fact]
    public void Map_SimplePropertiesWithSameNames_ShouldMapCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com"
        };

        // Act
        var viewModel = user.Map(new UserViewModel());

        // Assert
        Assert.Equal(1, viewModel.Id);
        Assert.Equal("John", viewModel.FirstName);
        Assert.Equal("Doe", viewModel.LastName);
        Assert.Equal("john.doe@example.com", viewModel.Email);
    }

    [Fact]
    public void Map_WithoutDestinationInstance_ShouldCreateAndMapCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane.smith@example.com"
        };

        // Act
        var viewModel = user.Map<User, UserViewModel>();

        // Assert
        Assert.NotNull(viewModel);
        Assert.Equal(2, viewModel.Id);
        Assert.Equal("Jane", viewModel.FirstName);
        Assert.Equal("Smith", viewModel.LastName);
        Assert.Equal("jane.smith@example.com", viewModel.Email);
    }

    [Fact]
    public void Map_BooleanProperty_ShouldMapCorrectly()
    {
        // Arrange
        var user = new User
        {
            Id = 1,
            IsActive = true
        };

        // Act
        var details = user.Map(new UserDetailsViewModel());

        // Assert
        Assert.True(details.IsActive);
    }

    [Fact]
    public void Map_WithCustomMapping_ShouldMapCorrectly()
    {
        // Arrange
        CreateMapping<User, UserDetailsViewModel>()
            .Add(u => u.FirstName, d => d.FullName);

        var user = new User
        {
            Id = 1,
            FirstName = "John",
            Email = "john@example.com",
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Act
        var details = user.Map(new UserDetailsViewModel());

        // Assert
        Assert.Equal(1, details.Id);
        Assert.Equal("John", details.FullName);
        Assert.Equal("john@example.com", details.Email);
        Assert.True(details.IsActive);
    }

    [Fact]
    public void Map_WithIgnoredProperties_ShouldNotMapIgnoredProperty()
    {
        // Arrange
        CreateMapping<User, UserViewModel>()
            .Ignore(vm => vm.DisplayName);

        var user = new User
        {
            Id = 2,
            FirstName = "Jane",
            LastName = "Smith",
            Email = "jane@example.com"
        };

        // Act
        var viewModel = user.Map(new UserViewModel());

        // Assert
        Assert.Equal(2, viewModel.Id);
        Assert.Equal("Jane", viewModel.FirstName);
        Assert.Null(viewModel.DisplayName); // Should not be mapped
    }

    [Fact]
    public void Map_WithConstantValue_ShouldSetConstantValue()
    {
        // Arrange
        CreateMapping<Product, ProductDto>()
            .Add(p => p.Price, 99.99m);

        var product = new Product
        {
            ProductId = 1,
            Name = "Test Product",
            Price = 50.00m
        };

        // Act
        var dto = product.Map(new ProductDto());

        // Assert
        Assert.Equal(1, dto.ProductId);
        Assert.Equal("Test Product", dto.Name);
        Assert.Equal(99.99m, dto.Price); // Should be the constant value
    }

    [Fact]
    public void Map_WithAttributeIgnored_ShouldNotMapProperty()
    {
        // Arrange
        var source = new EntityWithIgnoredProperty
        {
            Id = 1,
            Name = "Test",
            IgnoredProperty = "Should be ignored"
        };

        // Act
        var target = source.Map(new TargetEntity());

        // Assert
        Assert.Equal(1, target.Id);
        Assert.Equal("Test", target.Name);
        Assert.Null(target.IgnoredProperty); // Should not be mapped due to attribute
    }

    [Fact]
    public void Map_WithNullSource_ShouldThrowArgumentNullException()
    {
        // Arrange
        User? nullUser = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            nullUser!.Map(new UserViewModel()));
    }

    [Fact]
    public void Map_WithNullDestination_ShouldThrowArgumentNullException()
    {
        // Arrange
        var user = new User { Id = 1 };

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => 
            user.Map<User, UserViewModel>(null!));
    }

    [Fact]
    public void Create_ShouldReturnMapObject()
    {
        // Act
        var mapObject = MapExtension.Create<User, UserViewModel>();

        // Assert
        Assert.NotNull(mapObject);
        Assert.Equal(typeof(User), mapObject.Source);
        Assert.Equal(typeof(UserViewModel), mapObject.Destination);
    }

    [Fact]
    public void Create_CalledTwice_ShouldReturnSameMapping()
    {
        // Act
        var mapObject1 = MapExtension.Create<User, UserViewModel>();
        var mapObject2 = MapExtension.Create<User, UserViewModel>();

        // Assert
        Assert.Equal(mapObject1.MappingSettingId, mapObject2.MappingSettingId);
    }

    [Fact]
    public void Add_WithoutCreate_ShouldThrowException()
    {
        // Arrange
        var user = new User { Id = 1 };
        var newMapObject = MapExtension.Create<User, UserDetailsViewModel>();

        // Remove the mapping
        newMapObject.Remove();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            MapExtension.Create<User, UserDetailsViewModel>()
                .Remove()
                .Add(u => u.FirstName, d => d.FullName));
    }

    [Fact]
    public void Ignore_WithNullExpression_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mapObject = MapExtension.Create<User, UserViewModel>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            mapObject.Ignore(null!));
    }

    [Fact]
    public void Add_WithNullSourceExpression_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mapObject = MapExtension.Create<User, UserViewModel>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            mapObject.Add(null!, vm => vm.DisplayName));
    }

    [Fact]
    public void Add_WithNullDestinationExpression_ShouldThrowArgumentNullException()
    {
        // Arrange
        var mapObject = MapExtension.Create<User, UserViewModel>();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() =>
            mapObject.Add(u => u.FirstName, null!));
    }

    [Fact]
    public void Map_MultipleProperties_ShouldMapAll()
    {
        // Arrange
        var product = new Product
        {
            ProductId = 100,
            Name = "Laptop",
            Price = 999.99m,
            Stock = 50
        };

        // Act
        var dto = product.Map(new ProductDto());

        // Assert
        Assert.Equal(100, dto.ProductId);
        Assert.Equal("Laptop", dto.Name);
        Assert.Equal(999.99m, dto.Price);
    }

    [Fact]
    public void Map_ChainedMappingConfiguration_ShouldWork()
    {
        // Arrange
        CreateMapping<User, UserDetailsViewModel>()
            .Add(u => u.FirstName, d => d.FullName)
            .Ignore(d => d.CreatedDate)
            .Ignore(d => d.IsActive);

        var user = new User
        {
            Id = 5,
            FirstName = "Alice",
            Email = "alice@example.com",
            CreatedDate = DateTime.Now,
            IsActive = true
        };

        // Act
        var details = user.Map(new UserDetailsViewModel());

        // Assert
        Assert.Equal(5, details.Id);
        Assert.Equal("Alice", details.FullName);
        Assert.Equal("alice@example.com", details.Email);
        Assert.Equal(default(DateTime), details.CreatedDate); // Should be default
        Assert.False(details.IsActive); // Should be default
    }

    [Fact]
    public void Remove_ShouldRemoveMappingConfiguration()
    {
        // Arrange
        var mapObject = CreateMapping<Product, ProductDto>()
            .Add(p => p.Price, 100m);

        // Act
        mapObject.Remove();

        var product = new Product
        {
            ProductId = 1,
            Name = "Test",
            Price = 50m
        };

        var dto = product.Map(new ProductDto());

        // Assert
        Assert.Equal(50m, dto.Price); // Should use original value, not constant
    }
}
