using Fedora.API;

namespace Fedora.Example
{
    public class Config : IConfig
    {
        public bool Load { get; set; } = true;
    }
}