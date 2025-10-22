using OpenTK.Mathematics;

namespace TinyEngine.Core
{
    public class Transform
    {
        public Vector3 position { get; set; } = Vector3.Zero;
        public Vector3 rotation { get; set; } = Vector3.Zero;
        public Vector3 scale { get; set; } = Vector3.One;

        public Matrix4 GetMatrix()
        {
            Matrix4 scale = Matrix4.CreateScale(this.scale);
            Matrix4 rotationX = Matrix4.CreateRotationX(rotation.X);
            Matrix4 rotationY = Matrix4.CreateRotationY(rotation.Y);
            Matrix4 rotationZ = Matrix4.CreateRotationZ(rotation.Z);
            Matrix4 translation = Matrix4.CreateTranslation(position);  

            return scale * rotationZ * rotationY * rotationX * translation;
        }
    }
}
