using HarmonyLib;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using Mirror;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Sapiox.API
{
    public class FakePlayer
    {
        public Player Player { get; internal set; }
        public GameObject GameObject { get; internal set; }
        public string NickName => Player.Hub.nicknameSync.Network_myNickSync;
        public RoleType Role => Player.Hub.characterClassManager.CurClass;
        public Vector3 Position
        {
            get => Player.transform.position;
            set
            {
                Player.transform.position = value;
                Player.Hub.playerMovementSync.RealModelPosition = value;
            }
        }

        public FakePlayer(Vector3 pos, RoleType role = RoleType.ClassD, string nickname = "FakePlayer", float health = 100, int maxHealth = 150, bool godmode = false)
        {
            try
            {
                GameObject obj = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);
                GameObject = obj;

                Player = Server.GetPlayer(GameObject);


                Player.transform.localScale = Vector3.one;
                Player.transform.position = pos;
                Player.Hub.playerMovementSync.RealModelPosition = pos;
                Player.Hub.queryProcessor.NetworkPlayerId = QueryProcessor._idIterator;
                Player.Hub.queryProcessor._ipAddress = Server.Host.IPAddress;
                Player.Hub.characterClassManager.CurClass = role;
                Player.Hub.nicknameSync.Network_myNickSync = nickname;
                Player.GodMode = godmode;
                Player.Health = health;
                Player.MaxHealth = maxHealth;
                Player.RankName = "Admin";
                Player.RankColor = "red";
                Player.Hub.playerMovementSync.NetworkGrounded = true;


                NetworkServer.Spawn(GameObject);
                Server.FakePlayers.Add(Player);
            }
            catch (Exception e)
            {
                Log.Error($"Error:\n{e}");
            }
        }
        public void Spawn()
        {
            try
            {
                NetworkServer.Spawn(GameObject);
                Server.FakePlayers.Add(Player);
            }
            catch (Exception e)
            {
                Log.Error($"Error:\n{e}");
            }
        }

        public void DeSpawn()
        {
            NetworkServer.UnSpawn(GameObject);
            Server.FakePlayers.Remove(Player);
        }
    }

    //patches

    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))]
    internal static class OverridePositionPatch
    {
        [HarmonyPrefix]
        private static bool OverridePosition(PlayerMovementSync __instance, Vector3 pos, float rot, bool forceGround = false)
        {
            try
            {
                if (forceGround && Physics.Raycast(pos, Vector3.down, out var raycastHit, 100f, __instance.CollidableSurfaces))
                {
                    pos = raycastHit.point + Vector3.up * 1.23f * __instance.transform.localScale.y;
                }
                __instance.ForcePosition(pos);
                __instance.TargetSetRotation(__instance.connectionToClient, rot);
            }
            catch { }

            return false;
        }
    }

    [HarmonyPatch(typeof(NetworkBehaviour), nameof(NetworkBehaviour.SendTargetRPCInternal))]
    internal static class MirrorPatch
    {
        [HarmonyPrefix]
        private static bool TargetRPC(NetworkBehaviour __instance)
        {
            if (Server.GetPlayer(__instance) != null && Server.GetPlayer(__instance).IsFakePlayer) return false;
            return true;
        }
    }

    [HarmonyPatch(typeof(StandardHitregBase), nameof(StandardHitregBase.ShowHitIndicator))]
    internal static class ShowHitPatch
    {
        [HarmonyPrefix]
        private static bool HitInidcatorFix(uint netId, float damage, Vector3 origin)
        {
            if (!ReferenceHub.TryGetHubNetID(netId, out ReferenceHub referenceHub))
            {
                return false;
            }
            var player = Server.GetPlayer(referenceHub);
            if (player == null || player.IsFakePlayer)
            {
                return false;
            }
            foreach (ReferenceHub referenceHub2 in referenceHub.spectatorManager.ServerCurrentSpectatingPlayers)
            {
                referenceHub2.networkIdentity.connectionToClient.Send(new GunHitMessage
                {
                    Weapon = ItemType.None,
                    Damage = (byte)Mathf.Round(damage * 2.5f),
                    DamagePosition = origin
                }, 0);
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(FirearmExtensions), nameof(FirearmExtensions.ServerSendAudioMessage))]
    internal static class SendAudioPatch
    {
        [HarmonyPrefix]
        private static bool FirearmPlaySound(this Firearm firearm, byte clipId)
        {
            FirearmAudioClip firearmAudioClip = firearm.AudioClips[clipId];
            ReferenceHub owner = firearm.Owner;

            float num = firearmAudioClip.HasFlag(FirearmAudioFlags.ScaleDistance) ? (firearmAudioClip.MaxDistance * firearm.AttachmentsValue(AttachmentParam.GunshotLoudnessMultiplier)) : firearmAudioClip.MaxDistance;
            if (firearmAudioClip.HasFlag(FirearmAudioFlags.IsGunshot) && owner.transform.position.y > 900f)
            {
                num *= 2.3f;
            }

            float soundReach = num * num;
            foreach (ReferenceHub referenceHub in ReferenceHub.GetAllHubs().Values)
            {
                var player = Server.GetPlayer(referenceHub);
                if (player == null || player.IsFakePlayer)
                {
                    return false;
                }
                if (referenceHub != firearm.Owner)
                {
                    RoleType curClass = referenceHub.characterClassManager.CurClass;
                    if (curClass == RoleType.Spectator || curClass == RoleType.Scp079 || (referenceHub.transform.position - owner.transform.position).sqrMagnitude <= soundReach)
                    {
                        referenceHub.networkIdentity.connectionToClient.Send(new GunAudioMessage(owner, clipId, (byte)Mathf.RoundToInt(Mathf.Clamp(num, 0f, 255f)), referenceHub), 0);
                    }
                }
            }

            Action<Firearm, byte, float> serverSoundPlayed = FirearmExtensions.ServerSoundPlayed;
            if (serverSoundPlayed == null)
            {
                return false;
            }

            serverSoundPlayed.Invoke(firearm, clipId, num);
            return false;
        }
    }
}
