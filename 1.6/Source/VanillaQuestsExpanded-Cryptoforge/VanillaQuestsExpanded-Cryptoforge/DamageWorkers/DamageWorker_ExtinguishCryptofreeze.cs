
using RimWorld;
using Verse;
namespace VanillaQuestsExpandedCryptoforge
{
    public class DamageWorker_ExtinguishCryptofreeze : DamageWorker
    {
        private const float DamageAmountToFireSizeRatio = 0.01f;

        public override DamageResult Apply(DamageInfo dinfo, Thing victim)
        {
            DamageResult result = new DamageResult();
            Cryptofreeze fire = victim as Cryptofreeze;
            if (fire == null || fire.Destroyed)
            {
                Thing thing = victim?.GetAttachment(InternalDefOf.VQE_Cryptofreeze);
                if (thing != null)
                {
                    fire = (Cryptofreeze)thing;
                }
            }
            if (fire != null && !fire.Destroyed)
            {
                base.Apply(dinfo, victim);
                fire.fireSize -= dinfo.Amount * 0.01f;
                if (fire.fireSize < 0.1f)
                {
                    fire.Destroy();
                }
            }
            Pawn pawn = victim as Pawn;
            if (pawn != null)
            {
                Hediff hediff = HediffMaker.MakeHediff(dinfo.Def.hediff, pawn);
                hediff.Severity = dinfo.Amount;
                pawn.health.AddHediff(hediff, null, dinfo);
            }
            return result;
        }
    }
}