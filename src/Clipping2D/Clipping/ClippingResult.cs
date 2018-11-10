namespace Clipping2D.Clipping
{
    class ClippingResult
    {
        public LinePosition Position { get; set; }
        public double t0 { get; set; }
        public double t1 { get; set; }

        public ClippingResult()
        {
            Position = LinePosition.InsidePartial;
            t0 = 0;
            t1 = 1;
        }
    }
}
