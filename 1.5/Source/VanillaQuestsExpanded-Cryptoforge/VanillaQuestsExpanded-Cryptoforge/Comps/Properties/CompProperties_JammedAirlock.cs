
using RimWorld;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class CompProperties_JammedAirlock : CompProperties_Interactable
    {
        public ThingDef doorToConvertTo;

        public CompProperties_JammedAirlock()
        {
            compClass = typeof(CompJammedAirlock);
        }
    }
}
