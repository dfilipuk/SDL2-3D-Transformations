using System;
using System.Drawing;

namespace Clipping2D.Extension
{
    public static class PointOperations
    {
        private static readonly double _doublePrecision = 0.0000001;
        private static readonly double _IntPrecision = 10;

        public static bool IsPointBelongToLine(this Point point, (Point Start, Point End) line)
        {
            double mu;

            if (line.Start.X == line.End.X)
            {
                int dy = line.End.Y - line.Start.Y;
                mu = (point.Y - line.Start.Y) / (double)dy;
            }
            else if (line.Start.Y == line.End.Y)
            {
                int dx = line.End.X - line.Start.X;
                mu = (point.X - line.Start.X) / (double)dx;
            }
            else
            {
                int dx = line.End.X - line.Start.X;
                mu = (point.X - line.Start.X) / (double)dx;
            }
            

            return mu >= 0 && mu <= 1;
        }

        public static Point GetLinePoint(this double mu, (Point Start, Point End) line)
        {
            int dx = line.End.X - line.Start.X;
            int dy = line.End.Y - line.Start.Y;
            return new Point
            {
                X = (int)Math.Round(line.Start.X + dx * mu),
                Y = (int)Math.Round(line.Start.Y + dy * mu)
            };
        }
    }
}
