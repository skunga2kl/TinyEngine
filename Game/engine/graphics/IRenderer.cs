using OpenTK.Mathematics;

namespace TinyEngine.Graphics
{
    public interface IRenderer
    {
        void Draw(Shader shader, Matrix4 view, Matrix4 projection);
        void Cleanup();
    }
}
