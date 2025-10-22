using OpenTK.Mathematics;

namespace TinyEngine.TGraphics
{
    public class Light
    {
        public Vector3 Position { get; set; }
        public Vector3 Color { get; set; }
        public float Intensity { get; set; } = 1f;

        public Light() { }

        public Light(Vector3 position, Vector3 color, float intensity = 1f)
        {
            Position = position;
            Color = color;
            Intensity = intensity;
        }
    }
}
