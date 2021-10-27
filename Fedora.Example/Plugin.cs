using MEC;
using Sapiox.API;
using Sapiox.Events.EventArgs;
using System;
using System.Collections.Generic;
using UnityEngine;
using PlayerEvent = Sapiox.Events.Handlers.Player;

namespace Sapiox.Example
{
    [PluginInfo(
        Author = "Naku",
        Name = "Sapiox.Example",
        Description = "Example plugin",
        Version = "1.0.0"
        )]
    public class Plugin : Sapiox.API.Plugin
    {
        public override IConfig config { get; set; } = new Config();

        public override void Load()
        {
            base.Load();
            Server.RegisterRemoteAdminCommand(new ExampleCommand());
            Server.RegisterRemoteAdminCommand(new SpawnFakePlayer());
            PlayerEvent.Join += OnPlayerJoin;
            PlayerEvent.Leave += OnPlayerLeave;
            PlayerEvent.Ban += OnPlayerBan;
            PlayerEvent.Kick += OnPlayerKick;
        }

        public void OnPlayerBan(PlayerBanEventArgs ev)
        {
            Log.Info($"Player {ev.Target.NickName} has been banned for {ev.Reason} {ev.Duration}");
        }

        public void OnPlayerKick(PlayerKickEventArgs ev)
        {
            Log.Info($"Player {ev.Target.NickName} has been kicked for {ev.Reason}");
        }
        public void OnPlayerLeave(PlayerLeaveEventArgs ev)
        {
            Log.Info($"Player {ev.Player.NickName} has left the server!");
        }
        public void OnPlayerJoin(PlayerJoinEventArgs ev)
        {
            Log.Info($"Player {ev.NickName} has joined the server!");
        }
    }
}