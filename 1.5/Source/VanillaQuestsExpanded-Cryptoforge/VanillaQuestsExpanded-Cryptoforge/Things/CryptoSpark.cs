
using RimWorld;
using Verse;
namespace VanillaQuestsExpandedCryptoforge
{
    public class CryptoSpark : Projectile
    {
        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            Map map = base.Map;
            base.Impact(hitThing, blockedByShield);
            Thing instigator = launcher;
            Cryptofreeze fire;
            if ((fire = launcher as Cryptofreeze) != null)
            {
                instigator = fire.instigator;
            }
            Cryptofreeze.TryStartCryptoFreezeIn(base.Position, map, 0.1f, instigator);
        }
    }
}