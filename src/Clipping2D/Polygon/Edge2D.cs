using System.Collections.Generic;
using System.Drawing;

namespace Clipping2D.Polygon
{
    public class Edge2D
    {
        public int EdgeNumber { get; }
        public Point Start { get; }
        public Point End { get; }

        public List<(Point Start, Point End)> VisibleParts { get; }
        public List<(Point Start, Point End)> NotVisibleParts { get; }

        public Edge2D(Point start, Point end, int number)
        {
            Start = start;
            End = end;
            EdgeNumber = number;
            VisibleParts = new List<(Point Start, Point End)>();
            NotVisibleParts = new List<(Point Start, Point End)>();
            ResetClipping();
        }

        public void ResetClipping()
        {
            VisibleParts.Clear();
            NotVisibleParts.Clear();
            VisibleParts.Add((Start, End));
        }
    }
}
