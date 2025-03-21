using System.Collections.Generic;
using RimWorld.QuestGen;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestPart_CryptoforgeSternEnemies : QuestPart_SiteKeepWhileQuestActive
    {
        public List<PawnKindDef> enemyUnitPawns;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref enemyUnitPawns, "enemyUnitPawns", LookMode.Def);
        }

    }
}