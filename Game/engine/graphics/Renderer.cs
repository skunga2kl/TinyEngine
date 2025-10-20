using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TinyEngine.Graphics;

namespace TinyEngine
{
    public class Renderer
    {
        private readonly List<IRenderer> _objects = new();
        private Shader _shader;

        public void Initialize()
        {
            _shader = new Shader("shaders/vertexShader.glsl", "shaders/fragmentShader.glsl");
        }

        public void AddObject(IRenderer obj)
        {
            if (obj is Mesh mesh)
                mesh.Initialize();
            _objects.Add(obj);
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            foreach (var obj in _objects)
                obj.Draw(_shader, view, projection);
        }

        public void Cleanup()
        {
            foreach (var obj in _objects)
                obj.Cleanup();
            _shader.Delete();
        }
    }
}
