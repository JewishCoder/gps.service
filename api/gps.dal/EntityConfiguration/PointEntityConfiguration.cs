
using gps.dal.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gps.dal.EntityConfiguration
{
	internal class PointEntityConfiguration : IEntityTypeConfiguration<PointEntity>
	{
		public void Configure(EntityTypeBuilder<PointEntity> builder)
		{
			builder.ToTable("points");
			builder.ConfigureKey();
			builder.HasCreateOn();
			builder.HasModifiedOn();
			builder.HasDeletedOn();

			builder.Property(x => x.Name)
				.HasColumnName("name")
				.HasMaxLength(100);

			builder.Property(x => x.Latitude)
				.HasColumnName("latitude");

			builder.Property(x => x.Longitude)
				.HasColumnName("longitude");

			builder.HasIndex(x => new { x.Latitude, x.Longitude });
		}
	}
}
