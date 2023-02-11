namespace Diploma.WebApi.Models
{
    /// <summary>
    /// Землетрясение
    /// </summary>
    public class Earthquake
    {
        /// <summary>
        /// Номер землетрясения
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Время и дата землятресения
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Широта
        /// </summary>
        public double Latitude { get; set; }

        /// <summary>
        /// Долгота
        /// </summary>
        public double Longitude { get; set; }

        /// <summary>
        /// Магнитуда
        /// </summary>
        public double Magnitude { get; set; }

    }

}
