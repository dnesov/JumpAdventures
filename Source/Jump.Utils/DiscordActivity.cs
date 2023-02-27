using Godot;
using Discord;

namespace Jump.Utils
{
    public class DiscordActivity : Activity
    {

        public override void Register()
        {
            try
            {
                _client = new Discord.Discord(Constants.DISCORD_CLIENT_ID, (ulong)Discord.CreateFlags.Default);
                // _client.SetLogHook(LogLevel.Debug, (level, message) => Log(level, message));
            }
            catch (System.Exception e)
            {
                GD.PrintErr(e);
            }
        }

        public override void Set(ActivityData data)
        {
            var manager = _client.GetActivityManager();

            var activity = new Discord.Activity
            {
                State = data.State,
                Details = data.Details,
                Timestamps = {
                    Start = (long)OS.GetUnixTime(),
                },
                Assets = {
                    LargeImage = data.Assets.LargeAsset,
                    LargeText = data.Assets.LargeAssetText
                },
            };

            manager.UpdateActivity(activity, result => { });
        }

        public override void Process()
        {
            _client.RunCallbacks();
        }

        private void Log(LogLevel level, string message)
        {
            // GD.Print($"[DISCORD](${level.ToString()}) ${message}");
        }

        private Discord.Discord _client;
        private Discord.Discord.SetLogHookHandler _logHandler;
    }
}