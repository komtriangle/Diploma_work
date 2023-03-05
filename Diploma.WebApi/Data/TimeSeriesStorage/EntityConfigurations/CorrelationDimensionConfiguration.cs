using Diploma.WebApi.Data.TimeSeriesStorage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Diploma.WebApi.Data.TimeSeriesStorage.EntityConfigurations
{
	public class CorrelationDimensionConfiguration : IEntityTypeConfiguration<CorrelationDimension>
	{
		public void Configure(EntityTypeBuilder<CorrelationDimension> builder)
		{
			builder.ToTable("correlation_dimension_sakhalin");

			builder.Property(x => x.Time).HasColumnName("time");
			builder.Property(x => x.Id).HasColumnName("id");
			builder.Property(x => x.Value).HasColumnName("value");
		}
	}
}
