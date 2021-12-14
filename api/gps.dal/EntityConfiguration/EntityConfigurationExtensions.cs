using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using gps.common.Dal;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace gps.dal.EntityConfiguration
{
	internal static class EntityConfigurationExtensions
	{
		public static void ConfigureKey<T>(this EntityTypeBuilder<T> builder)
			   where T : class, IEntity
		{
			builder.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
		}

		public static void HasCreateOn<T>(this EntityTypeBuilder<T> builder)
			where T : class, ICreatedEntity
		{
			builder.Property(x => x.CreateOn)
				.HasColumnName("createOn")
				.IsRequired();

			builder.HasIndex(x => x.CreateOn);
		}

		public static void HasModifiedOn<T>(this EntityTypeBuilder<T> builder)
			where T : class, IModifiedEntity
		{
			builder.Property(x => x.ModifiedOn)
				.HasColumnName("modifiedOn")
				.IsRequired(false);

			builder.HasIndex(x => x.ModifiedOn);
		}

		public static void HasDeletedOn<T>(this EntityTypeBuilder<T> builder)
			where T : class, IDeletedEntity
		{
			builder.Property(x => x.DeletedOn)
				.HasColumnName("deletedOn")
				.IsRequired(false);

			builder.HasIndex(x => x.DeletedOn);
		}
	}
}
