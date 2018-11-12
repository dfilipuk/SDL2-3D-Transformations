using System;
using System.Collections.Generic;
using System.Drawing;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Rendering
{
    public class SingleObjectScene : IScene
    {
        private HomogeneousPoint3D _observerPosition;
        private IPolyhedron3D _object;

        private double _rotationAngleAroundVector = 0;
        private readonly Dictionary<Axis3D, double> _rotationAngleAroundAxis = new Dictionary<Axis3D, double>
        {
            { Axis3D.OX, 0 },
            { Axis3D.OY, 0 },
            { Axis3D.OZ, 0 }
        };
        private int _distanceBetweenScreenAndObserver;

        public SingleObjectScene(HomogeneousPoint3D observer, IPolyhedron3D initialObject, int distanceBetweenScreenAndObserver)
        {
            _observerPosition = observer;
            _object = initialObject;
            _distanceBetweenScreenAndObserver = distanceBetweenScreenAndObserver;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            Matrix affineMatrix = AffineTransformation.MoveOriginTo(_observerPosition);
            _object.Transform(affineMatrix);
            _object.TransformRotationCenter(affineMatrix);
        }

        /// <inheritdoc />
        public void MoveObserverTo(HomogeneousPoint3D point)
        {
            Matrix affineMatrix = AffineTransformation.MoveOriginTo(point);
            _object.Transform(affineMatrix);
            _object.TransformRotationCenter(affineMatrix);
            _observerPosition = _observerPosition.Add(point);
        }

        public void MoveDisplay(int moveDistance)
        {
            _distanceBetweenScreenAndObserver += moveDistance;
        }

        public void RotateAroundAxis(Axis3D axis, double angle)
        {
            Matrix affineMatrix = AffineTransformation.RotateAroundAxisAtPoint(axis, _object.RotationCenter, angle);
            _object.Transform(affineMatrix);
            _rotationAngleAroundAxis[axis] += angle;
        }

        public void RotateAroundVector(double angle)
        {
            Matrix affineMatrix = AffineTransformation.RotateAroundVector(_object.RotationVector, angle);
            _object.Transform(affineMatrix);
            _rotationAngleAroundVector += angle;
        }

        public void AddObject(IPolyhedron3D obj)
        {
            Matrix affineMatrix;
            _object = obj;
            Initialize();
            affineMatrix = AffineTransformation.RotateAroundAxisAtPoint(Axis3D.OX, _object.RotationCenter, _rotationAngleAroundAxis[Axis3D.OX]);
            _object.Transform(affineMatrix);
            affineMatrix = AffineTransformation.RotateAroundAxisAtPoint(Axis3D.OY, _object.RotationCenter, _rotationAngleAroundAxis[Axis3D.OY]);
            _object.Transform(affineMatrix);
            affineMatrix = AffineTransformation.RotateAroundAxisAtPoint(Axis3D.OZ, _object.RotationCenter, _rotationAngleAroundAxis[Axis3D.OZ]);
            _object.Transform(affineMatrix);
            affineMatrix = AffineTransformation.RotateAroundVector(_object.RotationVector, _rotationAngleAroundVector);
            _object.Transform(affineMatrix);
        }

        public void Render(IntPtr renderer, IPolygonDrawer visibleFacetDrawer, IPolygonDrawer notVisibleFacetDrawer, Point screenCenter)
        {
            _object.ProjectVertexesToScreen(_distanceBetweenScreenAndObserver, screenCenter);
            _object.CalculateVisibilityOfFacets();

            foreach (var facet in _object.VisibleFacets)
            {
                facet.Projection.Draw(renderer, visibleFacetDrawer);
            }

            foreach (var facet in _object.NotVisibleFacets)
            {
                facet.Projection.Draw(renderer, notVisibleFacetDrawer);
            }
        }
    }
}
