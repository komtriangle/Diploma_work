using Diploma.WebApi.Models;
using Microsoft.Extensions.Caching.Memory;

namespace Diploma.WebApi.Data.DataAccess
{
	public class EathQuakesData: IEathQuakesData
	{
		private readonly IMemoryCache _cache;
		private readonly ILoadData _loadData;
		private const string cacheKey = "EARTHQUAKES";


		public EathQuakesData(IMemoryCache cache, ILoadData loadData)
		{
			_cache = cache;
			_loadData = loadData;
		}

		public List<Earthquake> GetEarthquakes()
		{
			_cache.TryGetValue(cacheKey, out List<Earthquake> earthQuakes);

			if(earthQuakes == null)
			{
				earthQuakes = _loadData.Load();

				_cache.Set(cacheKey, earthQuakes);
			}

			return earthQuakes;
		}
	}
}
