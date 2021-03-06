using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gps.common.Dal;

namespace gps.dal.Entities
{
	internal class EntityBase : IEntity, ICreatedEntity, IModifiedEntity, IDeletedEntity
	{
		public long Id { get; set; }

		public DateTime CreateOn { get; set; }
		
		public DateTime? ModifiedOn { get; set; }
		
		public DateTime? DeletedOn { get; set; }
	}
}
