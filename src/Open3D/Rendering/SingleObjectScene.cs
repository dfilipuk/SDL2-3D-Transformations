using System;
using System.Drawing;
using Clipping2D.Drawer;
using Open3D.Geometry;
using Open3D.Geometry.Polyhedron;

namespace Open3D.Rendering
{
    public sealed class SingleObjectScene : Scene
    {
        private IPolyhedron3D _object;

        public SingleObjectScene(
            HomogeneousPoint3D observer, 
            IPolyhedron3D initialObject, 
            int distanceBetweenScreenAndObserver) : base(observer, distanceBetweenScreenAndObserver)
        {
            AddObject(initialObject);
        }

        /// <inheritdoc />
        public override void MoveObserverTo(HomogeneousPoint3D point)
        {
            MoveObserverToTransformation(point, _object);
            _observerPosition = _observerPosition.Add(point);
        }

        public override void RotateAroundAxis(Axis3D axis, double angle)
        {
            RotateAroundAxisTransformation(axis, angle, _object);
            _rotationAngleAroundAxis[axis] += angle;
        }

        public override void RotateAroundVector(double angle)
        {
            RotateAroundVectorTransformation(angle, _object);
            _rotationAngleAroundVector += angle;
        }

        public override void AddObject(IPolyhedron3D obj)
        {
            _object = obj;

            MoveObserverToTransformation(_observerPosition, _object);
            RotateAroundAxisTransformation(Axis3D.OX, _rotationAngleAroundAxis[Axis3D.OX], _object);
            RotateAroundAxisTransformation(Axis3D.OY, _rotationAngleAroundAxis[Axis3D.OY], _object);
            RotateAroundAxisTransformation(Axis3D.OZ, _rotationAngleAroundAxis[Axis3D.OZ], _object);
            RotateAroundVectorTransformation(_rotationAngleAroundVector, _object);
        }

        public override void Render(
            IntPtr renderer,
            IPolygonDrawer visibleFacetDrawer,
            IPolygonDrawer notVisibleFacetDrawer,
            Point screenCenter)
        {
            RenderObj(renderer, visibleFacetDrawer, notVisibleFacetDrawer, screenCenter, _object);
        }
    }
}
