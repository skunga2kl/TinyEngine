using System;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using TinyEngine.Camera;

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
            GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
            GL.Enable(EnableCap.DepthTest);

            _renderer = new Renderer();
            _renderer.Initialize();

            _camera = new FreeCam(new Vector3(0f, 0f, 3f), new Quaternion(0, 0, 0));
            CursorState = CursorState.Grabbed;

            float[] cubeVertices = {
                // positions         // colors
                -0.5f, -0.5f, -0.5f,  1f, 0f, 0f,
                 0.5f, -0.5f, -0.5f,  0f, 1f, 0f,
                 0.5f,  0.5f, -0.5f,  0f, 0f, 1f,
                -0.5f,  0.5f, -0.5f,  1f, 1f, 0f,
                -0.5f, -0.5f,  0.5f,  1f, 0f, 1f,
                 0.5f, -0.5f,  0.5f,  0f, 1f, 1f,
                 0.5f,  0.5f,  0.5f,  1f, 1f, 1f,
                -0.5f,  0.5f,  0.5f,  0f, 0f, 0f
            };

            uint[] cubeIndices = {
                0,1,2, 2,3,0,
                1,5,6, 6,2,1,
                7,6,5, 5,4,7,
                4,0,3, 3,7,4,
                4,5,1, 1,0,4,
                3,2,6, 6,7,3
            };

            _renderer.AddObject(new Mesh(cubeVertices, cubeIndices, Matrix4.CreateTranslation(-1f, 0f, 0f)));
            _renderer.AddObject(new Mesh(cubeVertices, cubeIndices, Matrix4.CreateTranslation(1f, 0f, 0f)));
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
                _camera.Position = new Vector3(0f, 0f, 3f);

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
