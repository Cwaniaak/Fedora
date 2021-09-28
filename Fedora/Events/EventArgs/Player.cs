using Fedora.API;

namespace Fedora.Events.EventArgs
{
    public class PlayerJoinEventArgs : System.EventArgs
    {
        public Player Player { internal set; get; }
        
        public string NickName { set; get; }
    }

    public class PlayerLeaveEventArgs : System.EventArgs
    {
        public Player Player { get; internal set; }
    }
}