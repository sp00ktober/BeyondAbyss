using BeyondAbyss.Singletons;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Winch.Core;

namespace BeyondAbyss.Patches.Transpiler
{
    [HarmonyPatch(typeof(TimeController))]
    internal class TimeController_Transpiler
    {
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(TimeController.GetTimePassageModifier))]
        public static IEnumerable<CodeInstruction> GetTimePassageModifier_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchStartForward(
                    new CodeMatch(OpCodes.Ldsfld),
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_GameConfigData"),
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_FishingTimePassageSpeedModifier"),
                    new CodeMatch(OpCodes.Ret));

            if (matcher.IsInvalid)
            {
                WinchCore.Log.Error("Could not match IL code in GetTimePassageModifier, not patching 1/2.");
                return instructions;
            }

            matcher
                .SetAndAdvance(OpCodes.Nop, null)
                .SetAndAdvance(OpCodes.Nop, null)
                .SetAndAdvance(OpCodes.Ldarg, 0)
                .Insert(Helper.Transpiler.EmitDelegate<Func<TimeController, float>>((instance) =>
                {
                    if (ConfigManager.INSTANCE.SleepingOnLand) return GameManager.Instance.GameConfigData.FishingTimePassageSpeedModifier;

                    return GameManager.Instance.GameConfigData.FishingTimePassageSpeedModifier * (instance.IsDaytime ? 1.2f : 0.8f);
                }));

            matcher
                .MatchEndForward(
                    new CodeMatch(OpCodes.Ldsfld),
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_Player"),
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_Controller"),
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "get_InputMagnitude"),
                    new CodeMatch(OpCodes.Ret));

            if (matcher.IsInvalid)
            {
                WinchCore.Log.Error("Could not match IL code in GetTimePassageModifier, not patching 2/2.");
                return instructions;
            }

            matcher
                .InsertAndAdvance(new CodeInstruction(OpCodes.Ldarg_0))
                .Insert(Helper.Transpiler.EmitDelegate<Func<float, TimeController, float>>((magnitude, instance) =>
                {
                    if (ConfigManager.INSTANCE.SleepingOnLand) return magnitude;

                    return magnitude * (instance.IsDaytime ? 1.2f : 0.8f);
                }));

            return matcher.InstructionEnumeration();
        }
    }
}
