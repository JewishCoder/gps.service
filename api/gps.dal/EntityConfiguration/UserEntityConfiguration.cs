
using gps.dal.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gps.dal.EntityConfiguration
{
	internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
	{
		public void Configure(EntityTypeBuilder<UserEntity> builder)
		{
			builder.ToTable("users");
			builder.ConfigureKey();
			builder.HasCreateOn();
			builder.HasModifiedOn();
			builder.HasDeletedOn();

			builder.Property(x => x.Name)
				.HasColumnName("name")
				.HasMaxLength(50);

			builder.Property(x => x.Login)
				.HasColumnName("login")
				.HasMaxLength(20);

			builder.HasIndex(x => x.Login)
				.IsUnique();

			builder.Property(x => x.Password)
				.HasColumnName("password")
				.HasMaxLength(256);

			builder.Property(x => x.Role)
				.HasColumnName("role");
		}
	}
}
