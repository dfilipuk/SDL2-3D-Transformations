using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Clipping2D.Clipping;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public class CompositePolyhedron3D : Polyhedron3D
    {
        private readonly IList<IPolyhedron3D> _polyhedrons;

        public CompositePolyhedron3D(
            HomogeneousPoint3D rotationCenter, 
            IList<IPolyhedron3D> polyhedrons, 
            (int startIndex, int endIndex) rotationAxis) : base(rotationCenter)
        {
            _polyhedrons = polyhedrons;
            RotationVector = (_polyhedrons[rotationAxis.startIndex].RotationVector.Start,
                _polyhedrons[rotationAxis.endIndex].RotationVector.End);
        }

        public override void Transform(Matrix affineMatrix)
        {
            foreach (var polyhedron in _polyhedrons)
            {
                polyhedron.Transform(affineMatrix);
            }
        }

        public override void TransformRotationCenter(Matrix affineMatrix)
        {
            foreach (var polyhedron in _polyhedrons)
            {
                polyhedron.TransformRotationCenter(affineMatrix);
            }

            RotationCenter.Transform(affineMatrix);
        }

        public override void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, Point screenCenter)
        {
            foreach (var polyhedron in _polyhedrons)
            {
                polyhedron.ProjectVertexesToScreen(distanceBetweenScreenAndObserver, screenCenter);
            }
        }

        public override void CalculateVisibilityOfFacets()
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

            PerformClipping();
        }

        private void PerformClipping()
        {
            var sortedFacets = VisibleFacets
                .OrderByDescending(p => p)
                .ToArray();

            for (int i = 0; i < sortedFacets.Length; i++)
            {
                for (int j = i + 1; j < sortedFacets.Length; j++)
                {
                    if (sortedFacets[i].CompareTo(sortedFacets[j]) > 0)
                    {
                        sortedFacets[i].Projection.ClipByPolygon(sortedFacets[j].Projection, ClippingType.External);
                    } 
                }
            }
        }
    }
}
