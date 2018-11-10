using System;
using System.Collections.Generic;
using System.Drawing;
using Clipping2D.Clipping;
using Clipping2D.Drawer;
using Clipping2D.Extension;

namespace Clipping2D.Polygon
{
    public class Polygon2D
    {
        private readonly double _precision = 0.00001;

        protected List<Edge2D> _edges;
        protected List<Point> _initialVertexes;
        protected Dictionary<int, List<double>> _normalInsideVectors;

        internal IEnumerable<Edge2D> Edges
        {
            get
            {
                foreach (Edge2D edge in _edges)
                {
                    yield return edge;
                }
            }
        }

        public Polygon2D(List<Point> vertexes)
        {
            _initialVertexes = vertexes;
            _normalInsideVectors = new Dictionary<int, List<double>>();
            _edges = new List<Edge2D>();

            CalculateInitialEdges();
        }

        public void Draw(IntPtr renderer, IPolygonDrawer polygonDrawer)
        {
            polygonDrawer.Draw(renderer, _edges);
        }

        public void ResetClipping()
        {
            foreach (var edge in _edges)
            {
                edge.ResetClipping();
            }
        }

        public void ClipByPolygon(Polygon2D polygon, ClippingType type)
        {
            foreach (Edge2D edge in _edges)
            {
                var visibleParts = edge.VisibleParts.ToArray();
                edge.VisibleParts.Clear();

                foreach (var visiblePart in visibleParts)
                {
                    ClippingResult result = ClippingService.ClipLineByPolygon(visiblePart, polygon);

                    if (result.Position == LinePosition.OutsideFully)
                    {
                        if (type == ClippingType.Inside)
                        {
                            edge.NotVisibleParts.Add(visiblePart);
                        }
                        else if (type == ClippingType.External)
                        {
                            edge.VisibleParts.Add(visiblePart);
                        }
                    }
                    else if (result.Position == LinePosition.InsideFully)
                    {
                        if (type == ClippingType.Inside)
                        {
                            edge.VisibleParts.Add(visiblePart);
                        }
                        else if (type == ClippingType.External)
                        {
                            edge.NotVisibleParts.Add(visiblePart);
                        }
                    }
                    else if (result.Position == LinePosition.InsidePartial)
                    {
                        if ((result.t0 == 0) && (result.t1 != 1))
                        {
                            var crossPoint = result.t1.GetLinePoint(visiblePart);

                            if (type == ClippingType.Inside)
                            {
                                edge.VisibleParts.Add((visiblePart.Start, crossPoint));
                                edge.NotVisibleParts.Add((crossPoint, visiblePart.End));
                            }
                            else if (type == ClippingType.External)
                            {
                                edge.NotVisibleParts.Add((visiblePart.Start, crossPoint));
                                edge.VisibleParts.Add((crossPoint, visiblePart.End));
                            }
                        }
                        else if ((result.t0 != 0) && (result.t1 == 1))
                        {
                            var crossPoint = result.t0.GetLinePoint(visiblePart);

                            if (type == ClippingType.Inside)
                            {
                                edge.VisibleParts.Add((crossPoint, visiblePart.End));
                                edge.NotVisibleParts.Add((visiblePart.Start, crossPoint));
                            }
                            else if (type == ClippingType.External)
                            {
                                edge.NotVisibleParts.Add((crossPoint, visiblePart.End));
                                edge.VisibleParts.Add((visiblePart.Start, crossPoint));
                            }
                        }
                        else
                        {
                            var crossPoint1 = result.t0.GetLinePoint(visiblePart);
                            var crossPoint2 = result.t1.GetLinePoint(visiblePart);

                            if (type == ClippingType.Inside)
                            {
                                edge.VisibleParts.Add((crossPoint1, crossPoint2));
                                edge.NotVisibleParts.Add((visiblePart.Start, crossPoint1));
                                edge.NotVisibleParts.Add((crossPoint2, visiblePart.End));
                            }
                            else if (type == ClippingType.External)
                            {
                                edge.NotVisibleParts.Add((crossPoint1, crossPoint2));
                                edge.VisibleParts.Add((visiblePart.Start, crossPoint1));
                                edge.VisibleParts.Add((crossPoint2, visiblePart.End));
                            }
                        }
                    }
                }
            }
        }

        internal List<double> GetNormalInsideVectorForEdge(int edgeIndex)
        {
            if (!_normalInsideVectors.ContainsKey(edgeIndex))
            {
                CalculateNormalInsideVectorForEdge(edgeIndex);
            }

            return _normalInsideVectors[edgeIndex];
        }

        internal PointPosition GetPointPosition(Point point)
        {
            PointPosition result = PointPosition.Inside;

            for (int i = 0; i < _edges.Count && result == PointPosition.Inside; i++)
            {
                var testVector = point.VectorTo(_edges[i].Start);
                var normalInsideVector = GetNormalInsideVectorForEdge(i);
                double scalarMultiplication = testVector.ScalarMultiplicationWith(normalInsideVector);

                if (scalarMultiplication > 0)
                {
                    result = PointPosition.Outside;
                }
                else if (Math.Abs(scalarMultiplication) <= _precision)
                {
                    if (point.IsPointBelongToLine((_edges[i].Start, _edges[i].End)))
                    {
                        result = PointPosition.OnEdge;
                    }
                    else
                    {
                        result = PointPosition.Outside;
                    }
                }
            }

            return result;
        }

        private void CalculateInitialEdges()
        {
            int vertexesCount = _initialVertexes.Count;
            _normalInsideVectors.Clear();
            _edges.Clear();

            for (int i = 0; i < vertexesCount; i++)
            {
                int nextVertexInd = (i + 1) % vertexesCount;
                _edges.Add(new Edge2D(_initialVertexes[i], _initialVertexes[nextVertexInd], i));
            }
        }

        private void CalculateNormalInsideVectorForEdge(int edgeInd)
        {
            int nextEdgeId = (edgeInd + 1) % _initialVertexes.Count;
            List<double> edgeVector = _edges[edgeInd].Start.VectorTo(_edges[edgeInd].End);
            List<double> testVector = _edges[edgeInd].Start.VectorTo(_edges[nextEdgeId].End);
            List<double> edgeInsideNormalVector = new List<double>();

            if (edgeVector[0] != 0)
            {
                edgeInsideNormalVector.AddRange(new[] { -edgeVector[1] / edgeVector[0], 1 });
            }
            else
            {
                edgeInsideNormalVector.AddRange(new[] { 1D, 0 });
            }

            if (edgeInsideNormalVector.ScalarMultiplicationWith(testVector) < 0)
            {
                edgeInsideNormalVector.MultiplyByScalar(-1);
            }

            _normalInsideVectors.Add(edgeInd, edgeInsideNormalVector);
        }
    }
}
