 using Diploma.WebApi.Logic.Formulas;
using Diploma.WebApi.Logic.Models;
using Diploma.WebApi.Models;

namespace Diploma.WebApi.Logic
{
	public static class EarthQuakesAnalyzer
	{

		private static double[] sigmas = new double[] { 0.10, 0.15, 0.20, 0.25, 0.30, 0.35, 0.40, 0.45, 0.50, 0.55, 0.60, 0.65,
			0.70, 0.75, 0.80, 0.85, 0.90, 0.95,
			1.05, 1.10, 1.15, 1.20, 1.25, 1.30, 1.35, 1.40, 1.45, 1.50, 1.55, 1.60, 1.65, 1.70, 1.75, 1.80, 1.85, 1.90,
			1.95, 2.00, 2.05, 2.10, 2.15, 2.20, 2.25, 2.30, 2.35, 2.40, 2.45, 2.50, 2.55, 2.60, 2.65, 2.70, 2.75, 2.80,
			2.85, 2.90, 2.95, 3.00, 3.05, 3.10, 3.15, 3.20, 3.25, 3.30, 3.35, 3.40, 3.45, 3.50, 3.55, 3.60, 3.65, 3.70, 
			3.75, 3.80, 3.85, 3.90, 3.95, 4.00, 4.05, 4.10, 4.15, 4.20, 4.25, 4.30, 4.35, 4.40, 4.45, 4.50, 4.55, 4.60,
			4.65, 4.70, 4.75, 4.80, 4.85, 4.90, 4.95, 5.00, 5.05, 5.10, 5.15, 5.20, 5.25, 5.30, 5.35, 5.40, 5.45, 5.50, 
			5.55, 5.60, 5.65, 5.70, 5.75, 5.80, 5.85, 5.90, 5.95, 6.00, 6.05, 6.10, 6.15, 6.20, 6.25, 6.30, 6.35, 6.40 };

		private static object _locker = new object();

		private static  int k = 120;
		/// <summary>
		/// Рассчитывает временной ряд максимальных
		/// магнитуд для каждого интервала
		/// </summary>
		/// <param name="earthquakes">Список землетрясений</param>
		/// <param name="intervals">Список интервалов. Каждый элемент содержит
		/// список Id землетрясений, относяшихся к этому интервалу</param>
		/// <returns></returns>
		public static TimeIntervalMagnitude[] MaxMagnitudeTimeSeries(List<Earthquake> earthquakes, TimeInterval[] intervals)
		{
			return intervals.Select(interval =>
			{
				var earthQuakesInInterval = earthquakes.Where(e => interval.EarthQuakes.Contains(e.Id));

				return new TimeIntervalMagnitude
				{
					Start = interval.Start,
					End = interval.End,
					Magnitude = earthQuakesInInterval.Any()
						? earthQuakesInInterval.Max(e => e.Magnitude)
						: 0
				};
			})
			.ToArray();
		}

