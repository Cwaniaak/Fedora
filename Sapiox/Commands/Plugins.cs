using CommandSystem;
using System;

namespace Sapiox.Commands
{
    public class Plugins : ICommand
    {
        public string Command { get; } = "plugins";

        public string[] Aliases { get; } = new string[] { "pluginlist" };

        public string Description { get; } = "Plugins list";

        public string pluginlist()
        {
            return string.Join("\n- ", SapioxManager.Plugins);
        }

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"Active Plugins:\n{pluginlist()}";
            return true;
        }
    }
}
