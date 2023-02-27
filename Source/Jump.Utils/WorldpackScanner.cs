using Jump.Levels;
using Godot;

using World = Jump.Levels.World;
using System.Collections.Generic;

namespace Jump.Utils
{
    /// <summary>
    /// Scans for Worldpacks with playable levels.
    /// </summary>
    public class WorldpackScanner
    {
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

        public World[] Scan(string directoryPath, bool isUser = true)
        {
            var result = new List<World>();
            var dir = new Directory();
            var error = dir.Open(directoryPath);

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
                    }
                }

                fileName = dir.GetNext();
            }

            dir.ListDirEnd();

            return result.ToArray();
        }
    }
}