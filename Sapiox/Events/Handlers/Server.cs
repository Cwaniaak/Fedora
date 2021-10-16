using System;

namespace Sapiox.Events.Handlers
{
    public class Server
    {
        public static event Action WaitingForPlayers;
        
        public static void OnWaitingForPlayers() => WaitingForPlayers?.Invoke();
    }
}