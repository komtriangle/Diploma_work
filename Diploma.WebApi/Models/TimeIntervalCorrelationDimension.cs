namespace Diploma.WebApi.Models
{
	/// <summary>
	/// Корреляционнная размерность для временного интервала
	/// </summary>
	public class TimeIntervalCorrelationDimension
	{
		/// <summary>
		/// Начало интервала
		/// </summary>
		public DateTime Start { get; set; }
		/// <summary>
		/// Конец интервала
		/// </summary>
		public DateTime End { get; set; }

		public double k_tmp { get; set; }
		public double b_tmp { get; set; }

		public List<double> Attractor_tmp { get; set; }

		/// <summary>
		/// Маасив корреляционных размерностей 
		/// (номер элемента - размерность вложения)
		/// </summary>
		public List<CorrelationDimension> CorrelationDimensions { get; set; }

		/// <summary>
		/// Значение корреляционной размерности
		/// </summary>
		public double CorrelationDimension { get; set; }

		/// <summary>
		/// Минимальная размерность вложения, при которой 
		/// достигается насыщение
		/// </summary>
		public int MinDimension { get; set; }
	}
}
