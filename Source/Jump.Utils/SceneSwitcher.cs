using Godot;
using Jump.Extensions;

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
            _loadingProgress = GetNode<TextureProgress>("%TextureProgress");
            _splashLoadingProgress = GetNode<TextureProgress>("%LoadingBarSplash");

            _loadingSplash = GetNode<Control>("%LoadingSplashUI");
            _loadingGeneric = GetNode<Control>("%LoadingUI");

            SetProcess(false);
        }

        public override void _Process(float delta)
        {
            if (!_loading) return;
            var error = _loader.Poll();

            switch (error)
            {
                case Error.Ok:
                    UpdateLoadingProgress();
                    break;

                case Error.FileEof:
                    EndLoadingScene();
                    break;
            }
        }

        public async void Load(string path, bool doTransition = false, float transitionSpeed = 1.0f)
        {
            if (_currentScenePath == path) return;

            _loadingWithSplash = false;
            _loadingGeneric.Show();

            _currentScenePath = path;

            _logger.Info($"Loading scene at {path}...");

            if (doTransition)
            {
                _logger.Info("Starting a transition animation...");
                Transition(transitionSpeed);
            }

            _loadingProgress.Value = 0.0f;

            await this.TimeInSeconds(0.3f);

            RemovePreviousScene();
            BeginLoadingScene();
        }

        public async void LoadWithSplash(string path)
        {
            if (_currentScenePath == path) return;

            _loadingWithSplash = true;

            _currentScenePath = path;

            _logger.Info($"Loading scene at {path}...");

            _splashLoadingProgress.Value = 0.0f;

            _loadingSplash.Show();
            _animationPlayer.Play("transition_splash");

            await this.TimeInSeconds(0.3f);

            RemovePreviousScene();
            BeginLoadingScene();
        }

        public void Reload()
        {
            RemovePreviousScene();
            BeginLoadingScene();
        }

        private void RemovePreviousScene()
        {
            if (_currentScene == null) return;
            _logger.Info($"Removing the current scene.");
            _currentScene.QueueFree();
        }

        private void BeginLoadingScene()
        {
            _logger.Info($"Starting loading the scene at {_currentScenePath}...");

            _loader = ResourceLoader.LoadInteractive(_currentScenePath);

            if (_loader == null)
            {
                OS.Alert($"There was an error creating a resource loader for {_currentScenePath}.", "Error opening the level!");
                _logger.Error($"There was an error creating a resource loader for {_currentScenePath}.");
                ReverseTransition();
                return;
            }

            SetProcess(true);
            _loading = true;
        }

        private void EndLoadingScene()
        {
            _logger.Info($"Finishing loading the scene at {_currentScenePath}...");
            SetProcess(false);
            _loading = false;

            var packedScene = _loader.GetResource() as PackedScene;

            if (packedScene == null)
            {
                OS.Alert($"Couldn't open level at \"{_currentScenePath}\" due to an error (is it missing?).", "Error opening the level!");
                FallbackToMenu();
                ReverseTransition();
                return;
            }

            _currentScene = packedScene.Instance();
            _currentScene.PauseMode = PauseModeEnum.Stop;

            AddChild(_currentScene);
            ReverseTransition();

            _loader.Dispose();

            _logger.Info($"Scene loaded!");
        }

        private void UpdateLoadingProgress()
        {
            var progress = (float)_loader.GetStage() / _loader.GetStageCount();

            if (_loadingWithSplash)
            {
                _splashLoadingProgress.Value = progress;
            }
            else
            {
                _loadingProgress.Value = (float)_loader.GetStage() / _loader.GetStageCount();
            }
        }

        private void FallbackToMenu()
        {
            var packedScene = GD.Load<PackedScene>(Constants.NEW_MAIN_MENU_PATH);
            _currentScene = packedScene.Instance();
            _currentScene.PauseMode = PauseModeEnum.Stop;
            AddChild(_currentScene);
        }

        private void Transition(float speed = 1.0f)
        {
            _animationPlayer.Play("transition_fade", default, speed);
        }

        private void ReverseTransition()
        {
            if (_loadingWithSplash)
            {
                _animationPlayer.Play("transition_splash_hide");
            }
            else
            {
                _animationPlayer.PlayBackwards("transition_fade");
            }
        }

        private void TransitionEnd()
        {
            RemovePreviousScene();
            BeginLoadingScene();
        }

        private Node _currentScene;
        private string _currentScenePath;

        [Export] private NodePath _animationPlayerPath;
        private TextureProgress _loadingProgress, _splashLoadingProgress;
        private Control _loadingSplash, _loadingGeneric;
        private AnimationPlayer _animationPlayer;
        private bool _loading;
        private ResourceInteractiveLoader _loader;

        private bool _loadingWithSplash;

        private Logger _logger = new(nameof(SceneSwitcher));
    }
}