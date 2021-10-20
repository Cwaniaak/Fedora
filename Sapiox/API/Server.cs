using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Assets._Scripts.Dissonance;
using CommandSystem;
using HarmonyLib;
using Mirror;
using Newtonsoft.Json;
using RemoteAdmin;
using UnityEngine;

namespace Sapiox.API
{
    public static class Server
    {
        public static string Name
        {
            get => ServerConsole._serverName;
            set => ServerConsole._serverName = value;
        }

        public static void SendDiscordWebhook(string token, string username, string content, string description = null,
            bool embed = false, int embedColor = 65417, string embedTitle = null, string thumbnailUrl = null)
        {
            WebRequest wr = (HttpWebRequest) WebRequest.Create(token);
            wr.ContentType = "application/json";
            wr.Method = "POST";
            if (embed)
            {
                using (var sw = new StreamWriter(wr.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        username = username,
                        content = content,
                        embeds = new[]
                        {
                            new
                            {
                                description = description,
                                title = embedTitle,
                                thumbnail = new
                                {
                                    url = thumbnailUrl
                                },
                                color = embedColor
                            }
                        }
                    });
                    sw.Write(json);
                }
                var response = (HttpWebResponse) wr.GetResponse();
            }
            else
            {
                using (var sw = new StreamWriter(wr.GetRequestStream()))
                {
                    string json = JsonConvert.SerializeObject(new
                    {
                        username = username,
                        content = content
                    });
                    sw.Write(json);
                }
                var response = (HttpWebResponse) wr.GetResponse();
            }
        }

        public static string Ip
        {
            get => ServerConsole.Ip;
            set => ServerConsole.Ip = value;
        }

        public static ushort Port
        {
            get => ServerStatic.ServerPort;
            set => ServerStatic.ServerPort = value;
        }

        public static bool FriendlyFire
        {
            get => ServerConsole.FriendlyFire;
            set => ServerConsole.FriendlyFire = value;
        }

        public static Player GetPlayer(NetworkConnection connection)
        {
            return GetPlayer(connection.identity);
        }

        public static Player GetPlayer(MonoBehaviour mono)
        {
            return mono?.gameObject?.GetComponent<Player>();
        }

        public static Player GetPlayer(GameObject gameObject)
        {
            return gameObject?.GetComponent<Player>();
        }

        public static Player GetPlayer(PlayableScps.PlayableScp scp)
        {
            return GetPlayer(scp?.Hub);
        }

        public static List<Player> GetPlayers(RoleType role)
        {
            return Players.Where(x => x.Role == role).ToList();
        }

        public static List<Player> GetPlayers(Team team)
        {
            return Players.Where(x => x.Team == team).ToList();
        }

        public static List<Player> GetPlayers(Faction fraction)
        {
            return Players.Where(x => x.Faction == fraction).ToList();
        }

        public static List<Player> GetPlayers(RoleType[] roles)
        {
            return Players.Where(x => roles.Any(y => x.Role == y)).ToList();
        }

        public static List<Player> GetPlayers(Team[] teams)
        {
            return Players.Where(x => teams.Any(y => x.Team == y)).ToList();
        }

        public static List<Player> GetPlayers(Faction[] fractions)
        {
            return Players.Where(x => fractions.Any(y => x.Faction == y)).ToList();
        }

        public static Player GetPlayer(int playerid) => Players.FirstOrDefault(x => x.Id == playerid);

        public static Player GetPlayer(string argument)
        {
            var players = Players;

            if (int.TryParse(argument, out var playerid))
            {
                var player = GetPlayer(playerid);
                if (player == null)
                    goto AA_001;

                return player;
            }

            else if (argument.Contains("@"))
            {
                var player = players.FirstOrDefault(x => x.UserId.ToLower() == argument);
                if (player == null)
                    goto AA_001;

                return player;
            }

            AA_001:
            return players.FirstOrDefault(x => x.NickName.ToLower() == argument.ToLower());
        }

        public static List<Player> Players =>
            PlayerManager.players.Select(x => x.gameObject.GetComponent<Player>()).ToList();

        public static void RegisterRemoteAdminCommand(ICommand command)
        {
            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(command);
        }

        public static void RegisterServerConsoleCommand(ICommand command)
        {
            GameCore.Console.singleton.ConsoleCommandHandler.RegisterCommand(command);
        }

        public static void RegisterClientConsoleCommand(ICommand command)
        {
            QueryProcessor.DotCommandHandler.RegisterCommand(command);
        }
    }
}