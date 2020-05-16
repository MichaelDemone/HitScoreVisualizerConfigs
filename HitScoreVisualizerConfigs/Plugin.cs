using HitScoreVisualizerConfigs.UI;
using IPA;
using IPA.Config;
using IPA.Config.Stores;
using System.IO;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace HitScoreVisualizerConfigs
{

    [Plugin(RuntimeOptions.SingleStartInit)]
    public class Plugin
    {
        internal static Plugin instance { get; private set; }
        internal static string Name => "HitScoreVisualizerConfigs";

        public static readonly string OriginalHSVConfigPath = $"{IPA.Utilities.UnityGame.UserDataPath}{Path.DirectorySeparatorChar}HitScoreVisualizerConfig.json";
        public static readonly string HSVConfigsDirectoryPath = $"{IPA.Utilities.UnityGame.UserDataPath}{Path.DirectorySeparatorChar}HSVConfigs{Path.DirectorySeparatorChar}";
        public static string[] HSVConfigPaths;

        [Init]
        public void InitWithConfig(IPALogger logger, Config conf)
        {
            instance = this;
            Logger.log = logger;
            Logger.log.Debug("Logger initialized.");

            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
            Logger.log.Debug("Config loaded");

            if (!Directory.Exists(HSVConfigsDirectoryPath))
            {
                Logger.log.Info("Creating HSV config directory");
                Directory.CreateDirectory(HSVConfigsDirectoryPath);
            }

            bool loaded = false;
            HSVConfigPaths = Directory.GetFiles(HSVConfigsDirectoryPath);

            foreach(var hsvConfigPath in HSVConfigPaths)
            {
                if(Path.GetFileNameWithoutExtension(hsvConfigPath) == Configuration.PluginConfig.Instance.DefaultProfile)
                {
                    Logger.log.Info("Loading default profile");
                    loaded = true;
                    Load(hsvConfigPath);
                }
            }

            if(!loaded)
            {
                Logger.log.Debug("Default profile not found");
                if(HSVConfigPaths.Length > 0)
                {
                    Logger.log.Debug($"Loading first profile");
                    Load(HSVConfigPaths[0]);
                } 
                else
                {
                    
                    if (File.Exists(OriginalHSVConfigPath))
                    {
                        Logger.log.Debug($"Copying HSV config into HSV config folder and loading it");
                        string newLocation = $"{HSVConfigsDirectoryPath}Default.json";
                        File.Copy(OriginalHSVConfigPath, newLocation);
                        Load(newLocation);
                        HSVConfigPaths = new string[]
                        {
                            "Default"
                        };
                    } 
                    else
                    {
                        Logger.log.Warn($"No HSV config files found");
                    }
                }
            }
        }
        
        public void Load(string path)
        {
            Logger.log.Debug($"Loading HSV config from path: {path}");
            Configuration.PluginConfig.Instance.DefaultProfile = Path.GetFileNameWithoutExtension(path);
            File.Copy(path, OriginalHSVConfigPath, true);
            HitScoreVisualizer.Config.load();
        }

        [OnStart]
        public void OnApplicationStart()
        {
            var obj = new GameObject("HitScoreVisualizerConfigsController");
            obj.AddComponent<HitScoreVisualizerConfigsController>();
            obj.AddComponent<SelectionController>();

            BeatSaberMarkupLanguage.GameplaySetup.GameplaySetup.instance.AddTab("Hit Score Configs", "HitScoreVisualizerConfigs.UI.BSML.SelectionUI.bsml", SelectionController.Instance);
        }
    }
}
