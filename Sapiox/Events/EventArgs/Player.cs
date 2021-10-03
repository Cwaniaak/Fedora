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
}