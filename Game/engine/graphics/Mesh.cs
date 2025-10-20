using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using TinyEngine.Graphics;

namespace TinyEngine
{
    public class Mesh : IRenderer
    {
        private int _vao, _vbo, _ebo;
        private readonly float[] _vertices;
        private readonly uint[] _indices;
        private Matrix4 _model;

        public Mesh(float[] vertices, uint[] indices, Matrix4? model = null)
        {
            _vertices = vertices;
            _indices = indices;
            _model = model ?? Matrix4.Identity;
        }

        public void Initialize()
        {
            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            _ebo = GL.GenBuffer();

            GL.BindVertexArray(_vao);

            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, _indices.Length * sizeof(uint), _indices, BufferUsageHint.StaticDraw);

            // position
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            // color
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
        }

        public void Draw(Shader shader, Matrix4 view, Matrix4 projection)
        {
            shader.Use();
            GL.BindVertexArray(_vao);

            GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "model"), false, ref _model);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "view"), false, ref view);
            GL.UniformMatrix4(GL.GetUniformLocation(shader.Handle, "projection"), false, ref projection);

            GL.DrawElements(PrimitiveType.Triangles, _indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        public void Cleanup()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteBuffer(_ebo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
