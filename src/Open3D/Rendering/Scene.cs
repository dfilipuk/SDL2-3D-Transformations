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

        protected void MoveObserverToTransformation(HomogeneousPoint3D point, IPolyhedron3D obj)
        {
            Matrix affineMatrix = AffineTransformation.MoveOriginTo(point);
            obj.Transform(affineMatrix);
            obj.TransformRotationCenter(affineMatrix);
        }

        protected void RotateAroundAxisTransformation(Axis3D axis, double angle, IPolyhedron3D obj)
        {
            Matrix affineMatrix = AffineTransformation.RotateAroundAxisAtPoint(axis, obj.RotationCenter, angle);
            obj.Transform(affineMatrix);
        }

        protected void RotateAroundVectorTransformation(double angle, IPolyhedron3D obj)
        {
            Matrix affineMatrix = AffineTransformation.RotateAroundVector(obj.RotationVector, angle);
            obj.Transform(affineMatrix);
        }

        protected void RenderObj(
            IntPtr renderer,
            IPolygonDrawer visibleFacetDrawer,
            IPolygonDrawer notVisibleFacetDrawer,
            Point screenCenter,
            IPolyhedron3D obj)
        {
            obj.ProjectVertexesToScreen(_distanceBetweenScreenAndObserver, screenCenter);
            obj.CalculateVisibilityOfFacets();

            foreach (var facet in obj.VisibleFacets)
            {
                facet.Projection.Draw(renderer, visibleFacetDrawer);
            }

            foreach (var facet in obj.NotVisibleFacets)
            {
                facet.Projection.Draw(renderer, notVisibleFacetDrawer);
            }
        }

        public virtual void MoveDisplay(int moveDistance)
        {
            _distanceBetweenScreenAndObserver += moveDistance;
        }

        public abstract void MoveObserverTo(HomogeneousPoint3D point);
        public abstract void RotateAroundAxis(Axis3D axis, double angle);
        public abstract void RotateAroundVector(double angle);
        public abstract void AddObject(IPolyhedron3D polyhedron);
        public abstract void Render(
            IntPtr renderer, 
            IPolygonDrawer visibleFacetDrawer, 
            IPolygonDrawer notVisibleFacetDrawer, 
            Point screenCenter);
    }
}
