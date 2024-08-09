<<<<<<< HEAD
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using QuickCommerce.Models;
using System.Threading.Tasks;
=======
﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using QuickCommerce.Models;
using QuickCommerce.Services;
>>>>>>> e40425a (Add .gitignore and remove ignored files from repo)

namespace QuickCommerce.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
<<<<<<< HEAD
    public class UserController : ControllerBase
    {
        private readonly IMongoCollection<User> _users;

        public UserController(IMongoDatabase database)
        {
            _users = database.GetCollection<User>("Users");
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            // Hash the password (use a proper hashing method in production)
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;

            await _users.InsertOneAsync(user);
            return Ok(user);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _users.Find(u => u.Email == request.Email).FirstOrDefaultAsync();
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
=======
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
>>>>>>> e40425a (Add .gitignore and remove ignored files from repo)
            {
                return Unauthorized("Invalid credentials");
            }

<<<<<<< HEAD
            // Generate JWT token (implement this in production)
            var token = "dummy-jwt-token";
            return Ok(new { Token = token });
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
=======
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

>>>>>>> e40425a (Add .gitignore and remove ignored files from repo)
    }
}
