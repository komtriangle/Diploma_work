using Diploma.WebApi.Models;

namespace Diploma.WebApi.Data
{
    public interface ILoadData
    {
        /// <summary>
        /// Выполняется загрузку данных о землетрясениях в память
        /// </summary>
        /// <returns></returns>
        List<Earthquake> Load();
    }
}
