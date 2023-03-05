namespace Diploma.WebApi.Models.Responses
{
	public class GetAllEarthQuakesResponse
	{
		public List<Earthquake> Earthquakes { get; set; }
		public List<GroupedEarthQuakes> GroupedEarthQuakes { get; set; }
		public double[] Magnitudes { get; set; }
	}
}
