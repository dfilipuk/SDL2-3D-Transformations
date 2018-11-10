using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Clipping2D.Extension
{
    public static class VectorOperations
    {
        public static List<double> VectorTo(this Point start, Point end)
        {
            var result = new List<double>();
            result.Add(end.X - start.X);
            result.Add(end.Y - start.Y);
            return result;
        }

        public static double ScalarMultiplicationWith(this List<double> vector1, List<double> vector2)
        {
            if (vector1.Count != vector2.Count)
            {
                throw new ArgumentException("Vectors with different demensions");
            }

            double result = 0;

            for (int i = 0; i < vector1.Count; i++)
            {
                result += vector1[i] * vector2[i];
            }

            return result;
        }

        public static void MultiplyByScalar(this List<double> vector, double scalar)
        {
            for (int i = 0; i < vector.Count; i++)
            {
                vector[i] *= scalar;
            }
        }

        public static double AngleWith(this List<double> vector1, List<double> vector2)
        {
            double res = Math.Acos(vector1.ScalarMultiplicationWith(vector2) / (vector1.VectorLength() * vector2.VectorLength()));
            return res;
        }

        public static double VectorLength(this List<double> vector)
        {
            return Math.Sqrt(vector.Sum(x => x * x));
        }
    }
}
