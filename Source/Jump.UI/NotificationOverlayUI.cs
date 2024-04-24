using System.Collections.Generic;
using Godot;
using Jump.Extensions;
using Jump.Utils;

namespace Jump.UI
{
    public class NotificationOverlayUI : UIElement
    {
        public NotificationOverlayUI()
        {
            _notificationUIElements = new Queue<NotificationUI>();
        }
        public override void _Ready()
        {
            base._Ready();
            OnDisplay();
        }
        protected override void OnDisplay()
        {
            GetNodes();
            SubscribeEvents();
        }

        protected override void OnHide()
        {
        }

        private void GetNodes()
        {
            _notificationManager = this.GetSingleton<NotificationManager>();
            _notificationContainer = GetNode<VBoxContainer>("%NotificationContainer");
        }

        private void SubscribeEvents()
        {
            _notificationManager.OnNotificationAdded += OnNotificationAdded;
            _notificationManager.OnNotificationExpired += OnNotificationExpired;
        }

        private void OnNotificationAdded(Notification notification)
        {
            var notificationNode = _notificationScene.Instance<NotificationUI>();
            _notificationContainer.AddChild(notificationNode);
            _notificationContainer.MoveChild(notificationNode, 0);

            notificationNode.Display();
            notificationNode.UpdateElements(notification);
            _notificationUIElements.Enqueue(notificationNode);
        }

        private void OnNotificationExpired(Notification notification)
        {
            var notificationNode = _notificationUIElements.Dequeue();
            notificationNode.Hide();
        }

        private Queue<NotificationUI> _notificationUIElements;
        [Export] private PackedScene _notificationScene;
        private NotificationManager _notificationManager;
        private VBoxContainer _notificationContainer;
    }
}