using System;

namespace Sapiox.Events.Handlers
{
    public class Server
    {
        public static event Action WaitingForPlayers;
        public static event Action RoundEnd;


        public static void OnWaitingForPlayers() => WaitingForPlayers?.Invoke();
        public static void OnRoundEnd() => RoundEnd?.Invoke();
    }
}