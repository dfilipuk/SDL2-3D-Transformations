using Open3D.Math;
using System.Drawing;

namespace Open3D.Geometry
{
    public class HomogeneousPoint3D
    {
        private Matrix _pointMatrix;

        public double X => _pointMatrix[0, 0];
        public double Y => _pointMatrix[1, 0];
        public double Z => _pointMatrix[2, 0];
        public double W => _pointMatrix[3, 0];
        public Point Projection { get; private set; }

        public HomogeneousPoint3D(double x, double y, double z, double w)
        {
            _pointMatrix = new Matrix(4, 1)
            {
                [0, 0] = x,
                [1, 0] = y,
                [2, 0] = z,
                [3, 0] = w
            };
        }

        public void Transform(Matrix affineMatrix)
        {
            _pointMatrix = affineMatrix.MultiplyBy(_pointMatrix);
        }

        public void Project(int d, Point center)
        {
            Projection = new Point()
            {
                X = (int) System.Math.Round(X / (Z / d)) - center.X,
                Y = (int) System.Math.Round(Y / (Z / d)) - center.Y
            };
        }
    }
}
