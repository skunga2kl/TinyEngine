using System;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TinyEngine.Camera;
using TinyEngine.Core;
using TinyEngine.TGraphics;

namespace TinyEngine
{
    public class Game : GameWindow
    {
        private Renderer _renderer;
        private FreeCam _camera;
        private Scene _scene;
        private DevConsole _devConsole;
        private Mesh _cube;

        private bool _firstMove = true;
        private Vector2 _lastMousePos;

        public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _renderer = new Renderer();
            _renderer.Initialize();

            _camera = new FreeCam(new Vector3(0f, 1.5f, 4f), Quaternion.Identity);
            CursorState = CursorState.Grabbed;

            _scene = new Scene();

            float[] cubeVertices = {
            // positions          // normals           // UVs
            // front face
            -0.5f, -0.5f,  0.5f,  0f,  0f,  1f,  0f, 0f,
            0.5f, -0.5f,  0.5f,  0f,  0f,  1f,  1f, 0f,
            0.5f,  0.5f,  0.5f,  0f,  0f,  1f,  1f, 1f,
            -0.5f,  0.5f,  0.5f,  0f,  0f,  1f,  0f, 1f,

            // back face
            -0.5f, -0.5f, -0.5f,  0f,  0f, -1f,  1f, 0f,
            0.5f, -0.5f, -0.5f,  0f,  0f, -1f,  0f, 0f,
            0.5f,  0.5f, -0.5f,  0f,  0f, -1f,  0f, 1f,
            -0.5f,  0.5f, -0.5f,  0f,  0f, -1f,  1f, 1f,

            // left face
            -0.5f, -0.5f, -0.5f, -1f,  0f,  0f,  0f, 0f,
            -0.5f, -0.5f,  0.5f, -1f,  0f,  0f,  1f, 0f,
            -0.5f,  0.5f,  0.5f, -1f,  0f,  0f,  1f, 1f,
            -0.5f,  0.5f, -0.5f, -1f,  0f,  0f,  0f, 1f,

            // right face
            0.5f, -0.5f, -0.5f,  1f,  0f,  0f,  1f, 0f,
            0.5f, -0.5f,  0.5f,  1f,  0f,  0f,  0f, 0f,
            0.5f,  0.5f,  0.5f,  1f,  0f,  0f,  0f, 1f,
            0.5f,  0.5f, -0.5f,  1f,  0f,  0f,  1f, 1f,

            // top face
            -0.5f,  0.5f, -0.5f,  0f,  1f,  0f,  0f, 1f,
            0.5f,  0.5f, -0.5f,  0f,  1f,  0f,  1f, 1f,
            0.5f,  0.5f,  0.5f,  0f,  1f,  0f,  1f, 0f,
            -0.5f,  0.5f,  0.5f,  0f,  1f,  0f,  0f, 0f,

            // bottom face
            -0.5f, -0.5f, -0.5f,  0f, -1f,  0f,  0f, 0f,
            0.5f, -0.5f, -0.5f,  0f, -1f,  0f,  1f, 0f,
            0.5f, -0.5f,  0.5f,  0f, -1f,  0f,  1f, 1f,
            -0.5f, -0.5f,  0.5f,  0f, -1f,  0f,  0f, 1f
            };

            uint[] cubeIndices = {
            0, 1, 2, 2, 3, 0,       // front
            4, 5, 6, 6, 7, 4,       // back
            8, 9,10,10,11, 8,       // left
            12,13,14,14,15,12,      // right
            16,17,18,18,19,16,      // top
            20,21,22,22,23,20       // bottom
            };

            var cubeTexture = new Texture("texture/cubetest.png");
            _cube = new Mesh(cubeVertices, cubeIndices)
            {
                Material = new Material()
                {
                    Ambient = new Vector3(0.2f),
                    Diffuse = new Vector3(0.5f),
                    Specular = new Vector3(0.5f),
                    Shininess = 32f,
                    DiffuseTexture = cubeTexture
                }
            };
            _cube.Transform.position = new Vector3(0f, 1f, 0f);
            _scene.Add(_cube);

