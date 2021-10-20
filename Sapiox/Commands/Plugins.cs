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

        public string pluginlist()
        {
            if (SapioxManager.Plugins.Count == 0)
                return string.Join("\n- ", SapioxManager.Plugins);
            else
                return "No plugins loaded.";
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            List<String> activeplugins = new List<string>();
            foreach(IPlugin plugins in SapioxManager.Plugins)
            {
                activeplugins.Add(plugins.Info.Name);
            }

                response = $"Active Plugins:\n- " + string.Join("\n- ", activeplugins);
            return true;
        }
    }
}
