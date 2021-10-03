using System.IO;
using System.Net;
using Newtonsoft.Json;
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
            Events.Handlers.Player.Ban += OnPlayerBan;
        }

        public void OnPlayerBan(Events.EventArgs.PlayerBanEventArgs ev)
        {
            //webhook
            string token =
                "https://canary.discord.com/api/webhooks/894256450384322561/b-Gi2cLMxUwpOWeMVFf_926Pr6xlm7eKSd-a-ryo8vfQ3m-2SjzGGledBhuI32gx5OdR";
            WebRequest wr = (HttpWebRequest) WebRequest.Create(token);
            wr.ContentType = "application/json";
            wr.Method = "POST";
            using (var sw = new StreamWriter(wr.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(new
                {
                    username = "CwanStars Ban",
                    embeds = new[]
                    {
                        new
                        {
                            description =
                                $"Gracz {ev.Target.NickName} ({ev.Target.UserId}) dostał bana\nPowód: {ev.Reason}\nCzas: {ev.Duration}",
                            title = "Testing Stuff",
                            color = 65417
                        }
                    }
                });

                sw.Write(json);
            }

            var responsee = (HttpWebResponse) wr.GetResponse();
        }
        public void OnPlayerLeave(Events.EventArgs.PlayerLeaveEventArgs ev)
        {
            Log.Info($"Player {ev.Player.NickName} has left the server!");
        }
        public void OnPlayerJoin(Events.EventArgs.PlayerJoinEventArgs ev)
        {
            Log.Info($"Player {ev.NickName} has joined the server!");
        }
    }
}