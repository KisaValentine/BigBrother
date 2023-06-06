using System;

namespace BigBrother.Utils
{
    internal class Maths
    {
        public static int CalculateEuclideanDistance(int x, int z)
        {
            return (int)Math.Sqrt(x * x + z * z);
        }
    }
}
