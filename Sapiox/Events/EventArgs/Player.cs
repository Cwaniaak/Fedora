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
        public string Reason { get; set; }
        public bool IsGlobalBan { get; set; }
        public long Duration { get; set; }
    }
}