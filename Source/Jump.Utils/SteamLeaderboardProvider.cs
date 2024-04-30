// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using Steamworks;
// using Steamworks.Data;

namespace Jump.Utils;

public class SteamLeaderboardProvider
{
    // public SteamLeaderboardProvider(SteamManager steamManager)
    // {
    //     _manager = steamManager;
    // }

    // public async Task<List<LeaderboardEntry>> GetEntriesAsync(string leaderboardId)
    // {
    //     var result = await SteamUserStats.FindOrCreateLeaderboardAsync(leaderboardId, LeaderboardSort.Ascending, LeaderboardDisplay.TimeMilliSeconds);

    //     List<LeaderboardEntry> entries = new List<LeaderboardEntry>();

    //     if (result is Leaderboard leaderboard)
    //     {
    //         var scores = await leaderboard.GetScoresAroundUserAsync();

    //         if (scores == null)
    //         {
    //             return null;
    //         }

    //         foreach (var score in scores)
    //         {
    //             var name = score.User.Name;
    //             var value = score.Score / 1000f;

    //             entries.Add(new LeaderboardEntry()
    //             {
    //                 Nick = name,
    //                 Time = value
    //             });
    //         }
    //     }

    //     return entries;
    // }

    // public async Task UploadAsync(string leaderboardId, float time)
    // {
    //     var result = await SteamUserStats.FindOrCreateLeaderboardAsync(leaderboardId, LeaderboardSort.Ascending, LeaderboardDisplay.TimeMilliSeconds);

    //     if (result is Leaderboard leaderboard)
    //     {
    //         var timeSpan = TimeSpan.FromSeconds(time);
    //         await leaderboard.SubmitScoreAsync((int)timeSpan.TotalMilliseconds);
    //     }
    // }

    // private SteamManager _manager;
}

// public class LeaderboardEntry
// {
//     public string Nick { get; set; }
//     public float Time { get; set; }
// }