using System.Collections.Generic;
using System.Drawing;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public interface IPolyhedron3D
    {
        HomogeneousPoint3D RotationCenter { get; }
        IEnumerable<Polygon3D> VisibleFacets { get; }
        (HomogeneousPoint3D Start, HomogeneousPoint3D End) RotationVector { get; }

        void Transform(Matrix affineMatrix);
        void TransformRotationCenter(Matrix affineMatrix);
        void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, Point screenCenter);
        void CalculateVisibleFacets();
    }
}
