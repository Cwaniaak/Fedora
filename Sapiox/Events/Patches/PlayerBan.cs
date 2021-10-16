using System;
using HarmonyLib;
using Sapiox.API;
using Sapiox.Events.EventArgs;
using UnityEngine;

namespace Sapiox.Events.Patches
{
    public class PlayerBan
    {
        [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(GameObject), typeof(long), typeof(string),
            typeof(string), typeof(bool))]
        internal static class PlayerBanPatch
        {
            [HarmonyPrefix]
            private static bool BanUser(GameObject user, long duration, string reason, bool isGlobalBan)
            {
                try
                {
                    var Target = Server.GetPlayer(user);
                    
                    if (duration == 0) Handlers.Player.OnKick(Target, ref reason);
                    else Handlers.Player.OnBan(Target, ref duration, ref isGlobalBan, ref reason);

                    return true;
                }
                catch (Exception e)
                {
                    Log.Error($"{typeof(PlayerBanPatch).FullName}.{nameof(BanUser)}:\n{e}");
                    return true;
                }
            }
        }
    }
}