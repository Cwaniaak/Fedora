using System;

namespace Sapiox.Events.Handlers
{
    public class EventHandler
    {
        public delegate void SapioxEvent<TEventArgs>(TEventArgs ev) where TEventArgs : System.EventArgs;
    }
}