using System;
using System.Reflection;

namespace Sapiox.API
{
    public static class Log
    {
        public static void Info(object message)
        {
            Send($"{message}", ConsoleColor.Green);
        }

        public static void Warn(object message)
        {
            Send($"{message}", ConsoleColor.Yellow);
        }

        public static void Error(object message)
        {
            Send($"{message}", ConsoleColor.Red);
        }

        public static void Send(string message, ConsoleColor color) => ServerConsole.AddLog(message, color);
    }
}