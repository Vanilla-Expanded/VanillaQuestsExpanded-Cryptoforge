using System.Collections.Generic;
using System.Linq;

using RimWorld;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Alert_CriticalCryptoGenerators : Alert_Critical
    {
        public Alert_CriticalCryptoGenerators()
        {
            defaultPriority = AlertPriority.Critical;
            defaultLabel = "VQE_Alert_CriticalCryptoGenerators".Translate();
            defaultExplanation = "VQE_Alert_CriticalCryptoGenerators_Desc".Translate();
        }

      

        public override AlertReport GetReport()
        {

            var map = Find.CurrentMap;
            if (map == null)
            {
                return AlertReport.Inactive;
            }

            return AlertReport.CulpritsAre(GetCriticalCryptoGenerators(map).ToList());
        }

        public static IEnumerable<Thing> GetCriticalCryptoGenerators(Map map)
        {
            var generators = map.GetComponent<MapComponent_CryptoBuildingsInMap>().criticalCryptoGenerators_InMap;

            foreach (var generator in generators)
            {


                yield return generator;
            }


            
        }
    }
}
