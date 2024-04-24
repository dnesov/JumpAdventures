using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using GodotFmod;
using Jump.Extensions;
using Jump.Unlocks;

namespace Jump.Utils
{
    public class NotificationManager : Node
    {
        public Action<Notification> OnNotificationAdded;
        public Action<Notification> OnNotificationExpired;

        public NotificationManager()
        {
            _notifications = new Queue<Notification>();
            _shownNotifications = new HashSet<string>();
        }

        public override void _Ready()
        {
            base._Ready();
            GetNodes();
            SubscribeEvents();
        }

        public async Task AddNotification(Notification notification)
        {
            _notifications.Enqueue(notification);
            OnNotificationAdded?.Invoke(notification);
            PlayNotificationSound();
            await YieldNotificationDequeueAsync(notification.Duration);
        }

        public async Task AddNotification(string title, string contents, float duration = 2.5f)
        {
            var notification = new Notification(title, contents, duration);
            await AddNotification(notification);
        }

        public async Task AddNotificationWithId(string title, string contents, string id = "", float duration = 2.5f)
        {
            if (id != string.Empty)
            {
                if (_shownNotifications.Contains(id)) return;
                _shownNotifications.Add(id);
            }
            var notification = new Notification(title, contents, duration, id);
            await AddNotification(notification);
        }

        public async Task AddNotificationWithId(Notification notification)
        {
            if (notification.Id != string.Empty)
            {
                if (_shownNotifications.Contains(notification.Id)) return;
                _shownNotifications.Add(notification.Id);
            }

            await AddNotification(notification);
        }

        private async Task YieldNotificationDequeueAsync(float duration)
        {
            await ToSignal(GetTree().CreateTimer(duration), "timeout");
            DequeueNotification();
        }

        private void DequeueNotification()
        {
            var notification = _notifications.Dequeue();
            OnNotificationExpired?.Invoke(notification);
        }

        private void GetNodes()
        {
            _game = this.GetSingleton<Game>();
            _fmodRuntime = this.GetSingleton<FmodRuntime>();
            _progressHandler = this.GetSingleton<ProgressHandler>();
            _unlocksDb = this.GetSingleton<UnlocksDatabase>();
        }

        private void SubscribeEvents()
        {
            _game.OnWin += DisplayUnlockNotifications;
            _game.OnWin += DisplayEssenceReminderNotification;
            _game.OnWin += DisplayWorldReminderNotification;
            _progressHandler.OnFragmentCollected += DisplayUnlockNotifications;
        }

        private async void DisplayUnlockNotifications()
        {
            var unlockables = _unlocksDb.GetUnlockableUnlocks();

            foreach (var unlockable in unlockables)
            {
                if (unlockable.HasCondition<EssenceUnlockCondition>()) continue;

                var notification = new Notification(unlockable.Title, string.Empty, 2.5f, id: unlockable.EntryId)
                {
                    Icon = unlockable.Icon,
                    IconModulate = unlockable.IconModulate
                };

                await AddNotificationWithId(notification);
            }
        }

        private async void DisplayEssenceReminderNotification()
        {
            var unlockables = _unlocksDb.GetUnlockableUnlocks();

            foreach (var unlockable in unlockables)
            {
                if (!unlockable.HasCondition<EssenceUnlockCondition>()) continue;

                var notification = new Notification("UI_NOTIFICATION_ESSENCEREMINDER", "UI_NOTIFICATION_ESSENCEREMINDER_DESC", 5f, id: "essence_reminder")
                {
                    Icon = unlockable.Icon,
                    IconModulate = unlockable.IconModulate
                };

                await AddNotificationWithId(notification);
                return;
            }
        }

        private async void DisplayWorldReminderNotification()
        {
            var unlockables = _unlocksDb.GetUnlockableUnlocks();

            foreach (var unlockable in unlockables)
            {
                if (unlockable is not WorldUnlockable worldUnlockable) continue;

                var desc = string.Format(Tr("UI_NOTIFICATION_WORLDREADYTOUNLOCK_DESC"), Tr(worldUnlockable.Title));
                var notification = new Notification("UI_NOTIFICATION_WORLDREADYTOUNLOCK", desc, 5f, id: worldUnlockable.EntryId)
                {
                    Icon = worldUnlockable.Icon,
                    IconModulate = worldUnlockable.IconModulate
                };

                await AddNotificationWithId(notification);
                return;
            }
        }

        private void PlayNotificationSound()
        {
            _fmodRuntime.PlayOneShot("event:/UI/Notification");
        }

        private Queue<Notification> _notifications;
        private HashSet<string> _shownNotifications;

        private Game _game;
        private FmodRuntime _fmodRuntime;
        private ProgressHandler _progressHandler;
        private UnlocksDatabase _unlocksDb;
    }

    public class Notification
    {
        public string Id => _id;
        public string Title => _title;
        public string Contents => _contents;
        public float Duration => _duration;
        public Texture Icon { get; set; }
        public Color IconModulate { get; set; }
        public Notification(string title, string contents, float duration = 2.5f)
        {
            _title = title;
            _contents = contents;
            _duration = duration;
        }

        public Notification(string title, string contents, float duration, string id)
        {
            _title = title;
            _contents = contents;
            _duration = duration;
            _id = id;
        }

        private string _title, _contents, _id;
        private float _duration;
    }
}