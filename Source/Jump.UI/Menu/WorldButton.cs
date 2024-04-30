using Godot;
using System;
using Jump.Utils;
using Jump.Unlocks;
using Jump.Extensions;

namespace Jump.UI.Menu
{
    public class WorldButton : JAButton<WorldButtonData>
    {
        public new Action<int> OnPressedAction;
        public int Order { get; set; }

        public override void _Ready()
        {
            base._Ready();

            RectPivotOffset = RectSize / 2;

            Connect("visibility_changed", this, nameof(VisibilityChanged));


            _tween = new Tween();
            AddChild(_tween);
        }

        public override void _Notification(int what)
        {
            if (what == NotificationTranslationChanged)
            {
                var orderFormatted = string.Format(Tr("UI_WORLD_ORDER"), Order);
                _worldOrderLabel.Text = orderFormatted;
            }
        }


        protected override void OnUpdateElements(WorldButtonData data)
        {
            // TODO: better to do this once in a _Ready instead.
            GetNodes();

            _data = data;
            var world = data.World;

            var orderFormatted = string.Format(Tr("UI_WORLD_ORDER"), Order);

            _worldOrderLabel.Text = orderFormatted;
            _worldNameLabel.Text = world.Name;

            _unavailableOverlay.Visible = !world.Playable;

            _unlockable = data.Unlockable;

            if (_unlockable != null)
            {
                if (!_unlockDb.IsUnlocked(_unlockable.EntryId) && _unlockable.HasCondition<EssenceUnlockCondition>())
                {
                    _essenceOverlay.Visible = true;

                    var condition = _unlockable.GetCondition<EssenceUnlockCondition>();
                    var progressHandler = this.GetSingleton<ProgressHandler>();
                    var label = _essenceOverlay.GetNode<Label>("%EssenceLabel");
                    label.Text = $"{progressHandler.Essence}/{condition.EssenceRequired}";
                }
            }

            FocusMode = world.Playable ? FocusModeEnum.All : FocusModeEnum.None;

            if (data.WorldSaveData == null) return;

            var completedLevels = data.WorldSaveData.GetCompletedLevelsAmount();
            var maxLevels = world.Levels.Length;

            float completedPercentage = (float)completedLevels / maxLevels;

            _progressLabel.Text = $"{completedLevels}/{maxLevels} ({completedPercentage:P2})";
            _fragmentLabel.Text = $"{data.WorldSaveData.FragmentsCollected}/6";

            if (world.Image == null) return;
            _worldImage.Texture = world.Image;
        }

        protected override void OnPressed()
        {

        }

        protected override void OnButtonDown()
        {
            PlayButtonDownAnimation();
        }

        protected override void OnButtonUp()
        {
            PlayButtonUpAnimation();
            UnlockOrOpenWorld();
            AcceptEvent();
        }

        protected override void OnFocusEntered()
        {
            PlayFocusAnimation();
        }

        protected override void OnFocusExited()
        {
            PlayFocusExitAnimation();
        }

        private void GetNodes()
        {
            if (_gotNodes) return;

            _unlockDb = this.GetSingleton<UnlocksDatabase>();

            _worldImage = GetNode<TextureRect>("%WorldImage");
            _playIcon = GetNode<TextureRect>("%PlayIcon");
            _worldOrderLabel = GetNode<Label>("%Order");
            _worldNameLabel = GetNode<Label>("%Name");

            _unavailableOverlay = GetNode<Control>("%UnavailableOverlay");
            _lockedOverlay = GetNode<Control>("%LockedOverlay");
            _essenceOverlay = GetNode<Control>("%EssenceLockedOverlay");

            _progressLabel = GetNode<Label>("%Progress");
            _fragmentLabel = GetNode<Label>("%FragmentCount");

            _gotNodes = true;
        }

        private void PlayButtonDownAnimation()
        {
            RectPivotOffset = RectSize / 2;

            var initialScale = Vector2.One;
            var targetScale = Vector2.One * 0.9f;

            _tween.InterpolateProperty(this, "rect_scale", initialScale, targetScale, tweenTime, Tween.TransitionType.Quad, Tween.EaseType.Out);
            _tween.Start();
        }

        private void PlayButtonUpAnimation()
        {
            RectPivotOffset = RectSize / 2;

            var initialScale = Vector2.One;

            _tween.InterpolateProperty(this, "rect_scale", RectScale, initialScale, tweenTime * 2f, Tween.TransitionType.Back, Tween.EaseType.Out);
            _tween.Start();
        }

