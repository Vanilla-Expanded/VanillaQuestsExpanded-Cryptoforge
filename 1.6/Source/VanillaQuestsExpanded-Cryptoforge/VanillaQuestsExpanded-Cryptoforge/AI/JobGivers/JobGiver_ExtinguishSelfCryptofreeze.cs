using RimWorld;
using VEF;
using Verse;
using Verse.AI;

namespace VanillaQuestsExpandedCryptoforge
{
    public class JobGiver_ExtinguishSelfCryptofreeze : ThinkNode_JobGiver
    {
        private const float ActivateChance = 0.1f;

        protected override Job TryGiveJob(Pawn pawn)
        {
            if (Rand.Value < ActivateChance)
            {
                Cryptofreeze fire = (Cryptofreeze)pawn.GetAttachment(InternalDefOf.VQE_Cryptofreeze);
                if (fire != null)
                {
                    return JobMaker.MakeJob(InternalDefOf.VGE_ExtinguishSelfCryptofreeze, fire);
                }
            }
            return null;
        }
    }
}