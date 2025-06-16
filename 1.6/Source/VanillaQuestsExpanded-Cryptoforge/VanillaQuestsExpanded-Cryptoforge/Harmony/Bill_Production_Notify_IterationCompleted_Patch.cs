using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    [HarmonyPatch(typeof(Bill_Production), "Notify_IterationCompleted")]
    public static class Bill_Production_Notify_IterationCompleted_Patch
    {
        public static void Postfix(Bill __instance)
        {
            if (__instance.billStack.billGiver is Building_WorkTable worktable && worktable.def == InternalDefOf.VQE_AncientCryptoforge)
            {
                var site = worktable.Map.Parent;
                var signal = "CryptoforgeUsed";
                Find.SignalManager.SendSignal(new Signal(signal, site.Named("SUBJECT")));
                QuestUtility.SendQuestTargetSignals(site.questTags, signal, site.Named("SUBJECT"));
            }
        }
    }
}