using CsvHelper;
using CsvHelper.Configuration;
using Diploma.WebApi.Configuration;
using Diploma.WebApi.Data.TimeSeriesStorage;
using Diploma.WebApi.Models;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;
using System.IO;

namespace Diploma.WebApi.Data
{
	public class LoadExcelData2 : ILoadData
	{
		private readonly string _filePath;
		private readonly ILogger<LoadExcelData> _logger;
		private readonly EarthQuakesDbContext _earthQuakesDbContext;

		public LoadExcelData2(IOptions<AppSettings> settings, EarthQuakesDbContext dbContext,
			ILogger<LoadExcelData> logger)
		{
			if(settings?.Value == null)
			{
				throw new ArgumentNullException(nameof(AppSettings));
			}

			if(dbContext == null)
			{
				throw new ArgumentNullException(nameof(dbContext));
			}

			_earthQuakesDbContext = dbContext;
			_filePath = settings.Value.FileDataPath;
			_logger = logger;
		}
		public List<Earthquake> Load()
		{
			var config = new CsvConfiguration(CultureInfo.CurrentCulture)
			{
				Delimiter = ",",
				Encoding = Encoding.UTF8
			};
			using(var reader = new StreamReader(_filePath))
			using(var csv = new CsvReader(reader, config))
			{
				var earthquakes = new List<Earthquake>();
				csv.Read();
				csv.ReadHeader();

				int count = 0;

				//_earthQuakesDbContext.EarthQuakes.RemoveRange(_earthQuakesDbContext.EarthQuakes);
				//_earthQuakesDbContext.SaveChanges();
				while(csv.Read())
				{
					count++;
					try
					{
						var earthquake = new Earthquake
						{
							Id = count,
							Time = csv.GetField<DateTime>("time"),
							Latitude = double.Parse(csv.GetField<string>("latitude"), CultureInfo.InvariantCulture),
							Longitude = double.Parse(csv.GetField<string>("longitude"), CultureInfo.InvariantCulture),
							Magnitude = double.Parse(csv.GetField<string>("mag"), CultureInfo.InvariantCulture)
						};

						earthquakes.Add(earthquake);

						//if(earthquake.Time > DateTime.UtcNow.AddYears(-25))
						//{
						//	earthquakes.Add(earthquake);
						//}

						//_earthQuakesDbContext.EarthQuakes.Add(new TimeSeriesStorage.Entities.EarthQuakes()
						//{
						//	Latitude = earthquake.Latitude,
						//	Longitude = earthquake.Longitude,
						//	Magnitude = earthquake.Magnitude,
						//	Time = earthquake.Time
						//});
					}
					catch(Exception ex)
					{
						_logger.LogError(ex, "Ошибка во время чтения записи о землетрясении из файла");
					}
				}
				//_earthQuakesDbContext.SaveChanges();
				return earthquakes;
			}
		}
	}
}
