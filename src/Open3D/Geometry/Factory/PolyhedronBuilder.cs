using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Geometry.Factory
{
    public static class PolyhedronBuilder
    {
        private static readonly AffineTransformationBuilder _affineTransformationBuilder = new AffineTransformationBuilder();

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
                    new [] { 0, 1, 2, 3 },
                    new [] { 4, 5, 6, 7 },
                    new [] { 0, 1, 5, 4 },
                    new [] { 7, 6, 2, 3 },
                    new [] { 0, 3, 7, 4 },
                    new [] { 1, 2, 6, 5 },
                });
            var currentGeometricCenter = new HomogeneousPoint3D(a / 2, b / 2, c / 2, 1);
            var originPoint = new HomogeneousPoint3D(
                currentGeometricCenter.X - geometricCenter.X,
                currentGeometricCenter.Y - geometricCenter.Y,
                currentGeometricCenter.Z - geometricCenter.Z,
                geometricCenter.W);
            Matrix affineMatrix = _affineTransformationBuilder.MoveOriginTo(originPoint);
            polyhedron.Transform(affineMatrix);

            return polyhedron;
        }
    }
}
