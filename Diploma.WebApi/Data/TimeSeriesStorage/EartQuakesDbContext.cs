using Diploma.WebApi.Data.TimeSeriesStorage.Entities;
using Diploma.WebApi.Data.TimeSeriesStorage.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace Diploma.WebApi.Data.TimeSeriesStorage
{
	public class EarthQuakesDbContext: DbContext
	{
		public EarthQuakesDbContext(DbContextOptions<EarthQuakesDbContext> options) : base(options)
		{

		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new EarthQuakesConfiguration());
			modelBuilder.ApplyConfiguration(new CorrelationDimensionConfiguration());
		}

		public DbSet<EarthQuakes> EarthQuakes { get; set; }
		public DbSet<CorrelationDimension> CorrelationDimensions { get; set; }
	}
}
