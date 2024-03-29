﻿ using Diploma.WebApi.Logic.Formulas;
using Diploma.WebApi.Logic.Models;
using Diploma.WebApi.Models;
using System.Diagnostics;

namespace Diploma.WebApi.Logic
{
	public static class EarthQuakesAnalyzer
	{

		private static double[] sigmas = new double[] {0.01, 0.02, 0.03, 0.04, 0.05, 0.06, 0.07, 0.08, 0.09, 0.1, 0.15, 0.2, 0.25, 0.3, 0.35, 0.4, 0.45, 0.5, 0.55, 0.6, 0.65, 0.7, 0.75, 0.8, 0.85, 0.9, 0.95, 1, 1.05,
			1.1, 1.15, 1.2, 1.25, 1.3, 1.35, 1.4, 1.45, 1.5, 1.55, 1.6, 1.65, 1.7, 1.75, 1.8, 1.85, 1.9, 1.95, 2, 2.05, 2.1, 2.15, 2.2, 2.25, 2.3, 2.35, 2.4, 2.45,
			2.5, 2.55, 2.6, 2.65, 2.7, 2.75, 2.8, 2.85, 2.9, 2.95, 3, 3.05, 3.1, 3.15, 3.2, 3.25, 3.3, 3.35, 3.4, 3.45, 3.5, 3.55, 3.6, 3.65, 3.7, 3.75, 3.8, 3.85,
			3.9, 3.95, 4, 4.05, 4.1, 4.15, 4.2, 4.25, 4.3, 4.35, 4.4, 4.45, 4.5, 4.55, 4.6, 4.65, 4.7, 4.75, 4.8, 4.85, 4.9, 4.95, 5, 5.05, 5.1, 5.15, 5.2, 5.25,
			5.3, 5.35, 5.4, 5.45, 5.5, 5.55, 5.6, 5.65, 5.7, 5.75, 5.8, 5.85, 5.9, 5.95, 6, 6.05, 6.1, 6.15, 6.2, 6.25, 6.3, 6.35, 6.4, 6.45, 6.5, 6.55, 6.6, 6.65, 6.7, 6.75, 6.8, 6.85
			, 6.9, 6.95, 7, 7.05, 7.1, 7.15, 7.2, 7.25, 7.3, 7.35, 7.4, 7.45, 7.5, 7.55, 7.6, 7.65, 7.7, 7.75, 7.8, 7.85, 7.9, 7.95, 8, 8.05, 8.1, 8.15, 8.2, 8.25, 8.3, 8.35, 8.4, 8.45,
			8.5, 8.55, 8.6, 8.65, 8.7, 8.75, 8.8, 8.85, 8.9, 8.95, 9, 9.05, 9.1, 9.15, 9.2, 9.25, 9.3, 9.35, 9.4, 9.45, 9.5, 9.55, 9.6, 9.65, 9.7, 9.75, 9.8, 9.85, 9.9, 9.95, 10, 10.05,
			10.1, 10.15, 10.2, 10.25, 10.3, 10.35, 10.4, 10.45, 10.5, 10.55, 10.6, 10.65, 10.7, 10.75, 10.8, 10.85, 10.9, 10.95, 11, 11.05, 11.1, 11.15, 11.2, 11.25, 11.3, 11.35, 11.4,
			11.45, 11.5, 11.55, 11.6, 11.65, 11.7, 11.75, 11.8, 11.85, 11.9, 11.95, 12, 12.05, 12.1, 12.15, 12.2, 12.25, 12.3, 12.35, 12.4, 12.45, 12.5, 12.55, 12.6, 12.65, 12.7, 12.75
			, 12.8, 12.85, 12.9, 12.95, 13, 13.05, 13.1, 13.15, 13.2, 13.25, 13.3, 13.35, 13.4, 13.45, 13.5, 13.55, 13.6, 13.65, 13.7, 13.75, 13.8, 13.85, 13.9, 13.95, 14, 14.05, 14.1,
			14.15, 14.2, 14.25, 14.3, 14.35, 14.4, 14.45, 14.5, 14.55, 14.6, 14.65, 14.7, 14.75, 14.8, 14.85, 14.9, 14.95, 15, 15.05, 15.1, 15.15, 15.2, 15.25, 15.3, 15.35, 15.4, 15.45,
			15.5, 15.55, 15.6, 15.65, 15.7, 15.75, 15.8, 15.85, 15.9, 15.95, 16, 16.05, 16.1, 16.15, 16.2, 16.25, 16.3, 16.35, 16.4, 16.45, 16.5, 16.55, 16.6, 16.65, 16.7, 16.75, 16.8, 16.85, 16.9, 16.95, 17, 17.05, 17.1, 17.15, 17.2, 17.25, 17.3, 17.35, 17.4, 17.45, 17.5, 17.55, 17.6, 17.65, 17.7, 17.75, 17.8, 17.85, 17.9, 17.95, 18, 18.05, 18.1, 18.15, 18.2, 18.25, 18.3, 18.35, 18.4, 18.45, 18.5, 18.55, 18.6, 18.65, 18.7, 18.75, 18.8, 18.85, 18.9, 18.95, 19, 19.05, 19.1, 19.15, 19.2, 19.25, 19.3, 19.35, 19.4, 19.45, 19.5, 19.55, 19.6, 19.65, 19.7, 19.75, 19.8, 19.85, 19.9, 19.95, 20, 20.05, 20.1, 20.15, 20.2, 20.25, 20.3, 20.35, 20.4, 20.45, 20.5, 20.55, 20.6, 20.65, 20.7, 20.75, 20.8, 20.85, 20.9, 20.95, 21, 21.05, 21.1, 21.15, 21.2, 21.25, 21.3, 21.35, 21.4, 21.45, 21.5, 21.55, 21.6, 21.65, 21.7, 21.75, 21.8, 21.85, 21.9, 21.95, 22, 22.05, 22.1, 22.15, 22.2, 22.25, 22.3, 22.35, 22.4, 22.45, 22.5, 22.55, 22.6, 22.65, 22.7, 22.75, 22.8, 22.85, 22.9, 22.95, 23, 23.05, 23.1, 23.15, 23.2, 23.25, 23.3, 23.35, 23.4, 23.45, 23.5, 23.55, 23.6, 23.65, 23.7, 23.75, 23.8, 23.85, 23.9, 23.95, 24, 24.05, 24.1, 24.15, 24.2, 24.25, 24.3, 24.35, 24.4, 24.45, 24.5, 24.55, 24.6, 24.65, 24.7, 24.75, 24.8, 24.85, 24.9, 24.95, 25, 25.05, 25.1, 25.15, 25.2, 25.25, 25.3, 25.35, 25.4, 25.45, 25.5, 25.55, 25.6, 25.65, 25.7, 25.75, 25.8, 25.85, 25.9, 25.95, 26, 26.05, 26.1, 26.15, 26.2, 26.25, 26.3, 26.35, 26.4, 26.45, 26.5, 26.55, 26.6, 26.65, 26.7, 26.75, 26.8, 26.85, 26.9, 26.95, 27, 27.05, 27.1, 27.15, 27.2, 27.25, 27.3, 27.35, 27.4, 27.45, 27.5, 27.55, 27.6, 27.65, 27.7, 27.75, 27.8, 27.85, 27.9, 27.95, 28, 28.05, 28.1, 28.15, 28.2, 28.25, 28.3, 28.35, 28.4, 28.45, 28.5, 28.55, 28.6, 28.65, 28.7, 28.75, 28.8, 28.85, 28.9, 28.95, 29, 29.05, 29.1, 29.15, 29.2, 29.25, 29.3, 29.35, 29.4, 29.45, 29.5, 29.55, 29.6, 29.65, 29.7, 29.75, 29.8, 29.85, 29.9, 29.95, 30, 30.05, 30.1, 30.15, 30.2, 30.25, 30.3, 30.35, 30.4, 30.45, 30.5, 30.55, 30.6, 30.65, 30.7, 30.75, 30.8, 30.85, 30.9, 30.95, 31, 31.05, 31.1, 31.15, 31.2, 31.25, 31.3, 31.35, 31.4, 31.45, 31.5, 31.55, 31.6, 31.65, 31.7, 31.75, 31.8, 31.85, 31.9, 31.95, 32, 32.05, 32.1, 32.15, 32.2, 32.25, 32.3, 32.35, 32.4, 32.45, 32.5, 32.55, 32.6, 32.65, 32.7, 32.75, 32.8, 32.85, 32.9, 32.95, 33, 33.05, 33.1, 33.15, 33.2, 33.25, 33.3, 33.35, 33.4, 33.45, 33.5, 33.55, 33.6, 33.65, 33.7, 33.75, 33.8, 33.85, 33.9, 33.95, 34, 34.05, 34.1, 34.15, 34.2, 34.25, 34.3, 34.35, 34.4, 34.45, 34.5, 34.55, 34.6, 34.65, 34.7, 34.75, 34.8, 34.85, 34.9, 34.95, 35};
		private static object _locker = new object();

