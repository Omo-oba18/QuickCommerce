using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using QuickCommerce.Models;
using QuickCommerce.Services;
using System.Threading.Tasks;
using MongoDB.Bson;

public class UserController : Controller
{
    private readonly UserService _userService;
    private readonly JwtService _jwtService;


    public UserController(IMongoDatabase database, JwtService jwtService)
    {
        _userService = new UserService(database);
        _jwtService = jwtService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var registeredUser = await _userService.RegisterUser(user);
        return Ok(registeredUser);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] QuickCommerce.Models.LoginRequest request)
    {
        var user = await _userService.AuthenticateUser(request);
        if (user == null)
        {
            return Unauthorized("Invalid credentials");
        }
        // Generate JWT token
        var token = _jwtService.GenerateToken(user);
        return Ok(new { Token = token });
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(string id)
    {
        // Convert the string id to ObjectId
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return BadRequest("Invalid user ID format.");
        }

        // Get the user by ObjectId
        var user = await _userService.GetUserById(objectId);
        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(string id, [FromBody] User updatedUser)
    {
        // Convert the string id to ObjectId
        if (!ObjectId.TryParse(id, out var objectId))
        {
            return BadRequest("Invalid user ID format.");
        }

        // Get the existing user
        var user = await _userService.GetUserById(objectId);
        if (user == null)
        {
            return NotFound();
        }

        // Prepare the updated user details
        updatedUser.Id = objectId; // Ensure the Id is set correctly
        updatedUser.UpdatedAt = DateTime.UtcNow;

        // Update the user in the database
        await _userService.UpdateUser(objectId, updatedUser);

        return NoContent();
    }

}
