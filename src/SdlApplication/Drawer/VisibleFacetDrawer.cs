using System;
using System.Collections.Generic;
using Clipping2D.Drawer;
using Clipping2D.Drawer.Utils;
using Clipping2D.Polygon;
using SDL2;

namespace SdlApplication.Drawer
{
    public class VisibleFacetDrawer : IPolygonDrawer
    {
        private readonly int _dottlesCount = 5;

        public bool EnabledNotVisibleParts { get; set; }

        public VisibleFacetDrawer(bool enabledNotVisibleParts)
        {
            EnabledNotVisibleParts = enabledNotVisibleParts;
        }

        public void Draw(IntPtr renderer, List<Edge2D> edges)
        {
            foreach (Edge2D edge in edges)
            {
                SDL.SDL_SetRenderDrawColor(renderer, 255, 255, 255, 0);

                foreach (var line in edge.VisibleParts)
                {
                    SDL.SDL_RenderDrawLine(renderer, line.Start.X, line.Start.Y, line.End.X, line.End.Y);
                }

                if (EnabledNotVisibleParts)
                {
                    foreach (var line in edge.NotVisibleParts)
                    {
                        DottledLine.Draw(line.Start, line.End, _dottlesCount,
                            (p1, p2) => SDL.SDL_RenderDrawLine(renderer, p1.X, p1.Y, p2.X, p2.Y));
                    }
                }
            }
        }
    }
}
