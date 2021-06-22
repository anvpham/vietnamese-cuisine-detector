using System.Net.Http.Headers;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AuthenticationService.Data;
using AuthenticationService.DTOs;
using AuthenticationService.Models;
using AuthenticationService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text.Json;

namespace AuthenticationService.Controllers
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        private readonly AppDbContext _dbContext;

        private readonly JWTService _jwtService;

        private readonly IHttpClientFactory _clientFactory;
        
        public AuthenticationController(IConfiguration configuration, AppDbContext dbContext, JWTService jwtService, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _jwtService = jwtService;
            _clientFactory = clientFactory;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestBody registerData)
        {
            if (registerData.Password.Length < 8)
            {
                return StatusCode(409, new { result = "Password length must be at least 8 characters" });
            }

            if (registerData.UserName.Contains(" ") || registerData.UserName.Any(ch => char.IsUpper(ch)))
            {
                return StatusCode(409, new { result = "UserName can not contain white space or upper characters" });
            }

            bool userNameExist =
                await _dbContext.UserCredentials.FirstOrDefaultAsync(uc => uc.UserName == registerData.UserName) != null;

            if (userNameExist)
            {
                return StatusCode(409, new { result = "The username is already being used, please try another one" });
            }

            Byte[] bytes = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(registerData.Password));
            string hashedPassword = BitConverter.ToString(bytes).Replace("-", "").ToLower();

            var userCredential = new UserCredential
            {
                UserName = registerData.UserName,
                Password = hashedPassword
            };

            await _dbContext.UserCredentials.AddAsync(userCredential);
            await _dbContext.SaveChangesAsync();

            HttpClient userServiceClient = _clientFactory.CreateClient("userService");

            string jsonString = JsonSerializer.Serialize(new CreateUserRequestBody {
                UserId = userCredential.UserId,
                UserName = userCredential.UserName
            });

            HttpContent httpContent = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, "api/createuser");

            request.Content = httpContent;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["AuthenticationServiceKey"]);

            await userServiceClient.SendAsync(request);

            string jwtToken = _jwtService.GenerateToken(userCredential.UserId, userCredential.UserName);
            
            return StatusCode(201, new { result = "Created successfully", jwtToken });
        }
    }
}