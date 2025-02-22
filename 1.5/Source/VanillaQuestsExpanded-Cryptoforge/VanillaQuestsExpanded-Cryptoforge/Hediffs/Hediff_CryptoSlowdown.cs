using RimWorld;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Hediff_CryptoSlowdown : HediffWithComps
    {
        public override float Severity
        {
            get => base.Severity;
            set
            {
                base.Severity = value;
                if (base.Severity >= 1f)
                {
                    Cryptofreeze.TryAttachCryptofreeze(pawn, 1, null);
                }
            }
        }
    }
}