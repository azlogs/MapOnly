
# Welcome to MapOnly Documentation
## Introduce

**MappOnly** is a simple .Net standard library, It uses for convert, mapping from an object to another object. If you want a library lightweight and just for mapping only, MapOnly is a choice for you.
# Dependence
``````
.Net Framework > 4.6.1
.Net standard 2.0
.Net Core
``````
# Setup
``` 
1. Download at https://www.nuget.org/packages/MapOnly
2. Install by command lines: Install-Package MapOnly -Version 0.2.0 ( current version)
3. Install by nuget manage: Search MapOnly -> select project want to install -> click install
```

# Method
## 1. Create
This method uses to create a mapping between 2 classes. If the classes had the same properties no need to do create a mapping. **MapOnly** automatic find and do the mapping.

### Syntax
```
using MapOnly;
...
MapExtension.Create<ClassSource, ClassDestination>()
...
``` 
### Example
```
MapExtension.Create<UserViewModel, User>()
``` 
## 2. Add

Normally, If the classes had the same properties name and the same data type, no need to use this method. This method uses to declare with **MapOnly**  a property of ClassSource mapping to a property of ClassDestination. or A property of ClassDestination mapping to a value.

### Syntax
```
using MapOnly;
...
MapExtension.Create<ClassSource, ClassDestination>().Add(a => a.Property1,b => b.Property2);
MapExtension.Create<ClassSource, ClassDestination>().Add(b => b.Property1,"Value");
...
```
### Example
```
MapExtension.Create<UserViewModel, User>()
			.Add(v => v.DisplayName, u => u.FullName)
			.Add(u.CreatedBy, "Admin"); // Version 0.2.0
```
## 3. Ignore
In the case, class Destination and class Source have some properties are different or we don't want to do the mapping for the properties.  We can use the **Ignore** method.

### Syntax
```
using MapOnly;
...
MapExtension.Create<ClassSource, ClassDestination>().Ingore(b => b.Property1); 
...
```
### Example
```
MapExtension.Create<UserViewModel, User>()
                .Add(x =>x.Birthday, a => a.UpdatedDate)
                .Ignore(u => u.CreatedDate)
                .Ignore(u => u.CreatedUser)
                .Ignore(u => u.IsActive)
                .Ignore(u => u.UpdatedDate)
                .Ignore(u => u.UpdatedUser);
``` 
## 4. Map
This method use to convert a object to another object follow setting or auto.

### Syntax
```
 // a is an instant of Class Destination
 // b is an instant of Class Source
 b.Map(a);
 // or var b = b.Map(a);
```
### Example
```
 var user = userViewModel.Map(new User());
 var viewModel = user.Map(new UserDetailsViewModel());
 var userListing = users
                  .Select(user => user.Map(new UserDetailsViewModel()))
                  .ToList();
``` 

# Config
If between two classes had the properties need to ignore, or not mapping, or properties the same type but not the same name. We need to create mapping before we call the Map method.
## Web
```
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
## Winform, Console...
```
// Can add config in the Main method in Program.cs file.
 static class Program
 {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
	        // Do the mapping config here
            MapOnlyConfig.Register();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new fmMain());
        } 
  }
```