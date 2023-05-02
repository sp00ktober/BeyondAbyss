using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using Winch.Core;

namespace BeyondAbyss.Singletons
{
    internal sealed class ConfigManager
    {
        private static readonly ConfigManager _instance = new ConfigManager();
        public static ConfigManager INSTANCE
        {
            get
            {
                return _instance;
            }
        }
        private string cfgPath { get; set; }
        private string cfgPath_DredgingMinigame { get; set; }
        private string cfgPath_FishingMinigame { get; set; }

        public Dictionary<HarvestDifficulty, HarvestDifficultyConfigData> DredgingDifficultyConfigs { get; private set; }
        public Dictionary<HarvestDifficulty, HarvestDifficultyConfigData> FishingDifficultyConfigs { get; private set; }
        public bool HasLoadedHarvestDifficulty
        {
            get
            {
                return
                    DredgingDifficultyConfigs.ContainsKey(HarvestDifficulty.VERY_EASY) &&
                    DredgingDifficultyConfigs.ContainsKey(HarvestDifficulty.EASY) &&
                    DredgingDifficultyConfigs.ContainsKey(HarvestDifficulty.MEDIUM) &&
                    DredgingDifficultyConfigs.ContainsKey(HarvestDifficulty.HARD) &&
                    DredgingDifficultyConfigs.ContainsKey(HarvestDifficulty.VERY_HARD) &&
                    FishingDifficultyConfigs.ContainsKey(HarvestDifficulty.VERY_EASY) &&
                    FishingDifficultyConfigs.ContainsKey(HarvestDifficulty.EASY) &&
                    FishingDifficultyConfigs.ContainsKey(HarvestDifficulty.MEDIUM) &&
                    FishingDifficultyConfigs.ContainsKey(HarvestDifficulty.HARD) &&
                    FishingDifficultyConfigs.ContainsKey(HarvestDifficulty.VERY_HARD);
            }
        }
        public bool SleepingOnLand { get; set; }
        static ConfigManager() { }
        private ConfigManager()
        {
            cfgPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Config/");
            cfgPath_DredgingMinigame = Path.Combine(cfgPath, "Dredging_Minigame/");
            cfgPath_FishingMinigame = Path.Combine(cfgPath, "Fishing_Minigame/");

            DredgingDifficultyConfigs = new Dictionary<HarvestDifficulty, HarvestDifficultyConfigData>();
            FishingDifficultyConfigs = new Dictionary<HarvestDifficulty, HarvestDifficultyConfigData>();

            LoadMinigameConfig();
        }

