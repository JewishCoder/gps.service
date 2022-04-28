using gps.common.Enums;

namespace gps.service.Models
{
	public class UpdateUserModel
	{
		public string? Login { get; init; }

		public string? Name { get; init; }

		public RoleType Role { get; init; }

		public string? Password { get; init; }
	}
}
