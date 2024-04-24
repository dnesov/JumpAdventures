using Godot;
using Jump.Extensions;
using Jump.Levels;
using System;
using System.Collections.Generic;

namespace Jump.UI.Menu
{
    public class LevelList : UIElement<LevelListData>
    {
        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            _levelContainer = GetNode<VBoxContainer>("%LevelContainer");
            _mainContainer = GetNode<VBoxContainer>("%MainContainer");
            _darken = GetNode<ColorRect>("%Darken");

            _owner = GetOwner<Control>();

            _mainContainerInitialPos = _mainContainer.RectPosition;
        }

        protected override void OnDisplay()
        {
            PlayShowAnimation();

            _levelContainer.GetParent<ScrollContainer>().ScrollVertical = 0;
        }

        protected override void OnHide()
        {
            PlayHideAnimation();
        }

        protected override void OnUpdateElements(LevelListData data)
        {
            ClearLevelButtons();

            short i = 0;
            foreach (var level in data.Levels)
            {
                var levelButton = CreateLevelButton(i, level);
                _levelContainer.AddChild(levelButton);

                if (i == 0)
                {
                    levelButton.GrabFocus();
                }

                i++;
            }
        }

        private void ClearLevelButtons()
        {
            foreach (LevelButton child in _levelContainer.GetChildren())
            {
                child.OnPressedAction -= OnLevelButtonPressed;
                child.QueueFree();
            }
        }

        private LevelButton CreateLevelButton(short idx, LevelButtonData level)
        {
            var levelButton = _levelButtonScene.Instance<LevelButton>();
            levelButton.LevelId = idx;
            levelButton.UpdateElements(level);
            levelButton.OnPressedAction += OnLevelButtonPressed;
            return levelButton;
        }

        private void OnLevelButtonPressed(short id)
        {
            var game = this.GetSingleton<Game>();
            var level = game.CurrentWorld.Levels[id];
            game.LoadLevel(level);
        }

        private void PlayHideAnimation()
        {
            _tweener = CreateTween().SetParallel();
            _tweener.SetTrans(Tween.TransitionType.Quad);
            _tweener.SetEase(Tween.EaseType.Out);
            _tweener.TweenProperty(_darken, "color:a", 0f, _tweenDuration);
            _tweener.TweenProperty(_owner, "modulate:a", 1.0f, _tweenDuration);
            _tweener.Parallel().TweenProperty(_mainContainer, "modulate:a", 0.0f, _tweenDuration);
            _tweener.Parallel().TweenProperty(_mainContainer, "rect_position:y", _mainContainerInitialPos.y - 110f, _tweenDuration);
            _tweener.Chain().TweenCallback(this, "hide");
        }

        private void PlayShowAnimation()
        {
            Visible = true;

            var pos = _mainContainer.RectPosition;
            pos.y = _mainContainerInitialPos.y - 110f;

            _mainContainer.RectPosition = pos;

            _tweener = CreateTween().SetParallel();
            _tweener.SetTrans(Tween.TransitionType.Quad);
            _tweener.SetEase(Tween.EaseType.Out);
            _tweener.TweenProperty(_owner, "modulate:a", 0.0f, _tweenDuration);
            _tweener.TweenProperty(_darken, "color:a", 0.5f, _tweenDuration);
            _tweener.TweenProperty(_mainContainer, "modulate:a", 1.0f, _tweenDuration);
            _tweener.TweenProperty(_mainContainer, "rect_position:y", _mainContainerInitialPos.y, _tweenDuration);
        }

        [Export] private PackedScene _levelButtonScene;
        private VBoxContainer _levelContainer;
        private VBoxContainer _mainContainer;
        private ColorRect _darken;
        private MenuUI _menu;
        private SceneTreeTween _tweener;

        private Vector2 _mainContainerInitialPos;

        private float _tweenDuration = 0.3f;

        private Control _owner;
    }

    public class LevelListData
    {
        public List<LevelButtonData> Levels;
    }
}