using Interactables.Interobjects.DoorUtils;
using Sapiox.API;

namespace Sapiox.Events.EventArgs
{
    public class PlayerJoinEventArgs : System.EventArgs
    {
        public Player Player { get; internal set; }
        public string NickName { get; set; }
    }

    public class PlayerLeaveEventArgs : System.EventArgs
    {
        public Player Player { get; internal set; }
    }

    public class PlayerBanEventArgs : System.EventArgs
    {
        public Player Target { get; internal set; }
        public Player Issuer { get; internal set; }
        public string Reason { get; set; }
        public bool IsGlobalBan { get; set; }
        public long Duration { get; set; }
    }

    public class PlayerKickEventArgs : System.EventArgs
    {
        public Player Target { get; internal set; }
        public Player Issuer { get; internal set; }
        public string Reason { get; set; }
    }
    public class InteractDoorEventArgs : System.EventArgs
    {
        public Player Player { get; internal set; }
        public DoorVariant Door { get; internal set; }
        public bool Allow { get; set; }
    }
}