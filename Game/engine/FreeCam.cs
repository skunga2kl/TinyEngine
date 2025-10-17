using OpenTK.Mathematics;

namespace window
{
    public class FreeCam
    {
        public Vector3 Position { get; set; }
        public float Pitch { get; set; } = 0f;
        public float Yaw { get; set; } = -90f;
        public float Speed { get; set; } = 3f;
        public float Sensitivity { get; set; } = 0.2f;

        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        public FreeCam(Vector3 startPosition)
        {
            Position = startPosition;
            UpdateVectors();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        public void Move(Vector3 dir, float deltaTime)
        {
            float velocity = Speed * deltaTime;
            Position += dir * velocity;
        }

        public void Rotate(float deltaX, float deltaY)
        {
            deltaX *= Sensitivity;
            deltaY *= Sensitivity;

            Yaw += deltaX;
            Pitch -= deltaY;

            Pitch = MathHelper.Clamp(Pitch, -89f, 89f);
            UpdateVectors();
        }

        private void UpdateVectors()
        {
            Vector3 front;
            front.X = MathF.Cos(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));
            front.Y = MathF.Sin(MathHelper.DegreesToRadians(Pitch));
            front.Z = MathF.Sin(MathHelper.DegreesToRadians(Yaw)) * MathF.Cos(MathHelper.DegreesToRadians(Pitch));

            _front = Vector3.Normalize(front);
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;
    }
}
