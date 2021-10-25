using HarmonyLib;
using Sapiox.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
    internal class RoundEnd
    {
        [HarmonyPrefix]
        private static void OnRoundEnd(ref string q)
        {
            if (q.StartsWith("Round finished! Anomalies: "))
            {
                try
                {
                    Handlers.Round.OnRoundEnd();
                }
                catch (Exception e)
                {
                    Log.Error($"{typeof(RoundEnd).FullName}.{nameof(OnRoundEnd)}:\n{e}");
                }
            }
        }
    }
}
