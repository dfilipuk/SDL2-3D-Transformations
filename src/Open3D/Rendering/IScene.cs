using System;
using System.Drawing;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Math;

namespace Open3D.Rendering
{
    public interface IScene
    {
        /// <summary>
        /// Initializes the scene.
        /// Should be called before any manipulations with scene.
        /// </summary>
        void Initialize();

        /// <summary>
        /// Before move observer at Origin.
        /// </summary>
        /// <param name="point"></param>
        void MoveObserverTo(HomogeneousPoint3D point);

        void MoveDisplay(int moveDistance);

        void RotateAroundAxis(Axis3D axis, double angle);

        void RotateAroundVector(double angle);

        void Render(IntPtr renderer, IPolygonDrawer drawer, Point screenCenter);
    }
}
