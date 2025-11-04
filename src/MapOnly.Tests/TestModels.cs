namespace MapOnly.Tests;

// Test models for mapping
public class User
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedDate { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsActive { get; set; }
}

public class UserViewModel
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? Email { get; set; }
}

public class UserDetailsViewModel
{
    public int Id { get; set; }
    public string? FullName { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

public class Product
{
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

public class ProductDto
{
    public int ProductId { get; set; }
    public string? Name { get; set; }
    public decimal Price { get; set; }
}

public class EntityWithIgnoredProperty
{
    public int Id { get; set; }
    public string? Name { get; set; }
    
    [MapAttribute(Ignored = true)]
    public string? IgnoredProperty { get; set; }
}

public class TargetEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? IgnoredProperty { get; set; }
}
