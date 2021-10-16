/*using System;
using HarmonyLib;
using Sapiox.API;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
    internal static class ServerNamePatch
    {
        [HarmonyPostfix]
        private static void ReloadServerName()
        {
            Server.Name += "<color=#00000000><size=1>Sapiox 1.0.0</size></color>";
        }
    }
}*/