using BeyondAbyss.Singletons;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Winch.Core;

namespace BeyondAbyss.Patches.Dynamic
{
    [HarmonyPatch(typeof(HarvestMinigameView))]
    internal class HarvestMinigameView_Patch
    {
        private static bool didOverwriteGameValuesWithConfig = false;

        [HarmonyPrefix]
        [HarmonyPatch(typeof(HarvestMinigameView), "RefreshHarvestTarget")]
        public static void RefreshHarvestTarget_Prefix()
        {
            try
            {
                if (ConfigManager.INSTANCE.HasLoadedHarvestDifficulty && !didOverwriteGameValuesWithConfig)
                {
                    // custom values defined in settings, overwrite game values.
                    WinchCore.Log.Info("Overwriting game defaults with custom ones.");

                    GameManager.Instance.GameConfigData.DredgingDifficultyConfigs[HarvestDifficulty.VERY_EASY] = ConfigManager.INSTANCE.DredgingDifficultyConfigs[HarvestDifficulty.VERY_EASY];
                    GameManager.Instance.GameConfigData.DredgingDifficultyConfigs[HarvestDifficulty.EASY] = ConfigManager.INSTANCE.DredgingDifficultyConfigs[HarvestDifficulty.EASY];
                    GameManager.Instance.GameConfigData.DredgingDifficultyConfigs[HarvestDifficulty.MEDIUM] = ConfigManager.INSTANCE.DredgingDifficultyConfigs[HarvestDifficulty.MEDIUM];
                    GameManager.Instance.GameConfigData.DredgingDifficultyConfigs[HarvestDifficulty.HARD] = ConfigManager.INSTANCE.DredgingDifficultyConfigs[HarvestDifficulty.HARD];
                    GameManager.Instance.GameConfigData.DredgingDifficultyConfigs[HarvestDifficulty.VERY_HARD] = ConfigManager.INSTANCE.DredgingDifficultyConfigs[HarvestDifficulty.VERY_HARD];

                    GameManager.Instance.GameConfigData.FishingDifficultyConfigs[HarvestDifficulty.VERY_EASY] = ConfigManager.INSTANCE.FishingDifficultyConfigs[HarvestDifficulty.VERY_EASY];
                    GameManager.Instance.GameConfigData.FishingDifficultyConfigs[HarvestDifficulty.EASY] = ConfigManager.INSTANCE.FishingDifficultyConfigs[HarvestDifficulty.EASY];
                    GameManager.Instance.GameConfigData.FishingDifficultyConfigs[HarvestDifficulty.MEDIUM] = ConfigManager.INSTANCE.FishingDifficultyConfigs[HarvestDifficulty.MEDIUM];
                    GameManager.Instance.GameConfigData.FishingDifficultyConfigs[HarvestDifficulty.HARD] = ConfigManager.INSTANCE.FishingDifficultyConfigs[HarvestDifficulty.HARD];
                    GameManager.Instance.GameConfigData.FishingDifficultyConfigs[HarvestDifficulty.VERY_HARD] = ConfigManager.INSTANCE.FishingDifficultyConfigs[HarvestDifficulty.VERY_HARD];

                    didOverwriteGameValuesWithConfig = true;
                }
                else
                {
                    // no custom values found, dumping game default ones into config.
                    WinchCore.Log.Info("No config file found or loaded incompletely, dumping defaults to config dir.");

                    foreach (KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> kvPair in GameManager.Instance.GameConfigData.DredgingDifficultyConfigs)
                    {
                        ConfigManager.INSTANCE.AddFishingDifficulty(kvPair.Value);
                    }
                    foreach (KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> kvPair in GameManager.Instance.GameConfigData.FishingDifficultyConfigs)
                    {
                        ConfigManager.INSTANCE.AddFishingDifficulty(kvPair.Value);
                    }

                    ConfigManager.INSTANCE.SaveMinigameConfig();
                }
            }
            catch(Exception e)
            {
                WinchCore.Log.Error(e.Message);
                WinchCore.Log.Error(e.StackTrace);
            }
        }
    }
}
