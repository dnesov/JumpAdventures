using Godot;

using World = Jump.Levels.World;
using System.Collections.Generic;
using System.Linq;


namespace Jump.Utils
{
    /// <summary>
    /// Scans for Worldpacks with playable levels.
    /// </summary>
    public class WorldpackScanner
    {
        public IReadOnlyDictionary<string, World> LoadedWorlds => _worlds;

        /// <summary>
        /// Creates a worldpacks folder in user://.
        /// </summary>
        /// <returns>A boolean indicating if the folder was created or not.</returns>
        public bool CreateWorldpackFolder()
        {
            var dir = new Directory();
            dir.Open("user://");

            if (dir.DirExists("worldpacks")) return false;

            dir.MakeDir("worldpacks");

            return true;
        }

        public List<World> Scan(string directoryPath, bool isUser = true)
        {
            var result = new List<World>();
            var dir = new Directory();
            var error = dir.Open(directoryPath);

            if (error != Godot.Error.Ok)
            {
                GD.PrintErr(error);
                return new List<World>();
            }

            dir.ListDirBegin();
            var fileName = dir.GetNext();

            while (fileName != "")
            {
                if (dir.CurrentIsDir())
                {
                    if (fileName != ".." && fileName != ".")
                    {
                        var globalDirPath = directoryPath + fileName + "/";
                        var world = World.NewFromConfig(globalDirPath + "world.yaml");

                        result.Add(world);
                        AddWorldToDictionary(world.UniqueId, world);
                    }
                }

                fileName = dir.GetNext();
            }

            dir.ListDirEnd();
            return result;
        }

        public List<World> ScanOrdered(string directoryPath, bool isUser = true)
        {
            var worldpacks = Scan(directoryPath, isUser);
            return worldpacks.OrderBy(w => w.Order).ToList();
        }

        public World LoadWorldById(string id)
        {
            if (_worlds.Count == 0) return null;
            if (!_worlds.ContainsKey(id)) return null;
            return _worlds[id];
        }

        private void AddWorldToDictionary(string id, World world)
        {
            if (_worlds.ContainsKey(id)) return;
            if (id == null) return;
            _worlds.Add(id, world);
        }

        private Dictionary<string, World> _worlds = new Dictionary<string, World>();
    }
}