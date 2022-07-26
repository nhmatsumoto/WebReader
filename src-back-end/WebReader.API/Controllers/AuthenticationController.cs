using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebReader.API.Domain.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebReader.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private static IConfiguration _configuration { get; set; }
        

        public AuthenticationController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // POST api/<AuthenticationController>
        [HttpPost("/webreader/security/login")]
        [AllowAnonymous]
        public IResult Post(UserDto user)
        {
            if (user.email == "admin" && user.password == "adminpassword")
            {
                var issuer = _configuration.GetSection("Jwt:Issuer").Value;
                var audience = _configuration.GetSection("Jwt:Audience").Value;  
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration.GetSection("Jwt:Key").Value);

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[]
                    {
                new Claim("Id", "1"),
                new Claim(JwtRegisteredClaimNames.Sub, user.email),
                new Claim(JwtRegisteredClaimNames.Email, user.email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),

                    Expires = DateTime.UtcNow.AddHours(6),
                    Audience = audience,
                    Issuer = issuer,
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature)
                };

                var token = jwtTokenHandler.CreateToken(tokenDescriptor);

                var jwtToken = jwtTokenHandler.WriteToken(token);

                return Results.Ok(jwtToken);
            }
            else
            {
                return Results.Unauthorized();
            }
        }

        }
    }
}
