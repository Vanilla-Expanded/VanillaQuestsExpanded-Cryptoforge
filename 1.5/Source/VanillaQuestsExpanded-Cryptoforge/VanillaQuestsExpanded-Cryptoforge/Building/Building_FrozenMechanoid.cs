using Mono.Unix.Native;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI.Group;
using static HarmonyLib.Code;
using static System.Collections.Specialized.BitVector32;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Building_FrozenMechanoid : Building_Trap
    {
      

        protected override void SpringSub(Pawn p)
        {
            IntVec3 pos = this.PositionHeld;
            Map map = this.Map;

            Effecter effecter = EffecterDefOf.Deflect_Metal_Bullet.Spawn();
            effecter.Trigger(this, this);

            CryptoBuildingDetails contentDetails = this.def.GetModExtension<CryptoBuildingDetails>();
            if (contentDetails != null && Find.FactionManager.OfMechanoids != null)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(contentDetails.frozenMechanoid, Find.FactionManager.OfMechanoids);
                GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(pos, map, 1), map);
                Lord lord = null;
                if (pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction).Any((Pawn p) => p != pawn))
                {
                    lord = ((Pawn)GenClosest.ClosestThing_Global(pawn.Position, pawn.Map.mapPawns.SpawnedPawnsInFaction(pawn.Faction), 99999f, (Thing p) => p != pawn && ((Pawn)p).GetLord() != null)).GetLord();
                }
                if (lord == null || !lord.CanAddPawn(pawn))
                {
                    lord = LordMaker.MakeNewLord(pawn.Faction, new LordJob_DefendPoint(pawn.Position), Find.CurrentMap);
                }
                if (lord != null && lord.LordJob.CanAutoAddPawns)
                {
                    lord.AddPawn(pawn);
                }
            }
            

            this.Destroy();



            int radius = 5;
            int numCells = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < numCells; i++)
            {
                IntVec3 intVec = pos + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && intVec!=pos)
                {
                    foreach (Thing thing in intVec.GetThingList(map))
                    {
                        if (thing != null  && thing is Building_FrozenMechanoid)
                        {
                            Building_FrozenMechanoid mechtrap = (Building_FrozenMechanoid)thing;
                            mechtrap.SpringSub(p); 
                        }
                    }


                }
            }


        }
    }
}
