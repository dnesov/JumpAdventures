// using Godot;
// using Jump.Extensions;
// using Jump.Utils;
// using System.Collections.Generic;

// namespace Jump.UI;

// public class LeaderboardUI : UIElement<LeaderboardData>
// {
//     public bool Loading
//     {
//         get => _loading;
//         set
//         {
//             if (value)
//             {
//                 _loadingUi.Show();
//                 _entriesUi.Hide();
//             }
//             else
//             {
//                 _loadingUi.Hide();
//                 _entriesUi.Show();
//             }
//         }
//     }

//     // Called when the node enters the scene tree for the first time.
//     public override void _Ready()
//     {
//         GetNodes();
//     }

//     protected override void OnDisplay()
//     {
//         Visible = true;
//         var noEntries = GetNode<Label>("%NoEntries");
//         noEntries.Hide();
//     }

//     protected override void OnHide()
//     {
//         Visible = false;
//     }

//     protected override void OnUpdateElements(LeaderboardData data)
//     {
//         var uploadButton = GetNode<Godot.Button>("%Upload");
//         uploadButton.Visible = data.CanUpload;

//         PopulateEntries(data.Entries);

//         _localCompleteTime = data.LocalCompleteTime;
//         _leaderboardId = data.LeaderboardId;
//     }

//     private async void Upload()
//     {
//         var noEntries = GetNode<Label>("%NoEntries");
//         noEntries.Hide();

//         // await _game.LeaderboardProvider.UploadAsync(_leaderboardId, _localCompleteTime);
//         Refresh();
//     }

//     private async void Refresh()
//     {
//         var noEntries = GetNode<Label>("%NoEntries");
//         noEntries.Hide();

//         Loading = true;
//         var entries = await _game.LeaderboardProvider.GetEntriesAsync(_leaderboardId);
//         Loading = false;

//         PopulateEntries(entries);
//     }

//     private void PopulateEntries(List<LeaderboardEntry> entries)
//     {
//         foreach (Control child in _entriesContainer.GetChildren())
//         {
//             child.QueueFree();
//         }

//         if (entries == null || entries.Count == 0)
//         {
//             var noEntries = GetNode<Label>("%NoEntries");
//             noEntries.Show();
//             return;
//         }

//         foreach (var entry in entries)
//         {
//             var entryScene = _leaderboardEntryScene.Instance<LeaderboardEntryUI>();

//             entryScene.Nick = entry.Nick;
//             entryScene.Time = entry.Time;

//             _entriesContainer.AddChild(entryScene);
//         }
//     }

//     private void GetNodes()
//     {
//         _loadingUi = GetNode<Control>("%Loading");
//         _entriesUi = GetNode<Control>("%Entries");
//         _entriesContainer = GetNode<VBoxContainer>("%EntriesContainer");

//         _game = this.GetSingleton<Game>();
//     }

//     [Export] private PackedScene _leaderboardEntryScene;
//     private bool _loading;
//     private Control _loadingUi;
//     private Control _entriesUi;
//     private VBoxContainer _entriesContainer;
//     private string _leaderboardId;
//     private float _localCompleteTime;

//     private Game _game;
// }

// public class LeaderboardData
// {
//     public string LeaderboardId { get; set; }
//     public List<LeaderboardEntry> Entries { get; set; }
//     public float LocalCompleteTime { get; set; }
//     public bool CanUpload { get; set; }
// }