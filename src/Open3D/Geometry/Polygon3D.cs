using System;
using System.Collections.Generic;
using System.Linq;
using Clipping2D.Polygon;

namespace Open3D.Geometry
{
    public class Polygon3D
    {
        private const int _minVertexesCount = 3;

        private readonly IList<HomogeneousPoint3D> _vertexes;

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
    }
}
