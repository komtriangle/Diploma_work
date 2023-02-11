namespace Diploma.WebApi.Models
{
	public class AutoCorrelactionFunctionResult
	{
		public TimeInterval TimeInterval { get; set; }
		public List<AutoCorrelationFunctionValue> Values { get; set; }
	}

	public class AutoCorrelationFunctionValue
	{
		/// <summary>
		/// Шаг в днях
		/// </summary>
		public int T { get; set; }
		public double Value { get; set; }
	}
}
