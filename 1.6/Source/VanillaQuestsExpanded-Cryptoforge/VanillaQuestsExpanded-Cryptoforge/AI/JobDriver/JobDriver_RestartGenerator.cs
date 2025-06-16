using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using VEF;

namespace VanillaQuestsExpandedCryptoforge
{
    public class JobDriver_RestartGenerator : JobDriver
    {
        public const int totalTime = 300; // 5 seconds


        public int totalTimer = 0;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A).Thing, this.job, 1, -1, null, true);
        }
        private Restartable Building => (Restartable)job.GetTarget(TargetIndex.A).Thing;

        protected override IEnumerable<Toil> MakeNewToils()
        {
            Thing building = this.job.GetTarget(TargetIndex.A).Thing;
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);

            Toil warning = ToilMaker.MakeToil("MakeNewToils");
            warning.initAction = delegate
            {
                Messages.Message("VQE_RestartWarning".Translate(), MessageTypeDefOf.NegativeEvent);
            };
            yield return warning;

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
                if (actor.skills != null)
                {
                    actor.skills.Learn(SkillDefOf.Intellectual, 0.025f);
                }

                actor.rotationTracker.FaceTarget(actor.CurJob.GetTarget(TargetIndex.A));

                totalTimer++;
                if (totalTimer > totalTime)
                {

                    actor.jobs.EndCurrentJob(JobCondition.Succeeded);


                }
            };

            study.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);

            study.defaultCompleteMode = ToilCompleteMode.Never;
            study.activeSkill = () => SkillDefOf.Intellectual;
            study.handlingFacing = true;
            study.AddFinishAction(delegate
            {

                Building.Notify_Restarted();
            });
            yield return study;



        }
    }
}
