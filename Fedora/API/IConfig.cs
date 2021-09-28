using System;

namespace Fedora.API
{
    public interface IConfig
    {
        bool Load { get; set; }
    }
}