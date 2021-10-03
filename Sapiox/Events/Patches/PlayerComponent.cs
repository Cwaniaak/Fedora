using HarmonyLib;
using Sapiox.API;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.LoadComponents))]
    internal static class PlayerComponent
    {
        [HarmonyPrefix]
        private static void LoadComponents(ReferenceHub __instance)
        {
            if (__instance.GetComponent<Player>() == null)
                __instance.gameObject.AddComponent<Player>();
        }
    }
}