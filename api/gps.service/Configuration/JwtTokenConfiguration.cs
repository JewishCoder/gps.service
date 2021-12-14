using System.Text;

using Microsoft.IdentityModel.Tokens;

namespace Gps.Service.Configuration
{
	public class JwtTokenConfiguration
	{
		public string? Issuer { get; set; }

		public string? Audience { get; set; }

		protected string? Secret { get; set; }

		public SecurityKey GetSecurityKey()
		{
			if(string.IsNullOrWhiteSpace(Secret))
			{
				throw new InvalidOperationException("Secret is null.");
			}

			return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secret));
		}
	}
}
