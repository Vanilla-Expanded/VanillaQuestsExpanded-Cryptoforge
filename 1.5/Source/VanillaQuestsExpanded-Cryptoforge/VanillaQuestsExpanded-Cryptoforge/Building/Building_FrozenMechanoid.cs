using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;


namespace VanillaQuestsExpandedCryptoforge
{
    public class Building_FrozenMechanoid : Building_Trap
    {
        public bool signalDelete = false;
        public int deletionCounter = 0;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.signalDelete, "signalDelete");
           
        }

        public override void Tick()
        {
            base.Tick();
            if (signalDelete)
            {
                deletionCounter++;
                if (deletionCounter > 10)
                {
                    this.Destroy();
                }
            }
        }


        protected override void SpringSub(Pawn p)
        {
            if(!this.signalDelete) {
                signalDelete = true;
                IntVec3 pos = this.PositionHeld;
                Map map = this.Map;

                PopUpMechanoid(pos, map);
                WakeUpOtherTraps(pos, map, 5, p);
            }
            


        }

        public void PopUpMechanoid(IntVec3 pos, Map map)
        {
            Effecter effecter = InternalDefOf.VQE_Effect_IceShatter.Spawn();
            effecter.Trigger(this, this);
            InternalDefOf.VQE_CrystalShatter.PlayOneShot(this);

            CryptoBuildingDetails contentDetails = this.def.GetModExtension<CryptoBuildingDetails>();
            if (contentDetails != null && Find.FactionManager.OfMechanoids != null)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(contentDetails.frozenMechanoids.RandomElement(), Find.FactionManager.OfMechanoids);
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

        }

        public void WakeUpOtherTraps(IntVec3 pos, Map map, int radius, Pawn p)
        {
           List<Building_FrozenMechanoid> buildingsToTrigger = new List<Building_FrozenMechanoid>();
            int numCells = GenRadial.NumCellsInRadius(radius);
            for (int i = 0; i < numCells; i++)
            {
                IntVec3 intVec = pos + GenRadial.RadialPattern[i];
                if (intVec.InBounds(map) && intVec != pos)
                {
                    foreach (Thing thing in intVec.GetThingList(map))
                    {
                        if (thing != null && thing is Building_FrozenMechanoid)
                        {
                            Building_FrozenMechanoid mechtrap = (Building_FrozenMechanoid)thing;
                            buildingsToTrigger.Add(mechtrap);
                        }
                    }


                }
            }
            if (buildingsToTrigger.Count>0)
            {
                foreach(Building_FrozenMechanoid thing in buildingsToTrigger)
                {
                    thing.SpringSub(p);

                }
            }

        }


    }
}
