﻿using System.Collections.Generic;
using Verse;
using Verse.AI;
using RimWorld;
using VFECore;

namespace VanillaQuestsExpandedCryptoforge
{
    public class JobDriver_StudyBlueprints : JobDriver
    {

        public const int totalTime = 1200; // 20 seconds
        public int totalTimer = 0;

        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return this.pawn.Reserve(this.job.GetTarget(TargetIndex.A).Thing, this.job, 1, -1, null, true);
        }
        private Studiable Building => (Studiable)job.GetTarget(TargetIndex.A).Thing;

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
            study.WithEffect(EffecterDefOf.Research, TargetIndex.A);
            study.defaultCompleteMode = ToilCompleteMode.Never;
            study.activeSkill = () => SkillDefOf.Intellectual;
            study.handlingFacing = true;
            study.AddFinishAction(delegate
            {
              
                pawn.mindState.inspirationHandler.TryStartInspiration(InspirationDefOf.Inspired_Creativity);

                Building.Study();
            });
            yield return study;



        }
    }
}
