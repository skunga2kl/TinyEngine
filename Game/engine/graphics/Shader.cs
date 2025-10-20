using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace TinyEngine
{
    public class Shader
    {
        public int Handle { get; private set; }

        public Shader(string VertexPath, string FragmentPath)
        {
            string vertexSource = File.ReadAllText(VertexPath);
            string fragmentSource = File.ReadAllText(FragmentPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexSource);
            GL.CompileShader(vertexShader);
            CheckCompileError(vertexShader, "VERTEX");

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentSource);
            GL.CompileShader(fragmentShader);
            CheckCompileError(fragmentShader, "FRAGMENT");

            Handle = GL.CreateProgram();
            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            GL.LinkProgram(Handle);
            CheckLinkError(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void Delete()
        {
            GL.DeleteProgram(Handle);
        }

        public void CheckCompileError(int Shader, string Type)
        {
            GL.GetShader(Shader, ShaderParameter.CompileStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetShaderInfoLog(Shader);
                Console.WriteLine($"shader compile error ({Type}):\n{info}");
            }
        }

        public void CheckLinkError(int program)
        {
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int success);
            if (success == 0)
            {
                string info = GL.GetProgramInfoLog(program);
                Console.WriteLine($"program link error:\n{info}");
            }
        }
    }
}
