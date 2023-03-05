using Microsoft.EntityFrameworkCore;

namespace Diploma.WebApi.Data.TimeSeriesStorage.Entities
{
	public class EarthQuakes
	{
		public long Id { get; set; }
		public DateTime Time { get; set; }
		public double Magnitude { get; set; }
		public double Latitude { get; set; }
		public double Longitude { get; set; }
	}
}
