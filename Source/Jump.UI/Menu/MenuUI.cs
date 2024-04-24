using Godot;
using Jump.Extensions;
using Jump.Unlocks;
using Jump.Utils;
using System;

namespace Jump.UI.Menu
{
    public class MenuUI : Control
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _mainSection = GetNode<MainSection>("%MainSection");
            _mainSection.Display();

            _game = this.GetSingleton<Game>();
            _game.EnableCursor();

            SubscribeEvents();
            StartMusic();
        }

        public override void _ExitTree()
        {
            base._ExitTree();
            UnsubscribeEvents();
        }

        public void DisplayMainSection() => _mainSection.Display();
        public void DisplaySettingsSection() => GetNode<SettingsSection>("%SettingsSection").Display();
        public void DisplayGameModeSection() => GetNode<GameModeSection>("%GameModeSection").Display();
        public void DisplayPlaySection() => GetNode<PlayGameSection>("%PlayGameSection").Display();
        public void DisplayExtrasSection() => GetNode<ExtrasSection>("%ExtrasSection").Display();
        public void DisplayCreditsSection() => GetNode<CreditsSection>("%CreditsSection").Display();
        public void DisplayCustomizeSection() => GetNode<CustomizeSection>("%CustomizeSection").Display();
        public void DisplayStatsSection() => GetNode<StatsSection>("%StatsSection").Display();

        public void Darken(float a)
        {
            var darkenRect = GetNode<ColorRect>("%Darken");
            var tween = darkenRect.CreateTween();

            tween.TweenProperty(darkenRect, "color:a", a, _darkenDuration);
        }

        public void UpdateTrackAndAmbience(string trackPath, string ambiencePath)
        {
            var soundtrackManager = this.GetSingleton<SoundtrackManager>();
            soundtrackManager.PlayTrack(trackPath);
            soundtrackManager.PlayAmbience(ambiencePath);
        }

        private void StartMusic()
        {
            // var lastWorldId = _game.DataHandler.Data.LastWorldId;
            // StartWorldMusic(lastWorldId);
            var soundtrackManager = this.GetSingleton<SoundtrackManager>();
            soundtrackManager.CurrentTrackState = TrackState.Menu;
            // soundtrackManager.PlayTrack(_mainMenuMusicPath);

        }

        private void StartWorldMusic(string worldId)
        {
            var lastWorld = _game.WorldpackScanner.LoadWorldById(worldId);
            UpdateTrackAndAmbience(lastWorld.TrackPath, lastWorld.AmbiencePath);
        }

        private void SubscribeEvents()
        {
            _game.OnWorldLoaded += StartWorldMusic;
        }

        private void UnsubscribeEvents()
        {
            _game.OnWorldLoaded -= StartWorldMusic;
        }

        private void OnWorldLoaded(string worldId) => StartWorldMusic(worldId);

        private MainSection _mainSection;
        private Game _game;

        private float _darkenDuration = 0.25f;
        private readonly string _mainMenuMusicPath = "event:/Soundtrack/Menu";
    }

}