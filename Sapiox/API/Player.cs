using RemoteAdmin;
using UnityEngine;

namespace Sapiox.API
{
    public class Player : MonoBehaviour
    {
        internal Player()
        {
            ReferenceHub Hub = GetComponent<ReferenceHub>();
        }

        public void RemoveDisplayInfo(PlayerInfoArea playerInfo) => Hub.nicknameSync.Network_playerInfoToShow &= ~playerInfo;

        public void AddDisplayInfo(PlayerInfoArea playerInfo) => Hub.nicknameSync.Network_playerInfoToShow |= playerInfo;
        
        public void Kick(string message) => ServerConsole.Disconnect(gameObject, message);
        public void Ban(int duration, string reason, string issuer = "Plugin") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(gameObject, duration, reason, issuer);
        public void Broadcast(ushort time, string message) => GetComponent<global::Broadcast>().TargetAddElement(Hub.characterClassManager.Connection, message, time, new global::Broadcast.BroadcastFlags());
        
        public Team Team => Hub.characterClassManager.CurRole.team;
        public Faction Faction => Hub.characterClassManager.Faction;
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
        public bool IsHost => Hub.isDedicatedServer;
        public string IPAddress => Hub.queryProcessor._ipAddress;
        public string NickName => Hub.nicknameSync.Network_myNickSync;

        public readonly ReferenceHub Hub;
    }
}