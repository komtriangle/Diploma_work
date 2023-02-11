using Diploma.WebApi.Data.DataAccess;
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

		public EarthQuakesController(IEathQuakesData eathQuakesData)
		{
			_eathQuakesData = eathQuakesData;
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
					filterOptions);

				TimeInterval[] timeIntervals = EarthQuakesAnalyzer.Format(earthquakesInGeograficCell, formatOptions);


				TimeIntervalMagnitude[] timeSeriasMaxMagnitudes = EarthQuakesAnalyzer.MaxMagnitudeTimeSeries(earthQuakes, 
					timeIntervals);

				List<TimeIntervalCorrelationDimension> timeIntervalCorrelationDimensions =
						EarthQuakesAnalyzer.CalculateCorrelationDimensions(earthQuakes, timeIntervals);

				AnalyzeEarthQuakesResponse response = new AnalyzeEarthQuakesResponse()
				{
					TimeSeriesMaxMagnitudes = timeSeriasMaxMagnitudes,
					TimeIntervalCorrelationDimensions = timeIntervalCorrelationDimensions.ToArray()
				};


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
	}
}
