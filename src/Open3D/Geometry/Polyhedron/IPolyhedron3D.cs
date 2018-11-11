using System.Collections.Generic;
using System.Drawing;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public interface IPolyhedron3D
    {
        double MaxZ { get; }
        double MinZ { get; }

        HomogeneousPoint3D RotationCenter { get; }
        (HomogeneousPoint3D Start, HomogeneousPoint3D End) RotationVector { get; }

        IEnumerable<Polygon3D> VisibleFacets { get; }
        IEnumerable<Polygon3D> NotVisibleFacets { get; }

        void Transform(Matrix affineMatrix);
        void TransformRotationCenter(Matrix affineMatrix);
        void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, Point screenCenter);
        void CalculateVisibilityOfFacets();
    }
}
