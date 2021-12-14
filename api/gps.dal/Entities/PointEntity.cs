using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AutoMapper;

using gps.common.Dto;

namespace gps.dal.Entities
{
	[AutoMap(typeof(PointDto), ReverseMap = true)]
	internal class PointEntity : EntityBase
	{
		public string Name { get; set; }

		public double X { get; set; }

		public double Y { get; set; }
	}
}
