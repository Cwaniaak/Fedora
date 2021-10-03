using Sapiox.Events.EventArgs;

namespace Sapiox.Events.Handlers
{
    public class Player
    {
        public static event EventHandler.OnSapioxEvent<PlayerJoinEventArgs> Join;
        public static event EventHandler.OnSapioxEvent<PlayerLeaveEventArgs> Leave;

        public static void OnJoin(API.Player ply, ref string Nickname)
        {
            var ev = new PlayerJoinEventArgs{Player = ply, NickName = Nickname};
            Join?.Invoke(ev);
        }

        public static void OnLeave(API.Player ply)
        {
            var ev = new PlayerLeaveEventArgs {Player = ply};
            Leave?.Invoke(ev);
        }
    }
}