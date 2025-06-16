using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using Verse;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestNode_Root_CryptoforgeChapter4 : QuestNode_Site
    {
        public override SitePartDef QuestSite => InternalDefOf.VQE_CryptoforgeChapter4Site;
        protected override void RunInt()
        {
            var allowedBiomes = new List<BiomeDef>() { BiomeDefOf.IceSheet, BiomeDefOf.SeaIce };
            if (!PrepareQuest(out Quest quest, out Slate slate, out Map map, out float points, out int tile, (int x) =>
            {
                return true;
            }, allowedBiomes))
            {
                Log.Error("Failed to find a suitable site tile for the Cryptoforge Chapter 4 quest.");
                return;
            }
            var site = GenerateSite(quest, slate, points, tile, Faction.OfPlayer, out string siteMapGeneratedSignal, failWhenMapRemoved: false);
            QuestPart_LegendaryCryptoforge cryptoforgeCraftingPart = new QuestPart_LegendaryCryptoforge();
            cryptoforgeCraftingPart.mapParent = site;
            cryptoforgeCraftingPart.inSignalEnable = siteMapGeneratedSignal;
            cryptoforgeCraftingPart.inSignal = QuestGenUtility.HardcodedSignalWithQuestID("site.CryptoforgeUsed");
            quest.AddPart(cryptoforgeCraftingPart);
        }

        protected override bool TestRunInt(Slate slate)
        {
            return true;
        }
    }
}