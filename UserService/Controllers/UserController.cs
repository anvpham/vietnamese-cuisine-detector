using UserService.DTOs;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace UserService.Controllers
{
    [ApiController, Route("api")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        
        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("createuser"), Authorize(Policy = "AuthenticationService")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequestBody data)
        {
            await _dbContext.Users.AddAsync(new User { 
                Id = data.UserId,
                Name = data.UserName
             });

            await _dbContext.SaveChangesAsync();

            return StatusCode(201);
        }
    }
}
