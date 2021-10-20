using Sapiox.API;

namespace Sapiox.Example
{
    public sealed class Config : IConfig
    {
        public bool Load { get; set; } = true;
        public string Test { get; set; } = "yo";
    }
}