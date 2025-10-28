using OpenTK.Mathematics;
using System.Collections.Generic;
using TinyEngine.TGraphics;

namespace TinyEngine.Core
{
    public class Scene
    {
        private readonly List<IRenderer> _objects = new();
        private readonly List<Light> _lights = new();

        public IEnumerable<IRenderer> Objects => _objects;
        public IEnumerable<Light> Lights => _lights;

        public void Add(IRenderer obj) => _objects.Add(obj);
        public void Add(Light light) => _lights.Add(light);

        public void LoadInto(Renderer renderer)
        {
            foreach (var obj in _objects)
                renderer.AddObject(obj);

            foreach (var light in _lights)
                renderer.AddLight(light);
        }

        public Mesh? FindObject(string name)
        {
            foreach (var obj in _objects)
            {
                if (obj is Mesh mesh && string.Equals(mesh.Name, name, StringComparison.OrdinalIgnoreCase))
                    return mesh;
            }
            return null;
        }

        public void ListObjectNames()
        {
            foreach (var obj in _objects)
                if (obj is Mesh m)
                    System.Console.WriteLine($" - {m.Name}");
        }

        public bool Remove(string name, Renderer renderer)
        {
            for (int i = 0; i < _objects.Count; i++)
            {
                if (_objects[i] is Mesh mesh &&
                    string.Equals(mesh.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    _objects.RemoveAt(i);
                    renderer.RemoveObject(mesh);

                    return true;
                }
            }

            return false;
        }
    }
}