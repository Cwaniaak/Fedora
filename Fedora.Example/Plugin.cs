using Sapiox.API;

namespace Sapiox.Example
{
    [PluginInfo(
        Author = "Naku",
        Name = "Sapiox.Example",
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
            Events.Handlers.Player.Leave += OnPlayerLeave;
        }

        public void OnPlayerLeave(Events.EventArgs.PlayerLeaveEventArgs ev)
        {
            Log.Info($"Player {ev.Player.NickName} has left the server!");
        }
        public void OnPlayerJoin(Events.EventArgs.PlayerJoinEventArgs ev)
        {
            ev.Player.Broadcast(5,$"Welcome on this server, {ev.NickName}!");
            Log.Info($"Player {ev.NickName} has joined the server!");
        }
    }
}