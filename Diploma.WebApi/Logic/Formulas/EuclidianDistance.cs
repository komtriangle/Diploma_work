namespace Diploma.WebApi.Logic.Formulas
{
	public static class EuclidianDistance
	{

		/// <summary>
		/// Рассчет Евклидова расстояния
		/// между точками
		/// </summary>
		/// <param name="a">Координаты первой точки</param>
		/// <param name="b">Координаты второй точки</param>
		/// <returns></returns>
		public static double Calculate(double[] a, double[] b)
		{
			if(a.Length != b.Length)
			{
				throw new ArgumentException("Массивы a и b должны иметь одинаковую длинну");
			}

			if(!a.Any())
			{
				throw new ArgumentException("Массимы не могут быть пустыми");
			}

			return Math.Sqrt(a.Select((_, i) =>
				Math.Pow(a[i] - b[i], 2))
				.Sum());
		}
	}
}
