using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TinyEngine.Camera;
using TinyEngine.Graphics;

namespace TinyEngine
{
    public class Game : GameWindow
    {
        private Renderer _renderer;
        private FreeCam _camera;
        private Vector2 _lastMousePos;
        private bool _firstMove = true;

        public Game(GameWindowSettings gws, NativeWindowSettings nws) : base(gws, nws) { }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _renderer = new Renderer();
            _renderer.Initialize();

            _camera = new FreeCam(new Vector3(0f, 1.5f, 4f), new Quaternion(0, 0, 0));
            CursorState = CursorState.Grabbed;

            float[] cubeVertices = {
        // positions          // normals          
        -0.5f, -0.5f, -0.5f,  0f,  0f, -1f,  0f, 0f,
         0.5f, -0.5f, -0.5f,  0f,  0f, -1f,  1f, 0f,
         0.5f,  0.5f, -0.5f,  0f,  0f, -1f,  1f, 1f,
        -0.5f,  0.5f, -0.5f,  0f,  0f, -1f,  0f, 1f,

        -0.5f, -0.5f,  0.5f,  0f,  0f,  1f,  0f, 0f,
         0.5f, -0.5f,  0.5f,  0f,  0f,  1f,  1f, 0f,
         0.5f,  0.5f,  0.5f,  0f,  0f,  1f,  1f, 1f,
        -0.5f,  0.5f,  0.5f,  0f,  0f,  1f,  0f, 1f,

        -0.5f,  0.5f,  0.5f, -1f,  0f,  0f,  0f, 0f,
        -0.5f,  0.5f, -0.5f, -1f,  0f,  0f,  1f, 0f,
        -0.5f, -0.5f, -0.5f, -1f,  0f,  0f,  1f, 1f,
        -0.5f, -0.5f,  0.5f, -1f,  0f,  0f,  0f, 1f,

         0.5f,  0.5f,  0.5f,  1f,  0f,  0f,  0f, 0f,
         0.5f,  0.5f, -0.5f,  1f,  0f,  0f,  1f, 0f,
         0.5f, -0.5f, -0.5f,  1f,  0f,  0f,  1f, 1f,
         0.5f, -0.5f,  0.5f,  1f,  0f,  0f,  0f, 1f,

        -0.5f, -0.5f, -0.5f,  0f, -1f,  0f,  0f, 0f,
         0.5f, -0.5f, -0.5f,  0f, -1f,  0f,  1f, 0f,
         0.5f, -0.5f,  0.5f,  0f, -1f,  0f,  1f, 1f,
        -0.5f, -0.5f,  0.5f,  0f, -1f,  0f,  0f, 1f,

        -0.5f,  0.5f, -0.5f,  0f,  1f,  0f,  0f, 0f,
         0.5f,  0.5f, -0.5f,  0f,  1f,  0f,  1f, 0f,
         0.5f,  0.5f,  0.5f,  0f,  1f,  0f,  1f, 1f,
        -0.5f,  0.5f,  0.5f,  0f,  1f,  0f,  0f, 1f
    };

            uint[] cubeIndices = {
        0, 1, 2, 2, 3, 0,
        4, 5, 6, 6, 7, 4,
        8, 9,10,10,11, 8,
        12,13,14,14,15,12,
        16,17,18,18,19,16,
        20,21,22,22,23,20
    };

            float[] floorVertices = {
        // positions        // normals     
        -5f, 0f, -5f,  0f, 1f, 0f,  0f, 0f,
         5f, 0f, -5f,  0f, 1f, 0f,  5f, 0f,
         5f, 0f,  5f,  0f, 1f, 0f,  5f, 5f,
        -5f, 0f,  5f,  0f, 1f, 0f,  0f, 5f
    };

            uint[] floorIndices = { 0, 1, 2, 2, 3, 0 };

            var dullCubeTexture = new Texture("texture/dulltest.png");
            var dullCube = new Mesh(cubeVertices, cubeIndices, Matrix4.CreateTranslation(1f, 0.5f, 0f))
            {
                Material = new Material()
                {
                    Ambient = new Vector3(0.2f, 0.1f, 0.1f),
                    Diffuse = new Vector3(0.6f, 0.3f, 0.3f),
                    Specular = new Vector3(0.1f),
                    Shininess = 4f,
                    DiffuseTexture = dullCubeTexture
                }
            };
            _renderer.AddObject(dullCube);

            var shinyCube = new Mesh(cubeVertices, cubeIndices, Matrix4.CreateTranslation(-1f, 0.5f, 0f));
            shinyCube.Material = new Material()
            {
                Ambient = new Vector3(0.1f, 0.1f, 0.2f),
                Diffuse = new Vector3(0.3f, 0.3f, 0.6f),
                Specular = new Vector3(1f),
                Shininess = 33f
            };
            _renderer.AddObject(shinyCube);

            var cubeTexture = new Texture("texture/cubetest.png");
            var texturedCube = new Mesh(cubeVertices, cubeIndices, Matrix4.CreateTranslation(0f, 0.5f, -2f))
            {
                Material = new Material()
                {
                    Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                    Diffuse = new Vector3(0.5f, 0.5f, 0.5f),
                    Specular = new Vector3(0.1f),
                    Shininess = 7f,
                    DiffuseTexture = cubeTexture
                }
            };
            _renderer.AddObject(texturedCube);

            var floorTexture = new Texture("texture/floortest.png");
            var floorMesh = new Mesh(floorVertices, floorIndices, Matrix4.Identity)
            {
                Material = new Material()
                {
                    Ambient = new Vector3(0.2f, 0.2f, 0.2f),
                    Diffuse = new Vector3(0.5f, 0.5f, 0.5f),
                    Specular = new Vector3(0.1f),
                    Shininess = 2f,
                    DiffuseTexture = floorTexture
                }
            };
            _renderer.AddObject(floorMesh);

            var light1 = new Light(new Vector3(2f, 4f, 2f), new Vector3(1f, 1f, 1f), 1.0f);
            _renderer.AddLight(light1);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(70f),
                Size.X / (float)Size.Y,
                0.1f,
                100f);

            _renderer.SetCameraPosition(_camera.Position);
            _renderer.Render(view, projection);

            SwapBuffers();
        }


        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            if (!IsFocused) return;

            var input = KeyboardState;
            float dt = (float)args.Time;

            Vector3 moveDir = Vector3.Zero;
            if (input.IsKeyDown(Keys.W)) moveDir += _camera.Front;
            if (input.IsKeyDown(Keys.S)) moveDir -= _camera.Front;
            if (input.IsKeyDown(Keys.A)) moveDir -= _camera.Right;
            if (input.IsKeyDown(Keys.D)) moveDir += _camera.Right;
            if (input.IsKeyDown(Keys.Space)) moveDir += _camera.Up;
            if (input.IsKeyDown(Keys.LeftShift)) moveDir -= _camera.Up;

            if (input.IsKeyDown(Keys.R))
                _camera.Position = Vector3.Zero;

            if (moveDir.LengthSquared > 0)
                _camera.Move(Vector3.Normalize(moveDir), dt);

            if (input.IsKeyDown(Keys.K))
                CursorState = CursorState.Normal;

            else if (input.IsKeyDown(Keys.L))
                CursorState = CursorState.Grabbed;

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

        protected override void OnUnload()
        {
            base.OnUnload();
            _renderer.Cleanup();
        }

        public static void Main()
        {
            var gws = GameWindowSettings.Default;
            var nws = new NativeWindowSettings()
            {
                Size = new Vector2i(1280, 720),
                Title = "window"
            };

            using (var game = new Game(gws, nws))
                game.Run();
        }
    }
}
