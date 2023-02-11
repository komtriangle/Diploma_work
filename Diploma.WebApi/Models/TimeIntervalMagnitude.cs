namespace Diploma.WebApi.Models
{
	/// <summary>
	/// Максимальная магнитуда во временном интервале
	/// </summary>
	public class TimeIntervalMagnitude
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
		/// Значение магнитуды
		/// </summary>
		public double Magnitude { get; set; }

		/// <summary>
		/// Строковое представление временного
		/// интервала
		/// </summary>
		public string DateIntervalStr
		{
			get
			{
				return $"{Start.ToShortDateString()}-{End.Date.ToShortDateString()}";
			}
		}
	}
}
