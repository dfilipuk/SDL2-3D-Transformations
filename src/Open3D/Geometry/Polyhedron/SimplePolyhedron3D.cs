using System.Collections.Generic;
using System.Drawing;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public class SimplePolyhedron3D : Polyhedron3D
    {
        private readonly IList<HomogeneousPoint3D> _vertexes;
        private readonly List<Polygon3D> _facets;

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
        public SimplePolyhedron3D(
            HomogeneousPoint3D rotationCenter,
            HomogeneousPoint3D geometricCenter,
            IList<HomogeneousPoint3D> vertexes, 
            IEnumerable<IEnumerable<int>> facetVertexes, 
            (int startVertexIndex, int endVertexIndex) rotationAxis) : base(rotationCenter, geometricCenter)
        {
            _vertexes = vertexes;
            _facets = new List<Polygon3D>();
            RotationVector = (_vertexes[rotationAxis.startVertexIndex], _vertexes[rotationAxis.endVertexIndex]);

            CreateFacets(facetVertexes);
        }

        public override void Transform(Matrix affineMatrix)
        {
            base.Transform(affineMatrix);

            foreach (var vertex in _vertexes)
            {
                vertex.Transform(affineMatrix);
            }
        }

        public override void TransformRotationCenter(Matrix affineMatrix)
        {
            RotationCenter.Transform(affineMatrix);
        }

        public override void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, Point screenCenter)
        {
            foreach (var vertex in _vertexes)
            {
                vertex.Project(distanceBetweenScreenAndObserver, screenCenter);
            }
        }

        public override void CalculateVisibilityOfFacets()
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
