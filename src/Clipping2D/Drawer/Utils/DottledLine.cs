using System;
using System.Drawing;
using Clipping2D.Extension;

namespace Clipping2D.Drawer.Utils
{
    public static class DottledLine
    {
        public static void Draw(Point start, Point end, int dottlesCount, Action<Point, Point> drawLine)
        {
            var lineVector = start.VectorTo(end);
            double mu = 1D / dottlesCount;

            if (mu < 1)
            {
                double currentMu = mu;
                bool drawCurrentDot = true;
                Point prevPoint = start;

                while (currentMu <= 1)
                {
                    Point currentPoint = currentMu.GetLinePoint((start, end));

                    if (drawCurrentDot)
                    {
                        drawLine(prevPoint, currentPoint);
                    }

                    prevPoint = currentPoint;
                    drawCurrentDot = !drawCurrentDot;
                    currentMu += mu;
                }
            }
        }
    }
}
