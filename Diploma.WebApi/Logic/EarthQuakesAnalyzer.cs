 using Diploma.WebApi.Logic.Formulas;
using Diploma.WebApi.Logic.Models;
using Diploma.WebApi.Models;

namespace Diploma.WebApi.Logic
{
	public static class EarthQuakesAnalyzer
	{

		private static double[] sigmas = new double[] {3, 2.9f, 2.8f, 2.7f, 2.6f, 2.5f, 2.4f, 2.3f, 2.2f, 2.1f, 2.0f,
			1.9f, 1.8f, 1.7f, 1.6f, 1.5f, 1.4f, 1.3f, 1.2f, 1.1f};

		private static  int k = 50;
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

			List<TimeIntervalCorrelationDimension> result = new List<TimeIntervalCorrelationDimension>();

			foreach(TimeInterval interval in intervals)
			{
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

				List<double> attractorSigma = new List<double>();

				double k_tmp = 0, b_tmp = 0;

				foreach(Attractor attractor in attractors)
				{
					var y = sigmas.Select(sigma => Math.Abs(Math.Log(correlationIntegral(attractor.Value, sigma))
						 / Math.Log(sigma)))
						.ToArray();

					attractorSigma = y.ToList();

					(b_tmp, k_tmp) = MNK.Calculate(sigmas, y);

					correlactionDimensions.Add(new CorrelationDimension
					{
						M = attractor.M,
						Value = k_tmp
					});
				}

				result.Add(new TimeIntervalCorrelationDimension
				{
					Start = interval.Start,
					End = interval.End,
					k_tmp = k_tmp,
					b_tmp = b_tmp,
					Attractor_tmp = attractorSigma,// .Select(v => Math.Log(v)).ToList(),
					CorrelationDimensions = correlactionDimensions,
					CorrelationDimension = 0,
					MinDimension = CalculateMinimalDimension(correlactionDimensions)

				});
			}

			return result;
		}

		private static List<Attractor> CalculateAttractors(TimeIntervalMagnitude[] magnitudes, int k)
		{
			List<Attractor> attractors = new List<Attractor>();
			for(int i = 2; i < k/2; i++)
			{
				Attractor attractor = new()
				{
					M = i
				};

				for(int j= 0; j < k - i; j++)
				{
					List<double> attractorItem = magnitudes.Skip(j).Take(i+1)
					.Select(m => m.Magnitude)
					.ToList();

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
