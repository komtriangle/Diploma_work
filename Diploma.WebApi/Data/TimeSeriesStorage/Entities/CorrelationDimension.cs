namespace Diploma.WebApi.Data.TimeSeriesStorage.Entities
{
	public class CorrelationDimension
	{
		public long Id { get; set; }
		public DateTime Time { get; set; }
		public double Value { get; set; }
	}
}
