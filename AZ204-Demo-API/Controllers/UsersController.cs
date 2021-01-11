using AZ204_Demo_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AZ204_Demo_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly ApiContext _context;
    private readonly ILogger _logger;
    public UsersController(ApiContext context, ILogger<UsersController> logger)
    {
      _context = context;
      _logger = logger;
    }

    public async Task<IActionResult> Get()
    {
      var users = await _context.Users
          .Include(u => u.Posts)
          .ToArrayAsync();

      var response = users.Select(u => new
      {
        id = u.Id,
        firstName = u.FirstName,
        lastName = u.LastName,
        posts = u.Posts.Select(p => p.Content)
      });

      return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
      var user = await _context.Users          
          .Include(u => u.Posts)
          .FirstOrDefaultAsync(u => u.Id.ToLower() == id.ToLower());
      if(user != null)
      {
        return Ok(user);
      }
      return NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] User user)
    {
      try
      {
        var inserted = await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return Created($"api/users/{inserted.Entity.Id}", inserted.Entity);
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Add user failed", user);
        return BadRequest();
      }
      
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User user)
    {
      try
      {
        var target = await _context.Users
          .FirstAsync(u => u.Id.ToLower() == id.ToLower());

        Models.User.CopyIfDifferent(target, user);
        _context.Attach(target);
        await _context.SaveChangesAsync();

        return NoContent();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Update user failed", user);
        return BadRequest();
      }
      
    }
  }
}
