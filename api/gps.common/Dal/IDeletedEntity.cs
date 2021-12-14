using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Dal
{
	public interface IDeletedEntity
	{
		DateTime? DeletedOn { get; set; }
	}
}
