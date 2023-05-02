using BeyondAbyss.Singletons;
using HarmonyLib;
using Winch.Core;

namespace BeyondAbyss.Patches.Dynamic
{
    [HarmonyPatch(typeof(PlayerSanity))]
    internal class PlayerSanity_Patch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PlayerSanity), "Update")]
        public static void Update_Prefix()
        {
            if (ConfigManager.INSTANCE.SleepingOnLand)
            {
                AccessTools.Field(typeof(GameConfigData), "sleepingSanityModifier").SetValue(GameManager.Instance.GameConfigData, 2.0f);
            }
            else
            {
                AccessTools.Field(typeof(GameConfigData), "sleepingSanityModifier").SetValue(GameManager.Instance.GameConfigData, 0.2f);
            }
        }
    }
}
