using AuthService.Model;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly JwtSettings _jwtSettings;

		public AuthController(IOptions<JwtSettings> jwtOptions)
		{
			_jwtSettings = jwtOptions.Value;
		}

		[HttpPost("token")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var connStr = AppConfigurations.GetConnectionString("PostgresDb");

			using var conn = new NpgsqlConnection(connStr);

			var user = await conn.QueryFirstOrDefaultAsync<UserDetails>(
				"SELECT * FROM userdetails WHERE userid = @UserId AND password = @Password",
				new { request.UserId, request.Password });

			if (user == null)
				return Unauthorized("Invalid credentials");

			var token = GenerateJwtToken(user.UserId);
			return Ok(new { Token = token });
		}

		private string GenerateJwtToken(string userId)
		{
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
			new Claim(ClaimTypes.Name, userId)
		};

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
				signingCredentials: credentials);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

	}
}
