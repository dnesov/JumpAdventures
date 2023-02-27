using Godot;
using Jump.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jump.UI.Menu
{
    public class PlayGameSection : MenuSection
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _game = GetTree().Root.GetNode<Game>("Game");
            GetNode<ButtonMinimal>("%Back").OnPressedAction += GoBack;
        }

        protected override void OnDisplay()
        {
            PlayDisplayAnimation();
            if (_worldpacks.Count != 0) return;
            _worldpacks = _game.GetWorldpacksOrdered(_campaignWorldpacksPath);
            UpdateElements();
        }
        protected override void OnHide() => PlayHideAnimation();

        protected override void OnUpdateElements()
        {
            base.OnUpdateElements();
            PopulateCampaignWorldButtons();
        }

        protected override void GoBack()
        {
            Hide();
            menu.ReturnToMainSection();
        }

        private void PopulateCampaignWorldButtons()
        {
            _worldpacks = GetCampaignWorldpacks();
            var worldContainer = GetNode<VBoxContainer>("%WorldContainer");

            if (worldContainer.GetChildren().Count != 0) return;
            if (_worldpacks == null) return;

            WorldSaveData worldSave;
            for (int i = 0; i < _worldpacks.Count; i++)
            {
                var worldpack = _worldpacks[i];
                if (worldpack.Hidden) continue;

                worldSave = _game.WorldSaveHandler.TryLoadData(worldpack.UniqueId);

                var worldpackButton = CreateCampaignWorldButton(i, worldpack, worldSave);
                worldContainer.AddChild(worldpackButton);
            }
        }

        private WorldButton CreateCampaignWorldButton(int index, Levels.World world, WorldSaveData saveData)
        {
            var worldButton = _campaignWorldButtonScene.Instance<WorldButton>();
            worldButton.Order = index + 1;
            worldButton.UpdateElements(new WorldButtonData
            {
                World = world,
                WorldSaveData = saveData
            });
            worldButton.OnPressed += OnWorldButtonPressed;
            return worldButton;
        }

        private CustomWorldButton CreateCustomWorldButton(int index, Levels.World world)
        {
            var worldButton = _customWorldButtonScene.Instance<CustomWorldButton>();
            worldButton.Order = index + 1;
            worldButton.UpdateElements(world);
            worldButton.OnPressed += OnWorldButtonPressed;
            return worldButton;
        }

        private void PopulateCustomWorldButtons()
        {
            _worldpacks = GetCustomWorldpacks();
            var worldContainer = GetNode<VBoxContainer>("%CustomWorldContainer");
            if (_worldpacks == null) return;
            if (worldContainer.GetChildren().Count != 0) return;

            for (int i = 0; i < _worldpacks.Count; i++)
            {
                var worldpack = _worldpacks[i];
                if (worldpack.Hidden) continue;
                var worldpackButton = CreateCustomWorldButton(i, worldpack);
                worldContainer.AddChild(worldpackButton);
            }
        }

        private void OnWorldButtonPressed(int worldId)
        {
            var world = _worldpacks[worldId];
            _game.LoadWorld(world);

            var levelList = GetNode<LevelList>("%LevelList");
            levelList.Display();

            var saveData = _game.WorldSaveHandler.TryLoadData(_game.CurrentWorld.UniqueId);
            List<LevelButtonData> levelButtonDatas = new List<LevelButtonData>();

            int i = 0;
            foreach (var level in world.Levels.ToList())
            {
                levelButtonDatas.Add(new LevelButtonData()
                {
                    Level = level,
                    Completed = saveData.LevelSaves[i] == null ? false : saveData.LevelSaves[i].Completed,
                    Order = i + 1,
                    IsUser = world.IsUser
                });
                i++;
            }
            levelList.UpdateElements(levelButtonDatas);
        }

        private void WorldListTabChanged(int tab)
        {
            if (tab == 0) PopulateCampaignWorldButtons();
            if (tab == 1) PopulateCustomWorldButtons();
        }
        private List<Levels.World> GetCampaignWorldpacks() => _game.GetWorldpacksOrdered(_campaignWorldpacksPath);
        private List<Levels.World> GetCustomWorldpacks() => _game.GetWorldpacks(_customWorldpacksPath).ToList();

        private Logger _logger = new Logger(nameof(PlayGameSection));
        [Export] private PackedScene _campaignWorldButtonScene;
        [Export] private PackedScene _customWorldButtonScene;
        private readonly string _campaignWorldpacksPath = "res://Levels/";
        private readonly string _customWorldpacksPath = "user://worldpacks/";
        private Game _game;
        private List<Levels.World> _worldpacks = new List<Levels.World>();
    }
}
