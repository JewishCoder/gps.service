using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using gps.common.Dto;
using gps.common.Enums;

namespace gps.dal.Entities
{

	[AutoMap(typeof(UserDto), ReverseMap = true)]
	internal class UserEntity : EntityBase
	{
		public string Login { get; set; }

		public string Name { get; set; }

		public string Password { get; set; }

		public RoleType Role { get; set; }
	}
}
