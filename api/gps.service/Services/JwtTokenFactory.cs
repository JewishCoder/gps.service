using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using gps.common.Dto;
using gps.service.Models;

using Gps.Service.Configuration;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace gps.service.Services
{
	public class JwtTokenFactory
	{
		public const string UserIdClaimType = "userId";
		public const string RoleIdClaimType = "roleId";

		public IOptions<JwtTokenConfiguration> Config { get; set; }

		public JwtTokenFactory(IOptions<JwtTokenConfiguration> configuration)
		{
			Config = configuration;
		}

		public TokenModel Create(UserDto user, TimeSpan? expiration = null)
		{
			var handler = new JwtSecurityTokenHandler();
			var token = CreateToken(user);
			var refreshToken = CreateRefreshToken(user);

			return new TokenModel
			{
				User = user,
				Token = handler.WriteToken(token),
				RefreshToken = handler.WriteToken(refreshToken),
				Expiration = token.ValidTo,
			};
		}

		public bool ValidateToken(UserDto user, string token)
		{
			var userId = TryGetUserFromToken(token);
			return userId.HasValue && user.Id == userId;
		}

		public long? TryGetUserFromToken(string token)
		{
			var handler = new JwtSecurityTokenHandler();
			if(handler.CanReadToken(token))
			{
				try
				{
					var claims = handler.ValidateToken(token, GetValidationParameters(), out var validatedToken);
					var userId = long.Parse(claims.FindFirstValue(UserIdClaimType));
					return userId;
				}
				catch(Exception exc)
				{
					Console.WriteLine(exc);
					return null;
				}
			}

			return null;
		}

		private TokenValidationParameters GetValidationParameters()
			=> new()
			{
				ValidateIssuer = true,
				ValidIssuer = Config.Value.Issuer,
				ValidateAudience = true,
				RequireAudience = true,
				ValidAudience = Config.Value.Audience,
				ValidateLifetime = true,
				RequireExpirationTime = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = Config.Value.GetSecurityKey(),
			};

		private JwtSecurityToken CreateToken(UserDto user, TimeSpan? expiration = null)
		{
			var expiresInMinutes = expiration.HasValue ? expiration.Value.TotalMinutes : 15;
			var payload = new JwtPayload(
				issuer: Config.Value.Issuer,
				audience: Config.Value.Audience,
				claims: CreateClaims(user),
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddMinutes(expiresInMinutes));


			return new JwtSecurityToken(CreateHeader(), payload);
		}

		private JwtSecurityToken CreateRefreshToken(UserDto user)
		{
			var payload = new JwtPayload(
				issuer: Config.Value.Issuer,
				audience: Config.Value.Audience,
				claims: CreateClaims(user),
				notBefore: DateTime.UtcNow,
				expires: DateTime.UtcNow.AddDays(3));


			return new JwtSecurityToken(CreateHeader(), payload);
		}

		private List<Claim> CreateClaims(UserDto user)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
				new Claim(UserIdClaimType, user.Id.ToString()),
				new Claim(RoleIdClaimType, ((int)user.Role).ToString()),
			};

			return claims;
		}

		private JwtHeader CreateHeader()
		{
			var credentionals = new SigningCredentials(Config.Value.GetSecurityKey(), SecurityAlgorithms.HmacSha256);
			return new JwtHeader(credentionals);
		}
	}
}
