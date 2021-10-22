using CommandSystem;
using Sapiox.API;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sapiox.Commands
{
    public class Plugins : ICommand
    {
        public string Command { get; } = "plugins";

        public string[] Aliases { get; } = new string[] { "pluginlist" };

        public string Description { get; } = "Plugins list";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<string> activeplugins = new List<string>();
            foreach (IPlugin plugin in SapioxManager.Plugins)
            {
                activeplugins.Add($"{plugin.Info.Name} (Version: {plugin.Info.Version})");
            }

            response = $"Active Plugins:\n- " + string.Join("\n- ", activeplugins);
            return true;
        }
    }
}