		public static List<TimeIntervalCorrelationDimension> CalculateCorrelationDimensions(List<Earthquake> earthquakes, TimeInterval[] intervals)
		{
			int totalCount = intervals.Count() * (k / 2 - 1);
			int count = 0;

			List<TimeIntervalCorrelationDimension> result = new List<TimeIntervalCorrelationDimension>();

			intervals.AsParallel().WithDegreeOfParallelism(10)
				.ForAll(interval =>
				{
					Console.WriteLine($"Временной интервал: {interval.Start}-{interval.End}");
					List<Earthquake> earthquakesInInterval = earthquakes
						.Where(e => interval.EarthQuakes.Contains(e.Id))
						.ToList();

					FormatOptions options = new FormatOptions
					{
						IntervalDays = (interval.End - interval.Start).Days / k,
						StepDays = (interval.End - interval.Start).Days / k
					};
					TimeInterval[] subintervals = CreateIntervals(earthquakesInInterval, options);

					TimeIntervalMagnitude[] timeIntervalMagnitudes = MaxMagnitudeTimeSeries(earthquakesInInterval, subintervals);

					List<Attractor> attractors = CalculateAttractors(timeIntervalMagnitudes, subintervals.Length);

					List<CorrelationDimension> correlactionDimensions = new List<CorrelationDimension>();

					List<AttractorSigma> attractorSigma = new List<AttractorSigma>();

					double k_tmp = 0, b_tmp = 0;

					attractors.AsParallel()
					  .WithDegreeOfParallelism(20)
					  .ForAll(attractor =>
					  {
						  Interlocked.Increment(ref count);
						  Console.WriteLine($"{count*100.0/totalCount}%");
						  Console.WriteLine($"Аттрактор размера: {attractor.M}");
						  (double, double)[] y = sigmas.Select(sigma => (correlationIntegral(attractor.Value, sigma), sigma))
							.ToArray();

						  double maxBorder = Math.Log(y.Min(x => x.Item1) + 0.01);
						  var a = Math.Log(y.Max(x => x.Item1));
						  double minBorder = Math.Log(y.Max(x => x.Item1) - 0.5);

						  var new_y = y.Where(v => Math.Log(v.Item1) >= maxBorder && Math.Log(v.Item1) <= minBorder).ToArray();

						  if(!new_y.Any())
						  {
							  new_y = y;
						  }

						  y = new_y;


						  (b_tmp, k_tmp) = MNK.Calculate(y.Select(v => v.Item2).ToArray(),
								y.Select(v => v.Item1).ToArray());


						  double[] attractoSigmaValues = sigmas.Select(sigma =>
						  {
							  double value = Math.Log(correlationIntegral(attractor.Value, sigma));

							  if(double.IsInfinity(value))
							  {
								  Console.WriteLine($"Интеграл корреляционного интеграла Infinity.  {interval.Start}-{interval.End}. {attractor.M}");
								  value = 0;
							  }

							  return value;
						  })
							.ToArray();


						  if(Double.IsInfinity(k_tmp))
						  {
							  Console.WriteLine($"k_tmp Infinity.  {interval.Start}-{interval.End}. {attractor.M}");

							  k_tmp = 0.0;
						  }

						  if(double.IsInfinity(b_tmp))
						  {
							  Console.WriteLine($"b_tmp Infinity.  {interval.Start}-{interval.End}. {attractor.M}");

							  b_tmp = 0.0;
						  }

						  lock(_locker)
						  {
							  attractorSigma.Add(new AttractorSigma
							  {
								  Values = attractoSigmaValues,
								  K = k_tmp,
								  B = b_tmp
							  });

							  correlactionDimensions.Add(new CorrelationDimension
							  {
								  M = attractor.M,
								  Value = k_tmp
							  });


							  result.Add(new TimeIntervalCorrelationDimension
							  {
								  Start = interval.Start,
								  End = interval.End,
								  k_tmp = k_tmp,
								  b_tmp = b_tmp,
								  Attractor_tmp = attractorSigma,
								  CorrelationDimensions = correlactionDimensions,
								  CorrelationDimension = 0,
								  MinDimension = CalculateMinimalDimension(correlactionDimensions)

							  });
						  }
					  });
				});

				  

			return result.OrderBy(x => x.Start).ToList();
		}

		private static List<Attractor> CalculateAttractors(TimeIntervalMagnitude[] magnitudes, int n)
		{
			List<Attractor> attractors = new List<Attractor>();
			for(int k = 2; k < n / 2; k++)
			{
				Attractor attractor = new()
				{
					M = k
				};

				for(int i = 0; i < n - k + 1; i++)
				{
					List<double> attractorItem = magnitudes.Skip(i).Take(k).Select(x => x.Magnitude).ToList();
					attractor.Value.Add(attractorItem);
				}
				attractors.Add(attractor);
			}

			return attractors;
		}

		/// <summary>
		/// Корреляционный интеграл
		/// </summary>
		/// <returns></returns>
		private static Func<List<List<double>>, double, double> correlationIntegral
		{
			get
			{
				return (attractor, sigma) =>
				{
					double sum = 0;
					for(int i = 0; i < attractor.Count; i++)
					{
						for(int j = 0; j < attractor.Count; j++)
						{
							if(i != j)
							{
								sum += Heaviside.Calculate(sigma - EuclidianDistance.Calculate(attractor[i].ToArray(), attractor[j].ToArray()));
							}
						}
					}
					sum = sum == 0 ? 1 : sum;
					return sum / Math.Pow(attractor.Count, 2);
				};
			}
		}


