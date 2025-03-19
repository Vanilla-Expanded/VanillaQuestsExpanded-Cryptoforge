using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestNode_Root_CryptoforgeChapter1 : QuestNode_Site
    {
        public override SitePartDef QuestSite => InternalDefOf.VQE_CryptoforgeChapter1Site;
        protected override void RunInt()
        {
            bool seaIcePresent = Find.WorldGrid.tiles.Any(x => x.biome == BiomeDefOf.SeaIce);
            if (!PrepareQuest(out Quest quest, out Slate slate, out Map map, out float points, out int tile, delegate (int x)
            {
                if (seaIcePresent)
                {
                    return Find.WorldGrid[x].biome == BiomeDefOf.SeaIce;
                }
                return true;
            }))
            {
                Log.Error("Failed to find a suitable site tile for the Cryptoforge Chapter1 quest.");
                return;
            }
            var hostileFaction = Faction.OfMechanoids ?? Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
            var site = GenerateSite(quest, slate, points, tile, hostileFaction, 
            out string siteMapGeneratedSignal, failWhenMapRemoved: false);
            QuestPart_RelayScanners questPart_ScanSignalsCounter = new QuestPart_RelayScanners();
            questPart_ScanSignalsCounter.site = site;
            questPart_ScanSignalsCounter.inSignal = QuestGenUtility.HardcodedSignalWithQuestID("site.ScannedRelay");
            quest.AddPart(questPart_ScanSignalsCounter);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}