using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CommandSystem.Commands;
using CommandSystem.Commands.Shared;
using Sapiox.API;
using HarmonyLib;

namespace Sapiox
{
    public class SapioxManager
    {
        private static bool IsLoaded = false;
        private string _SapioxDirectory;
        private string _PluginDirectory;
        private readonly List<IPlugin> _plugins = new List<IPlugin>();

        public string SapioxDirectory
        {
            get
            {
                if (!Directory.Exists(_SapioxDirectory))
                    Directory.CreateDirectory(_SapioxDirectory);

                return _SapioxDirectory;
            }
            private set => _SapioxDirectory = value;
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
            new SapioxManager();
        }

        private void PatchMethods()
        {
            try
            {
                var instance = new Harmony("Sapiox.patches");
                instance.PatchAll();
                Log.Info("Harmony Patching was sucessfully!");
            }
            catch (Exception e)
            {
                Log.Error($"Harmony Patching threw an error:\n\n {e}");
            }
        }

        internal SapioxManager()
        {
            var localpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sapiox");
            SapioxDirectory = Directory.Exists(localpath)
                ? localpath
                : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sapiox");
            PluginDirectory = Path.Combine(SapioxDirectory, "plugins");

            CustomNetworkManager.Modded = true;
            BuildInfoCommand.ModDescription = string.Join(
                "\n",
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith("Sapiox.", StringComparison.OrdinalIgnoreCase))
                    .Select(a => $"{a.GetName().Name} - Version {a.GetName().Version.ToString(3)}"));
            PatchMethods();
            ActivatePlugins();
            Log.Info("Sapiox.Loader: Sapiox is now Loaded!");
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
                    Log.Error($"Sapiox.Loader: Loading Assembly of {pluginpath} failed!\n{e}");
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
                    Log.Error($"Sapiox.Loader: {infoTypePair.Value.Key.Assembly.GetName().Name} could not be activated!\n{e}");
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
                    Log.Info($"Sapiox.Loader: Loaded plugin {plugin.Info.Name}@{plugin.Info.Version}!");
                }
                catch (Exception e)
                {
                    Log.Error($"Sapiox.Loader: {plugin.Info.Name} Loading failed!\n{e}");
                }
            }
        }
    }
}