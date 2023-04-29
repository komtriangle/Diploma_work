using System.ComponentModel.DataAnnotations;

namespace Diploma.WebApi.Models.Requests
{
	public class AnalyzeEathQuakesRequest

	{
		/// <summary>
		/// Значение "От" широты
		/// </summary>
		[Required(ErrorMessage = "Не указано начальное значение широты")]
		[Range(-90, 90, ErrorMessage = "Значение широты должно быть от -90 до 90")]
		public float LatitudeStart { get; set; }

		/// <summary>
		/// Значение "До" широты
		/// </summary>
		[Required(ErrorMessage = "Не указано конечное значение широты")]
		[Range(-90, 90, ErrorMessage = "Значение широты должно быть от -90 до 90")]
		public float LatitudeEnd { get; set; }


		/// <summary>
		/// Значение "От" долготы
		/// </summary>
		[Required(ErrorMessage = "Не указано начальное значение долготы")]
		[Range(-180, 360, ErrorMessage = "Значение долготы должно быть от -180 до 180")]

		public float LongtitudeStart { get; set; }

		/// <summary>
		/// Значение "До" долготы
		/// </summary>
		[Required(ErrorMessage = "Не указано конечное значение долготы")]
		[Range(-180, 360, ErrorMessage = "Значение долготы должно быть от -180 до 180")]
		public float LongtitudeEnd { get; set; }

		/// <summary>
		/// Длина временного интервала, на который будет
		/// разбиваться весь интервал (в днях)
		/// </summary>
		[Required(ErrorMessage = "Не указана длина интвервала (в днях)")]
		[Range(1, int.MaxValue, ErrorMessage = "Размер интервала должен быть больше 0")]
		public int IntervalDays { get; set; }

		/// <summary>
		/// Шаг, с которым будет сдвигаться окно 
		/// подынтервалов длины IntervalDays
		/// </summary>
		[Required(ErrorMessage = "Не указан шаг интервала (в днях)")]
		[Range(1, int.MaxValue, ErrorMessage = "Размер шага должен быть больше 0")]
		public int StepDays { get; set; }
	}
}
