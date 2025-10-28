using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
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
        private bool _cursorVisible = false;

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
                    Diffuse = new Vector3(0.3f),
                    Specular = new Vector3(0.4f),
                    Shininess = 5f,
                    DiffuseTexture = cubeTexture
                }
            };
            _cube.Transform.position = new Vector3(0f, 1f, 0f);

            _cube.Name = "superman";
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
                if (args.Length < 4)
                {
                    Console.WriteLine("Usage: move <name> x y z");
                    return;
                }

                string name = args[0];

                if (float.TryParse(args[1], out float x) &&
                    float.TryParse(args[2], out float y) &&
                    float.TryParse(args[3], out float z))
                {
                    var mesh = _scene.FindObject(name);
                    if (mesh == null)
                    {
                        Console.WriteLine($"Object '{name}' not found.");
                        return;
                    }

                    mesh.Transform.position = new Vector3(x, y, z);
                    Console.WriteLine($"Moved '{name}' to ({x}, {y}, {z}).");
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

            _devConsole.Register("close", args =>
            {
                Close();
            });

            _devConsole.Register("rotate", args =>
            {
                if (args.Length < 4)
                {
                    Console.WriteLine("Usage: rotate <name> x y z");
                    return;
                }

                string name = args[0];

                if (float.TryParse(args[1], out float x) &&
                    float.TryParse(args[2], out float y) &&
                    float.TryParse(args[3], out float z))
                {
                    var mesh = _scene.FindObject(name);
                    if (mesh == null)
                    {
                        Console.WriteLine($"Object '{name}' not found.");
                        return;
                    }

                    Vector3 Rotation = new Vector3(
                        MathHelper.DegreesToRadians(x),
                        MathHelper.DegreesToRadians(y),
                        MathHelper.DegreesToRadians(z)
                    );

                    mesh.Transform.rotation = Rotation;

                    Console.WriteLine($"Rotated '{name}' to {x}, {y}, {z} degrees.");
                }
                else
                {
                    Console.WriteLine("Invalid arguments. Usage: rotate <name> x y z");
                }
            });

            _devConsole.Register("listobj", args =>
            {
                _scene.ListObjectNames();
            });

            _devConsole.Register("setname", args =>
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: setname <oldName> <newName>");
                    return;
                }

                string oldName = args[0];
                string newName = args[1];

                var mesh = _scene.FindObject(oldName);
                if (mesh == null)
                {
                    Console.WriteLine($"Object '{oldName}' not found.");
                    return;
                }

                mesh.Name = newName;
                Console.WriteLine($"Renamed '{oldName}' to '{newName}'.");
            });

            _devConsole.Register("delete", args =>
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("Usage: delete <name>");
                    return;
                }

                string name = args[0];

                if (_scene.Remove(name, _renderer))
                    Console.WriteLine($"Deleted '{name}' from scene.");
                else
                    Console.WriteLine($"Object '{name}' not found.");
            });

            _devConsole.Register("spawn", args =>
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("Usage: spawn <name> [x y z]");
                    return;
                }

                string name = args[0];
                Vector3 position = Vector3.Zero;

                if (args.Length >= 4 &&
                    float.TryParse(args[1], out float x) &&
                    float.TryParse(args[2], out float y) &&
                    float.TryParse(args[3], out float z))
                {
                    position = new Vector3(x, y, z);
                }

                GLThread.Enqueue(() =>
                {
                    var mesh = Mesh.CreateCube();
                    mesh.Name = name;
                    mesh.Transform.position = position;

                    _scene.Add(mesh);
                    _renderer.AddObject(mesh);

                    Console.WriteLine($"Spawned '{name}' at ({position.X}, {position.Y}, {position.Z}).");
                });
            });

            _devConsole.Register("setmaterial", args =>
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: addmaterial <objectName> <preset>");
                    Console.WriteLine("Presets: metal, plastic, gold, matte");
                    return;
                }

                string objectName = args[0];
                string preset = args[1].ToLower();

                GLThread.Enqueue(() =>
                {
                    var mesh = _scene.FindObject(objectName);
                    if (mesh == null)
                    {
                        Console.WriteLine($"Object '{objectName}' not found.");
                        return;
                    }

                    var mat = new Material();

                    switch (preset)
                    {
                        case "metal":
                            mat.Ambient = new Vector3(0.25f, 0.25f, 0.25f);
                            mat.Diffuse = new Vector3(0.4f, 0.4f, 0.4f);
                            mat.Specular = new Vector3(0.77f, 0.77f, 0.77f);
                            mat.Shininess = 64f;
                            break;

                        case "plastic":
                            mat.Ambient = new Vector3(0.1f, 0.1f, 0.1f);
                            mat.Diffuse = new Vector3(0.6f, 0.6f, 0.6f);
                            mat.Specular = new Vector3(0.5f, 0.5f, 0.5f);
                            mat.Shininess = 32f;
                            break;

                        case "gold":
                            mat.Ambient = new Vector3(0.24725f, 0.1995f, 0.0745f);
                            mat.Diffuse = new Vector3(0.75164f, 0.60648f, 0.22648f);
                            mat.Specular = new Vector3(0.628281f, 0.555802f, 0.366065f);
                            mat.Shininess = 51.2f;
                            break;

                        case "matte":
                            mat.Ambient = new Vector3(0.2f);
                            mat.Diffuse = new Vector3(0.5f);
                            mat.Specular = Vector3.Zero;
                            mat.Shininess = 1f;
                            break;

                        default:
                            Console.WriteLine($"Unknown preset '{preset}'.");
                            return;
                    }

                    mesh.Material = mat;
                    Console.WriteLine($"Set material '{preset}' for '{objectName}'.");
                });
            });

            _devConsole.Register("addtexture", args =>
            {
                if (args.Length < 2)
                {
                    Console.WriteLine("Usage: addtexture <objectName> <textureFile>");
                    return;
                }

                string objectName = args[0];

                string baseDir = Path.Combine(AppContext.BaseDirectory, "texture");
                string texturePath = Path.Combine(baseDir, args[1]);

                if (!File.Exists(texturePath))
                {
                    Console.WriteLine($"Texture file not found: {texturePath}");
                    return;
                }

                GLThread.Enqueue(() =>
                {
                    var mesh = _scene.FindObject(objectName);
                    if (mesh == null)
                    {
                        Console.WriteLine($"Object '{objectName}' not found.");
                        return;
                    }

                    try
                    {
                        var texture = new Texture(texturePath);
                        mesh.Material.DiffuseTexture = texture;
                        Console.WriteLine($"Applied texture '{Path.GetFileName(texturePath)}' to '{objectName}'.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to load texture '{texturePath}': {ex.Message}");
                    }
                });
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

            if (input.IsKeyPressed(Keys.F10))
            {
                _cursorVisible = !_cursorVisible;

                CursorState = _cursorVisible ? CursorState.Normal : CursorState.Grabbed;
            }

            if (_cursorVisible)
                return;

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

            GLThread.ExecutePending();

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

    public static class GLThread
    {
        private static readonly ConcurrentQueue<Action> _actions = new();

        public static void Enqueue(Action action)
        {
            _actions.Enqueue(action);
        }

        public static void ExecutePending()
        {
            while (_actions.TryDequeue(out var action))
                action();
        }
    }
}
