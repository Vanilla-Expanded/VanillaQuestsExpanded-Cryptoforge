using RimWorld;
using RimWorld.Planet;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestPart_SiteKeepWhileQuestActive : QuestPartActivable
    {
        public Site site;
        public Faction siteFaction;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref site, "site");
            Scribe_References.Look(ref siteFaction, "siteFaction");
        }
    }
}