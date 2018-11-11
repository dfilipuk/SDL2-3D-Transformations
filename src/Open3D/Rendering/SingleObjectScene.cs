using System;
using System.Drawing;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Rendering
{
    public class SingleObjectScene : IScene
    {
        private readonly HomogeneousPoint3D _observerInitialPosition;
        private readonly IPolyhedron3D _polyhedron;
        private readonly AffineTransformationBuilder _affineTransformationBuilder;
        private int _distanceBetweenScreenAndObserver;

        public SingleObjectScene(HomogeneousPoint3D observer, IPolyhedron3D polyhedron, int distanceBetweenScreenAndObserver)
        {
            _observerInitialPosition = observer;
            _polyhedron = polyhedron;
            _distanceBetweenScreenAndObserver = distanceBetweenScreenAndObserver;
            _affineTransformationBuilder = new AffineTransformationBuilder();
        }

        /// <inheritdoc />
        public void Initialize()
        {
            MoveObserverTo(_observerInitialPosition);
        }

        /// <inheritdoc />
        public void MoveObserverTo(HomogeneousPoint3D point)
        {
            Matrix affineMatrix = _affineTransformationBuilder.MoveOriginTo(point);
            _polyhedron.Transform(affineMatrix);
            _polyhedron.TransformRotationCenter(affineMatrix);
        }

        public void MoveDisplay(int moveDistance)
        {
            _distanceBetweenScreenAndObserver += moveDistance;
        }

        public void RotateAroundAxis(Axis3D axis, double angle)
        {
            Matrix affineMatrix = _affineTransformationBuilder.RotateAroundAxisAtPoint(axis, _polyhedron.RotationCenter, angle);
            _polyhedron.Transform(affineMatrix);
        }

        public void Render(IntPtr renderer, IPolygonDrawer drawer, Point screenCenter)
        {
            _polyhedron.ProjectVertexesToScreen(_distanceBetweenScreenAndObserver, screenCenter);
            _polyhedron.CalculateVisibleFacets();

            foreach (var facet in _polyhedron.VisibleFacets)
            {
                facet.Projection.Draw(renderer, drawer);
            }
        }
    }
}
