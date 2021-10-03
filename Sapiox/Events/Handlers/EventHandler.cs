namespace Sapiox.Events.Handlers
{
    public class EventHandler
    {
        public delegate void OnSapioxEvent<TEventArgs>(TEventArgs ev) where TEventArgs : System.EventArgs;
    }
}