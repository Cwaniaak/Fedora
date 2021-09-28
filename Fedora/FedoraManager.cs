using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandSystem.Commands;
using CommandSystem.Commands.Shared;
using Fedora.API;
using HarmonyLib;

namespace Fedora
{
    public class FedoraManager
    {
        private static bool IsLoaded = false;
        private string _FedoraDirectory;
        private string _PluginDirectory;
        private readonly List<IPlugin> _plugins = new List<IPlugin>();

        public string FedoraDirectory
        {
            get
            {
                if (!Directory.Exists(_FedoraDirectory))
                    Directory.CreateDirectory(_FedoraDirectory);

                return _FedoraDirectory;
            }
            private set => _FedoraDirectory = value;
        }

        public string PluginDirectory
        {
            get
            {
                if (!Directory.Exists(_PluginDirectory))
                    Directory.CreateDirectory(_PluginDirectory);

                return _PluginDirectory;
            }
            private set => _PluginDirectory = value;
        }

        public static void Init()
        {
            if (IsLoaded) return;
            IsLoaded = true;
            new FedoraManager();
        }

        private void PatchMethods()
        {
            try
            {
                var instance = new Harmony("fedora.patches");
                instance.PatchAll();
                Log.Info("Harmony Patching was sucessfully!");
            }
            catch (Exception e)
            {
                Log.Error($"Harmony Patching threw an error:\n\n {e}");
            }
        }

        internal FedoraManager()
        {
            var localpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fedora");
            FedoraDirectory = Directory.Exists(localpath)
                ? localpath
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Fedora");
            PluginDirectory = Path.Combine(FedoraDirectory, "plugins");

            CustomNetworkManager.Modded = true;
            BuildInfoCommand.ModDescription = string.Join(
                "\n",
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith("Fedora.", StringComparison.OrdinalIgnoreCase))
                    .Select(a => $"{a.GetName().Name} - Version {a.GetName().Version.ToString(3)}"));
            PatchMethods();
            ActivatePlugins();
            Log.Info("Fedora.Loader: Fedora is now Loaded!");
        }
        
        internal void ActivatePlugins()
        {
            var paths = Directory.GetFiles(PluginDirectory, "*.dll").ToList();

            var dictionary = new Dictionary<PluginInfo, KeyValuePair<Type, List<Type>>>();

            foreach (var pluginpath in paths)
            {
                try
                {
                    var assembly = Assembly.Load(File.ReadAllBytes(pluginpath));
                    foreach (var type in assembly.GetTypes())
                    {
                        if (!typeof(IPlugin).IsAssignableFrom(type))
                            continue;

                        var infos = type.GetCustomAttribute<PluginInfo>();

                        if (infos == null)
                        {
                            infos = new PluginInfo();
                        }

                        var allTypes = assembly.GetTypes().ToList();
                        allTypes.Remove(type);
                        dictionary.Add(infos, new KeyValuePair<Type, List<Type>>(type, allTypes));
                        break;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Fedora.Loader: Loading Assembly of {pluginpath} failed!\n{e}");
                }
            }

            foreach (var infoTypePair in dictionary)
            {
                try
                {
                    IPlugin plugin = (IPlugin) Activator.CreateInstance(infoTypePair.Value.Key);
                    plugin.Info = infoTypePair.Key;
                    plugin.PluginDirectory = GetPluginDirectory(plugin.Info);
                    _plugins.Add(plugin);
                }
                catch (Exception e)
                {
                    Log.Error($"Fedora.Loader: {infoTypePair.Value.Key.Assembly.GetName().Name} could not be activated!\n{e}");
                }
            }

            LoadPlugins();
        }

        public string GetPluginDirectory(PluginInfo infos)
        {
            return Path.Combine(PluginDirectory, infos.Name);
        }

        private void LoadPlugins()
        {
            foreach (var plugin in _plugins)
            {
                try
                {
                    plugin.Load();
                    Log.Info($"Fedora.Loader: Loaded plugin {plugin.Info.Name}@{plugin.Info.Version}!");
                }
                catch (Exception e)
                {
                    Log.Error($"Fedora.Loader: {plugin.Info.Name} Loading failed!\n{e}");
                }
            }
        }
    }
}