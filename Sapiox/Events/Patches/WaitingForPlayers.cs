using System;
using HarmonyLib;
using Sapiox.API;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
    internal class WaitingForPlayers
    {
        [HarmonyPrefix]
        private static void OnWaitForPlayers(ref string q)
        {
            if (q == "Waiting for players...")
            {
                try
                {
                    Handlers.Round.OnWaitingForPlayers();
                }
                catch (Exception e)
                {
                    Log.Error($"{typeof(WaitingForPlayers).FullName}.{nameof(OnWaitForPlayers)}:\n{e}");
                }
            }
        }
    }
}