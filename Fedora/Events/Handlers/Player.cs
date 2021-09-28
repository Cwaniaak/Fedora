using Fedora.Events.EventArgs;

namespace Fedora.Events.Handlers
{
    public class Player
    {
        public static event EventHandler.OnFedoraEvent<PlayerJoinEventArgs> Join;
        public static event EventHandler.OnFedoraEvent<PlayerLeaveEventArgs> Leave;
    }
}