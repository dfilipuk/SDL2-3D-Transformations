using System;
using System.Collections.Generic;
using Clipping2D.Drawer;
using Clipping2D.Polygon;
using SDL2;

namespace SdlApplication.Drawer
{
    public class PolygonDrawer : IPolygonDrawer
    {
        public void Draw(IntPtr renderer, List<Edge2D> edges)
        {
            foreach (Edge2D edge in edges)
            {
                SDL.SDL_SetRenderDrawColor(renderer, 0, 255, 255, 255);

                foreach (var line in edge.VisibleParts)
                {
                    SDL.SDL_RenderDrawLine(renderer, line.Start.X, line.Start.Y, line.End.X, line.End.Y);
                }
            }
        }
    }
}
