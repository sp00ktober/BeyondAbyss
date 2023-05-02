using BeyondAbyss.Singletons;
using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using Winch.Core;

namespace BeyondAbyss.Patches.Dynamic
{
    [HarmonyPatch(typeof(BoatActionsDestinationUI))]
    internal class BoatActionsDestinationUI_Patch
    {
        private static GameObject newButton = null;
        private static GameObject newIris = null;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(BoatActionsDestinationUI.Init))]
        public static void Init_Postfix(BoatActionsDestinationUI __instance)
        {
            if (GameManager.Instance.SaveData.Funds < 30) return;

            GameObject restButton = GameObject.Find("BoatActionSubDestinationButton (1)");
            GameObject researchButton = GameObject.Find("BoatActionSubDestinationButton (2)");

            GameObject eyeOpenRed = GameObject.Find("EyeOpenRed");
            GameObject iris = GameObject.Find("Iris");

            if(restButton != null && researchButton != null && eyeOpenRed != null && iris != null)
            {
                newButton = GameObject.Instantiate(restButton);
                newButton.transform.parent = restButton.transform.parent;
                newButton.transform.localScale = Vector3.one;

                SubDestinationButton destButton = newButton.GetComponent<SubDestinationButton>();
                destButton.OnButtonDeselectAction = new System.Action<BaseDestination>((baseDestination) =>
                {
                    ((GameObject)AccessTools.Field(typeof(BoatActionsDestinationUI), "headerContainer").GetValue(__instance)).SetActive(false);
                });
                destButton.OnButtonSelectAction = new System.Action<BaseDestination>((baseDestination) =>
                {
                    ((LocalizeStringEvent)AccessTools.Field(typeof(BoatActionsDestinationUI), "localizedHeaderString").GetValue(__instance)).StringReference.SetReference(LanguageManager.STRING_TABLE, "beyondabyss.dock.sanitysleep");
                    ((GameObject)AccessTools.Field(typeof(BoatActionsDestinationUI), "headerContainer").GetValue(__instance)).SetActive(true);
                });
                destButton.BasicButtonWrapper.OnClick = new System.Action(() =>
                {
                    decimal loss = (decimal)(-1.0f * UnityEngine.Random.Range(30.0f, (float)GameManager.Instance.SaveData.Funds / 100.0f * 10.0f));
                    GameManager.Instance.AddFunds(loss);
                    ConfigManager.INSTANCE.SleepingOnLand = true;
                    GameEvents.Instance.OnTimeForcefullyPassingChanged += new System.Action<bool, string, TimePassageMode>((isForcefullyPassing, reason, mode) =>
                    {
                        if (!isForcefullyPassing)
                        {
                            ConfigManager.INSTANCE.SleepingOnLand = false;
                            GameManager.Instance.UI.ShowNotification(NotificationType.MONEY_LOST, "notification.funds-removed", new object[]
                            {
                                string.Concat(new string[]
                                {
                                    "<color=#",
                                    GameManager.Instance.LanguageManager.GetColorCode(DredgeColorTypeEnum.NEGATIVE),
                                    ">$",
                                    loss.ToString("n2", LocalizationSettings.SelectedLocale.Formatter),
                                    "</color>"
                                })
                            });
                        }
                    });
                    restButton.GetComponent<SubDestinationButton>().BasicButtonWrapper.OnClick();
                });

                newIris = GameObject.Instantiate(iris);
                newIris.transform.parent = newButton.transform.GetChild(1);

                newButton.GetComponentsInChildren<UnityEngine.UI.Image>()[1].sprite = eyeOpenRed.GetComponent<UnityEngine.UI.Image>().sprite;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BoatActionsDestinationUI), "OnDestroy")]
        public static void OnDestroy_Postfix()
        {
            if(newButton != null && newIris != null)
            {
                GameObject.Destroy(newButton);
                GameObject.Destroy(newIris);
                newButton = null;
                newIris = null;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(BoatActionsDestinationUI), "LateUpdate")]
        public static void LateUpdate_Postfix()
        {
            GameObject eyeOpenRed = GameObject.Find("EyeOpenRed");
            GameObject iris = GameObject.Find("Iris");

            if (newButton != null && eyeOpenRed != null && iris != null)
            {
                newIris.transform.localPosition = iris.transform.localPosition;

                UnityEngine.UI.Image[] images = iris.GetComponentsInChildren<UnityEngine.UI.Image>();
                for (int i = 0; i < images.Length; i++)
                {
                    try
                    {
                        newIris.GetComponentsInChildren<UnityEngine.UI.Image>()[i].sprite = images[i].sprite;
                        newIris.GetComponentsInChildren<UnityEngine.UI.Image>()[i].color = images[i].color;
                    }
                    catch(Exception e)
                    {
                        string message = e.Message + "\n" + e.StackTrace;
                        WinchCore.Log.Error(message);
                    }
                }
            }
        }
    }
}
