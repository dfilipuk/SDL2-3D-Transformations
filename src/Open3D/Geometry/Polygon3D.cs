using System.Collections.Generic;
using System.Linq;
using Clipping2D.Polygon;

namespace Open3D.Geometry
{
    public class Polygon3D
    {
        private readonly IList<HomogeneousPoint3D> _vertexes;

        public Polygon2D Projection { get; private set; }

        public Polygon3D(IList<HomogeneousPoint3D> vertexes)
        {
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
            return true;
        }
    }
}
