using HarmonyLib;

namespace BeyondAbyss.Patches.Dynamic
{
    [HarmonyPatch(typeof(TimeController))]
    internal class TimeController_Patch
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(TimeController.GetTimePassageModifier))]
        public static void GetTimePassageModifier_Postfix(TimeController __instance, ref float __result)
        {
            if (__instance.IsDaytime)
            {
                __result *= 1.5f;
            }
            else
            {
                __result *= 0.5f;
            }
        }
    }
}
