using System;
using System.Collections.Generic;
using System.Linq;
using Clipping2D.Polygon;
using Open3D.Geometry.Polyhedron;

namespace Open3D.Geometry
{
    public class Polygon3D : IComparable<Polygon3D>
    {
        private const int _minVertexesCount = 3;
        private const double _precision = 0.001;

        private IPolyhedron3D _parent;
        private readonly IList<HomogeneousPoint3D> _vertexes;

        public double MaxX { get; private set; }
        public double MinX { get; private set; }
        public double MaxY { get; private set; }
        public double MinY { get; private set; }
        public double MaxZ { get; private set; }
        public double MinZ { get; private set; }
        public double MaxXProj { get; private set; }
        public double MinXProj { get; private set; }
        public double MaxYProj { get; private set; }
        public double MinYProj { get; private set; }

        public Polygon2D Projection { get; private set; }

        public Polygon3D(IList<HomogeneousPoint3D> vertexes, IPolyhedron3D parent)
        {
            if (vertexes.Count < _minVertexesCount)
            {
                throw new ArgumentException($"{_minVertexesCount} and more vertexes required");
            }

            _parent = parent;
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

        public void CalculateDemensions()
        {
            MaxX = _vertexes.Max(v => v.X);
            MinX = _vertexes.Min(v => v.X);
            MaxY = _vertexes.Max(v => v.Y);
            MinY = _vertexes.Min(v => v.Y);
            MaxZ = _vertexes.Max(v => v.Z);
            MinZ = _vertexes.Min(v => v.Z);
            MaxXProj = _vertexes.Max(v => v.Projection.X);
            MinXProj = _vertexes.Min(v => v.Projection.X);
            MaxYProj = _vertexes.Max(v => v.Projection.Y);
            MinYProj = _vertexes.Min(v => v.Projection.Y);
        }

        public int CompareTo(Polygon3D obj)
        {
            if (_parent == obj._parent)
            {
                return 0;
            }

            if (System.Math.Abs(MaxZ - obj.MaxZ) >= _precision)
            {
                return (int)((MaxZ - obj.MaxZ) / _precision);
            }

            if (System.Math.Abs(MinZ - obj.MinZ) >= _precision)
            {
                return (int)((MinZ - obj.MinZ) / _precision);
            }

            if (System.Math.Abs(MinX - obj.MinX) >= _precision)
            {
                return (int)((MinX - obj.MinX) / _precision);
            }

            if (System.Math.Abs(MinY - obj.MinY) >= _precision)
            {
                return (int)((MinY - obj.MinY) / _precision);
            }

            return 0;
        }
    }
}
