using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gps.common.Dto
{
	public class PointDto
	{
		public long Id { get; set; }

		public string Name { get; set; }

		public double X { get; set; }

		public double Y { get; set; }

		public DateTime CreateOn { get; set; }

		public DateTime? ModifiedOn { get; set; }

		public DateTime? DeletedOn { get; set; }
	}
}
