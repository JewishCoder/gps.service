using gps.common.Dto;

namespace gps.service.Models
{
	public class TokenModel
	{
		public UserDto User { get; init; }

		public string Token { get; init; }

		public string RefreshToken { get; init; }

		public DateTime Expiration { get; init; }
	}
}
