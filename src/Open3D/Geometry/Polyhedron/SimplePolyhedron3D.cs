using System.Collections.Generic;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public class SimplePolyhedron3D : IPolyhedron3D
    {
        private readonly HomogeneousPoint3D _rotationCenter;
        private readonly IList<HomogeneousPoint3D> _vertexes;
        private readonly List<Polygon3D> _facets;
        private readonly List<Polygon3D> _visibleFacets;

        public IEnumerable<Polygon3D> VisibleFacets
        {
            get
            {
                foreach (var facet in _visibleFacets)
                {
                    yield return facet;
                }
            }
        }

        public SimplePolyhedron3D(HomogeneousPoint3D rotationCenter, IList<HomogeneousPoint3D> vertexes, 
            IEnumerable<IEnumerable<int>> facetVertexes)
        {
            _vertexes = vertexes;
            _facets = new List<Polygon3D>();
            _rotationCenter = rotationCenter;
            CreateFacets(facetVertexes);
        }

        public void Transform(Matrix affineMatrix)
        {
            _rotationCenter.Transform(affineMatrix);

            foreach (var vertex in _vertexes)
            {
                vertex.Transform(affineMatrix);
            }
        }

        public void ProjectVertexesToScreen(double distanceBetweenScreenAndObserver, int screenCenterX, int screenCenterY)
        {
            foreach (var vertex in _vertexes)
            {
                vertex.Project(distanceBetweenScreenAndObserver, screenCenterX, screenCenterY);
            }
        }

        public void CalculateVisibleFacets()
        {
            _visibleFacets.Clear();

            foreach (var facet in _facets)
            {
                if (facet.IsVisibleFromOriginInPositiveZDirection())
                {
                    facet.CreateProjection();
                    _visibleFacets.Add(facet);
                }
            }
        }

        private void CreateFacets(IEnumerable<IEnumerable<int>> facetVertexes)
        {
            foreach (var vertexes in facetVertexes)
            {
                var vertexesList = new List<HomogeneousPoint3D>();

                foreach (int vertexIndex in vertexes)
                {
                    vertexesList.Add(_vertexes[vertexIndex]);
                }

                _facets.Add(new Polygon3D(vertexesList));
            }
        }
    }
}
