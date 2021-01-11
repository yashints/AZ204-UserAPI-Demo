using AZ204_Demo_API.Models;
using Microsoft.AspNetCore.Http;
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

    /// <summary>
    /// Get all users, no need to pass anything.
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///     Get /api/users
    /// </remarks>
    /// <returns>
    /// List of users in an array
    /// </returns>
    [HttpGet]
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

    /// <summary>
    /// Getting a single user
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200">Returns the user</response>
    /// <response code="404">If it cannot find the user</response>  
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
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

    /// <summary>
    /// Adding a new user
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     POST /api/users
    ///     {
    ///        "id": 1,
    ///        "firstName": "John",
    ///        "lastName": "Doe",
    ///        "posts": ["First Post"]
    ///     }
    ///
    /// </remarks>
    /// <param name="user"></param>
    /// <returns>
    /// The newly created user
    /// </returns>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If the item is null</response>     
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
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

    /// <summary>
    /// Updating a user
    /// </summary>
    /// <remarks>
    /// Sample request:
    ///
    ///     PUT /api/users/123
    ///     {
    ///        "firstName": "Jane",
    ///        "lastName": "Doe 2"
    ///     }
    ///
    /// </remarks>
    /// <param name="id"></param>
    /// <param name="user"></param>
    /// <returns>The updated user</returns>
    /// <response code="201">Returns the newly created user</response>
    /// <response code="400">If the item is null</response> 
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
