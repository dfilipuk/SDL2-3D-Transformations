using Open3D.Math;
using System.Drawing;

namespace Open3D.Geometry
{
    public class HomogeneousPoint3D
    {
        private Matrix _pointMatrix;

        public int X => _pointMatrix[0, 0];
        public int Y => _pointMatrix[1, 0];
        public int Z => _pointMatrix[2, 0];
        public int W => _pointMatrix[3, 0];
        public Point Projection { get; private set; }

        public HomogeneousPoint3D(int x, int y, int z, int w)
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

        public void Project(double d, int centerX, int centerY)
        {
            Projection = new Point()
            {
                X = (int) System.Math.Round(X / (Z / d)) - centerX,
                Y = (int) System.Math.Round(Y / (Z / d)) - centerY
            };
        }
    }
}
