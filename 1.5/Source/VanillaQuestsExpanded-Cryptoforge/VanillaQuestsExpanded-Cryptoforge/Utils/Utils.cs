using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using UnityEngine;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Utils
    {

        public static void CreateMechRaid(Map map, float pointMultiplier)
        {
            float points = StorytellerUtility.DefaultThreatPointsNow(map);

            Faction faction = Find.FactionManager.OfMechanoids;
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(IncidentCategoryDefOf.ThreatBig, map);
            parms.forced = true;
            parms.target = map;
            parms.points = points * pointMultiplier;
            var pawnGroupMakerParms = new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = faction,
                points = points,
                raidStrategy = RaidStrategyDefOf.ImmediateAttack
            };
            var minPoints = faction.def.MinPointsToGeneratePawnGroup(pawnGroupMakerParms.groupKind, pawnGroupMakerParms);
            if (minPoints > parms.points && minPoints < float.MaxValue)
            {
                parms.points = minPoints;
            }
            
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

        public static void ThrowExtendedAirPuffUp(Vector3 loc, Map map, float size, float speedMultiplier)
        {
            if (loc.ToIntVec3().ShouldSpawnMotesAt(map))
            {
                FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc + new Vector3(Rand.Range(-0.02f, 0.02f), 0f, Rand.Range(-0.02f, 0.02f)), map, FleckDefOf.AirPuff, Rand.Range(1.5f, 2.5f) * size);
                dataStatic.rotationRate = Rand.RangeInclusive(-240, 240);
                dataStatic.velocityAngle = Rand.Range(-45, 45);
                dataStatic.velocitySpeed = Rand.Range(1.2f, 1.5f) * speedMultiplier;
                map.flecks.CreateFleck(dataStatic);
            }
        }

    }
}
