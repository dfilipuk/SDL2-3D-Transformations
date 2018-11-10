using System;
using System.Drawing;
using Clipping2D.Drawer;
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

        void RotateAroundAxis(Axis3D axis, double angle);

        void Render(IntPtr renderer, IPolygonDrawer drawer, Point screenCenter);
    }
}
