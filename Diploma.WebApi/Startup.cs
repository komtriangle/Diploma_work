using Diploma.WebApi.Configuration;
using Diploma.WebApi.Data;
using Diploma.WebApi.Data.DataAccess;
using Diploma.WebApi.Data.TimeSeriesStorage;
using Microsoft.EntityFrameworkCore;

namespace Diploma
{
	public class Startup
	{

		private IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddCors();
			services.AddControllers();
			services.AddEndpointsApiExplorer();
			services.AddSwaggerGen();
			services.AddMemoryCache();

			services.Configure<AppSettings>(_configuration.GetSection("AppSettings"));
			services.AddTransient<ILoadData, LoadExcelData2>();
			services.AddTransient<IEathQuakesData, EathQuakesData>();

			AppSettings appSettings = _configuration.GetSection("AppSettings").Get<AppSettings>();

			services.AddDbContext<EarthQuakesDbContext>(options =>
		   {
			   options.UseNpgsql(appSettings.TimeSeriesDbConnectionString);
			   options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
		   });

			AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if(env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseCors(x => x
			   .AllowAnyMethod()
			   .AllowAnyHeader()
			   .SetIsOriginAllowed(origin => true) // allow any origin
			   .AllowCredentials());

			app.UseSwagger();
			app.UseSwaggerUI();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}