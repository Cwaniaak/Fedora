using CommandSystem;
using Sapiox.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sapiox.Example
{
    public class ExampleCommand : ICommand
    {
        public string Command { get; } = "SpawnWorkStation";

        public string[] Aliases { get; } = new string[] { "spawnbench" };

        public string Description { get; } = "Sapiox example command.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            Player player = Server.GetPlayer(sender);
            Map.SpawnWorkStation(player.Position, new Vector3(0f, 0f, 0f), new Vector3(1f, 1f, 1f));
            response = $"Spawned work station at {player.NickName} position!";
            return true;
        }
    }
}
