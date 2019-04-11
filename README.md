
**MappOnly is a simple .Net standard library, It use for convert, mapping from an object to another object.

1. Dependences***
	.Net Framework > 4.6.1
	.Net standard 2.0
	.Net Core
	
2. Setting
	- Download at https://www.nuget.org/packages/MapOnly<br/>
	Install by command lines<br/>
	- *Install-Package MapOnly -Version 0.0.1 ( current version)*<br/>
	Install by nuget manage<br/>
	- Search MapOnly -> select project want to install -> click install<br/>
 
3. How to use?<br/>
 3.1 Use for mapping 2 objects has the same properties (No need to do any setting )<br/>
  ex:
  
 ```csharp
   class A{
   
     public GUID Id { get; set;}
     public string FirstName { get; set;}
     public string LastName { get; set;} 
     public string UpdatedBY { get; set; }
     public string UpdatedDate { get;set; }
     
    }
			
 class A_Display{
 
   public GUID Id { get; set;}
   public string FirstName { get; set;}
   public string LastName { get; set;} 
   
   }
			
  //Systax:
  var a = GetDataById(aId); // get a object
  var a_display = new A_Display();
  a.Map(a_display); // doing the mapping
   
```
 3.2 Setting<br/>
  Use setting to ignore or mapping to another property name<br/>
  3.2.1 Ignore<br/>
  ex: 

```csharp
  class A{
  
    public GUID Id { get; set;}
    public string FirstName { get; set;}
    public string LastName { get; set;} 
    public string UpdatedBY { get; set; }
    public string UpdatedDate { get;set; }
    
   }
				
   class A_Display{
  
     public GUID Id { get; set;}
     public string FirstName { get; set;}
     public string LastName { get; set;}
     public string DisplayName { 
      get {  
        return $"{FirstName} {LastName}"; 
       }
     } 
    
   } 
```
  Class A_Display has property "DisplayName" is readonly, in class B doesn't have. If we do the mapping the MapOnly will throw the exception ("DisplayName" not found in A class)<br/>
  With MapOnly we can do like this:<br/>
  3.2.1.1 Add addtribute in the top property we want to ignore <br/>
```csharp
class A_Display{
   public GUID Id { get; set;}
   public string FirstName { get; set;}
   public string LastName { get; set;}
   
   [Map(Ignore = true)] // here
   public string DisplayName { 
     get{ return $"{FirstName} {LastName}"; }
   } 
  } 
   //And do the mapping: 

   var a = GetDataById(aId); // get a object
   var a_display = new A_Display();
   a.Map(a_display);
 ```
 3.2.1.2 Ignore in MapOnly setting <br/>
				We can ignore propety by using MapOnly setting <br/>
    Syntax:
    
	```csharp MapExtension.Create<A, A_Display>().Ignore(x => x.DisplayName);```
 3.2.2 Mapping to another propety name<br/>
			Có nhiều trường hợp giữa entity và viewmodel khác tên property, và cách ánh xạ sang thuộc tính khác ở trong MapOnly như sau:
			Sometimes, between Entity and View Model has the properties, not the same name. with MapOnly we can do like example below:
```csharp	
   class A{
     public GUID Id { get; set;} 
     public string FirstName { get; set;}
     public string LastName { get; set;} 
     public string UpdatedBY { get; set; }
     public string UpdatedDate { get;set; }  
     public string Status { get;set; } 
   }

   class A_Display{
     public GUID Id { get; set;}
     public string FirstName { get; set;}
     public string LastName { get; set;}
     public string DisplayName { 
     get{
       return $"{FirstName} {LastName}";
     }
   }
     public string ProgressStatus { get; set; }
   }
				
   MapExtension.Create<A, A_Display>()
       .Ignore(x => x.DisplayName)
       .Add(source => source.Status, destination => destination.ProgressStatus); // mapping status -> ProgressStatus
```
3.2 Using for asp.net 
```csharp
		Create a MapOnlySetting class in App_Start/MapOnlySetting.cs
		
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
4. Q & A<br/>
	Current version haven't support map 2 collection has difference type yet. I will update in next version.
	Any trouble please raise your issue here
