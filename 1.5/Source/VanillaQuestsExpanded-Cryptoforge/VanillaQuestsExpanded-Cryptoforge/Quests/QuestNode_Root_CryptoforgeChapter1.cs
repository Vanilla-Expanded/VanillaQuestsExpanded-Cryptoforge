using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;
using System.Collections.Generic;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestNode_Root_CryptoforgeChapter1 : QuestNode_Site
    {
        public override SitePartDef QuestSite => InternalDefOf.VQE_CryptoforgeChapter1Site;
        protected override void RunInt()
        {
            var allowedBiomes = new List<BiomeDef>() { BiomeDefOf.IceSheet, BiomeDefOf.SeaIce, BiomeDefOf.Tundra, BiomeDefOf.BorealForest };
            if (!PrepareQuest(out Quest quest, out Slate slate, out Map map, out float points, out int tile, delegate (int x)
            {
                return Find.WorldGrid[x].hilliness == Hilliness.Flat;
            }, allowedBiomes))
            {
                Log.Error("Failed to find a suitable site tile for the Cryptoforge Chapter1 quest.");
                return;
            }
            var hostileFaction = Faction.OfMechanoids ?? Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
            var site = GenerateSite(quest, slate, points, tile, hostileFaction,
            out string siteMapGeneratedSignal, failWhenMapRemoved: false);
            QuestPart_EndQuestOnScanSignals questPart_ScanSignalsCounter = new QuestPart_EndQuestOnScanSignals();
            questPart_ScanSignalsCounter.mapParent = site;
            questPart_ScanSignalsCounter.inSignalEnable = siteMapGeneratedSignal;
            questPart_ScanSignalsCounter.scanningBuilding = InternalDefOf.VQE_FrozenScanningRelay;
            questPart_ScanSignalsCounter.maxSignalsCount = 2;
            questPart_ScanSignalsCounter.inSignal = QuestGenUtility.HardcodedSignalWithQuestID("site.Scanned_" + InternalDefOf.VQE_FrozenScanningRelay.defName);
            quest.AddPart(questPart_ScanSignalsCounter);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}