using System;
using System.Collections.Generic;
using System.Drawing;
using Clipping2D.Extension;
using Clipping2D.Polygon;

namespace Clipping2D.Clipping
{
    class ClippingService
    {
        private static readonly double _precision = 0.00001;

        public static ClippingResult ClipLineByPolygon((Point Start, Point End) line, Polygon2D polygon)
        {
            var result = new ClippingResult();
            List<double> lineVector = line.Start.VectorTo(line.End);
            bool crossingPointsExist = false;

            foreach (Edge2D edge in polygon.Edges)
            {
                if (result.Position != LinePosition.OutsideFully)
                {
                    List<double> insideNormalVector = polygon.GetNormalInsideVectorForEdge(edge.EdgeNumber);
                    List<double> wVector = edge.Start.VectorTo(line.Start);
                    double q = insideNormalVector.ScalarMultiplicationWith(wVector);
                    double p = insideNormalVector.ScalarMultiplicationWith(lineVector);

                    if (Math.Abs(p) <= _precision)
                    {
                        if (q < 0)
                        {
                            result.Position = LinePosition.OutsideFully;
                        }
                    }
                    else
                    {
                        double t = -q / p;
                        var crossPoint = t.GetLinePoint(line);

                        if (crossPoint.IsPointBelongToLine((edge.Start, edge.End)))
                        {
                            if ((p < 0) && (t > result.t0) && (t < result.t1))
                            {
                                result.t1 = t;
                                crossingPointsExist = true;
                            }
                            if ((p > 0) && (t > result.t0) && (t < result.t1))
                            {
                                result.t0 = t;
                                crossingPointsExist = true;
                            }
                        }
                    }
                }
            }

            if (!crossingPointsExist)
            {
                result.Position = GetLinePosition(line, polygon);
            }

            return result;
        }

        private static LinePosition GetLinePosition((Point Start, Point End) line, Polygon2D polygon)
        {
            PointPosition startPointPosition = polygon.GetPointPosition(line.Start);
            PointPosition endPointPosition = polygon.GetPointPosition(line.End);

            if ((startPointPosition == PointPosition.Inside && endPointPosition == PointPosition.Inside)
                || (startPointPosition == PointPosition.OnEdge && endPointPosition == PointPosition.OnEdge)
                || (startPointPosition == PointPosition.OnEdge && endPointPosition == PointPosition.Inside)
                || (startPointPosition == PointPosition.Inside && endPointPosition == PointPosition.OnEdge))
            {
                return LinePosition.InsideFully;
            }

            if ((startPointPosition == PointPosition.Outside && endPointPosition == PointPosition.Outside)
                || (startPointPosition == PointPosition.OnEdge && endPointPosition == PointPosition.Outside)
                || (startPointPosition == PointPosition.Outside && endPointPosition == PointPosition.OnEdge))
            {
                return LinePosition.OutsideFully;
            }

            return LinePosition.InsidePartial;
        }
    }
}
