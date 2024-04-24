using Godot;
using Jump.Entities;
using System;

namespace Jump.Editor
{
    public class Editor : Node
    {
        public bool IsPlayerOnScene { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GetNodes();
            RegisterEditorElements();
        }

        public override void _Input(InputEvent @event)
        {
            if (@event is InputEventMouseButton mouseButton)
            {
                if (mouseButton.ButtonMask == (int)ButtonList.Right)
                {
                    ShowToolPopup(mouseButton.Position);
                }
            }
        }

        public Player AddPlayerAt(Vector2 at)
        {
            IsPlayerOnScene = true;

            var playerScene = GD.Load<PackedScene>(_playerScenePath);
            var player = playerScene.Instance<Player>();
            _levelRoot.AddChild(player);
            player.GlobalPosition = at;

            return null;
        }

        public PolygonGround AddPolygonGroundAt(Vector2 at)
        {
            return null;
        }

        private void GetNodes()
        {
            _levelRoot = GetNode<Node2D>("%LevelRoot");
            _toolPopup = GetNode<EditorUIElement>("%ToolPopup");
        }

        private void RegisterEditorElements()
        {
            _toolPopup.RegisterEditor(this);
        }

        private void ShowToolPopup(Vector2 at)
        {
            _toolPopup.Display();
            _toolPopup.RectPosition = at;
        }

        private Node GetNodeAt(Vector2 at)
        {
            return null;
        }

        private EditorUIElement _toolPopup;

        private Node2D _levelRoot;
        private Player _player;

        private readonly string _playerScenePath = "res://Prefabs/Player/Player.tscn";
    }
}