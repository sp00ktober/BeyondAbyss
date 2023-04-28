using BeyondAbyss.Singletons;
using FluffyUnderware.DevTools.Extensions;
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

                    PatchGameValues();
                    didOverwriteGameValuesWithConfig = true;
                }
                else
                {
                    // no custom values found, dumping game default ones into config.
                    WinchCore.Log.Info("No config file found or loaded incompletely, dumping mod defaults to config dir.");

                    foreach (KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> kvPair in GameManager.Instance.GameConfigData.DredgingDifficultyConfigs)
                    {
                        ConfigManager.INSTANCE.AddDredgingDifficulty(GetModDefaultValue(true, kvPair.Key, kvPair.Value));
                    }
                    foreach (KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> kvPair in GameManager.Instance.GameConfigData.FishingDifficultyConfigs)
                    {
                        ConfigManager.INSTANCE.AddFishingDifficulty(GetModDefaultValue(false, kvPair.Key, kvPair.Value));
                    }

                    ConfigManager.INSTANCE.SaveMinigameConfig();

                    PatchGameValues();
                    didOverwriteGameValuesWithConfig = true;
                }
            }
            catch(Exception e)
            {
                WinchCore.Log.Error(e.Message);
                WinchCore.Log.Error(e.StackTrace);
            }
        }

        private static void PatchGameValues()
        {
            Dictionary<HarvestDifficulty, HarvestDifficultyConfigData> customDredgingConfig = ConfigManager.INSTANCE.DredgingDifficultyConfigs;
            foreach(KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> entry in customDredgingConfig)
            {
                GameManager.Instance.GameConfigData.DredgingDifficultyConfigs[entry.Key] = entry.Value;
            }

            Dictionary<HarvestDifficulty, HarvestDifficultyConfigData> customFishingConfig = ConfigManager.INSTANCE.FishingDifficultyConfigs;
            foreach (KeyValuePair<HarvestDifficulty, HarvestDifficultyConfigData> entry in customFishingConfig)
            {
                GameManager.Instance.GameConfigData.FishingDifficultyConfigs[entry.Key] = entry.Value;
            }
        }

        private static HarvestDifficultyConfigData GetModDefaultValue(bool isDredge, HarvestDifficulty difficulty, HarvestDifficultyConfigData data)
        {
            if (!isDredge)
            {
                switch (difficulty)
                {
                    case HarvestDifficulty.VERY_EASY:
                        data.secondsToPassivelyCatch = 6.0f;
                        data.rotationSpeed = 140.0f;
                        data.specialTargetWidth = 18;

                        data.numPendulumSegments = 2;

                        data.valueFactor = 0.9f;
                        data.speedFactor = 1.1f;

                        data.diamondScaleUpTimeSec = 1.5f;
                        data.timeBetweenDiamondTargetsMin = 1.0f;
                        break;
                    case HarvestDifficulty.EASY:
                        data.secondsToPassivelyCatch = 8.0f;
                        data.rotationSpeed = 170.0f;
                        data.specialTargetWidth = 13;
                        data.minTargetWidth = 20;
                        data.maxTargetWidth = 25;

                        data.numPendulumSegments = 2;

                        data.valueFactor = 0.7f;
                        data.speedFactor = 1.3f;

                        data.diamondScaleUpTimeSec = 1.2f;
                        data.timeBetweenDiamondTargetsMin = 0.9f;
                        break;
                    case HarvestDifficulty.MEDIUM:
                        data.secondsToPassivelyCatch = 10.0f;
                        data.rotationSpeed = 190.0f;
                        data.specialTargetWidth = 9;
                        data.minTargetWidth = 15;
                        data.maxTargetWidth = 20;

                        data.numPendulumSegments = 3;

                        data.valueFactor = 0.6f;
                        data.speedFactor = 1.4f;

                        data.diamondScaleUpTimeSec = 1.0f;
                        data.timeBetweenDiamondTargetsMin = 0.7f;
                        break;
                    case HarvestDifficulty.HARD:
                        data.secondsToPassivelyCatch = 12.0f;
                        data.rotationSpeed = 230.0f;
                        data.specialTargetWidth = 7;
                        data.minTargetWidth = 12;
                        data.maxTargetWidth = 17;

                        data.numPendulumSegments = 3;

                        data.valueFactor = 0.5f;
                        data.speedFactor = 1.5f;

                        data.diamondScaleUpTimeSec = 0.8f;
                        data.timeBetweenDiamondTargetsMin = 0.5f;
                        break;
                    case HarvestDifficulty.VERY_HARD:
                        data.secondsToPassivelyCatch = 15.0f;
                        data.rotationSpeed = 280.0f;
                        data.specialTargetWidth = 6;
                        data.minTargetWidth = 10;
                        data.maxTargetWidth = 13;

                        data.numPendulumSegments = 3;

                        data.valueFactor = 0.3f;
                        data.speedFactor = 1.8f;

                        data.diamondScaleUpTimeSec = 0.6f;
                        data.timeBetweenDiamondTargetsMin = 0.4f;
                        break;
                }
            }
            else
            {
                switch (difficulty)
                {
                    case HarvestDifficulty.VERY_EASY:
                        data.secondsToPassivelyCatch = 7.0f;
                        data.rotationSpeed = 130.0f;
                        data.minTargets = 6;
                        data.maxTargets = 7;
                        break;
                    case HarvestDifficulty.EASY:
                        data.secondsToPassivelyCatch = 8.0f;
                        data.rotationSpeed = 150.0f;
                        data.minTargets = 7;
                        data.maxTargets = 7;
                        break;
                    case HarvestDifficulty.MEDIUM:
                        data.secondsToPassivelyCatch = 9.0f;
                        data.rotationSpeed = 160.0f;
                        data.minTargets = 8;
                        data.maxTargets = 8;
                        break;
                    case HarvestDifficulty.HARD:
                        data.secondsToPassivelyCatch = 11.0f;
                        data.rotationSpeed = 170.0f;
                        data.minTargets = 9;
                        data.maxTargets = 9;
                        break;
                    case HarvestDifficulty.VERY_HARD:
                        data.secondsToPassivelyCatch = 13.0f;
                        data.rotationSpeed = 200.0f;
                        data.minTargets = 10;
                        data.maxTargets = 10;
                        break;
                }
            }

            return data;
        }
    }
}
