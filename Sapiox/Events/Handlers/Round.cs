using System;

namespace Sapiox.Events.Handlers
{
    public class Round
    {
        public static event Action WaitingForPlayers;
        public static event Action End;
        public static event Action Restart;
        public static event Action Start;

        public static void OnWaitingForPlayers() => WaitingForPlayers?.Invoke();
        public static void OnRoundStart() => Start?.Invoke();
        public static void OnRoundEnd() => End?.Invoke();
        public static void OnRoundRestart() => Restart?.Invoke();
    }
}