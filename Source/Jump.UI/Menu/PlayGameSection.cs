using Godot;
using Jump.Extensions;
using Jump.Unlocks;
using Jump.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Jump.UI.Menu
{
    // TODO: fix memory leak. Unsubscribe from events of created worldpack buttons or find a better solution.
    public class PlayGameSection : MenuSection
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            base._Ready();
            _game = this.GetSingleton<Game>();
        }

        public override void Focus()
        {
            base.Focus();
            var worldContainer = GetNode<VBoxContainer>("%WorldContainer");
            var button = worldContainer.GetChild<WorldButton>(0);

            var secondButton = worldContainer.GetChild<WorldButton>(1);

            button.FocusNeighbourBottom = secondButton.GetPath();
            button.GrabFocus();
        }

        protected override void OnDisplay()
        {
            base.OnDisplay();

            _campaignWorlds = _game.WorldpackScanner.ScanOrdered(_campaignWorldpacksPath);
            UpdateElements();

            PlayDisplayAnimation();
            Focus();
        }
        protected override void OnHide()
        {
            base.OnHide();
            PlayHideAnimation();
        }

        protected override void OnUpdateElements()
        {
            base.OnUpdateElements();
            PopulateCampaignWorldButtons();
        }

        protected override void OnGoBack()
        {
            if (_levelSelectOpen)
            {
                var levelList = GetNode<LevelList>("%LevelList");
                levelList.Hide();
                _levelSelectOpen = false;

                Focus();
                return;
            }
            Hide();
            menu.DisplayMainSection();
        }

        private void PopulateCampaignWorldButtons()
        {
            var worldContainer = GetNode<VBoxContainer>("%WorldContainer");
            if (worldContainer.GetChildren().Count != 0) return;

            if (_campaignWorlds == null) return;

            WorldSaveData worldSave;
            for (int i = 0; i < _campaignWorlds.Count; i++)
            {
                var worldpack = _campaignWorlds[i];
                if (worldpack.Hidden) continue;

                worldSave = _game.WorldSaveHandler.TryLoadData(worldpack.UniqueId);

                var worldpackButton = CreateCampaignWorldButton();

                worldContainer.AddChild(worldpackButton);

                var database = this.GetSingleton<UnlocksDatabase>();

                // var isUnlocked = world.UnlockId != string.Empty ? database.CanUnlock(world.UnlockId) : true;
                var unlockable = database.GetUnlockable(worldpack.UnlockId);

                worldpackButton.Order = i + 1;

                worldpackButton.UpdateElements(new WorldButtonData
                {
                    World = worldpack,
                    WorldSaveData = worldSave,
                    Unlockable = unlockable
                });
            }
        }

        private WorldButton CreateCampaignWorldButton()
        {
            var worldButton = _campaignWorldButtonScene.Instance<WorldButton>();
            worldButton.OnPressedAction += OnWorldButtonPressed;
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
            _campaignWorlds = GetCustomWorldpacks();
            var worldContainer = GetNode<VBoxContainer>("%CustomWorldContainer");
            if (_campaignWorlds == null) return;
            if (worldContainer.GetChildren().Count != 0) return;

            for (int i = 0; i < _campaignWorlds.Count; i++)
            {
                var worldpack = _campaignWorlds[i];
                if (worldpack.Hidden) continue;
                var worldpackButton = CreateCustomWorldButton(i, worldpack);
                worldContainer.AddChild(worldpackButton);
            }
        }

        private void OnWorldButtonPressed(int worldId)
        {
            var world = _campaignWorlds[worldId];
            _game.LoadWorld(world);

            var levelList = GetNode<LevelList>("%LevelList");

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

            levelList.UpdateElements(new LevelListData()
            {
                Levels = levelButtonDatas
            });

            levelList.Display();
            _levelSelectOpen = true;
        }

        private void WorldListTabChanged(int tab)
        {
            if (tab == 0) PopulateCampaignWorldButtons();
            if (tab == 1) PopulateCustomWorldButtons();
        }

        private List<Levels.World> GetCustomWorldpacks() => _game.WorldpackScanner.Scan(_customWorldpacksPath);

        private Logger _logger = new Logger(nameof(PlayGameSection));
        [Export] private PackedScene _campaignWorldButtonScene;
        [Export] private PackedScene _customWorldButtonScene;
        private readonly string _campaignWorldpacksPath = "res://Levels/";
        private readonly string _customWorldpacksPath = "user://worldpacks/";
        private Game _game;
        private List<Levels.World> _campaignWorlds = new List<Levels.World>();

        private bool _levelSelectOpen;
    }
}
