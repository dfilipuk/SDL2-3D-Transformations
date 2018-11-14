using System;
using System.Drawing;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Geometry.Polyhedron;

namespace Open3D.Rendering
{
    public interface IScene
    {
        /// <summary>
        /// Before move observer at Origin.
        /// </summary>
        /// <param name="point"></param>
        void MoveObserverTo(HomogeneousPoint3D point);

        void MoveDisplay(int moveDistance);

        void RotateAroundAxis(Axis3D axis, double angle);

        void RotateAroundVector(double angle);

        void AddObject(IPolyhedron3D polyhedron);

        void Render(IntPtr renderer, IPolygonDrawer visibleFacetDrawer, IPolygonDrawer notVisibleFacetDrawer, Point screenCenter);
    }
}
