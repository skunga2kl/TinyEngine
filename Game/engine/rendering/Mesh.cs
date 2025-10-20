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

        public Material Material { get; set; } = new();

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

            int vertexSize = _vertices.Length % 8 == 0 ? 8 : 6;

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, vertexSize * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, vertexSize * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            if (vertexSize >= 8)
            {
                GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, vertexSize * sizeof(float), 6 * sizeof(float));
                GL.EnableVertexAttribArray(2);
            }

            GL.BindVertexArray(0);
        }

        public void Draw(Shader shader, Matrix4 view, Matrix4 projection)
        {
            shader.Use();

            shader.SetMatrix4("model", _model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            shader.SetVector3("material.ambient", Material.Ambient);
            shader.SetVector3("material.diffuse", Material.Diffuse);
            shader.SetVector3("material.specular", Material.Specular);
            shader.SetFloat("material.shininess", Material.Shininess);

            if (Material.DiffuseTexture != null)
            {
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, Material.DiffuseTexture.Handle);
                shader.SetInt("material.hasTexture", 1);
                shader.SetInt("material.diffuseTexture", 0);
            }
            else
            {
                shader.SetInt("material.hasTexture", 0);
            }

            GL.BindVertexArray(_vao);
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
