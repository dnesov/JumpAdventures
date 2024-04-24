using Godot;
using YamlDotNet.Serialization;

using File = Godot.File;
using System;

namespace Jump.Levels
{
    public class World
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string Author { get; private set; }
        public string UniqueId { get; private set; }
        public string TrackPath { get; private set; }
        public string AmbiencePath { get; private set; }
        public string PreviewImageModulate { get; private set; } = "#ffffff";
        public bool Hidden { get; private set; }
        public short Order { get; private set; }
        public bool Playable { get; private set; } = true;
        [YamlIgnore] public bool IsUser { get; set; }
        public Level[] Levels { get; set; }
        [YamlIgnore] public Texture Image { get; set; }
        public string UnlockId { get; set; } = "";

        public Level CurrentLevel => Levels[_currentLevelIdx];

        public static World NewFromConfig(string worldConfigPath)
        {
            var worldFolderPath = worldConfigPath.Replace("world.yaml", "");
            var file = new File();
            var error = file.Open(worldConfigPath, File.ModeFlags.Read);

            if (error != Error.Ok)
            {
                GD.PushError($"Failed to read world config at: {worldConfigPath} (Error: {error})");
                return new World();
            }

            var deserializer = new DeserializerBuilder()
            .Build();

            var world = deserializer.Deserialize<World>(file.GetAsText());

            // set global level paths

            if (world.Levels != null)
            {
                foreach (var level in world.Levels)
                {
                    level.GlobalPath = worldFolderPath + level.Path;
                }
            }

            world.IsUser = worldFolderPath.Contains("user");

            // Load world image, if exists.
            var worldImagePath = $"{worldFolderPath}/world_image.jpg";

            var imageExists = worldImagePath.Contains("res://") ? ResourceLoader.Exists(worldImagePath)
            : file.FileExists(worldImagePath);

            if (!imageExists) return world;

            world.Image = worldImagePath.Contains("res://") ? LoadImageRes(worldImagePath) : LoadImageUser(worldImagePath);
            return world;
        }

        private static Texture LoadImageRes(string path)
        {
            var imageTexture = GD.Load<Texture>(path);
            return imageTexture;
        }

        private static ImageTexture LoadImageUser(string path)
        {
            var textureFile = new File();
            textureFile.Open(path, File.ModeFlags.Read);
            var bytes = textureFile.GetBuffer((long)textureFile.GetLen());

            var image = new Image();
            var data = image.LoadJpgFromBuffer(bytes);
            var imageTexture = new ImageTexture();
            imageTexture.CreateFromImage(image);
            textureFile.Close();

            return imageTexture;
        }
        public bool CanGetNextLevel()
        {
            if (_currentLevelIdx + 1 > Levels.Length - 1) return false;
            return Levels[_currentLevelIdx + 1] != null;
        }
        public Level GetNextLevel()
        {
            _currentLevelIdx++;
            var level = Levels[_currentLevelIdx];
            return level;
        }

        public void UpdateLevelIdxFrom(Level level)
        {
            _currentLevelIdx = GetLevelIdx(level);
        }

        public int GetLevelIdx(Level level)
        {
            return Array.IndexOf(Levels, level);
        }

        public override string ToString()
        {
            var levelsString = "";

            int i = 0;

            if (Levels != null)
            {
                foreach (var level in Levels)
                {
                    i++;
                    levelsString += $"Level {i}: {level.ToString()}\n";
                }
            }

            return @$"
=== {Name} ===
Description: {Description}
ID: {UniqueId}
{levelsString}
            ";
        }

        private int _currentLevelIdx;
    }
}