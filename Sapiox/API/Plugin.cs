using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Sapiox.API
{    
    public interface IPlugin
    {
        PluginInfo Info { get; set; }
        IConfig config { get; }
        
        string PluginDirectory { get; set; }

        void Load();

        void ReloadConfigs();
    }

    public abstract class Plugin : IPlugin
    {
        public PluginInfo Info { get; set; }

        public virtual IConfig config { get; }

        private string _pluginDirectory;

        public virtual void Load() => Log.Info($"{Info.Name} by {Info.Author} has been enabled!");

        public virtual void ReloadConfigs()
        {
        }

        public string PluginDirectory
        {
            get
            {
                if (_pluginDirectory == null)
                    return null;

                if (!Directory.Exists(_pluginDirectory))
                    Directory.CreateDirectory(_pluginDirectory);

                return _pluginDirectory;
            }
            set => _pluginDirectory = value;
        }
    }
}