		/// <summary>
		/// группирует Землетрясения по интервалам
		/// </summary>
		/// <param name="earthquakes">Список  землетрясений</param>
		/// <param name="options">Правила форматирования</param>
		/// <returns>Массив интервалов. Каждый массик содержит номера землятрений, 
		/// которые попали в этот интервал</returns>
		public static TimeInterval[] Format(List<Earthquake> earthquakes, FormatOptions options)
		{
			TimeInterval[] eathQuakesIntervals = CreateIntervals(earthquakes, options);
			return eathQuakesIntervals;
		}


		/// <summary>
		/// Группирует землетрясения по интервалам
		/// </summary>
		/// <param name="earthquakes">Список землетрясений</param>
		/// <param name="options">Правила группировки</param>
		/// <returns>Массив интервалов. Каждый массик содержит номера землятрений, 
		/// которые попали в этот интервал</returns>
		internal static TimeInterval[] CreateIntervals(List<Earthquake> earthquakes, FormatOptions options)
		{
			DateTime firstEarthQuakeDate = GetFirstEarthQuakeDate(earthquakes);
			DateTime lastEarthQuakeDate = GetLastEarthQuakeDate(earthquakes);

			int countIntervals = ((int)((lastEarthQuakeDate - firstEarthQuakeDate).TotalDays) -
				options.IntervalDays + options.StepDays) / options.StepDays;

			(DateTime, DateTime)[] timeIntervals = new (DateTime, DateTime)[countIntervals];

			for(int i = 0; i < countIntervals; i++)
			{
				timeIntervals[i] = (firstEarthQuakeDate.AddDays(i * options.StepDays),
					firstEarthQuakeDate.AddDays(i * options.StepDays + options.IntervalDays));
			}

			return timeIntervals.Select(timeInterval =>
			{
				return new TimeInterval
				{
					Start = timeInterval.Item1,
					End = timeInterval.Item2,
					EarthQuakes = earthquakes
				   .Where(e => e.Time >= timeInterval.Item1 &&
						e.Time <= timeInterval.Item2)
					 .Select(e => e.Id)
					 .ToArray()
				};
			})
			.ToArray();


		}

		/// <summary>
		/// Фильтрует список землетрясений по интервалам
		/// широты и долготы
		/// </summary>
		/// <param name="earthquakes"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static List<Earthquake> FilterByLogtitudeLatitute(List<Earthquake> earthquakes, FilterOptions options)
		{
			List<Earthquake> result = earthquakes
				.Where(e => e.Longitude >= options.LongtitudeStart &&
					e.Longitude <= options.LongtitudeEnd &&
					e.Latitude >= options.LatitudeStart &&
					e.Longitude <= options.LongtitudeEnd)
				.ToList();

			return result;
		}



		/// <summary>
		/// Дата первого землетрясения
		/// </summary>
		/// <param name="earthquakes">Список землетрясения</param>
		/// <returns></returns>
		public static DateTime GetFirstEarthQuakeDate(List<Earthquake> earthquakes)
		{
			return earthquakes.Min(x => x.Time);
		}

		/// <summary>
		/// Дата последнего землетрясения
		/// </summary>
		/// <param name="earthquakes">Список землетрясения</param>
		/// <returns></returns>
		public static DateTime GetLastEarthQuakeDate(List<Earthquake> earthquakes)
		{
			return earthquakes.Max(x => x.Time);
		}

		/// <summary>
		/// Рассчет минимальное размерности вложения,
		/// при которой наступает насыщение корреляционной размерности
		/// </summary>
		/// <param name="attractors"></param>
		/// <returns></returns>
		private static int CalculateMinimalDimension(List<CorrelationDimension> correlationDimensions)
		{
			return correlationDimensions
				.OrderByDescending(c => Math.Abs(c.Value))
				.First()
				.M;
		}


	}
}
