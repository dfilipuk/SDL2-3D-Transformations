using System;
using System.Collections.Generic;
using System.Linq;
using Clipping2D.Polygon;

namespace Open3D.Geometry
{
    public class Polygon3D : IComparable<Polygon3D>
    {
        private const int _minVertexesCount = 3;
        private const double _precision = 0.1;

        private readonly IList<HomogeneousPoint3D> _vertexes;

        public double MaxZ => _vertexes.Max(v => v.Z);
        public double MinZ => _vertexes.Min(v => v.Z);

        public Polygon2D Projection { get; private set; }

        public Polygon3D(IList<HomogeneousPoint3D> vertexes)
        {
            if (vertexes.Count < _minVertexesCount)
            {
                throw new ArgumentException($"{_minVertexesCount} and more vertexes required");
            }

            _vertexes = vertexes;
        }

        public void CreateProjection()
        {
            var vertexes2D = _vertexes
                .Select(v => v.Projection)
                .ToList();
            Projection = new Polygon2D(vertexes2D);
        }

        public bool IsVisibleFromOriginInPositiveZDirection()
        {
            var p0 = new HomogeneousPoint3D(_vertexes[0].Projection.X, _vertexes[0].Projection.Y, 0, 0);
            var p1 = new HomogeneousPoint3D(_vertexes[1].Projection.X, _vertexes[1].Projection.Y, 0, 0);
            var p2 = new HomogeneousPoint3D(_vertexes[2].Projection.X, _vertexes[2].Projection.Y, 0, 0);

            HomogeneousPoint3D v1 = p0.VectorTo(p1);
            HomogeneousPoint3D v2 = p0.VectorTo(p2);

            double normalVectorZ = v1.X * v2.Y - v1.Y * v2.X;

            return normalVectorZ < 0;
        }

        public int CompareTo(Polygon3D polygon)
        {
            var maxZ = MaxZ;
            var pMaxZ = polygon.MaxZ;

            var minZ = MinZ;
            var pMinZ = polygon.MinZ;

            var meanZ = maxZ - minZ;
            var pMeanZ = pMaxZ - pMinZ;

            if (System.Math.Abs(maxZ - pMaxZ) <= _precision)
            {
                if (System.Math.Abs(minZ - pMinZ) <= _precision)
                {
                    var minX = _vertexes.Min(v => v.Projection.X);
                    var pMinX = polygon._vertexes.Min(v => v.Projection.X);

                    if (System.Math.Abs(minX - pMinX) <= _precision)
                    {
                        var minY = _vertexes.Min(v => v.Projection.Y);
                        var pMinY = polygon._vertexes.Min(v => v.Projection.Y);

                        if (System.Math.Abs(minY - pMinY) <= _precision)
                        {
                            return 0;
                        }

                        return (int)((minY - pMinY) / _precision);
                    }

                    return (int)((minX - pMinX) / _precision);
                }

                return (int)((minZ - pMinZ) / _precision);
            }

            return (int)((maxZ - pMaxZ) / _precision);


            //if (System.Math.Abs(minZ - pMinZ) <= _precision)
            //{
            //    if (System.Math.Abs(maxZ - pMaxZ) <= _precision)
            //    {
            //        return 0;
            //    }

            //    return (int)((maxZ - pMaxZ) / _precision);
            //}

            //if (System.Math.Abs(minZ - pMinZ) <= _precision)
            //{
            //    if (System.Math.Abs(meanZ - pMeanZ) <= _precision)
            //    {
            //        if (System.Math.Abs(maxZ - pMaxZ) <= _precision)
            //        {
            //            return 0;
            //        }

            //        return (int)((maxZ - pMaxZ) / _precision);
            //    }

            //    return (int)((meanZ - pMeanZ) / _precision);
            //}

            //return (int)((minZ - pMinZ) / _precision);
        }
    }
}
