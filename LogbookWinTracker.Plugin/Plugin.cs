using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using TrainworksReloaded.Core;
using TrainworksReloaded.Core.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LogbookWinTracker.Plugin
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger = new(MyPluginInfo.PLUGIN_GUID);
        
        // Plugin startup logic. This function is automatically called when your plugin initializes
        public void Awake()
        {
            Logger = base.Logger;

            var builder = Railhead.GetBuilder();
            builder.Configure(
                MyPluginInfo.PLUGIN_GUID,
                c =>
                {
                    // Be sure to include any new json files if you add more.
                    c.AddMergedJsonFile(
                        "json/plugin.json",
                        "json/global.json"
                    );
                }
            );

            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

            // Uncomment if you need Harmony Patch support.
            var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();
        }
    }

    [HarmonyPatch(typeof(CompendiumSectionChecklist), "InitializeCovenantRankMeter")]
    public static class Patch_ReplaceCovenantMeter
    {
        public static void Postfix(CompendiumSectionChecklist __instance)
        {
            var nativeMeter = AccessTools.Field(typeof(CompendiumSectionChecklist), "covenantRankMeter")
                                        .GetValue(__instance) as CovenantRankMeter;
            var spChallengeUI = AccessTools.Field(typeof(CompendiumSectionChecklist), "spChallengeProgressUI")
                                           .GetValue(__instance) as SpChallengeProgressUI;

            if (nativeMeter == null || spChallengeUI == null) return;

            nativeMeter.gameObject.SetActive(false);
            Transform sidebar = spChallengeUI.transform.parent;

            // Calculate win metrics
            (int cov10Wins, int titanWins, int totalCombos) = CalculateWinCounts();

            // 1. Covenant 10 Wins Block
            GameObject cov10Block = sidebar.Find("Custom_Covenant10_Block")?.gameObject;
            if (cov10Block == null)
            {
                cov10Block = UnityEngine.Object.Instantiate(spChallengeUI.gameObject, sidebar);
                cov10Block.name = "Custom_Covenant10_Block";
                cov10Block.transform.SetSiblingIndex(nativeMeter.transform.GetSiblingIndex());
            }
            UpdateBlockUI(cov10Block, "Covenant 10 Wins", cov10Wins, totalCombos);

            // 2. Titan Wins Block
            GameObject titanBlock = sidebar.Find("Custom_Titan_Block")?.gameObject;
            if (titanBlock == null)
            {
                titanBlock = UnityEngine.Object.Instantiate(spChallengeUI.gameObject, sidebar);
                titanBlock.name = "Custom_Titan_Block";
                titanBlock.transform.SetSiblingIndex(cov10Block.transform.GetSiblingIndex() + 1);
            }
            UpdateBlockUI(titanBlock, "Titan Wins", titanWins, totalCombos);
        }

        private static (int cov10Wins, int titanWins, int totalCombos) CalculateWinCounts()
        {
            int cov10Wins = 0;
            int titanWins = 0;
            int totalCombos = 0;

            var allGameManagers = UnityEngine.Object.FindObjectOfType<AllGameManagers>();
            if (allGameManagers == null) return (0, 0, 0);

            var saveManager = allGameManagers.GetSaveManager();
            var allGameData = allGameManagers.GetAllGameData();
            if (saveManager == null || allGameData == null) return (0, 0, 0);

            var metagameSave = saveManager.GetMetagameSave();
            if (metagameSave == null) return (0, 0, 0);

            var availableClans = new List<ClassData>();
            foreach (var clan in allGameData.GetAllClassDatas())
            {
                if (saveManager.IsUnlockedAndAvailableWhenStartingRun(clan))
                {
                    availableClans.Add(clan);
                }
            }

            int[] championIndexes = { 0, 1 };

            foreach (var mainClan in availableClans)
            {
                string mainClanId = mainClan.GetID();
                int mainClanLevel = metagameSave.GetLevel(mainClanId);

                foreach (var subClan in availableClans)
                {
                    string subClanId = subClan.GetID();
                    if (mainClanId == subClanId) continue;

                    foreach (int mainChampIndex in championIndexes)
                    {
                        if (mainChampIndex == 1 && mainClanLevel < 5) continue;

                        totalCombos++;

                        var winData = metagameSave.GetClassCombinationWinAscensionLevel(mainClanId, subClanId, mainChampIndex);

                        if (winData.highestAscensionLevel >= 10) cov10Wins++;
                        if (winData.isTfbVictory) titanWins++;
                    }
                }
            }

            return (cov10Wins, titanWins, totalCombos);
        }

        private static void UpdateBlockUI(GameObject block, string headerText, int wins, int total)
        {
            float fillRatio = total > 0 ? (float)wins / total : 0f;

            // 1. Update Header Text
            Transform labelRoot = block.transform.Find("Label root");
            if (labelRoot != null)
            {
                var tmpText = labelRoot.GetComponentInChildren<TMP_Text>(true);
                if (tmpText != null) tmpText.text = headerText;
                else
                {
                    var uiText = labelRoot.GetComponentInChildren<Text>(true);
                    if (uiText != null) uiText.text = headerText;
                }
            }

            // 2. Update Progress Counter Text
            TMP_Text[] allTmpTexts = block.GetComponentsInChildren<TMP_Text>(true);
            Transform countRoot = block.transform.Find("Count root") ?? block.transform.Find("Progress root");
            if (countRoot != null)
            {
                var tmpCount = countRoot.GetComponentInChildren<TMP_Text>(true);
                if (tmpCount != null) tmpCount.text = $"{wins} / {total}";
            }
            else if (allTmpTexts.Length >= 2)
            {
                allTmpTexts[1].text = $"{wins} / {total}";
            }

            // 3. Update Progress Bar Meter Fill
            UpdateMeterProgress(block, fillRatio);
        }

        private static void UpdateMeterProgress(GameObject block, float fillRatio)
        {
            var slider = block.GetComponentInChildren<Slider>(true);
            if (slider != null)
            {
                slider.value = fillRatio;
                return;
            }

            string[] fillTargetNames = { "Fill", "Progress", "Bar", "Meter", "ProgressFill", "BarFill" };
            Image fillImage = null;

            foreach (string targetName in fillTargetNames)
            {
                Transform fillTransform = block.transform.Find(targetName) ?? block.transform.Find($"Progress root/{targetName}");
                if (fillTransform != null)
                {
                    fillImage = fillTransform.GetComponent<Image>();
                    if (fillImage != null) break;
                }
            }

            if (fillImage == null)
            {
                foreach (var img in block.GetComponentsInChildren<Image>(true))
                {
                    if (img.type == Image.Type.Filled)
                    {
                        fillImage = img;
                        break;
                    }
                }
            }

            if (fillImage != null)
            {
                fillImage.type = Image.Type.Filled;
                fillImage.fillAmount = fillRatio;
            }
        }
    }

    [HarmonyPatch(typeof(CompendiumSectionChecklist), nameof(CompendiumSectionChecklist.ApplyScreenInput))]
    public static class Patch_CompendiumSectionChecklist_ApplyScreenInput
    {
        public static bool Prefix(ref bool __result)
        {
            __result = false;
            return false;
        }
    }
}
