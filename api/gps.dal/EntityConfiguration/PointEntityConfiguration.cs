using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

			builder.HasIndex(x => x.Name)
				.IsUnique();

			builder.Property(x => x.X)
				.HasColumnName("latitude");

			builder.Property(x => x.Y)
				.HasColumnName("longitude");

			builder.HasIndex(x => new { x.X, x.Y });
		}
	}
}
