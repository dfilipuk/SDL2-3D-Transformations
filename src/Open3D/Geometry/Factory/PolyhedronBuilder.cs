using System.Collections.Generic;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Geometry.Factory
{
    public static class PolyhedronBuilder
    {
        /// <summary>
        /// Creates <see cref="SimplePolyhedron3D"/> object with specified parameters.
        /// </summary>
        /// <param name="a">Size along OX axis.</param>
        /// <param name="b">Size along OY axis.</param>
        /// <param name="c">Size along OZ axis.</param>
        /// <param name="geometricCenter">Geometric center of figure.</param>
        /// <param name="rotationCenter">Rotation center of figure.</param>
        /// <returns></returns>
        public static IPolyhedron3D CreateSimplePolyhedron(double a, double b, double c, HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter)
        {
            var polyhedron = new SimplePolyhedron3D(
                rotationCenter,
                new[]
                {
                    new HomogeneousPoint3D(0, 0, 0, 1),
                    new HomogeneousPoint3D(0, 0, c, 1),
                    new HomogeneousPoint3D(a, 0, c, 1),
                    new HomogeneousPoint3D(a, 0, 0, 1),
                    new HomogeneousPoint3D(0, b, 0, 1),
                    new HomogeneousPoint3D(0, b, c, 1),
                    new HomogeneousPoint3D(a, b, c, 1),
                    new HomogeneousPoint3D(a, b, 0, 1),
                },
                new[]
                {
                    new [] { 0, 3, 2, 1 },
                    new [] { 4, 5, 6, 7 },
                    new [] { 0, 1, 5, 4 },
                    new [] { 7, 6, 2, 3 },
                    new [] { 0, 4, 7, 3 },
                    new [] { 1, 2, 6, 5 },
                },
                (0, 6));

            MovePolyhedronGeometricCenter(new HomogeneousPoint3D(a / 2, b / 2, c / 2, 1), geometricCenter, polyhedron);

            return polyhedron;
        }

        /// <summary>
        /// Creates <see cref="CompositePolyhedron3D"/> object with specified parameters.
        /// </summary>
        /// <param name="a">Size along OX axis.</param>
        /// <param name="b">Size along OY axis.</param>
        /// <param name="c">Size along OZ axis.</param>
        /// <param name="geometricCenter">Geometric center of figure.</param>
        /// <param name="rotationCenter">Rotation center of figure.</param>
        /// <returns></returns>
        public static IPolyhedron3D CreateCompositePolyhedron(double a, double b, double c,
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter)
        {
            var delta = 0;

            var polyhedrons = new List<IPolyhedron3D>
            {
                //CreateSimplePolyhedron(a, b, c / 3,
                //    new HomogeneousPoint3D(0, 0, -2 * c / 6, 1),
                //    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                //CreateSimplePolyhedron(a, b, c / 3,
                //    new HomogeneousPoint3D(0, 0, 2 * c / 6, 1),
                //    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                //CreateSimplePolyhedron(a / 3, b, c / 3,
                //    new HomogeneousPoint3D(-2 * a / 6, 0, 0, 1),
                //    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                //CreateSimplePolyhedron(a / 3, b, c / 3,
                //    new HomogeneousPoint3D(2 * a / 6, 0, 0, 1),
                //    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W))

                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(0, 0, -2 * c / 6 + delta, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(0, 0, 2 * c / 6 - delta, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(-2 * a / 6 + delta, 0, 0, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(2 * a / 6 - delta, 0, 0, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(-2 * a / 6 + delta, 0, -2 * c / 6 + delta, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(2 * a / 6 - delta, 0, -2 * c / 6 + delta, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(2 * a / 6 - delta, 0, 2 * c / 6 - delta, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
                CreateSimplePolyhedron(a / 3, b, c / 3,
                    new HomogeneousPoint3D(-2 * a / 6 + delta, 0, 2 * c / 6 - delta, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W)),
            };

            var result = new CompositePolyhedron3D(rotationCenter, polyhedrons, (0, 1));

            MovePolyhedronGeometricCenter(new HomogeneousPoint3D(0, 0, 0, 1), geometricCenter, result);

            return result;
        }

        private static void MovePolyhedronGeometricCenter(HomogeneousPoint3D currentCenter,
            HomogeneousPoint3D needCenter, IPolyhedron3D polyhedron)
        {
            var originPoint = new HomogeneousPoint3D(
                currentCenter.X - needCenter.X,
                currentCenter.Y - needCenter.Y,
                currentCenter.Z - needCenter.Z,
                needCenter.W);
            Matrix affineMatrix = AffineTransformation.MoveOriginTo(originPoint);
            polyhedron.Transform(affineMatrix);
        }
    }
}
