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
    public static class SapioxManager
    {
        public static bool IsLoaded = false;
        private static string _sapioxDirectory;
        private static string _pluginDirectory;
        private static string _configDirectory;
        public static List<IPlugin> Plugins = new List<IPlugin>();

        public static string SapioxDirectory
        {
            get
            {
                if (!Directory.Exists(_sapioxDirectory))
                    Directory.CreateDirectory(_sapioxDirectory);

                return _sapioxDirectory;
            }
            private set => _sapioxDirectory = value;
        }

        public static string PluginDirectory
        {
            get
            {
                if (!Directory.Exists(_pluginDirectory))
                    Directory.CreateDirectory(_pluginDirectory);

                return _pluginDirectory;
            }
            private set => _pluginDirectory = value;
        }

        public static string ConfigDirectory
        {
            get
            {
                if (!Directory.Exists(_configDirectory))
                    Directory.CreateDirectory(_configDirectory);
                
                return _configDirectory;
            }
            private set => _configDirectory = value;
        }

        static void PatchMethods()
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

        public static void LoadSapiox()
        {
            if (IsLoaded) return;

            var localpath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Sapiox");
            SapioxDirectory = Directory.Exists(localpath) ? localpath : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Sapiox");
            ConfigDirectory = Path.Combine(SapioxDirectory, "configs");
            PluginDirectory = Path.Combine(SapioxDirectory, "plugins");
            CustomNetworkManager.Modded = true;
            BuildInfoCommand.ModDescription = string.Join(
                "\n",
                AppDomain.CurrentDomain.GetAssemblies().Select(a => $"{a.GetName().Name} - Version {a.GetName().Version.ToString(3)}"));
            try
            {
                PatchMethods();
                ActivatePlugins();
                IsLoaded = true;
            }
            catch (Exception e)
            {
                Log.Error($"Sapiox.Loader: Error:\n{e}");
                return;
            }

            Log.Info("Sapiox.Loader: Sapiox is now Loaded!");
        }
        
        static void ActivatePlugins()
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
                    Plugins.Add(plugin);
                }
                catch (Exception e)
                {
                    Log.Error($"Sapiox.Loader: {infoTypePair.Value.Key.Assembly.GetName().Name} could not be activated!\n{e}");
                }
            }
            LoadPlugins();
        }

        public static string GetPluginDirectory(PluginInfo infos)
        {
            return Path.Combine(PluginDirectory, infos.Name);
        }

        public static void LoadPlugins()
        {
            foreach (var plugin in Plugins)
            {
                try
                {
                    plugin.Load();
                }
                catch (Exception e)
                {
                    Log.Error($"Sapiox.Loader: {plugin.Info.Name} Loading failed!\n{e}");
                }
            }
        }
    }
}