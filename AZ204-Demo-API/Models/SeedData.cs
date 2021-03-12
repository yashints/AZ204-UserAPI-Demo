using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AZ204_Demo_API.Models
{
  public class SeedData
  {
    public static void AddTestData(ApiContext context)
    {
      var testUser1 = new User
      {
        Id = "abc123",
        FirstName = "Luke",
        LastName = "Skywalker"
      };

      context.Users.Add(testUser1);

      var testPost1 = new Post
      {
        Id = "def234",
        UserId = testUser1.Id,
        Content = "First post"
      };
      
      var testUser2 = new User
      {
        Id = "abc124",
        FirstName = "Dark",
        LastName = "Vadar"
      };

      context.Users.Add(testUser2);

      var testPost1 = new Post
      {
        Id = "def235",
        UserId = testUser2.Id,
        Content = "First post"
      };

      context.Posts.Add(testPost1);

      context.SaveChanges();
    }
  }
}
