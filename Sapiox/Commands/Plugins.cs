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
            foreach(IPlugin plugins in SapioxManager.Plugins)
            {
                activeplugins.Add("Name: "+plugins.Info.Name+"\nVersion: "+plugins.Info.Version+"\nAuthor: "+plugins.Info.Author);
            }

                response = $"Active Plugins:\n-\n" + string.Join("\n- ", activeplugins);
            return true;
        }
    }
}
