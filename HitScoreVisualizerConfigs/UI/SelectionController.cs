using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.ViewControllers;
using System.Collections.Generic;

namespace HitScoreVisualizerConfigs.UI
{
    [ViewDefinition("HitScoreVisualizerConfigs.UI.BSML.SelectionUI.bsml")]
    class SelectionController : BSMLAutomaticViewController
    {
        public static SelectionController Instance;
        void Awake()
        {
            Instance = this;
            selected = Configuration.PluginConfig.Instance.DefaultProfile;
        }

        [UIValue("interactable")]
        public bool interactable = true;

        [UIAction("config-selected")]
        public void ConfigSelected()
        {
            Logger.log.Debug("New config selected");

        }

        [UIValue("list-options")]
        public List<object> contents
        {
            get
            {
                List<object> list = new List<object>();
                string selected = Configuration.PluginConfig.Instance.DefaultProfile;

                foreach (var path in Plugin.HSVConfigPaths)
                {
                    string configName = System.IO.Path.GetFileNameWithoutExtension(path);
                    list.Add(configName);
                }
                if(list.Count == 0)
                {
                    interactable = false;
                    list.Add("NoConfigsFound");
                }
                return list;
            }
        }

        [UIValue("selected")]
        private string selected;

        [UIAction("value-change")]
        public void OnChange(string val)
        {
            Logger.log.Debug($"Value change. New value: {selected}");
            var path = Plugin.HSVConfigsDirectoryPath + val + ".json";
            Plugin.instance.Load(path);
        }
    }
}
