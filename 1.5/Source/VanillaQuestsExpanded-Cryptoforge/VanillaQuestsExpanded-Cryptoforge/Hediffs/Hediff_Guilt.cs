using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
namespace VanillaQuestsExpandedCryptoforge
{
    public class ThoughtWorker_GuiltHediff : ThoughtWorker
    {
        protected override ThoughtState CurrentStateInternal(Pawn p)
        {
            Hediff firstHediffOfDef = p.health.hediffSet.GetFirstHediffOfDef(def.hediff);
            if (firstHediffOfDef?.def.stages == null)
            {
                return ThoughtState.Inactive;
            }
            return ThoughtState.ActiveAtStage(Mathf.Min(firstHediffOfDef.CurStageIndex, firstHediffOfDef.def.stages.Count - 1, def.stages.Count - 1));
        }
    }
    public class Hediff_Guilt : HediffWithComps
    {
        private const float GuiltSeverityIncreasePerDay = 0.005f;
        private const float GuiltRecoveryPerDay = 0.1f;
        private const float XpReductionFactor = 0.00000125f;
        private const float AcceptanceSeverityThreshold = 1.0f;
        private bool acceptanceStage;
        public override Color LabelColor => acceptanceStage ? Color.yellow : base.LabelColor;
        public override bool ShouldRemove => acceptanceStage && Severity <= 0;
        public override void Tick()
        {
            base.Tick();
            if (acceptanceStage is false)
            {
                Severity += GuiltSeverityIncreasePerDay / GenDate.TicksPerDay;
                if (Severity >= AcceptanceSeverityThreshold)
                {
                    acceptanceStage = true;
                }
            }
            else
            {
                Severity -= GuiltRecoveryPerDay / GenDate.TicksPerDay;
            }
        }

        public void Notify_SkillGained(SkillDef skill, float xp)
        {
            var skillRecord = pawn.skills.GetSkill(skill);
            if (skillRecord != null && skillRecord.passion > Passion.None)
            {
                Severity -= xp * XpReductionFactor;
                Severity = Mathf.Max(Severity, 0f);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref acceptanceStage, "acceptanceStage");
        }
    }
}
