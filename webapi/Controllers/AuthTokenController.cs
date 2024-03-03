using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using webapi.Context;
using webapi.Models.DTO;

namespace webapi.Controllers
{
    [Route("api/[controller]")]
    public class AuthTokenController : Controller
    {
        public IConfiguration _configuration;
        public readonly ServerContext _context;

        public AuthTokenController(IConfiguration configuration, ServerContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        private JwtSecurityToken GetToken(List<Claim> authClaims)
        {
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                expires: DateTime.Now.AddDays(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );
            return token;
        }

        [HttpPost]
        public async Task<ActionResult> Login([FromBody] UserLogin model)
        {
            if (model != null && !string.IsNullOrEmpty(model.Username) && !string.IsNullOrEmpty(model.Password))
            {
                var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);

                if (user != null)
                {
                    // Check if password matches the hash password in the database
                    bool passwordMatches = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

                    if (passwordMatches == true)
                    {
                        // User found, create claims and generate token
                        var authClaims = new List<Claim>
                        {
                            new Claim("User Id", user.Id.ToString()),
                            new Claim("Name", user.Name),
                            new Claim("Role", user.UserRole),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                        };

                        // Generate JWT token
                        var token = GetToken(authClaims);

                        return Json(new
                        {
                            token = new JwtSecurityTokenHandler().WriteToken(token),
                            expiration = token.ValidTo
                        });
                    }
                    else
                    {
                        return BadRequest("Username or password is incorrect!");
                    }
                }
                else
                {
                    return NotFound($"User '{model.Username}' not found!");
                }
            }
            else
            {
                return BadRequest("Username or password is required!");
            }
        }
    }
}

