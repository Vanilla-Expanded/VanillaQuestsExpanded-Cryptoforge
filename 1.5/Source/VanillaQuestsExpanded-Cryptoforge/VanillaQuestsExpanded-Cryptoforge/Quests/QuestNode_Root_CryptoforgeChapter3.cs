using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;
using System.Collections.Generic;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestNode_Root_CryptoforgeChapter3 : QuestNode_Site
    {
        public override SitePartDef QuestSite => InternalDefOf.VQE_CryptoforgeChapter3Site;
        protected override void RunInt()
        {
            var allowedBiomes = new List<BiomeDef>() { BiomeDefOf.IceSheet, BiomeDefOf.SeaIce};
            if (!PrepareQuest(out Quest quest, out Slate slate, out Map map, out float points, out int tile, (int x) =>
            {
                return true;
            }, allowedBiomes))
            {
                Log.Error("Failed to find a suitable site tile for the Cryptoforge Chapter 3 quest.");
                return;
            }
            var hostileFaction = Faction.OfInsects;
            var site = GenerateSite(quest, slate, points, tile, hostileFaction, out string siteMapGeneratedSignal, failWhenMapRemoved: false);
            List<PawnKindDef> enemyUnitPawns = GeneratePawnKindList(hostileFaction, points, site);
            slate.Set("ListOfEnemies", FormatPawnListToString(enemyUnitPawns));
            QuestPart_CryptoforgeBow questPart = new QuestPart_CryptoforgeBow();
            questPart.site = site;
            questPart.enemyUnitPawns = enemyUnitPawns;
            questPart.siteFaction = hostileFaction;
            quest.AddPart(questPart);

            QuestPart_EndQuestOnScanSignals questPart_ScanSignalsCounter = new QuestPart_EndQuestOnScanSignals();
            questPart_ScanSignalsCounter.site = site;
            questPart_ScanSignalsCounter.inSignalEnable = siteMapGeneratedSignal;
            questPart_ScanSignalsCounter.scanningBuilding = InternalDefOf.VQE_AncientBlackBox;
            questPart_ScanSignalsCounter.maxSignalsCount = 1;
            questPart_ScanSignalsCounter.inSignal = QuestGenUtility.HardcodedSignalWithQuestID("site.Studied_" + InternalDefOf.VQE_AncientBlackBox.defName);
            quest.AddPart(questPart_ScanSignalsCounter);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}