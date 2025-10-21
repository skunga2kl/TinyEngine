using OpenTK.Mathematics;

namespace TinyEngine.Graphics
{  
    public class Material
    {
        public Vector3 Ambient { get; set; } = new(0.2f);
        public Vector3 Diffuse { get; set; } = new(0.5f);
        public Vector3 Specular { get; set; } = new(1.0f);
        public float Shininess { get; set; } = 20f;

        public Texture DiffuseTexture { get; set; }
        public bool UseTexture => DiffuseTexture != null;
    }
}
