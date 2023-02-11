using System.ComponentModel.DataAnnotations;

namespace Diploma.WebApi.Models.Requests
{
	/// <summary>
	/// Модель запроса для рассчета
	/// значения автокорреляционной функции
	/// </summary>
	public class AutoCorrelationRequest
	{
		/// <summary>
		/// Значение "От" широты
		/// </summary>
		[Required(ErrorMessage = "Не указано начальное значение широты")]
		[Range(0, 90, ErrorMessage = "Значение широты должно быть от 0 до 90")]
		public float LatitudeStart { get; set; }

		/// <summary>
		/// Значение "До" широты
		/// </summary>
		[Required(ErrorMessage = "Не указано конечное значение широты")]
		[Range(0, 90, ErrorMessage = "Значение широты должно быть от 0 до 90")]
		public float LatitudeEnd { get; set; }


		/// <summary>
		/// Значение "От" долготы
		/// </summary>
		[Required(ErrorMessage = "Не указано начальное значение долготы")]
		[Range(0, 180, ErrorMessage = "Значение долготы должно быть от 0 до 180")]

		public float LongtitudeStart { get; set; }

		/// <summary>
		/// Значение "До" долготы
		/// </summary>
		[Required(ErrorMessage = "Не указано конечное значение долготы")]
		[Range(0, 180, ErrorMessage = "Значение долготы должно быть от 0 до 180")]
		public float LongtitudeEnd { get; set; }
	}
}
