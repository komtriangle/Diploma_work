using Diploma.WebApi.Data.DataAccess;
using Diploma.WebApi.Data.TimeSeriesStorage;
using Diploma.WebApi.Data.TimeSeriesStorage.Entities;
using Diploma.WebApi.Logic;
using Diploma.WebApi.Logic.Models;
using Diploma.WebApi.Models;
using Diploma.WebApi.Models.Requests;
using Diploma.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EarthQuakesController : ControllerBase
	{
		private readonly IEathQuakesData _eathQuakesData;
		private readonly EarthQuakesDbContext _dbContext;

		public EarthQuakesController(IEathQuakesData eathQuakesData, EarthQuakesDbContext dbContext)
		{
			_eathQuakesData = eathQuakesData;
			_dbContext = dbContext;
		}

		[HttpPost("AnalyzeEarthQuakes")]
		public async  Task<ActionResult> PredictEarthQuakes([FromBody] AnalyzeEathQuakesRequest request)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			if(request.LongtitudeStart >= request.LongtitudeEnd)
			{
				return BadRequest("Значение долготы \"От\" должно быть меньше значение долготы \"До\"");
			}

			if(request.LatitudeStart >= request.LatitudeEnd)
			{
				return BadRequest("Значение долготы \"От\" должно быть меньше значение долготы \"До\"");
			}

			FormatOptions formatOptions = new FormatOptions()
			{
				LatitudeStart = request.LatitudeStart,
				LatitudeEnd = request.LatitudeEnd,
				LongtitudeStart = request.LongtitudeStart,
				LongtitudeEnd = request.LongtitudeEnd,
				IntervalDays = request.IntervalDays,
				StepDays = request.StepDays
			};

			try
			{
				var earthQuakes = _eathQuakesData.GetEarthquakes();

				FilterOptions filterOptions = new FilterOptions
				{
					LatitudeStart = request.LatitudeStart,
					LatitudeEnd = request.LatitudeEnd,
					LongtitudeStart = request.LongtitudeStart,
					LongtitudeEnd = request.LongtitudeEnd
				};

				List<Earthquake> earthquakesInGeograficCell = EarthQuakesAnalyzer.FilterByLogtitudeLatitute(earthQuakes,
					filterOptions).Where(x => x.Magnitude > 4.1 ).ToList();

				//earthquakesInGeograficCell = earthquakesInGeograficCell.Where(x => x.Time > new DateTime(1990, 1, 1) &&
				//x.Time < new DateTime(2005, 1, 1)).ToList();

				TimeInterval[] timeIntervals = EarthQuakesAnalyzer.Format(earthquakesInGeograficCell, formatOptions);


				TimeIntervalMagnitude[] timeSeriasMaxMagnitudes = EarthQuakesAnalyzer.MaxMagnitudeTimeSeries(earthquakesInGeograficCell, 
					timeIntervals);

				List<TimeIntervalCorrelationDimension> timeIntervalCorrelationDimensions =
						EarthQuakesAnalyzer.CalculateCorrelationDimensions(earthquakesInGeograficCell, timeIntervals);

				List<double> correlationDimensionDifferences = new List<double>() {0, 0};

				for(int i =2;i< timeIntervalCorrelationDimensions.Count; i++)
				{
					List<double> tmp = new List<double>() { timeIntervalCorrelationDimensions[i].MinDimension,
					timeIntervalCorrelationDimensions[i-1].MinDimension,
					timeIntervalCorrelationDimensions[i-2].MinDimension
					// timeIntervalCorrelationDimensions[i-3].MinDimension
					 };
					correlationDimensionDifferences.Add(tmp.Max() - tmp.Min());
				}

				AnalyzeEarthQuakesResponse response = new AnalyzeEarthQuakesResponse()
				{
					TimeSeriesMaxMagnitudes = timeSeriasMaxMagnitudes,
					TimeIntervalCorrelationDimensions = timeIntervalCorrelationDimensions.ToArray(),
					CorrelationDimensionDifferences = correlationDimensionDifferences.ToArray()
				};

			    _dbContext.Magnitudes.RemoveRange(_dbContext.Magnitudes);
				_dbContext.CorrelationDimensions.RemoveRange(_dbContext.CorrelationDimensions);


				for (int i = 0; i < timeIntervalCorrelationDimensions.Count; i++)
				{
					await _dbContext.Magnitudes.AddAsync(new Data.TimeSeriesStorage.Entities.Magnitudes
					{
						Time = timeSeriasMaxMagnitudes[i].Start,
						Value = timeSeriasMaxMagnitudes[i].Magnitude
					});

					await _dbContext.CorrelationDimensions.AddAsync(new Data.TimeSeriesStorage.Entities.CorrelationDimension
					{
						Time = timeIntervalCorrelationDimensions[i].Start,
						Value = correlationDimensionDifferences[i]
					});
				}

				await _dbContext.SaveChangesAsync();

				var result = new JsonResult(response);


				return result;
			}
			catch(Exception ex)
			{
				return BadRequest($"Ошибка во время анализа: {ex.Message}");
			}
		}

		[HttpPost("AutoCorrelation")]
		public ActionResult AutoCorrelationFunction(AnalyzeEathQuakesRequest request)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			if(request.LongtitudeStart >= request.LongtitudeEnd)
			{
				return BadRequest("Значение долготы \"От\" должно быть меньше значение долготы \"До\"");
			}

			if(request.LatitudeStart >= request.LatitudeEnd)
			{
				return BadRequest("Значение долготы \"От\" должно быть меньше значение долготы \"До\"");
			}

			var earthQuakes = _eathQuakesData.GetEarthquakes();

			FilterOptions filterOptions = new FilterOptions
			{
				LatitudeStart = request.LatitudeStart,
				LatitudeEnd = request.LatitudeEnd,
				LongtitudeStart = request.LongtitudeStart,
				LongtitudeEnd = request.LongtitudeEnd
			};

			List<Earthquake> earthquakesInGeograficCell = EarthQuakesAnalyzer.FilterByLogtitudeLatitute(earthQuakes,
				filterOptions);

			FormatOptions formatOptions = new FormatOptions
			{
				StepDays = request.StepDays,
				IntervalDays = request.IntervalDays,
			};

			TimeInterval[] timeIntervals = EarthQuakesAnalyzer.Format(earthquakesInGeograficCell, formatOptions);


			List<AutoCorrelactionFunctionResult> result = AutoCorrelation.Calculate(earthquakesInGeograficCell, timeIntervals);

			return new JsonResult(result);
		}

		/// <summary>
		/// Получание списка всех 
		/// зеслетрясений в заданной области
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("GetAllEarthQuakes")]
		public  ActionResult GetAllEarthQuakes([FromBody] GetEarthQuakesRequest request)
		{
			if(!ModelState.IsValid)
				return BadRequest(ModelState);

			if(request.LongtitudeStart >= request.LongtitudeEnd)
			{
				return BadRequest("Значение долготы \"От\" должно быть меньше значение долготы \"До\"");
			}

			if(request.LatitudeStart >= request.LatitudeEnd)
			{
				return BadRequest("Значение долготы \"От\" должно быть меньше значение долготы \"До\"");
			}

			var earthQuakes = _eathQuakesData.GetEarthquakes();

			FilterOptions filterOptions = new FilterOptions
			{
				LatitudeStart = request.LatitudeStart,
				LatitudeEnd = request.LatitudeEnd,
				LongtitudeStart = request.LongtitudeStart,
				LongtitudeEnd = request.LongtitudeEnd
			};

			List<Earthquake> earthquakesInGeograficCell = EarthQuakesAnalyzer.FilterByLogtitudeLatitute(earthQuakes,
				filterOptions);

			int days = 30;

			DateTime firstDate = earthquakesInGeograficCell.Min(t => t.Time);
			DateTime lastDate = earthquakesInGeograficCell.Max(t => t.Time);

			int totalDays = (int)(lastDate - firstDate).TotalDays;

			List<double> magnitudes = new List<double>();

			for(int i =0;i< totalDays; i += days)
			{
				double? maxMagnitude = earthquakesInGeograficCell.Where(x => x.Time >= firstDate.AddDays(i) &&
				x.Time <= firstDate.AddDays(i+days)).OrderByDescending(x => x.Magnitude).FirstOrDefault()?.Magnitude;

				if(maxMagnitude == null)
				{
					magnitudes.Add(0);
				}
				else
				{
					magnitudes.Add(maxMagnitude.Value);
				}
				
			}



			GetAllEarthQuakesResponse response = new GetAllEarthQuakesResponse
			{
				Magnitudes = magnitudes.ToArray()
			};


			return Ok(response);
		}

	}
}
