using System;
using Open3D.Geometry;

namespace Open3D.Math
{
    public static class AffineTransformation
    {
        public static Matrix MoveOriginTo(HomogeneousPoint3D point)
        {
            Matrix result = GetZeroMatrix();
            result[0, 3] = -point.X;
            result[1, 3] = -point.Y;
            result[2, 3] = -point.Z;

            return result;
        }

        public static Matrix RotateAroundXAxis(double angle)
        {
            Matrix result = GetZeroMatrix();
            result[1, 1] = System.Math.Cos(angle);
            result[2, 2] = System.Math.Cos(angle);
            result[1, 2] = -System.Math.Sin(angle);
            result[2, 1] = System.Math.Sin(angle);

            return result;
        }

        public static Matrix RotateAroundYAxis(double angle)
        {
            Matrix result = GetZeroMatrix();
            result[0, 0] = System.Math.Cos(angle);
            result[2, 2] = System.Math.Cos(angle);
            result[0, 2] = System.Math.Sin(angle);
            result[2, 0] = -System.Math.Sin(angle);

            return result;
        }

        public static Matrix RotateAroundZAxis(double angle)
        {
            Matrix result = GetZeroMatrix();
            result[0, 0] = System.Math.Cos(angle);
            result[1, 1] = System.Math.Cos(angle);
            result[0, 1] = -System.Math.Sin(angle);
            result[1, 0] = System.Math.Sin(angle);

            return result;
        }

        public static Matrix RotateAroundAxisAtPoint(Axis3D axis, HomogeneousPoint3D point, double angle)
        {
            var returnPoint = new HomogeneousPoint3D(-point.X, -point.Y, -point.Z, point.W);
            Matrix moveOriginToPoint = MoveOriginTo(point);
            Matrix rotationMatrix;
            Matrix returnOrigin = MoveOriginTo(returnPoint);

            switch (axis)
            {
                case Axis3D.OX:
                    rotationMatrix = RotateAroundXAxis(angle);
                    break;
                case Axis3D.OY:
                    rotationMatrix = RotateAroundYAxis(angle);
                    break;
                case Axis3D.OZ:
                    rotationMatrix = RotateAroundZAxis(angle);
                    break;
                default:
                    throw new ArgumentException("Unknown axis");
            }

            return returnOrigin.MultiplyBy(rotationMatrix.MultiplyBy(moveOriginToPoint));
        }

        public static Matrix RotateAroundVector((HomogeneousPoint3D Start, HomogeneousPoint3D End) vector, double angle)
        {
            var returnPoint = new HomogeneousPoint3D(-vector.Start.X, -vector.Start.Y, -vector.Start.Z, vector.Start.W);
            Matrix moveOriginToPoint = MoveOriginTo(vector.Start);
            Matrix returnOrigin = MoveOriginTo(returnPoint);

            // https://en.wikipedia.org/wiki/Rotation_matrix

            Matrix rotationMatrix = GetZeroMatrix();
            HomogeneousPoint3D rotationVector = vector.Start.VectorTo(vector.End).GetUnitVector();
            rotationMatrix[0, 0] = System.Math.Cos(angle) + (1 - System.Math.Cos(angle)) * rotationVector.X * rotationVector.X;
            rotationMatrix[0, 1] = (1 - System.Math.Cos(angle)) * rotationVector.X * rotationVector.Y - System.Math.Sin(angle) * rotationVector.Z;
            rotationMatrix[0, 2] = (1 - System.Math.Cos(angle)) * rotationVector.X * rotationVector.Z + System.Math.Sin(angle) * rotationVector.Y;
            rotationMatrix[1, 0] = (1 - System.Math.Cos(angle)) * rotationVector.X * rotationVector.Y + System.Math.Sin(angle) * rotationVector.Z;
            rotationMatrix[1, 1] = System.Math.Cos(angle) + (1 - System.Math.Cos(angle)) * rotationVector.Y * rotationVector.Y;
            rotationMatrix[1, 2] = (1 - System.Math.Cos(angle)) * rotationVector.Y * rotationVector.Z - System.Math.Sin(angle) * rotationVector.X;
            rotationMatrix[2, 0] = (1 - System.Math.Cos(angle)) * rotationVector.X * rotationVector.Z - System.Math.Sin(angle) * rotationVector.Y;
            rotationMatrix[2, 1] = (1 - System.Math.Cos(angle)) * rotationVector.Y * rotationVector.Z + System.Math.Sin(angle) * rotationVector.X;
            rotationMatrix[2, 2] = System.Math.Cos(angle) + (1 - System.Math.Cos(angle)) * rotationVector.Z * rotationVector.Z;

            return returnOrigin.MultiplyBy(rotationMatrix.MultiplyBy(moveOriginToPoint));
        }

        public static Matrix GetZeroMatrix()
        {
            return new Matrix(4, 4)
            {
                [0, 0] = 1,
                [1, 1] = 1,
                [2, 2] = 1,
                [3, 3] = 1
            };
        }
    }
}
