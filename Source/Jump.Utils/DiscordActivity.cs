using Godot;
using Discord;
using System;

namespace Jump.Utils
{
    public class DiscordActivity : Activity
    {

        public override void Register()
        {
            try
            {
                _client = new Discord.Discord(Constants.DISCORD_CLIENT_ID, (ulong)CreateFlags.NoRequireDiscord);
                _client.SetLogHook(LogLevel.Debug, (level, message) => Log(level, message));

                _isRunning = true;
            }
            catch (System.Exception e)
            {
                _logger.Error(e);
            }
        }

        public override void Set(ActivityData data)
        {
            if (!_isRunning) return;

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
            if (!_isRunning) return;

            try
            {
                _client.RunCallbacks();
            }
            catch (Exception e)
            {
                _logger.Error(e);
                _isRunning = false;
            }
        }

        private void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Info:
                    _logger.Info(message);
                    break;
                case LogLevel.Warn:
                    _logger.Warn(message);
                    break;
                case LogLevel.Error:
                    _logger.Error(message);
                    break;
            }
        }

        public override void Shutdown()
        {
            _client.Dispose();
        }

        private Discord.Discord _client;
        private Discord.Discord.SetLogHookHandler _logHandler;
        private Logger _logger = new(nameof(DiscordActivity));

        private bool _isRunning;
    }
}