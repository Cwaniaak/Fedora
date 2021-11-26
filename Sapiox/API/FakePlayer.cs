using HarmonyLib;
using InventorySystem;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments;
using InventorySystem.Items.Firearms.BasicMessages;
using InventorySystem.Items.Firearms.Modules;
using MEC;
using Mirror;
using RemoteAdmin;
using Sapiox.API.Enum;
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
        public MovementDirection MoveDirection { get; set; }
        public float SneakSpeed { get; set; } = 1.8f;
        public float WalkSpeed { get; set; }
        public float RunSpeed { get; set; }

        private ItemType _currentItem;
        public bool IsWallOnFront { get; internal set; }
        public bool IsWallOnRight { get; internal set; }
        public bool IsWallOnLeft { get; internal set; }
        public bool IsWallBehind { get; internal set; }

        public PlayerMovementState MoveState
        {
            get => Player.Hub.animationController.MoveState;
            set
            {
                Player.Hub.animationController.MoveState = value;
                Player.Hub.animationController.RpcReceiveState((byte)value);
            }
        }
        public string NickName
        {
            get => Player.Hub.nicknameSync.Network_myNickSync;
            set => Player.Hub.nicknameSync.Network_myNickSync = value;
        }
        public RoleType Role
        {
            get => Player.Hub.characterClassManager.CurClass;
            set => Player.Hub.characterClassManager.CurClass = value;
        }
        public Vector3 Position
        {
            get => Player.transform.position;
            set
            {
                Player.transform.position = value;
                Player.Hub.playerMovementSync.RealModelPosition = value;
            }
        }
        public ItemType CurrentItem
        {
            get => _currentItem;
            set
            {
                Player.Hub.inventory.NetworkCurItem = new InventorySystem.Items.ItemIdentifier(value, 0);
                _currentItem = value;
            }
        }
        public Vector3 Scale
        {
            get => Player.transform.localScale;
            set => Player.transform.localScale = value;
        }
        public Vector2 Rotation
        {
            get => Player.Rotation;
            set
            {
                Player.Rotation = value;
                Player.Hub.PlayerCameraReference.rotation = Quaternion.Euler(new Vector3(value.x, value.y, 90f));
            }
        }
        public void RotateToPosition(Vector3 pos)
        {
            var rot = Quaternion.LookRotation((pos - GameObject.transform.position).normalized);
            Rotation = new Vector2(rot.eulerAngles.x, rot.eulerAngles.y);
        }

        public FakePlayer(Vector3 pos, Quaternion rot, RoleType role = RoleType.ClassD, string nickname = "FakePlayer", bool godmode = false)
        {
            try
            {
                GameObject = UnityEngine.Object.Instantiate(NetworkManager.singleton.playerPrefab);

                Player = Server.GetPlayer(GameObject);
                Scale = Vector3.one;
                Position = pos;
                Rotation = new Vector2(rot.eulerAngles.x, rot.eulerAngles.y);
                Player.Hub.queryProcessor.NetworkPlayerId = QueryProcessor._idIterator;
                Player.Hub.queryProcessor._ipAddress = Server.Host.IPAddress;
                Role = role;
                NickName = nickname;
                Player.GodMode = godmode;
                Player.Health = Player.MaxHealth;
                Player.MaxHealth = Player.MaxHealth = Player.Hub.characterClassManager.Classes.SafeGet((int)Player.Role).maxHP;
                Player.RankName = "Admin";
                Player.RankColor = "red";
                Player.Hub.playerMovementSync.NetworkGrounded = true;
                RunSpeed = CharacterClassManager._staticClasses[(int)role].runSpeed;
                WalkSpeed = CharacterClassManager._staticClasses[(int)role].walkSpeed;
                Timing.RunCoroutine(Update());


                NetworkServer.Spawn(GameObject);
                Server.FakePlayers.Add(Player);
                PlayerManager.AddPlayer(GameObject, CustomNetworkManager.slots);
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
            UnityEngine.Object.Destroy(GameObject);
            Server.FakePlayers.Remove(Player);
        }

        private IEnumerator<float> Update()
        {
            for (; ; )
            {
                yield return Timing.WaitForSeconds(0.1f);
                try
                {
                    if (GameObject == null) yield break;
                    if (MoveDirection == MovementDirection.None) continue;

                    var speed = 0f;

                    switch (MoveState)
                    {
                        case PlayerMovementState.Sneaking:
                            speed = SneakSpeed;
                            break;

                        case PlayerMovementState.Sprinting:
                            speed = RunSpeed * Server.SprintSpeed;
                            break;

                        case PlayerMovementState.Walking:
                            speed = WalkSpeed * Server.WalkSpeed;
                            break;
                    }

                    switch (MoveDirection)
                    {
                        case MovementDirection.Forward:
                            var pos = Position + Player.Hub.PlayerCameraReference.forward / 10 * speed;

                            if (!Physics.Linecast(Position, pos, Player.MovementSync.CollidableSurfaces))
                            {
                                Player.MovementSync.OverridePosition(pos, 0f, true);
                                IsWallOnFront = false;
                            }
                            else IsWallOnFront = true;
                            break;

                        case MovementDirection.BackWards:
                            pos = Position - Player.Hub.PlayerCameraReference.forward / 10 * speed;

                            if (!Physics.Linecast(Position, pos, Player.MovementSync.CollidableSurfaces))
                            {
                                Player.MovementSync.OverridePosition(pos, 0f, true);
                                IsWallBehind = false;
                            }
                            else IsWallBehind = true;
                            break;

                        case MovementDirection.Right:
                            pos = Position + Quaternion.AngleAxis(90, Vector3.up) * Player.Hub.PlayerCameraReference.forward / 10 * speed;

                            if (!Physics.Linecast(Position, pos, Player.MovementSync.CollidableSurfaces))
                            {
                                Player.MovementSync.OverridePosition(pos, 0f, true);
                                IsWallOnRight = false;
                            }
                            else IsWallOnRight = true;
                            break;

                        case MovementDirection.Left:
                            pos = Position - Quaternion.AngleAxis(90, Vector3.up) * Player.Hub.PlayerCameraReference.forward / 10 * speed;

                            if (!Physics.Linecast(Position, pos, Player.MovementSync.CollidableSurfaces))
                            {
                                Player.MovementSync.OverridePosition(pos, 0f, true);
                                IsWallOnLeft = false;
                            }
                            else IsWallOnLeft = true;
                            break;
                    }

                    if (IsWallOnLeft || IsWallOnRight || IsWallBehind || IsWallOnFront) MoveDirection = MovementDirection.None;
                }
                catch (Exception e)
                {
                    Log.Error($"FakePlayer Update Failed:\n{e}");
                }
            }
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
