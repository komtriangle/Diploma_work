using Diploma.WebApi.Logic.Formulas;
using System;
using Xunit;

namespace Diploma.Tests.Formulas
{
	public class EuclidianDistanceTests
	{
		private const double e = 0.001;
		[Theory]
		[InlineData(new double[] {1}, new double[] {2}, 1)]
		[InlineData(new double[] {1,1}, new double[] {2,5}, 4.123)]
		[InlineData(new double[] {-1,1,0}, new double[] {2,6,11}, 12.45)]
		[InlineData(new double[] {6,0,4,4}, new double[] {-2,3,-10,7}, 16.673)]
		public void CalculateDistance(double[] a, double[] b, double result)
		{
			double distance = EuclidianDistance.Calculate(a, b);

			Assert.True(Math.Abs(distance - result) < e);
		}

	}
}
