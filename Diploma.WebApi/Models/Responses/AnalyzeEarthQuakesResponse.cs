namespace Diploma.WebApi.Models.Responses
{
	public class AnalyzeEarthQuakesResponse
	{
		/// <summary>
		/// Временной ряд максимальных магнитуд в интервалах
		/// </summary>
		public TimeIntervalMagnitude[] TimeSeriesMaxMagnitudes { get; set; }


		/// <summary>
		/// Временной ряд корреляционных размерностей
		/// </summary>
		public TimeIntervalCorrelationDimension[] TimeIntervalCorrelationDimensions { get; set; }
		public double[] CorrelationDimensionDifferences { get; set; }
	}
}
