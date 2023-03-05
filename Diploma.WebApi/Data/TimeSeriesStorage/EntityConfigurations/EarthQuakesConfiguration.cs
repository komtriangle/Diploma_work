using Diploma.WebApi.Data.TimeSeriesStorage.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Diploma.WebApi.Data.TimeSeriesStorage.EntityConfigurations
{
	public class EarthQuakesConfiguration : IEntityTypeConfiguration<EarthQuakes>
	{
		public void Configure(EntityTypeBuilder<EarthQuakes> builder)
		{
			builder.ToTable("earthquakes_sakhalin");

			builder.Property(x => x.Time).HasColumnName("time");
			builder.Property(x => x.Id).HasColumnName("id");
			builder.Property(x => x.Latitude).HasColumnName("latitude");
			builder.Property(x => x.Longitude).HasColumnName("longitude");
			builder.Property(x => x.Magnitude).HasColumnName("magnitude");
		}
	}
}
