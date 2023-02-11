namespace Diploma.WebApi.Logic.Formulas
{
    internal static class Heaviside
    {
        public static int Calculate(double x)
        {
            return x >= 0 ? 1 : 0;
        }
    }
}
