using RimWorld;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Bullet_Cryptobolter : Bullet
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing, blockedByShield);
            if (hitThing is Pawn targetPawn)
            {
                float heatArmor = targetPawn.GetStatValue(StatDefOf.ArmorRating_Heat);
                float severity = 2f - heatArmor;
                if (severity < 0.01f) severity = 0.01f;
                HealthUtility.AdjustSeverity(targetPawn, InternalDefOf.VQE_CryptoSlowdown, severity);
            }
        }
    }
}