            float[] floorVertices = {
                -5f, 0f, -5f, 0f, 1f, 0f, 0f, 0f,
                 5f, 0f, -5f, 0f, 1f, 0f, 5f, 0f,
                 5f, 0f,  5f, 0f, 1f, 0f, 5f, 5f,
                -5f, 0f,  5f, 0f, 1f, 0f, 0f, 5f
            };

            uint[] floorIndices = { 0, 1, 2, 2, 3, 0 };

            var floorTexture = new Texture("texture/floortest.png");
            var floor = new Mesh(floorVertices, floorIndices)
            {
                Material = new Material()
                {
                    Ambient = new Vector3(0.2f),
                    Diffuse = new Vector3(0.5f),
                    Specular = new Vector3(0.1f),
                    Shininess = 2f,
                    DiffuseTexture = floorTexture
                }
            };
            _scene.Add(floor);

            var light = new Light
            {
                Position = new Vector3(2f, 4f, 2f),
                Color = new Vector3(1f),
                Intensity = 1.5f
            };
            _scene.Add(light);

            _scene.LoadInto(_renderer);

            _devConsole = new DevConsole();

            _devConsole.Register("move", args =>
            {
                if (args.Length < 3) { Console.WriteLine("Usage: move x y z"); return; }

                if (float.TryParse(args[0], out float x) &&
                    float.TryParse(args[1], out float y) &&
                    float.TryParse(args[2], out float z))
                {
                    _cube.Transform.position = new Vector3(x, y, z);
                    Console.WriteLine($"Moved cube to: {_cube.Transform.position}");
                }
            });

            _devConsole.Register("scale", args =>
            {
                if (args.Length < 3) { Console.WriteLine("Usage: scale x y z"); return; }

                if (float.TryParse(args[0], out float x) &&
                    float.TryParse(args[1], out float y) &&
                    float.TryParse(args[2], out float z))
                {
                    _cube.Transform.scale = new Vector3(x, y, z);
                    Console.WriteLine($"Scaled cube to: {_cube.Transform.scale}");
                }
            });

            Task.Run(() =>
            {
                while (true)
                {
                    Console.Write("> ");
                    string? input = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(input))
                        _devConsole.Execute(input);
                }
            });
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (!IsFocused) return;

            float dt = (float)args.Time;
            var input = KeyboardState;

            Vector3 moveDir = Vector3.Zero;
            if (input.IsKeyDown(Keys.W)) moveDir += _camera.Front;
            if (input.IsKeyDown(Keys.S)) moveDir -= _camera.Front;
            if (input.IsKeyDown(Keys.A)) moveDir -= _camera.Right;
            if (input.IsKeyDown(Keys.D)) moveDir += _camera.Right;
            if (input.IsKeyDown(Keys.Space)) moveDir += _camera.Up;
            if (input.IsKeyDown(Keys.LeftShift)) moveDir -= _camera.Up;

            if (moveDir.LengthSquared > 0)
                _camera.Move(Vector3.Normalize(moveDir), dt);

            var mouse = MouseState;
            if (_firstMove)
            {
                _lastMousePos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var delta = new Vector2(mouse.X - _lastMousePos.X, mouse.Y - _lastMousePos.Y);
                _lastMousePos = new Vector2(mouse.X, mouse.Y);
                _camera.Rotate(delta.X, delta.Y);
            }

            if (input.IsKeyPressed(Keys.Escape))
                Close();
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(70f),
                Size.X / (float)Size.Y,
                0.1f, 100f);

            _renderer.SetCameraPosition(_camera.Position);
            _renderer.Render(view, projection);

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _renderer.Cleanup();
        }

        public static void Main()
        {
            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings
            {
                Size = new Vector2i(1280, 720),
                Title = "TinyEngine"
            };

            using var game = new Game(gws, nws);
            game.Run();
        }
    }
}
