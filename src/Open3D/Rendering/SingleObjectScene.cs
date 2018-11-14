using Open3D.Geometry;
using Open3D.Geometry.Polyhedron;
using Open3D.Math;

namespace Open3D.Rendering
{
    public sealed class SingleObjectScene : Scene
    {
        public SingleObjectScene(
            HomogeneousPoint3D observer, 
            IPolyhedron3D initialObject, 
            int distanceBetweenScreenAndObserver) : base(observer, distanceBetweenScreenAndObserver)
        {
            AddObject(initialObject);
        }

        public override void AddObject(IPolyhedron3D obj)
        {
            _object = obj;

            MoveObserverToTransformation(_observerPosition);
            RotateAroundAxisTransformation(Axis3D.OX, _rotationAngleAroundAxis[Axis3D.OX]);
            RotateAroundAxisTransformation(Axis3D.OY, _rotationAngleAroundAxis[Axis3D.OY]);
            RotateAroundAxisTransformation(Axis3D.OZ, _rotationAngleAroundAxis[Axis3D.OZ]);
            RotateAroundVectorTransformation(_rotationAngleAroundVector);
        }
    }
}
