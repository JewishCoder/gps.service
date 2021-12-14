using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gps.common.Dal;
using gps.common.Enums;

namespace gps.common.Dto
{
	public class UserDto : IEntity, ICreatedEntity, IModifiedEntity, IDeletedEntity
	{
		public long Id { get; set; }

		public string Login { get; set; }

		public string Name { get; set; }

		public RoleType Role { get; set; }

		public DateTime CreateOn { get; set; }
		
		public DateTime? ModifiedOn { get; set; }
		
		public DateTime? DeletedOn { get; set; }
	}
}
