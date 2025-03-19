using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using VFECore;

namespace VanillaQuestsExpandedCryptoforge
{
    public class JobDriver_InitiateScan : JobDriver
    {
        public const int totalTime = 300; // 5 seconds

      
        public int totalTimer = 0;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A).Thing, this.job, 1, -1, null, true);
        }
        private Scannable Building => (Scannable)job.GetTarget(TargetIndex.A).Thing;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Thing building = this.job.GetTarget(TargetIndex.A).Thing;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            if (TargetA.Thing.def.hasInteractionCell)
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            }
            else
            {
                yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch);
            }

            Toil study = ToilMaker.MakeToil("MakeNewToils");
           
            study.tickAction = delegate
            {
                Pawn actor = study.actor;
               

                actor.rotationTracker.FaceTarget(actor.CurJob.GetTarget(TargetIndex.A));

                totalTimer++;
                if (totalTimer > totalTime)
                {
                
                    actor.jobs.EndCurrentJob(JobCondition.Succeeded);


                }
            };

            study.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
          
            study.defaultCompleteMode = ToilCompleteMode.Never;
          
            study.handlingFacing = true;
            study.AddFinishAction(delegate
            {
   
                Building.Notify_BeginScan();
            });
            yield return study;



        }
    }
}
