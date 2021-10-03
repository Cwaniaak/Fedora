using System;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using UnityEngine;

namespace Sapiox.API
{
    public class Server
    {
        public static string Name
        {
            get => ServerConsole._serverName;
            set
            {
                ServerConsole._serverName = value;
            }
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

        public static List<Player> Players =>
            PlayerManager.players.Select(x => x.GetComponent<Player>()).ToList();
    }
}