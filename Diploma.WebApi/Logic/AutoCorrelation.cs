using Diploma.WebApi.Extensions;
using Diploma.WebApi.Logic.Models;
using Diploma.WebApi.Models;

namespace Diploma.WebApi.Logic
{
	public static class AutoCorrelation
	{
		public static List<AutoCorrelactionFunctionResult> Calculate(List<Earthquake> earthquakes, TimeInterval[] timeIntervals)
		{

			List<AutoCorrelactionFunctionResult> results = new List<AutoCorrelactionFunctionResult>();

			foreach(TimeInterval timeInterval in timeIntervals.Where((v, i) => i%5 == 0))
			{
				List<Earthquake> earthquakesInTimeInterval = timeInterval.GetEarthquakes(earthquakes);

				results.Add(new AutoCorrelactionFunctionResult
				{
					TimeInterval = timeInterval,
					Values = CalculateForInterval(earthquakesInTimeInterval)
				});
			}

			return results;
		}


		/// <summary>
		/// Рассчитывает оптимальное значение 
		/// автокорреляционной функции в днях
		/// </summary>
		/// <param name="earthquakes"></param>
		/// <returns></returns>
		private static List<AutoCorrelationFunctionValue> CalculateForInterval(List<Earthquake> earthquakes)
		{
			List<Earthquake> centerterEarthQuakes = CenterEarthQuakes(earthquakes);

			DateTime firstEarthQuakeDate = EarthQuakesAnalyzer.GetFirstEarthQuakeDate(centerterEarthQuakes);
			DateTime lastEarthQuakeDate = EarthQuakesAnalyzer.GetLastEarthQuakeDate(centerterEarthQuakes);

			int daysBettwenFirstAndLast = (lastEarthQuakeDate - firstEarthQuakeDate).Days;

			int minT = (int)Math.Ceiling(daysBettwenFirstAndLast* 1.0/ earthquakes.Count);

			int maxT = (int)Math.Ceiling(daysBettwenFirstAndLast / 10.0);

			List<AutoCorrelationFunctionValue> funcValues = new List<AutoCorrelationFunctionValue>();

			for(int i = minT; i <= maxT; i++)
			{
				FormatOptions options = new()
				{
					IntervalDays = i,
					StepDays = 1
				};

				TimeInterval[] intervals = EarthQuakesAnalyzer.CreateIntervals(centerterEarthQuakes, options);

				funcValues.Add(new AutoCorrelationFunctionValue
				{
					T= i,
					Value = IntervalCalculate(centerterEarthQuakes, intervals, daysBettwenFirstAndLast, i),

				});
			}

			return funcValues;

		}

		private static double IntervalCalculate(List<Earthquake> earthquakes, TimeInterval[] intervals, int totalDays, int t)
		{
			TimeIntervalMagnitude[] intervalMagnitudes = EarthQuakesAnalyzer.MaxMagnitudeTimeSeries(earthquakes, intervals);
			double r = 0;

			for(int i = 0; i< intervals.Length-1; i++)
			{
				r += intervalMagnitudes[i].Magnitude*intervalMagnitudes[i+1].Magnitude;
			}
			return r / intervals.Length;
		}

		/// <summary>
		/// Центрирует ряд землетрясений
		/// (центрирются значения магнитуды)
		/// </summary>
		/// <param name="earthquakes"></param>
		/// <returns></returns>
		private static List<Earthquake> CenterEarthQuakes(List<Earthquake> earthquakes)
		{
			double meanMagnitude = earthquakes.Average(e => e.Magnitude);

			return earthquakes.Select(x =>
			{
				return new Earthquake
				{
					Magnitude = x.Magnitude - meanMagnitude,
					Id = x.Id,
					Latitude = x.Latitude,
					Longitude = x.Longitude,
					Time = x.Time,
				};
			})
			.ToList();
		}
	}
}
