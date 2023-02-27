using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;

namespace Jump.Utils
{
    public class NotificationManager : Node
    {
        public Action<Notification> OnNotificationAdded;
        public Action<Notification> OnNotificationExpired;

        public NotificationManager()
        {
            _notifications = new Queue<Notification>();
        }
        public async Task AddNotification(string title, string contents, float duration = 2.5f)
        {
            var notification = new Notification(title, contents, duration);
            _notifications.Enqueue(notification);
            OnNotificationAdded?.Invoke(notification);
            await YieldNotificationDequeueAsync(duration);
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

        private Queue<Notification> _notifications;
    }

    public class Notification
    {
        public string Title => _title;
        public string Contents => _contents;
        public float Duration => _duration;
        public Notification(string title, string contents, float duration)
        {
            _title = title;
            _contents = contents;
            _duration = duration;
        }

        private string _title, _contents;
        private float _duration;
    }
}