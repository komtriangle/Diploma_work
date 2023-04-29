using Diploma.WebApi.Data.TimeSeriesStorage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diploma.WebApi.Data.TimeSeriesStorage.EntityConfigurations
{
	public class MagnitudesConfiguration: IEntityTypeConfiguration<Magnitudes>
	{
		public void Configure (EntityTypeBuilder<Magnitudes> builder)
		{
			builder.ToTable("magnitudes_taivan");

			builder.Property(x => x.Time).HasColumnName("time");
			builder.Property(x => x.Id).HasColumnName("id");
			builder.Property(x => x.Value).HasColumnName("value");
		}
	}
}
