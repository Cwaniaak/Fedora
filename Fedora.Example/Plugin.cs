using Fedora.API;

namespace Fedora.Example
{
    [PluginInfo(
        Author = "Naku",
        Name = "Fedora.Example",
        Description = "Example plugin",
        Version = "1.0.0"
        )]
    public class Plugin : API.Plugin
    {
        public override IConfig config { get; } = new Config();

        public override void Load()
        {
            base.Load();
            Events.Handlers.Player.Join += OnPlayerJoin;
        }

        public void OnPlayerJoin(Events.EventArgs.PlayerJoinEventArgs ev)
        {
            Log.Info($"Player {ev.NickName} has joined the server!");
        }
    }
}