using AutoMapper;

using gps.common.Dto;

namespace gps.service.Models
{
	[AutoMap(typeof(PointDto), ReverseMap = true)]
	public class PointModel
	{
		public string Name { get; set; }

		public double X { get; set; }

		public double Y { get; set; }
	}
}
