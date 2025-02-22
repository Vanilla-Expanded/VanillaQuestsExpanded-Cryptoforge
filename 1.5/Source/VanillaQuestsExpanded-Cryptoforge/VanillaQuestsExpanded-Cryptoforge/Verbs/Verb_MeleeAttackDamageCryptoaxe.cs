using RimWorld;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Verb_MeleeAttackDamageCryptoaxe : Verb_MeleeAttackDamage
    {
        protected override DamageWorker.DamageResult ApplyMeleeDamageToTarget(LocalTargetInfo target)
        {
            DamageWorker.DamageResult result = base.ApplyMeleeDamageToTarget(target);
            if (target.Thing is Pawn targetPawn)
            {
                float heatArmor = targetPawn.GetStatValue(StatDefOf.ArmorRating_Heat);
                float severity = (2f - heatArmor) / 4f;
                if (severity < 0.01f) severity = 0.01f;
                HealthUtility.AdjustSeverity(targetPawn, InternalDefOf.VQE_CryptoSlowdown, severity);
            }
            return result;
        }
    }
}