
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;
namespace VanillaQuestsExpandedCryptoforge
{
    public class CompJammedAirlock : CompInteractable
    {
        public Building Door => (Building)parent;

        public new CompProperties_JammedAirlock Props => (CompProperties_JammedAirlock)props;

       

        public override void OrderForceTarget(LocalTargetInfo target)
        {
            OrderActivation(target.Pawn);
        }

        public override string CompInspectStringExtra()
        {
           
            return "VQE_AirlockJammed".Translate();
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
        {
            
                AcceptanceReport acceptanceReport = CanInteract(selPawn);
                FloatMenuOption floatMenuOption = new FloatMenuOption(Props.jobString.CapitalizeFirst(), delegate
                {
                    OrderActivation(selPawn);
                });
                if (!acceptanceReport.Accepted)
                {
                    floatMenuOption.Disabled = true;
                    floatMenuOption.Label = floatMenuOption.Label + " (" + acceptanceReport.Reason + ")";
                }
                yield return floatMenuOption;
            
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
           
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
        }

        protected override void OnInteracted(Pawn caster)
        {
           
            if (caster.IsColonist)
            {
                parent.Map.fogGrid.FloodUnfogAdjacent(parent.Position, sendLetters: false);
            }
            
            Thing thingToMake = GenSpawn.Spawn(ThingMaker.MakeThing(InternalDefOf.VQE_ForcedAncientAirlock), this.parent.PositionHeld, this.parent.Map);
            thingToMake.Rotation = this.parent.Rotation;
            if (this.parent.Spawned)
            {
               
                this.parent.DeSpawn();
            }
        }

        private void OrderActivation(Pawn pawn)
        {
            Job job = JobMaker.MakeJob(JobDefOf.InteractThing, parent);
            job.count = 1;
            job.playerForced = true;
            pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc);
        }

       
    }
}