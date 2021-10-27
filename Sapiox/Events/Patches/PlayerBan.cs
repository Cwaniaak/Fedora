using System;
using HarmonyLib;
using Sapiox.API;
using Sapiox.Events.EventArgs;
using UnityEngine;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(BanPlayer), nameof(BanPlayer.BanUser), typeof(GameObject), typeof(long), typeof(string),
        typeof(string), typeof(bool))]
    internal static class PlayerBan
    {
        [HarmonyPrefix]
        private static bool BanUser(GameObject user, long duration, string reason, string issuer, bool isGlobalBan)
        {
            try
            {
                var allow = true;
                var Target = Server.GetPlayer(user);

                if (duration == 0) Handlers.Player.OnKick(Target, Server.GetPlayer(issuer), ref reason);
                else Handlers.Player.OnBan(Target, Server.GetPlayer(issuer), ref isGlobalBan, ref duration, ref reason, ref allow);

                return allow;
            }
            catch (Exception e)
            {
                Log.Error($"{typeof(PlayerBan).FullName}.{nameof(BanUser)}:\n{e}");
                return true;
            }
        }
    }
}