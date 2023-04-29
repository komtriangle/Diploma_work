namespace Diploma.WebApi.Logic.Formulas
{
    public static class MNK
    {

        public static (double, double) Calculate(double[] x, double[] y)
        {

            if(x == null || y == null)
            {
                throw new ArgumentNullException();
            }

            if(x.Length != y.Length)
            {
                throw new Exception("Lengty x and y must be the same");
            }

            double denumerator = CalculateDenumerator(x, y);

            double a = CalculateANumerator(x, y) / denumerator;
            double b = CalculateBNumerator(x, y) / denumerator;

            return (a, b);
        }

        private static double CalculateBNumerator(double[] x, double[] y)
        {
            return x.Length * x.Select((_, i) => x[i] * y[i]).Sum() - x.Sum() * y.Sum();
        }

        private static double CalculateANumerator(double[] x, double[] y)
        {
            return -x.Sum() * x.Select((_, i) => x[i] * y[i]).Sum() + x.Sum(v => v * v) * y.Sum();
        }

        private static double CalculateDenumerator(double[] x, double[] y)
		{
            return x.Length * x.Sum(v => v * v) - Math.Pow(x.Sum(), 2);

        }
    }
}
