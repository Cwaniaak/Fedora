using Sapiox.Events.EventArgs;

namespace Sapiox.Events.Handlers
{
    public class Player
    {
        public static event EventHandler.OnSapioxEvent<PlayerJoinEventArgs> Join;
        public static event EventHandler.OnSapioxEvent<PlayerLeaveEventArgs> Leave;
        public static event EventHandler.OnSapioxEvent<PlayerBanEventArgs> Ban; 

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

        public static void OnBan(API.Player target, ref long duration, ref bool globalban, ref string reason)
        {
            var ev = new PlayerBanEventArgs {Target = target, IsGlobalBan = globalban, Duration = duration, Reason = reason};
            Ban?.Invoke(ev);
        }
    }
}