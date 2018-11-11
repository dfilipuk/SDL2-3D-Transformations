using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Clipping2D.Clipping;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public class CompositePolyhedron3D : IPolyhedron3D
    {
        private IList<IPolyhedron3D> _polyhedrons;
        private readonly List<Polygon3D> _visibleFacets;
        private readonly List<Polygon3D> _notVisibleFacets;

        public double MaxZ { get; private set; }
        public double MinZ { get; private set; }

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

        public CompositePolyhedron3D(HomogeneousPoint3D rotationCenter, IList<IPolyhedron3D> polyhedrons, (int startIndex, int endIndex) rotationAxis)
        {
            RotationCenter = rotationCenter;
            _polyhedrons = polyhedrons;
            RotationVector = (_polyhedrons[rotationAxis.startIndex].RotationVector.Start,
                _polyhedrons[rotationAxis.endIndex].RotationVector.End);
            _visibleFacets = new List<Polygon3D>();
            _notVisibleFacets = new List<Polygon3D>();
            CalculateMinMaxZ();
        }

        public void Transform(Matrix affineMatrix)
        {
            foreach (var polyhedron in _polyhedrons)
            {
                polyhedron.Transform(affineMatrix);
            }

            CalculateMinMaxZ();
        }

        public void TransformRotationCenter(Matrix affineMatrix)
        {
            foreach (var polyhedron in _polyhedrons)
            {
                polyhedron.TransformRotationCenter(affineMatrix);
            }

            RotationCenter.Transform(affineMatrix);
        }

        public void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, Point screenCenter)
        {
            foreach (var polyhedron in _polyhedrons)
            {
                polyhedron.ProjectVertexesToScreen(distanceBetweenScreenAndObserver, screenCenter);
            }
        }

        public void CalculateVisibilityOfFacets()
        {
            _visibleFacets.Clear();
            _notVisibleFacets.Clear();

            foreach (var polyhedron in _polyhedrons)
            {
                polyhedron.CalculateVisibilityOfFacets();
            }

            foreach (var polyhedron in _polyhedrons)
            {
                foreach (var facet in polyhedron.VisibleFacets)
                {
                    _visibleFacets.Add(facet);
                }

                foreach (var facet in polyhedron.NotVisibleFacets)
                {
                    _notVisibleFacets.Add(facet);
                }
            }

            //PerformClipping();
        }

        public void PerformClipping()
        {
            var sortedFacets = VisibleFacets
                .OrderByDescending(p => p)
                .ToArray();

            for (int i = 0; i < sortedFacets.Length; i++)
            {
                for (int j = i + 1; j < sortedFacets.Length; j++)
                {
                    sortedFacets[i].Projection.ClipByPolygon(sortedFacets[j].Projection, ClippingType.External);
                }
            }
        }

        private void CalculateMinMaxZ()
        {
            MinZ = _polyhedrons.Min(p => p.MinZ);
            MaxZ = _polyhedrons.Max(p => p.MaxZ);
        }
    }
}
