using gps.common.Dto;

using System.Text.Json.Serialization;

namespace gps.service.Models
{
	public class TokenModel
	{
		public UserDto User { get; init; }

		public string Token { get; init; }

		[JsonIgnore]
		public string RefreshToken { get; init; }

		public DateTime Expiration { get; init; }
	}
}
