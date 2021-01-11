using Microsoft.EntityFrameworkCore;

namespace AZ204_Demo_API.Models
{
  public class ApiContext: DbContext
  {
    public ApiContext(DbContextOptions<ApiContext> options)
            : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts { get; set; }
  }
}
