using System;
using System.Collections.Generic;
using Clipping2D.Drawer;
using Clipping2D.Drawer.Utils;
using Clipping2D.Polygon;
using SDL2;

namespace SdlApplication.Drawer
{
    public class NotVisibleFacetDrawer : IPolygonDrawer
    {
        private readonly int _dottlesCount = 5;

        public void Draw(IntPtr renderer, List<Edge2D> edges)
        {
            foreach (Edge2D edge in edges)
            {
                SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 0);

                foreach (var line in edge.VisibleParts)
                {
                    DottledLine.Draw(line.Start, line.End, _dottlesCount,
                        (p1, p2) => SDL.SDL_RenderDrawLine(renderer, p1.X, p1.Y, p2.X, p2.Y));
                }
            }
        }
    }
}
