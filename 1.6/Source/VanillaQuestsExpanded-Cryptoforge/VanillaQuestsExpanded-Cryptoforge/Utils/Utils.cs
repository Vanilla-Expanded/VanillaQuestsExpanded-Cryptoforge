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
            var raidStragegy = InternalDefOf.ImmediateAttackBreaching;
            var pawnGroupMakerParms = new PawnGroupMakerParms
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = faction,
                points = points,
                raidStrategy = raidStragegy
            };
            parms.faction = faction;
            var minPoints = faction.def.MinPointsToGeneratePawnGroup(pawnGroupMakerParms.groupKind, pawnGroupMakerParms);
            parms.raidStrategy = raidStragegy;
            if (minPoints > parms.points && minPoints < float.MaxValue)
            {
                parms.points = minPoints;
            }
            
            while (parms.points < 9999 &&
                (parms.raidStrategy.Worker.CanUseWith(parms, PawnGroupKindDefOf.Combat) is false))
            {
                parms.points += 50f;
            }
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;
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
