using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    [HarmonyPatch(typeof(Site), "ShouldRemoveMapNow")]
    public static class Site_ShouldRemoveMapNow_Patch
    {
        public static void Postfix(Site __instance, ref bool __result, ref bool alsoRemoveWorldObject)
        {
            if (__result && __instance.parts != null)
            {
                foreach (var quest in Find.QuestManager.QuestsListForReading.Where(x => x.State == QuestState.Ongoing))
                {
                    List<QuestPart> questParts = quest.PartsListForReading;
                    for (var i = 0; i < questParts.Count; i++)
                    {
                        if (questParts[i] is QuestPart_RelayScanners relayScannersPart
                            && relayScannersPart.site == __instance)
                        {
                            alsoRemoveWorldObject = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}