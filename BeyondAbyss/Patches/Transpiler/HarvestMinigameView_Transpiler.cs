using Cinemachine;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using UnityEngine;
using Winch.Core;

namespace BeyondAbyss.Patches.Transpiler
{
    [HarmonyPatch(typeof(HarvestMinigameView))]
    internal class HarvestMinigameView_Transpiler
    {
        [HarmonyTranspiler]
        [HarmonyPatch(typeof(HarvestMinigameView), "SpawnItem")]
        public static IEnumerable<CodeInstruction> SpawnItem_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            CodeMatcher matcher = new CodeMatcher(instructions)
                .MatchEndForward(
                    new CodeMatch(OpCodes.Ldarg_0),
                    new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(HarvestMinigameView), "currentPOI")),
                    new CodeMatch(OpCodes.Ldc_I4_0),
                    new CodeMatch(i => i.opcode == OpCodes.Callvirt && ((MethodInfo)i.operand).Name == "SetIsCurrentlySpecial"));

            if (matcher.IsInvalid)
            {
                WinchCore.Log.Error("Could not find IL code in HarvestMinigameView, skipping patch (no sanity effect on aberration catch).");
                return instructions;
            }

            matcher
                .Advance(1)
                .Insert(Helper.Transpiler.EmitDelegate<System.Action>(() =>
                {
                    AccessTools.Field(typeof(PlayerSanity), "_sanity").SetValue(GameManager.Instance.Player.Sanity, GameManager.Instance.Player.Sanity.CurrentSanity + 0.15f);
                    new Thread(new ThreadStart(() =>
                    {
                        float defaultFOV = (float)AccessTools.Field(typeof(PlayerCamera), "defaultFOV").GetValue(GameManager.Instance.PlayerCamera);
                        float hasteFOV = (float)AccessTools.Field(typeof(PlayerCamera), "hasteFOV").GetValue(GameManager.Instance.PlayerCamera);
                        CinemachineFreeLook cmfl = (CinemachineFreeLook)AccessTools.Field(typeof(PlayerCamera), "cinemachineCamera").GetValue(GameManager.Instance.PlayerCamera);

                        float value = 1f;
                        while(value >= 0f)
                        {
                            cmfl.m_Lens.FieldOfView = Mathf.Lerp(defaultFOV, hasteFOV, value);
                            value -= 0.01f;

                            Thread.Sleep(50);
                        }
                    })).Start();
                }));

            return matcher.InstructionEnumeration();
        }
    }
}
