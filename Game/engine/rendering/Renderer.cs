using System.Collections.Generic;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TinyEngine.TGraphics;

namespace TinyEngine
{
    public class Renderer
    {
        private readonly List<IRenderer> _objects = new();
        private readonly List<Light> _lights = new(); 
        private Shader _shader;

        private Vector3 _cameraPos = Vector3.Zero;

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

        public void AddLight(Light light)
        {
            _lights.Add(light);
        }

        public void SetCameraPosition(Vector3 position)
        {
            _cameraPos = position;
        }

        public void Render(Matrix4 view, Matrix4 projection)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            for (int i = 0; i < _lights.Count; i++)
            {
                var light = _lights[i];
                string prefix = $"lights[{i}]";

                GL.Uniform3(GL.GetUniformLocation(_shader.Handle, $"{prefix}.position"), light.Position);
                GL.Uniform3(GL.GetUniformLocation(_shader.Handle, $"{prefix}.color"), light.Color);
                GL.Uniform1(GL.GetUniformLocation(_shader.Handle, $"{prefix}.intensity"), light.Intensity);
            }

            GL.Uniform1(GL.GetUniformLocation(_shader.Handle, "lightCount"), _lights.Count);
            GL.Uniform3(GL.GetUniformLocation(_shader.Handle, "viewPos"), _cameraPos);

            foreach (var obj in _objects)
                obj.Draw(_shader, view, projection);
        }

        public void ClearObjects()
        {
            _objects.Clear();
            _lights.Clear();
        }

        public void Cleanup()
        {
            foreach (var obj in _objects)
                obj.Cleanup();
            _shader.Delete();
        }
    }
}
