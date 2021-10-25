using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using Sapiox.API;
using Sapiox.Events.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sapiox.Events.Patches
{
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractDoor
    {
        private static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                bool allow = true;

                if (__instance.ActiveLocks != 0)
                {
                    DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)__instance.ActiveLocks);
                    if ((!mode.HasFlagFast(DoorLockMode.CanClose) || !mode.HasFlagFast(DoorLockMode.CanOpen)) && (!mode.HasFlagFast(DoorLockMode.ScpOverride) || ply.characterClassManager.CurRole.team != 0) && (mode == DoorLockMode.FullLock || (__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanClose)) || (!__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanOpen))))
                    {
                        __instance.LockBypassDenied(ply, colliderId);
                        allow = false;
                        return false;
                    }
                }

                if (__instance.AllowInteracting(ply, colliderId))
                {
                    if (ply.characterClassManager.CurClass == RoleType.Scp079 || __instance.RequiredPermissions.CheckPermissions(ply.inventory.CurInstance, ply))
                    {
                        allow = true;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }

                Handlers.Player.OnInteractDoor(Server.GetPlayer(ply), __instance, ref allow);

                if (allow && __instance.AllowInteracting(ply, colliderId))
                {
                    __instance.NetworkTargetState = !__instance.TargetState;
                    __instance._triggerPlayer = ply;
                }
                else if (__instance.AllowInteracting(ply, colliderId))
                {
                    __instance.PermissionsDenied(ply, colliderId);
                    DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
                }

                return false;
            }
            catch (Exception ex)
            {
                Log.Error($"{typeof(InteractDoor).FullName}.{nameof(Prefix)}:\n{ex}");
                return true;
            }
        }
    }
}
