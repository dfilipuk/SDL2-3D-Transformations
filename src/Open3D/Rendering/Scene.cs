using System;
using System.Collections.Generic;
using System.Drawing;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Rendering
{
    public abstract class Scene : IScene
    {
        protected HomogeneousPoint3D _observerPosition;
        protected IPolyhedron3D _object;

        protected double _rotationAngleAroundVector = 0;
        protected readonly Dictionary<Axis3D, double> _rotationAngleAroundAxis = new Dictionary<Axis3D, double>
        {
            { Axis3D.OX, 0 },
            { Axis3D.OY, 0 },
            { Axis3D.OZ, 0 }
        };
        protected int _distanceBetweenScreenAndObserver;

        protected Scene(
            HomogeneousPoint3D observer, 
            int distanceBetweenScreenAndObserver)
        {
            _observerPosition = observer;
            _distanceBetweenScreenAndObserver = distanceBetweenScreenAndObserver;
        }

        protected void MoveObserverToTransformation(HomogeneousPoint3D point)
        {
            Matrix affineMatrix = AffineTransformation.MoveOriginTo(point);
            _object.Transform(affineMatrix);
            _object.TransformRotationCenter(affineMatrix);
        }

        protected void RotateAroundAxisTransformation(Axis3D axis, double angle)
        {
            Matrix affineMatrix = AffineTransformation.RotateAroundAxisAtPoint(axis, _object.RotationCenter, angle);
            _object.Transform(affineMatrix);
        }

        protected void RotateAroundVectorTransformation(double angle)
        {
            Matrix affineMatrix = AffineTransformation.RotateAroundVector(_object.RotationVector, angle);
            _object.Transform(affineMatrix);
        }

        public abstract void AddObject(IPolyhedron3D polyhedron);

        /// <inheritdoc />
        public void MoveObserverTo(HomogeneousPoint3D point)
        {
            MoveObserverToTransformation(point);
            _observerPosition = _observerPosition.Add(point);
        }

        public void MoveDisplay(int moveDistance)
        {
            _distanceBetweenScreenAndObserver += moveDistance;
        }

        public void RotateAroundAxis(Axis3D axis, double angle)
        {
            RotateAroundAxisTransformation(axis, angle);
            _rotationAngleAroundAxis[axis] += angle;
        }

        public void RotateAroundVector(double angle)
        {
            RotateAroundVectorTransformation(angle);
            _rotationAngleAroundVector += angle;
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
