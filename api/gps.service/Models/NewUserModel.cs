using AutoMapper;

using gps.common.Dto;
using gps.common.Enums;

namespace gps.service.Models
{
	[AutoMap(typeof(UserDto), ReverseMap = true)]
	public class NewUserModel
	{
		public string Login { get; set; }

		public string Name { get; set; }

		public RoleType Role { get; set; }

		public string Password { get; set; }
	}
}
