using Diploma.WebApi.Models;

namespace Diploma.WebApi.Extensions
{
	public static class TimeIntervalExtensions
	{
		public static List<Earthquake> GetEarthquakes(this TimeInterval timeInterval, List<Earthquake> earthquakes)
		{
			return earthquakes
					.Where(e => timeInterval.EarthQuakes.Contains(e.Id))
					.ToList();
		}
	}
}
