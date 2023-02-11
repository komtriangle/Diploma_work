namespace Diploma.WebApi.Logic.Models
{
	/// <summary>
	/// Модель для фильтрации землетрясений 
	/// по широта и долготе
	/// </summary>
	public class FilterOptions
	{
		/// <summary>
		/// Значение "От" широты
		/// </summary>
		public float LatitudeStart { get; set; }

		/// <summary>
		/// Значение "До" широты
		/// </summary>
		public float LatitudeEnd { get; set; }


		/// <summary>
		/// Значение "От" долготы
		/// </summary>
		public float LongtitudeStart { get; set; }

		/// <summary>
		/// Значение "До" долготы
		/// </summary>
		public float LongtitudeEnd { get; set; }
	}
}
