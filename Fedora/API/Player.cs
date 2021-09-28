using UnityEngine;

namespace Fedora.API
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
        public readonly ReferenceHub Hub;
    }
}