		private static  int k = 20;
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
			return intervals.Select((interval, i) =>
			{
				IEnumerable<Earthquake> earthQuakesInInterval;
				if(i == 0)
				{
					earthQuakesInInterval = earthquakes.Where(e => interval.EarthQuakes.Contains(e.Id));
				}
				else
				{
					earthQuakesInInterval = earthquakes.Where(e => interval.EarthQuakes.Contains(e.Id) && !intervals[i-1].EarthQuakes.Contains(e.Id));
				}



				return new TimeIntervalMagnitude
				{
					Start = interval.Start,
					End = interval.End,
					Magnitude = earthQuakesInInterval.Any()
						? earthQuakesInInterval.Max(e => e.Magnitude)
						: 4.1
				};
			})
			.ToArray();
		}

		public static List<TimeIntervalCorrelationDimension> CalculateCorrelationDimensions(List<Earthquake> earthquakes, TimeInterval[] intervals)
		{
			int totalCount = intervals.Count() * (k / 2 - 1);
			int count = 0;
			Stopwatch sw = Stopwatch.StartNew();

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
					TimeInterval[] subintervals = CreateIntervals(earthquakesInInterval, options, interval.Start, interval.End);

					TimeIntervalMagnitude[] timeIntervalMagnitudes = MaxMagnitudeTimeSeries(earthquakesInInterval, subintervals);
					Console.WriteLine(String.Join(", ", subintervals.Select(x => $"{x.Start.ToShortDateString()}-{x.End.ToShortDateString()}")));
					Console.WriteLine(String.Join(", ", timeIntervalMagnitudes.Select(x => x.Magnitude.ToString(System.Globalization.CultureInfo.InvariantCulture))));

					List<Attractor> attractors = CalculateAttractors(timeIntervalMagnitudes, subintervals.Length);

					List<CorrelationDimension> correlactionDimensions = new List<CorrelationDimension>();

					List<AttractorSigma> attractorSigma = new List<AttractorSigma>();

					


					attractors.AsParallel()
					  .WithDegreeOfParallelism(50)
					  .ForAll(attractor =>
					  {
						  double k_tmp = 0, b_tmp = 0;

						  Interlocked.Increment(ref count);
						  Console.WriteLine($"{count*100.0/totalCount}%.  {sw.ElapsedMilliseconds/1000.0} сек.");
						  Console.WriteLine($"Аттрактор размера: {attractor.M}");

						  (double, double)[] y = sigmas.Select(sigma => (correlationIntegral(attractor.Value, sigma), sigma))
							.ToArray();

						  double maxBorder = Math.Log(y.Min(x => x.Item1)) - 10;
						  double minBorder = Math.Log(y.Max(x => x.Item1));

						  var new_y = y.Where(v => Math.Log(v.Item1) > maxBorder && Math.Log(v.Item1) < minBorder).ToArray();

						  //if(!new_y.Any())
						  //{
							 // minBorder = Math.Log(y.Max(x => x.Item1) - 0.4);

							 // new_y = y.Where(v => Math.Log(v.Item1) >= maxBorder && Math.Log(v.Item1) <= minBorder).ToArray();

							 // if(!new_y.Any())
							 // {

								//  minBorder = Math.Log(y.Max(x => x.Item1) - 0.35);

								//  new_y = y.Where(v => Math.Log(v.Item1) >= maxBorder && Math.Log(v.Item1) <= minBorder).ToArray();
							 // }

							 // if(!new_y.Any())
							 // {
								//  minBorder = Math.Log(y.Max(x => x.Item1) - 0.35);

								//  new_y = y.Where(v => Math.Log(v.Item1) >= maxBorder && Math.Log(v.Item1) <= minBorder).ToArray();
							 // }

							 // if(!new_y.Any())
							 // {
								//  minBorder = Math.Log(y.Max(x => x.Item1) - 0.35);

								//  new_y = y.Where(v => Math.Log(v.Item1) >= maxBorder && Math.Log(v.Item1) <= minBorder).ToArray();
							 // }

							 // if(!new_y.Any())
							 // {
								//  new_y = y;
							 // }
						  //}

						  y = new_y;


						  (b_tmp, k_tmp) = MNK.Calculate(y.Select(v => Math.Log(v.Item2)).ToArray(),
								y.Select(v => Math.Log(v.Item1)).ToArray());


						  double[] attractoSigmaValues = sigmas.Select(sigma =>
						  {
							  double value = Math.Log(correlationIntegral(attractor.Value, sigma));

							  if(double.IsInfinity(value))
							  {
								  Console.WriteLine($"Интеграл корреляционного интеграла Infinity.  {interval.Start}-{interval.End}. {attractor.M}");
								  value = 0.5;
							  }

							  return value;
						  })
							.ToArray();

						  if(k_tmp < 5)
						  {

						  }

						  k_tmp = k_tmp > double.MaxValue
							? 150
							: k_tmp;

						  k_tmp = k_tmp < double.MinValue
							? 0.5
							: k_tmp;

						  b_tmp = b_tmp > double.MaxValue
							? 100
							: b_tmp;

						  b_tmp = b_tmp < double.MinValue
							? 0
							: b_tmp;


						  if(double.IsNaN(k_tmp))
						  {
							  k_tmp = 0.5;
						  }

						  if(double.IsNaN(b_tmp))
						  {
							  b_tmp = 0;
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
						  }
					  });


					int minDimension = CalculateMinimalDimension(correlactionDimensions.OrderBy(x => x.M).ToList());

					if(double.IsInfinity(minDimension))
					{
						minDimension = 100;
					}

					lock(_locker)
					{
						var corrDimensions = correlactionDimensions.OrderBy(x => x.M).ToList();

						(double b_tmp, double k_tmp) = MNK.Calculate(corrDimensions.Select(x => (double)x.M).ToArray(),
							corrDimensions.Select(x => x.Value).ToArray());

						result.Add(new TimeIntervalCorrelationDimension
						{
							Start = interval.Start,
							End = interval.End,
							Attractor_tmp = attractorSigma,
							CorrelationDimensions = corrDimensions,
							CorrelationDimension = 0,
							MinDimension = minDimension,
							k_tmp_dim = k_tmp,
							b_tmp_dim = b_tmp

						});
					}
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
		internal static TimeInterval[] CreateIntervals(List<Earthquake> earthquakes, FormatOptions options, DateTime? firstEarthQuakeDate = null, DateTime? lastEarthQuakeDate = null)
		{
			if(firstEarthQuakeDate == null)
			{
				firstEarthQuakeDate = GetFirstEarthQuakeDate(earthquakes);
			}
			
			if(lastEarthQuakeDate == null)
			{
				lastEarthQuakeDate = GetLastEarthQuakeDate(earthquakes);
			}

			int countIntervals = ((int)((lastEarthQuakeDate.Value - firstEarthQuakeDate.Value).TotalDays) -
				options.IntervalDays + options.StepDays) / options.StepDays;

			(DateTime, DateTime)[] timeIntervals = new (DateTime, DateTime)[countIntervals];

			for(int i = 0; i < countIntervals; i++)
			{
				timeIntervals[i] = (firstEarthQuakeDate.Value.AddDays(i * options.StepDays),
					firstEarthQuakeDate.Value.AddDays(i * options.StepDays + options.IntervalDays));
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

			return earthquakes;
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
			for(int i =1; i < correlationDimensions.Count; i++)
			{
				if(correlationDimensions[i].Value < correlationDimensions[i - 1].Value)
				{
					return correlationDimensions[i].M;
				}
			}

			return correlationDimensions.Last().M;
		}


	}
}
