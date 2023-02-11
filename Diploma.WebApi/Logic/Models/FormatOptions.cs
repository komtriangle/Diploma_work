namespace Diploma.WebApi.Logic.Models
{
	public class FormatOptions
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

		/// <summary>
		/// Длина временного интервала, на который будет
		/// разбиваться весь интервал (в днях)
		/// </summary>
		public int IntervalDays { get; set; }

		/// <summary>
		/// Шаг, с которым будет сдвигаться окно 
		/// подынтервалов длины IntervalDays
		/// </summary>
		public int StepDays { get; set; }
	}
}
