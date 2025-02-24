using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Utils
    {

        public static void CreateMechRaid(Map map, float pointMultiplier)
        {
            float points = StorytellerUtility.DefaultThreatPointsNow(map);

            Faction faction = Find.FactionManager.OfMechanoids;

            StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
            IncidentParms parms = storytellerComp.GenerateParms(IncidentCategoryDefOf.ThreatBig, map);
            parms.forced = true;
            parms.target = map;
            parms.points = points * pointMultiplier;
            parms.faction = faction;

            List<RaidStrategyDef> source = DefDatabase<RaidStrategyDef>.AllDefs.Where((RaidStrategyDef s) => s.Worker.CanUseWith(parms, PawnGroupKindDefOf.Combat)).ToList();
            parms.raidStrategy = source.RandomElement();
            if (parms.raidStrategy != null)
            {
                List<PawnsArrivalModeDef> source2 = DefDatabase<PawnsArrivalModeDef>.AllDefs.Where((PawnsArrivalModeDef a) => a.Worker.CanUseWith(parms) && parms.raidStrategy.arriveModes.Contains(a)).ToList();
                parms.raidArrivalMode = source2.RandomElement();
            }


            IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);

        }




    }
}
