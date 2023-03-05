using Diploma.WebApi.Data.DataAccess;
using Diploma.WebApi.Data.TimeSeriesStorage;
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
		public ActionResult PredictEarthQuakes([FromBody] AnalyzeEathQuakesRequest request)
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
					filterOptions).Where(x => x.Magnitude > 4.3).ToList();

				TimeInterval[] timeIntervals = EarthQuakesAnalyzer.Format(earthquakesInGeograficCell, formatOptions);


				TimeIntervalMagnitude[] timeSeriasMaxMagnitudes = EarthQuakesAnalyzer.MaxMagnitudeTimeSeries(earthquakesInGeograficCell, 
					timeIntervals);

				List<TimeIntervalCorrelationDimension> timeIntervalCorrelationDimensions =
						EarthQuakesAnalyzer.CalculateCorrelationDimensions(earthquakesInGeograficCell, timeIntervals);

				AnalyzeEarthQuakesResponse response = new AnalyzeEarthQuakesResponse()
				{
					TimeSeriesMaxMagnitudes = timeSeriasMaxMagnitudes,
					TimeIntervalCorrelationDimensions = timeIntervalCorrelationDimensions.ToArray()
				};

			//	_dbContext.CorrelationDimensions.RemoveRange(_dbContext.CorrelationDimensions);
				//_dbContext.SaveChanges();

				foreach(var interval in timeIntervalCorrelationDimensions)
				{
					_dbContext.CorrelationDimensions.Add(new Data.TimeSeriesStorage.Entities.CorrelationDimension()
					{
						Time = interval.Start,
						Value = interval.MinDimension
					});
				}

				//_dbContext.SaveChanges();


				return new JsonResult(response);
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

			GetAllEarthQuakesResponse response = new GetAllEarthQuakesResponse
			{
				Earthquakes = earthquakesInGeograficCell.GroupBy(x => (x.Time.Year,x.Time.Month))
				.Select(x => x.OrderByDescending(x => x.Magnitude).First()).ToList(),
				GroupedEarthQuakes = earthQuakes.GroupBy(x => x.Magnitude).Select(x => new GroupedEarthQuakes
				{
					Count = x.Count(),
					MagnitudeValue = x.First().Magnitude
				}).OrderBy(x => x.MagnitudeValue).ToList(),
				Magnitudes = earthquakesInGeograficCell.GroupBy(x => (x.Time.Year, x.Time.Month))
				.Select(x => x.OrderByDescending(x => x.Magnitude).First().Magnitude).ToArray()
			};

			return Ok(response);
		}

	}
}
