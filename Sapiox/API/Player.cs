using Hints;
using InventorySystem.Items;
using RemoteAdmin;
using UnityEngine;

namespace Sapiox.API
{
    public class Player : MonoBehaviour
    {

        public void RemoveDisplayInfo(PlayerInfoArea playerInfo) => Hub.nicknameSync.Network_playerInfoToShow &= ~playerInfo;

        public void AddDisplayInfo(PlayerInfoArea playerInfo) => Hub.nicknameSync.Network_playerInfoToShow |= playerInfo;
        
        public void Kick(string message) => ServerConsole.Disconnect(gameObject, message);
        public void Ban(int duration, string reason, string issuer = "Plugin") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(gameObject, duration, reason, issuer);
        public void Broadcast(ushort time, string message) => GetComponent<global::Broadcast>().TargetAddElement(Hub.characterClassManager.connectionToClient, message, time, new global::Broadcast.BroadcastFlags());
        public void Hint(string message, float duration = 5f) => Hub.hints.Show(new TextHint(message, null, HintEffectPresets.FadeInAndOut(duration), duration));

        public Team Team => Hub.characterClassManager.CurRole.team;
        public Faction Faction => Hub.characterClassManager.Faction;
        public int Id => Hub.playerId;
        public RoleType Role
        {
            get => Hub.characterClassManager.CurClass;
            set => Hub.characterClassManager.SetPlayersClass(value, gameObject, CharacterClassManager.SpawnReason.None);
        }
        public string UserId
        {
            get => Hub.characterClassManager.UserId;
            set => Hub.characterClassManager.UserId = value;
        }
        public PlayerMovementState MoveState
        {
            get => Hub.animationController.MoveState;
            set => Hub.animationController.UserCode_CmdChangeSpeedState((byte)value);
        }
        public bool IsHost => Hub.isDedicatedServer;
        public string IPAddress => Hub.networkIdentity.connectionToClient.address;
        public bool IsUsingRadio => Radio.UsingRadio;
        public Radio Radio => Hub.radio;

        public string RankName
        {
            get => Hub.serverRoles.Network_myText;
            set => Hub.serverRoles.SetText(value);
        }
        
        public string RankColor
        {
            get => Hub.serverRoles.Network_myColor;
            set => Hub.serverRoles.SetColor(value);
        }
        
        public bool IsUsingVoiceChat => Radio.UsingVoiceChat;

        public string NickName => Hub.nicknameSync.Network_myNickSync;

           public string DisplayName
        {
            get => Hub.nicknameSync.Network_displayName;
            set => Hub.nicknameSync.Network_displayName = value;
        }
        
        public ItemIdentifier CurrentItem
        {
            get => Hub.inventory.NetworkCurItem;
            set => Hub.inventory.NetworkCurItem = value;
        }
        
        public Transform Camera
        {
            get => Hub.PlayerCameraReference;
            set => Hub.PlayerCameraReference = value;
        }
        public void PlayFootStep(float volume, bool running) => Hub.footstepSync.PlayFootstepSound(volume, running);

        public ReferenceHub Hub => GetComponent<ReferenceHub>();
    }
}