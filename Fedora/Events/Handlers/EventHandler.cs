namespace Fedora.Events.Handlers
{
    public class EventHandler
    {
        public delegate void OnFedoraEvent<TEvent>(TEvent ev) where TEvent : System.EventArgs;
    }
}