namespace Diploma.WebApi.Models
{
	/// <summary>
	/// Временной интервал
	/// </summary>
	public class TimeInterval
	{
		/// <summary>
		/// Начало интервала
		/// </summary>
		public DateTime Start { get; set; }

		/// <summary>
		/// Конец интервала
		/// </summary>
		public DateTime End { get; set; }

		/// <summary>
		/// Список Id землетрясений
		/// </summary>
		public int[] EarthQuakes { get; set; }

	}

}
