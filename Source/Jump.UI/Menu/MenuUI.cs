using Godot;
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
            StartMusic();
        }

        public void ReturnToMainSection() => _mainSection.Display();
        public void DisplaySettingsSection() => GetNode<SettingsSection>("%SettingsSection").Display();
        public void DisplayPlaySection() => GetNode<PlayGameSection>("%PlayGameSection").Display();
        public void DisplayExtrasSection() => GetNode<ExtrasSection>("%ExtrasSection").Display();
        public void DisplayCreditsSection() => GetNode<CreditsSection>("%CreditsSection").Display();
        public void DisplayCustomizeSection() => GetNode<CustomizeSection>("%CustomizeSection").Display();

        private void StartMusic()
        {
            var soundtrackManager = GetTree().Root.GetNode<SoundtrackManager>("SoundtrackManager");
            // TODO: do not use hardcoded stuff here.
            soundtrackManager.PlayAmbient("event:/Ambience/World1Amb");
            soundtrackManager.PlayTrack("event:/Soundtrack/World1");
            soundtrackManager.CurrentTrackState = TrackState.Menu;
        }
        private MainSection _mainSection;
        private Logger _logger = new Logger(nameof(MenuUI));
    }

}