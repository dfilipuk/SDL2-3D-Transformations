﻿using System.Collections.Generic;
using Open3D.Math;

namespace Open3D.Geometry.Polyhedron
{
    public interface IPolyhedron3D
    {
        HomogeneousPoint3D RotationCenter { get; }
        IEnumerable<Polygon3D> VisibleFacets { get; }

        void Transform(Matrix affineMatrix);
        void ProjectVertexesToScreen(int distanceBetweenScreenAndObserver, int screenCenterX, int screenCenterY);
        void CalculateVisibleFacets();
    }
}
