using System.Collections.Generic;
using System.Drawing;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public class SimplePolyhedron3D : IPolyhedron3D
    {
        private readonly IList<HomogeneousPoint3D> _vertexes;
        private readonly List<Polygon3D> _facets;
        private readonly List<Polygon3D> _visibleFacets;
        private readonly List<Polygon3D> _notVisibleFacets;

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

        public IEnumerable<Polygon3D> NotVisibleFacets
        {
            get
            {
                foreach (var facet in _notVisibleFacets)
                {
                    yield return facet;
                }
            }
        }

        //
        // CLOCKWISE
        //
        //      ->--
        //      |  |
        //      |  |
        //      --<-
        //

        //
        // COUNTERCLOCKWISE
        //
        //      -<--
        //      |  |
        //      |  |
        //      -->-
        //

        /// <summary>
        /// Vertexes for facet should be in COUNTERCLOCKWISE order
        /// from point when this facet is visible for observer.
        /// </summary>
        /// <param name="rotationCenter">Rotation center.</param>
        /// <param name="vertexes">List of vertexes.</param>
        /// <param name="facetVertexes">Vertexes for every facet.</param>
        /// <param name="rotationAxis">Vertexes which determines rotation axis.</param>
        public SimplePolyhedron3D(HomogeneousPoint3D rotationCenter, IList<HomogeneousPoint3D> vertexes, 
            IEnumerable<IEnumerable<int>> facetVertexes, (int startVertexIndex, int endVertexIndex) rotationAxis)
        {
            RotationCenter = rotationCenter;
            _vertexes = vertexes;
            _facets = new List<Polygon3D>();
            _visibleFacets = new List<Polygon3D>();
            _notVisibleFacets = new List<Polygon3D>();
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

        public void CalculateVisibilityOfFacets()
        {
            _visibleFacets.Clear();
            _notVisibleFacets.Clear();

            foreach (var facet in _facets)
            {
                facet.CreateProjection();
                facet.CalculateDemensions();

                if (facet.IsVisibleFromOriginInPositiveZDirection())
                {
                    _visibleFacets.Add(facet);
                }
                else
                {
                    _notVisibleFacets.Add(facet);
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

                _facets.Add(new Polygon3D(vertexesList, this));
            }
        }
    }
}
