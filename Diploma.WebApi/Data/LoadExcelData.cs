using CsvHelper;
using CsvHelper.Configuration;
using Diploma.WebApi.Configuration;
using Diploma.WebApi.Models;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace Diploma.WebApi.Data
{
	public class LoadExcelData : ILoadData
	{
		private readonly string _filePath;
		private readonly ILogger<LoadExcelData> _logger;

		public LoadExcelData(IOptions<AppSettings> settings, ILogger<LoadExcelData> logger)
		{
			if(settings?.Value == null)
			{
				throw new ArgumentNullException(nameof(AppSettings));
			}

			_filePath = settings.Value.FileDataPath;
			_logger = logger;
		}
		public List<Earthquake> Load()
		{
			var config = new CsvConfiguration(CultureInfo.CurrentCulture)
			{
				Delimiter = ";",
				Encoding = Encoding.UTF8
			};
			using(var reader = new StreamReader(_filePath))
			using(var csv = new CsvReader(reader, config))
			{
				var earthquakes = new List<Earthquake>();
				csv.Read();
				csv.ReadHeader();

				int count = 0;
				while(csv.Read())
				{
					count++;
					try
					{
						var earthquake = new Earthquake
						{
							Id = count,
							Time = csv.GetField<DateTime>("Date"),
							Latitude = csv.GetField<double>("Latitude"),
							Longitude = csv.GetField<double>("Longtitude"),
							Magnitude = csv.GetField<double>("Magnitude")
						};
						earthquake.Time.AddHours(csv.GetField<int>("Hour"));
						earthquake.Time.AddMinutes(csv.GetField<int>("Minute"));
						earthquake.Time.AddSeconds(csv.GetField<double>("Second"));
						earthquakes.Add(earthquake);

					}
					catch(Exception ex)
					{
						_logger.LogError(ex, "Ошибка во время чтения записи о землетрясении из файла");
					}
				}
				return earthquakes;
			}
		}
	}
}
