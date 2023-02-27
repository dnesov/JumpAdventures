using Godot;

namespace Jump.Utils
{
    /// <summary>
    /// Autoload for managing scene switches and optionally playing a transition animation.
    /// </summary>
    public class SceneSwitcher : Node
    {
        public override void _Ready()
        {
            _animationPlayer = GetNode<AnimationPlayer>(_animationPlayerPath);
        }
        public void Load(string path, bool doTransition = false, TransitionType transition = TransitionType.Fade, float transitionSpeed = 1.0f)
        {
            if (_currentScenePath == path) return;
            _currentScenePath = path;

            if (doTransition)
            {
                Transition(transition, transitionSpeed);
                return;
            }

            RemovePreviousScene();
            SwapScenes();
        }
        public void Reload()
        {
            RemovePreviousScene();
            SwapScenes();
        }

        private void RemovePreviousScene()
        {
            if (_currentScene == null) return;
            _currentScene.Free();
        }

        private void SwapScenes()
        {
            var packedScene = GD.Load<PackedScene>(_currentScenePath);

            var game = GetTree().Root.GetNode<Game>("Game");
            if (packedScene is null)
            {
                packedScene = GD.Load<PackedScene>(game.DataHandler.Data.UseNewMenu ? Constants.NEW_MAIN_MENU_PATH : Constants.OLD_MAIN_MENU_PATH);
                OS.Alert($"Couldn't open level at \"{_currentScenePath}\" due to an error (is it missing?).", "Error opening the level!");
            }
            _currentScene = packedScene.Instance();
            _currentScene.PauseMode = PauseModeEnum.Stop;
            AddChild(_currentScene);
        }

        private void Transition(TransitionType transition = TransitionType.Fade, float speed = 1.0f)
        {
            _animationPlayer.Play($"transition_{transition.ToString().ToLower()}", default, speed);
        }

        private void TransitionEnd()
        {
            RemovePreviousScene();
            SwapScenes();
        }

        private Node _currentScene;
        private string _currentScenePath;

        [Export] private NodePath _animationPlayerPath;
        private AnimationPlayer _animationPlayer;
    }
}

public enum TransitionType
{
    Paper,
    Fade
}