using Diploma.WebApi.Models;

namespace Diploma.WebApi.Data.DataAccess
{
	public interface IEathQuakesData
	{
		List<Earthquake> GetEarthquakes();
	}
}
