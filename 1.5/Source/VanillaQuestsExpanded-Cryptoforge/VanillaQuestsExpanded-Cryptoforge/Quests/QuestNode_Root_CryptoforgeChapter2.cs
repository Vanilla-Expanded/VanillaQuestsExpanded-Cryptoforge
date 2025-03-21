using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;
using System.Collections.Generic;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestNode_Root_CryptoforgeChapter2 : QuestNode_Site
    {
        public override SitePartDef QuestSite => InternalDefOf.VQE_CryptoforgeChapter2Site;

        protected override void RunInt()
        {
            var allowedBiomes = new List<BiomeDef>() { BiomeDefOf.IceSheet, BiomeDefOf.SeaIce};
            if (!PrepareQuest(out Quest quest, out Slate slate, out Map map, out float points, out int tile, (int x) =>
            {
                return true;
            }, allowedBiomes))
            {
                Log.Error("Failed to find a suitable site tile for the Cryptoforge Chapter 2 quest.");
                return;
            }
            var hostileFaction = Faction.OfMechanoids ?? Find.FactionManager.RandomEnemyFaction(allowNonHumanlike: false);
            var site = GenerateSite(quest, slate, points, tile, hostileFaction, out string siteMapGeneratedSignal, failWhenMapRemoved: false);
            List<PawnKindDef> enemyUnitPawns = GeneratePawnKindList(hostileFaction, points, site);
            slate.Set("ListOfEnemies", FormatPawnListToString(enemyUnitPawns));
            QuestPart_CryptoforgeSternEnemies questPart = new QuestPart_CryptoforgeSternEnemies();
            questPart.site = site;
            questPart.enemyUnitPawns = enemyUnitPawns;
            questPart.siteFaction = hostileFaction;
            questPart.signalListenMode = QuestPart.SignalListenMode.NotYetAcceptedOnly;
            quest.AddPart(questPart);

        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}