        private bool EnsureMinigameConfigDirExists()
        {
            try
            {
                if (!Directory.Exists(cfgPath_DredgingMinigame))
                {
                    WinchCore.Log.Info($"Creating dir {cfgPath_DredgingMinigame}");
                    Directory.CreateDirectory(cfgPath_DredgingMinigame);
                }
                if (!Directory.Exists(cfgPath_FishingMinigame))
                {
                    WinchCore.Log.Info($"Creating dir {cfgPath_FishingMinigame}");
                    Directory.CreateDirectory(cfgPath_FishingMinigame);
                }
            }
            catch(Exception e)
            {
                WinchCore.Log.Error(e.Message);
                WinchCore.Log.Error(e.StackTrace);

                return false;
            }

            return true;
        }
        private void LoadMinigameConfig()
        {
            WinchCore.Log.Info("Trying to load custom config...");
            if (!EnsureMinigameConfigDirExists()) return;

            string[] cfgs_dredging = Directory.GetFiles(cfgPath_DredgingMinigame).Where(f => f.EndsWith(".json")).ToArray();
            string[] cfgs_fishing = Directory.GetFiles(cfgPath_FishingMinigame).Where(f => f.EndsWith(".json")).ToArray();

            foreach(string file in cfgs_dredging)
            {
                WinchCore.Log.Info($"Parsing {file}");

                string content = File.ReadAllText(file);
                JObject jobj = JObject.Parse(content);

                if(jobj != null)
                {
                    HarvestDifficultyConfigData hconfig = jobj.ToObject<HarvestDifficultyConfigData>();
                    if(hconfig != null)
                    {
                        AddDredgingDifficulty(hconfig);
                    }
                }
            }
            foreach (string file in cfgs_fishing)
            {
                WinchCore.Log.Info($"Parsing {file}");

                string content = File.ReadAllText(file);
                JObject jobj = JObject.Parse(content);

                if (jobj != null)
                {
                    HarvestDifficultyConfigData hconfig = jobj.ToObject<HarvestDifficultyConfigData>();
                    if (hconfig != null)
                    {
                        AddFishingDifficulty(hconfig);
                    }
                }
            }
        }
        public void SaveMinigameConfig()
        {
            WinchCore.Log.Info("Trying to save custom config...");
            if (!HasLoadedHarvestDifficulty) return;
            if (!EnsureMinigameConfigDirExists()) return;

            foreach(KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> kvPair in DredgingDifficultyConfigs)
            {
                JObject jobj = (JObject)JToken.FromObject(kvPair.Value);
                if(jobj != null)
                {
                    string file = Path.Combine(cfgPath_DredgingMinigame, kvPair.Value.name.Replace(" ", "_") + ".json");
                    WinchCore.Log.Info($"Saving to {file}");

                    File.WriteAllText(file, jobj.ToString());
                }
            }
            foreach (KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> kvPair in FishingDifficultyConfigs)
            {
                JObject jobj = (JObject)JToken.FromObject(kvPair.Value);
                if (jobj != null)
                {
                    string file = Path.Combine(cfgPath_FishingMinigame, kvPair.Value.name.Replace(" ", "_") + ".json");
                    WinchCore.Log.Info($"Saving to {file}");

                    File.WriteAllText(file, jobj.ToString());
                }
            }
        }
        public void AddDredgingDifficulty(HarvestDifficultyConfigData hconfig)
        {
            if (hconfig == null) return;

            switch (hconfig.name)
            {
                case "Dredging 1 - Very Easy":
                    DredgingDifficultyConfigs.Add(HarvestDifficulty.VERY_EASY, hconfig);
                    break;
                case "Dredging 2 - Easy":
                    DredgingDifficultyConfigs.Add(HarvestDifficulty.EASY, hconfig);
                    break;
                case "Dredging 3 - Medium":
                    DredgingDifficultyConfigs.Add(HarvestDifficulty.MEDIUM, hconfig);
                    break;
                case "Dredging 4 - Hard":
                    DredgingDifficultyConfigs.Add(HarvestDifficulty.HARD, hconfig);
                    break;
                case "Dredging 5 - Very Hard":
                    DredgingDifficultyConfigs.Add(HarvestDifficulty.VERY_HARD, hconfig);
                    break;
            }
        }
        public void AddFishingDifficulty(HarvestDifficultyConfigData hconfig)
        {
            if (hconfig == null) return;

            switch (hconfig.name)
            {
                case "Fishing 1 - Very Easy":
                    FishingDifficultyConfigs.Add(HarvestDifficulty.VERY_EASY, hconfig);
                    break;
                case "Fishing 2 - Easy":
                    FishingDifficultyConfigs.Add(HarvestDifficulty.EASY, hconfig);
                    break;
                case "Fishing 3 - Medium":
                    FishingDifficultyConfigs.Add(HarvestDifficulty.MEDIUM, hconfig);
                    break;
                case "Fishing 4 - Hard":
                    FishingDifficultyConfigs.Add(HarvestDifficulty.HARD, hconfig);
                    break;
                case "Fishing 5 - Very Hard":
                    FishingDifficultyConfigs.Add(HarvestDifficulty.VERY_HARD, hconfig);
                    break;
            }
        }
    }
}
