# MAP ONLY

MapOnly is a simple map an object to another object

## How to use?

1. The destination has the same properties with source object

 ```csharp
  // News Entity
   public class NewsModel
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public int ViewNumber { get; set; }

        public DateTime? ShowDate { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string CreatedUser { get; set; }

        public string UpdatedUser { get; set; } 
    }
    
   // news View model
    public class NewsViewModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int ViewNumber { get; set; }
 
        public DateTime? ShowDate { get; set; }
    }
```

 ```csharp
 using MapOnly;
 //...
  var news = newsService.GetById('75ce973c-1813-4ee3-a354-a8348b207b87');
  NewsViewModel newsViewModel = new NewsViewModel();
  news.Map(newsViewModel); // Or MapExtension.Map(news, newsViewModel);
```
2. Ignore property
 There are 2 options to ignore property:
  2.1 Using attribute in destination model
  
  ```csharp
    using MapOnly;
    // news View model
    public class NewsViewModel
    {
        public string Title { get; set; }

        public string Content { get; set; }

        public int ViewNumber { get; set; }
 
        public DateTime? ShowDate { get; set; }
        
        public string CreatedUser { get; set; }
        
        [Map(Ignored = true)]
        public string CreatedUserDisplayname { 
          get
          {
              if (string.IsNullOrEmpty(CreatedUser) return string.Empty();
              return userService.GetByUsername(CreatedUser);
          } 
        }
    }
    
  var news = newsService.GetById('75ce973c-1813-4ee3-a354-a8348b207b87');
  NewsViewModel newsViewModel = new NewsViewModel();
  news.Map(newsViewModel); // Or MapExtension.Map(news, newsViewModel); 
  ```
  2.2 Using MapOnly setting:
  
   ```csharp
   MapExtension.Create<NewsModel, NewsViewModel>()
                .Add(source => source.Content, destination => destination.Content)
                .Add(source => source.Title, destination => destination.Title)
                .Ignore(x => x.ShowDate)
                .Ignore(x => x.CreatedUserDisplayname);
   ```
   
## Support or Contact
Any trouble please raise your issue here

## New feature, new version are comming
