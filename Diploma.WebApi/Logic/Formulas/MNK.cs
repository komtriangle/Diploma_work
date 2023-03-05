namespace Diploma.WebApi.Logic.Formulas
{
    public static class MNK
    {

        public static (double, double) Calculate(double[] x, double[] y)
        {
            double[] ln_x = x.Select(v => Math.Log(v)).ToArray();
            double[] ln_y = y.Select(v => Math.Log(v)).ToArray();

            if(x == null || y == null)
            {
                throw new ArgumentNullException();
            }

            if(x.Length != y.Length)
            {
                throw new Exception("Lengty x and y must be the same");
            }

            double denumerator = CalculateDenumerator(ln_x, ln_y);

            double a = CalculateANumerator(ln_x, ln_y) / denumerator;
            double b = CalculateBNumerator(ln_x, ln_y) / denumerator;

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
