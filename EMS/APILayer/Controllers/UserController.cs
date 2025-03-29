using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using EMS2.Models;
using EMS2.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
namespace EMS2.Controllers {

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _config;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _config = configuration;

        }

        //add
        [HttpPost("adduser")]
        public async Task<IActionResult> AddUser([FromBody]Users user)
        {
            await _userService.AddUserAsync(user);
            return Ok();
        }

        //getall
        [HttpGet("getallusers")]

        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        //getbyid
        [HttpGet("Getuserbyid/{id}")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(user);
        }

        //delete
        [HttpDelete("deleteuser/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUserAsync(id);
            return Ok();
        }

        //update
        [HttpPost("updateuser/{id}")]
        public async Task<IActionResult> UpdateUser(Guid id,[FromBody]Users user)
        {
            var existingUser = await _userService.GetUserByIdAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            await _userService.UpdateUserAsync(existingUser);
            return NoContent();
        }


        [HttpPost]
        [Route("ValidateUser")]
        [Consumes("application/json")] // Expecting application/json content
        [Produces("application/json")] // Returning application/json content
        public async Task<IActionResult> ValidateUser([FromBody] Users user)
        {
            // Fetch user details by email
            var users = await _userService.GetByEmailAsync(user.Email);

            if (users.Any()) // Check if user exists
            {
                var existingUser = users.First();

                // Compare the provided password with the stored hashed password
                if (BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password)) // Password matches
                {
                    var token = GenerateToken(existingUser); // Generate token for the user

                    // Return the token and user role
                    var response = new
                    {
                        Token = token,
                        Role = existingUser.Role // Assuming 'Role' is a field in the user model
                    };

                    return Ok(response); // Return 200 OK with token and role
                }
            }
            return Unauthorized(); // Return 401 Unauthorized
        }

        [NonAction]
        public string GenerateToken(Users user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
