using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Sapiox.API;
using Sapiox.Events.EventArgs;
using HarmonyLib;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.UpdateNickname))]
    internal static class PlayerJoinPatch
    {
        [HarmonyPrefix]
        private static void UpdateNickname(NicknameSync __instance, ref string n)
        {
            try
            {
                var player = Server.GetPlayer(__instance);
                Handlers.Player.OnJoin(player, ref n);
            }
            catch (Exception e)
            {
                Log.Error($"{typeof(PlayerJoinPatch).FullName}.{nameof(UpdateNickname)}:\n{e}");
            }
        }
    }
}