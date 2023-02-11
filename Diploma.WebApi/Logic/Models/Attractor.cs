namespace Diploma.WebApi.Logic.Models
{
	/// <summary>
	/// Модель аттрактора
	/// </summary>
	public class Attractor
	{
		/// <summary>
		/// Размерность вложения, на которой
		/// был построен аттрактор
		/// </summary>
		public int M { get; set; }

		/// <summary>
		/// Объект аттрактор
		/// </summary>
		public List<List<double>> Value { get; set; }

		public Attractor()
		{
			Value = new List<List<double>>();
		}
	}
}
