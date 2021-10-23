using System;
using System.Reflection;

namespace Sapiox.API
{
    public static class Log
    {
        public static void Info(string message) => Info((object)message);

        public static void Info(object message)
        {
            var name = Assembly.GetCallingAssembly().GetName().Name;
            Send($"{name}: {message}", ConsoleColor.Green);
        }

        public static void Warn(string message) => Warn((object)message);

        public static void Warn(object message)
        {
            var name = Assembly.GetCallingAssembly().GetName().Name;
            Send($"{name}: {message}", ConsoleColor.Yellow);
        }

        public static void Error(string message) => Error((object)message);

        public static void Error(object message)
        {
            var name = Assembly.GetCallingAssembly().GetName().Name;
            Send($"{name}: {message}", ConsoleColor.Red);
        }

        public static void Send(string message, ConsoleColor color) => ServerConsole.AddLog(message, color);
    }
}