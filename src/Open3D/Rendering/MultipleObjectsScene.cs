using System;
using System.Collections.Generic;
using System.Drawing;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Rendering
{
    public sealed class MultipleObjectsScene : Scene
    {
        private const int Radius = 1000;

        private readonly List<IPolyhedron3D> _objects;
        private IPolyhedron3D _compositeObject;

        public MultipleObjectsScene(
            HomogeneousPoint3D observer,
            IPolyhedron3D initialObject,
            int distanceBetweenScreenAndObserver) : base(observer, distanceBetweenScreenAndObserver)
        {
            _objects = new List<IPolyhedron3D>();

            AddObject(initialObject);
        }

        /// <inheritdoc />
        public override void MoveObserverTo(HomogeneousPoint3D point)
        {
            foreach (var o in _objects)
            {
                MoveObserverToTransformation(point, o);
            }
            
            _observerPosition = _observerPosition.Add(point);
        }

        public override void RotateAroundAxis(Axis3D axis, double angle)
        {
            foreach (var o in _objects)
            {
                RotateAroundAxisTransformation(axis, angle, o);
            } 

            _rotationAngleAroundAxis[axis] += angle;
        }

        public override void RotateAroundVector(double angle)
        {
            foreach (var o in _objects)
            {
                RotateAroundVectorTransformation(angle, o);
            }
            
            _rotationAngleAroundVector += angle;
        }

        public override void AddObject(IPolyhedron3D obj)
        {
            _objects.Add(obj);

            CalculateObjectsPosition();

            _compositeObject = new CompositePolyhedron3D(
                new HomogeneousPoint3D(0, 0, 0, 1),
                new HomogeneousPoint3D(0, 0, 0, 1),
                _objects.ToArray(),
                (0, 0));

            _observerPosition = new HomogeneousPoint3D(0, 0, -Radius, 1);
            MoveObserverToTransformation(_observerPosition, _compositeObject);
        }

        public override void Render(
            IntPtr renderer,
            IPolygonDrawer visibleFacetDrawer,
            IPolygonDrawer notVisibleFacetDrawer,
            Point screenCenter)
        {
            RenderObj(renderer, visibleFacetDrawer, notVisibleFacetDrawer, screenCenter, _compositeObject);
        }

        private void CalculateObjectsPosition()
        {
            int objectsCount = _objects.Count;

            if (objectsCount == 1)
            {
                MovePolyhedronTo(
                    _objects[0].GeometricCenter,
                    new HomogeneousPoint3D(0, 0, 0, 1),
                    _objects[0]);
            }
            else
            {
                for (int i = 0; i < objectsCount; i++)
                {
                    double x = Radius * System.Math.Cos(2 * System.Math.PI * i / objectsCount);
                    double y = Radius * System.Math.Sin(2 * System.Math.PI * i / objectsCount);

                    MovePolyhedronTo(
                        _objects[i].GeometricCenter,
                        new HomogeneousPoint3D(x, y, 0, 1),
                        _objects[i]);
                }
            }        
        }

        private void MovePolyhedronTo(HomogeneousPoint3D currentCenter,
            HomogeneousPoint3D needCenter, IPolyhedron3D polyhedron)
        {
            var originPoint = new HomogeneousPoint3D(
                currentCenter.X - needCenter.X,
                currentCenter.Y - needCenter.Y,
                currentCenter.Z - needCenter.Z,
                needCenter.W);
            Matrix affineMatrix = AffineTransformation.MoveOriginTo(originPoint);
            polyhedron.Transform(affineMatrix);
            polyhedron.TransformRotationCenter(affineMatrix);
        }
    }
}
