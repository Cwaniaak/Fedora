using HarmonyLib;
using Sapiox.API;
using System;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcRoundStarted))]
    internal static class RoundStart
    {
        [HarmonyPrefix]
        private static void OnRoundStart()
        {
            try
            {
                Sapiox.Events.Handlers.Round.OnRoundStart();
            }
            catch (Exception e)
            {
                Log.Error($"{typeof(RoundStart).FullName}.{nameof(OnRoundStart)}:\n{e}");
            }
        }
    }
}
