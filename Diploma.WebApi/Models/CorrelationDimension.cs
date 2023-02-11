namespace Diploma.WebApi.Models
{
	/// <summary>
	/// Модель корреляционной размерность
	/// </summary>
	public class CorrelationDimension
	{
		/// <summary>
		/// Размерность вложения
		/// </summary>
		public int M { get; set; }

		/// <summary>
		/// Значение размерности
		/// </summary>
		public double Value { get; set; }
	}
}
