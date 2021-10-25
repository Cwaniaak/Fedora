using HarmonyLib;
using Sapiox.API;
using System;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Roundrestart))]
    internal static class RoundRestart
    {
        [HarmonyPrefix]
        private static void OnRoundRestart()
        {
            try
            {
                Handlers.Round.OnRoundRestart();
            }
            catch (Exception e)
            {
                Log.Error($"{typeof(RoundRestart).FullName}.{nameof(OnRoundRestart)}:\n{e}");
            }
        }
    }
}
