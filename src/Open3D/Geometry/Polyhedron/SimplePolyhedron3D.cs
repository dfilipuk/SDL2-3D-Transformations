using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public class SimplePolyhedron3D : IPolyhedron3D
    {
        private readonly IList<HomogeneousPoint3D> _vertexes;
        private readonly List<Polygon3D> _facets;
        private readonly List<Polygon3D> _visibleFacets;

        public HomogeneousPoint3D RotationCenter { get; }
        public (HomogeneousPoint3D Start, HomogeneousPoint3D End) RotationVector { get; }

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
            IEnumerable<IEnumerable<int>> facetVertexes, (int startVertexIndex, int endVertexIndex) rotationAxis)
        {
            RotationCenter = rotationCenter;
            _vertexes = vertexes;
            _facets = new List<Polygon3D>();
            _visibleFacets = new List<Polygon3D>();
            RotationVector = (_vertexes[rotationAxis.startVertexIndex], _vertexes[rotationAxis.endVertexIndex]);
            CreateFacets(facetVertexes);
        }

        public void Transform(Matrix affineMatrix)
        {
            foreach (var vertex in _vertexes)
            {
                vertex.Transform(affineMatrix);
            }
        }

        public void TransformRotationCenter(Matrix affineMatrix)
        {
            RotationCenter.Transform(affineMatrix);
        }

        public void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, Point screenCenter)
        {
            foreach (var vertex in _vertexes)
            {
                vertex.Project(distanceBetweenScreenAndObserver, screenCenter);
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
