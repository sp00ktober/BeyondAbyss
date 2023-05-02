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
            // 2 is default
            AccessTools.Field(typeof(GameConfigData), "sleepingSanityModifier").SetValue(GameManager.Instance.GameConfigData, 0.2f);
        }
    }
}