        private void PlayFocusAnimation()
        {
            var outline = GetNode<Panel>("%Outline");
            var tween = CreateTween();
            tween.SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(outline, "modulate:a", 1f, tweenTime);

            tween.SetTrans(Tween.TransitionType.Expo);
            tween.TweenProperty(_worldImage, "rect_rotation", 1f, tweenTime * 5f);
            tween.TweenProperty(_worldImage, "rect_scale", Vector2.One * 1.1f, tweenTime * 5f);

            tween.TweenProperty(_playIcon, "rect_position:x", 0, tweenTime * 4f);
            tween.TweenProperty(_playIcon, "modulate:a", 1f, tweenTime * 4f);
        }

        private void VisibilityChanged()
        {
            var canUnlock = _unlockable != null && _unlockDb.CanUnlock(_unlockable.EntryId);
            var unlocked = _unlockable != null && _unlockDb.IsUnlocked(_unlockable.EntryId);

            var readyToUnlockOverlay = GetNode<Panel>("%ReadyToUnlockOverlay");

            if (canUnlock && !unlocked)
            {
                readyToUnlockOverlay.Show();
            }
            else
            {
                readyToUnlockOverlay.Hide();
            }

            if (_unlockable != null)
            {
                if (_unlockable.HasCondition<EssenceUnlockCondition>())
                {
                    var condition = _unlockable.GetCondition<EssenceUnlockCondition>();
                    var progressHandler = this.GetSingleton<ProgressHandler>();
                    var label = _essenceOverlay.GetNode<Label>("%EssenceLabel");
                    label.Text = $"{progressHandler.Essence}/{condition.EssenceRequired}";
                }
            }
        }

        private void PlayFocusExitAnimation()
        {
            var outline = GetNode<Panel>("%Outline");
            var tween = CreateTween();
            tween.SetParallel();
            tween.SetEase(Tween.EaseType.Out);
            tween.TweenProperty(outline, "modulate:a", 0f, tweenTime);

            tween.SetTrans(Tween.TransitionType.Expo);
            tween.TweenProperty(_worldImage, "rect_rotation", 0f, tweenTime * 5f);
            tween.TweenProperty(_worldImage, "rect_scale", Vector2.One, tweenTime * 5f);

            tween.TweenProperty(_playIcon, "rect_position:x", 64f, tweenTime * 4f);
            tween.TweenProperty(_playIcon, "modulate:a", 0f, tweenTime * 4f);
        }

        private void UnlockOrOpenWorld()
        {
            if (_unlockable != null)
            {
                if (_unlockDb.IsUnlocked(_unlockable.EntryId))
                {
                    fmodRuntime.PlayOneShot("event:/UI/WorldPress");
                    OnPressedAction?.Invoke(Order - 1);
                    return;
                }

                if (_unlockable.HasCondition<EssenceUnlockCondition>())
                {
                    var condition = _unlockable.GetCondition<EssenceUnlockCondition>();
                    bool unlocked = _unlockDb.TryUnlock(_unlockable.EntryId);

                    if (unlocked)
                    {
                        var progressHandler = this.GetSingleton<ProgressHandler>();
                        progressHandler.Essence -= condition.EssenceRequired;

                        fmodRuntime.PlayOneShot("event:/UI/Unlock");
                        _essenceOverlay.Hide();

                        var readyToUnlockOverlay = GetNode<Panel>("%ReadyToUnlockOverlay");
                        readyToUnlockOverlay.Hide();
                    }
                    else
                    {
                        return;
                    }
                }
            }

            fmodRuntime.PlayOneShot("event:/UI/WorldPress");
            OnPressedAction?.Invoke(Order - 1);
        }

        private Tween _tween;
        private bool _gotNodes;
        private WorldButtonData _data;
        private TextureRect _worldImage, _playIcon;
        private Label _worldOrderLabel, _worldNameLabel;
        private Label _progressLabel, _fragmentLabel;
        private Control _unavailableOverlay, _lockedOverlay, _essenceOverlay;

        private UnlockableBase _unlockable;
        private UnlocksDatabase _unlockDb;
    }

    public class WorldButtonData
    {
        public Levels.World World;
        public WorldSaveData WorldSaveData;
        public UnlockableBase Unlockable;
    }
}
