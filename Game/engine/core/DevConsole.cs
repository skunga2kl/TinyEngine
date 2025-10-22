using System;
using System.Collections.Generic;

namespace TinyEngine.Core
{
    public class DevConsole
    {
        private readonly Dictionary<string, Action<string[]>> _commands = new();

        public void Register(string name, Action<string[]> action)
        {
            _commands[name.ToLower()] = action;
        }

        public void Execute(string input)
        {
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            var cmd = parts[0].ToLower();
            var args = parts.Length > 1 ? parts[1..] : Array.Empty<string>();

            if (_commands.TryGetValue(cmd, out var action))
                action.Invoke(args);
            else
                Console.WriteLine($"Unknown command: {cmd}");
        }
    }
}
