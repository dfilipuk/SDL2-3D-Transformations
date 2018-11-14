using System.Collections.Generic;
using System.Drawing;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public abstract class Polyhedron3D : IPolyhedron3D
    {
        protected readonly List<Polygon3D> _visibleFacets;
        protected readonly List<Polygon3D> _notVisibleFacets;

        public HomogeneousPoint3D RotationCenter { get; }
        public HomogeneousPoint3D GeometricCenter { get; }
        public (HomogeneousPoint3D Start, HomogeneousPoint3D End) RotationVector { get; protected set; }

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

        public Polyhedron3D(HomogeneousPoint3D rotationCenter, HomogeneousPoint3D geometricCenter)
        {
            RotationCenter = rotationCenter;
            GeometricCenter = geometricCenter;
            _visibleFacets = new List<Polygon3D>();
            _notVisibleFacets = new List<Polygon3D>();
        }

        public virtual void Transform(Matrix affineMatrix)
        {
            GeometricCenter.Transform(affineMatrix);
        }

        public abstract void TransformRotationCenter(Matrix affineMatrix);
        public abstract void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, Point screenCenter);
        public abstract void CalculateVisibilityOfFacets();
    }
}
