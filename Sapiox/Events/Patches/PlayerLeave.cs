using System;
using Sapiox.API;
using Sapiox.Events.EventArgs;
using HarmonyLib;
using Mirror;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect))]
    internal static class PlayerLeave
    {
        [HarmonyPrefix]
        private static void OnServerDisconnect(NetworkConnection conn)
        {
            try
            {
                Player player = Server.GetPlayer(conn);
                Handlers.Player.OnLeave(player);
            }
            catch (Exception e)
            {
                Log.Error($"{typeof(PlayerLeave).FullName}.{nameof(OnServerDisconnect)}:\n{e}");
            }
        }
    }
}