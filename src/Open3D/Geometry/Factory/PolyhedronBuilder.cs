using System;
using System.Collections.Generic;
using System.Linq;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Geometry.Factory
{
    public static class PolyhedronBuilder
    {
        public static IPolyhedron3D CreatePolyhedron(
            PolyhedronType type,
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter,
            params double[] args)
        {
            switch (type)
            {
                case PolyhedronType.Parallelepiped:
                    return CreateSingleSimplePolyhedron(
                        args[0], args[0], args[0],
                        geometricCenter,
                        rotationCenter);
                case PolyhedronType.ParallelepipedWithHole:
                    return CreateCompositePolyhedron(
                        args[0], args[0], args[0],
                        geometricCenter,
                        rotationCenter);
                case PolyhedronType.ManyParallelepipeds:
                    return CreateManySimplePolyhedrons(
                        args[0], args[0], args[0],
                        geometricCenter,
                        rotationCenter,
                        50);
                case PolyhedronType.Cubes:
                    return CreateCubes(
                        args[0] / 2, args[0] / 2, args[0] / 2,
                        geometricCenter,
                        rotationCenter);
                default:
                    throw new ArgumentException($"No builder for type {type}");
            }
        }
        
        /// <summary>
        /// Creates <see cref="IPolyhedron3D"/> object with specified parameters.
        /// </summary>
        /// <param name="a">Size along OX axis.</param>
        /// <param name="b">Size along OY axis.</param>
        /// <param name="c">Size along OZ axis.</param>
        /// <param name="geometricCenter">Geometric center of figure.</param>
        /// <param name="rotationCenter">Rotation center of figure.</param>
        /// <returns></returns>
        public static IPolyhedron3D CreateSingleSimplePolyhedron(
            double a, double b, double c, 
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter)
        {
            return CreateParallelepiped(a, b, c, geometricCenter, rotationCenter, new int[0]);
        }

        /// <summary>
        /// Creates <see cref="IPolyhedron3D"/> object with specified parameters.
        /// </summary>
        /// <param name="a">Size along OX axis.</param>
        /// <param name="b">Size along OY axis.</param>
        /// <param name="c">Size along OZ axis.</param>
        /// <param name="geometricCenter">Geometric center of figure.</param>
        /// <param name="rotationCenter">Rotation center of figure.</param>
        /// <param name="distance">Distance from Origin to each polyhedron geometric center.</param>
        /// <returns></returns>
        public static IPolyhedron3D CreateManySimplePolyhedrons(
            double a, double b, double c,
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter,
            double distance)
        {
            return CreateParallelepipedWithHole(a, b, c, geometricCenter, rotationCenter, distance, true);
        }

        /// <summary>
        /// Creates <see cref="IPolyhedron3D"/> object with specified parameters.
        /// </summary>
        /// <param name="a">Size along OX axis.</param>
        /// <param name="b">Size along OY axis.</param>
        /// <param name="c">Size along OZ axis.</param>
        /// <param name="geometricCenter">Geometric center of figure.</param>
        /// <param name="rotationCenter">Rotation center of figure.</param>
        /// <returns></returns>
        public static IPolyhedron3D CreateCompositePolyhedron(
            double a, double b, double c,
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter)
        {
            return CreateParallelepipedWithHole(a, b, c, geometricCenter, rotationCenter, 0, false);
        }

        public static IPolyhedron3D CreateCubes(
            double a, double b, double c,
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter)
        {
            var cubes = new List<IPolyhedron3D>
            {
                CreateParallelepiped(
                    a, b, c,
                    new HomogeneousPoint3D(0, 0, 0, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    new [] { 1, 2, 4 }),
                CreateParallelepiped(
                    a, b, c,
                    new HomogeneousPoint3D(a, 0, 0, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    new [] { 3 }),
                CreateParallelepiped(
                    a, b, c,
                    new HomogeneousPoint3D(0, 0, c, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    new [] { 0 }),
                CreateParallelepiped(
                    a, b, c,
                    new HomogeneousPoint3D(0, -b, 0, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    new [] { 5 }),
            };

            var result = new CompositePolyhedron3D(rotationCenter, cubes, (0, 0));

            MovePolyhedronGeometricCenter(new HomogeneousPoint3D(a / 2, -b / 2, c / 2, 1), geometricCenter, result);

            return result;
        }

        private static IPolyhedron3D CreateParallelepipedWithHole(
            double a, double b, double c,
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter,
            double distance, bool showInvisibleFacets)
        {
            var parallelepiped = new List<IPolyhedron3D>
            {
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(0, 0, -2 * c / 6 - distance, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {2,3}),
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(0, 0, 2 * c / 6 + distance, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {2,3}),
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(-2 * a / 6 - distance, 0, 0, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {0,1}),
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(2 * a / 6 + distance, 0, 0, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {0,1}),
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(-2 * a / 6 - distance, 0, -2 * c / 6 - distance, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {1,2}),
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(2 * a / 6 + distance, 0, -2 * c / 6 - distance, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {1,3}),
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(2 * a / 6 + distance, 0, 2 * c / 6 + distance, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {0,3}),
                CreateParallelepiped(
                    a / 3, b, c / 3,
                    new HomogeneousPoint3D(-2 * a / 6 - distance, 0, 2 * c / 6 + distance, 1),
                    new HomogeneousPoint3D(rotationCenter.X, rotationCenter.Y, rotationCenter.Z, rotationCenter.W),
                    showInvisibleFacets ? new int[0] : new [] {0,2}),
            };

            var result = new CompositePolyhedron3D(rotationCenter, parallelepiped, (0, 1));

            MovePolyhedronGeometricCenter(new HomogeneousPoint3D(0, 0, 0, 1), geometricCenter, result);

            return result;
        }

        /// <summary>
        /// 0 - Front
        /// 1 - Back
        /// 2 - Right
        /// 3 - Left
        /// 4 - Top
        /// 5 - Bottom
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="geometricCenter"></param>
        /// <param name="rotationCenter"></param>
        /// <param name="notVisibleFacets"></param>
        /// <returns></returns>
        private static IPolyhedron3D CreateParallelepiped(
            double a, double b, double c,
            HomogeneousPoint3D geometricCenter,
            HomogeneousPoint3D rotationCenter, 
            int[] notVisibleFacets)
        {
            var allFacets = new List<int[]>
            {
                new[] {0, 4, 7, 3},
                new[] {1, 2, 6, 5},
                new[] {7, 6, 2, 3},
                new[] {0, 1, 5, 4},
                new[] {0, 3, 2, 1},
                new[] {4, 5, 6, 7},    
            };
            var visibleFacets = new List<int[]>();

            for (int i = 0; i < allFacets.Count; i++)
            {
                if (!notVisibleFacets.Contains(i))
                {
                    visibleFacets.Add(allFacets[i]);
                }
            }

            var parallelepiped = new SimplePolyhedron3D(
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
                }, visibleFacets, (0, 6));

            MovePolyhedronGeometricCenter(new HomogeneousPoint3D(a / 2, b / 2, c / 2, 1), geometricCenter, parallelepiped);

            return parallelepiped;
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
