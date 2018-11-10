using System;
using System.Collections.Generic;
using Clipping2D.Polygon;

namespace Clipping2D.Drawer
{
    public interface IPolygonDrawer
    {
        void Draw(IntPtr renderer, List<Edge2D> edges);
    }
}
