using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using RimWorld.Planet;

namespace VanillaQuestsExpandedCryptoforge
{
    public class MapComponent_CryptoBuildingsInMap : MapComponent
    {


        public HashSet<Thing> scannables_InMap = new HashSet<Thing>();
        public HashSet<Thing> restartables_InMap = new HashSet<Thing>();
        public HashSet<Thing> criticalCryptoGenerators_InMap = new HashSet<Thing>();



        public MapComponent_CryptoBuildingsInMap(Map map) : base(map)
        {
        }



        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Collections.Look(ref this.scannables_InMap, "scannables_InMap", LookMode.Reference);
            Scribe_Collections.Look(ref this.restartables_InMap, "restartables_InMap", LookMode.Reference);

            Scribe_Collections.Look(ref this.criticalCryptoGenerators_InMap, "criticalCryptoGenerators_InMap", LookMode.Reference);

        }


        public void AddScannablesToMap(Thing thing)
        {
            if (!scannables_InMap.Contains(thing))
            {
                scannables_InMap.Add(thing);
            }
        }

        public void RemoveScannablesFromMap(Thing thing)
        {
            if (scannables_InMap.Contains(thing))
            {
                scannables_InMap.Remove(thing);
            }

        }

        public void AddCriticalCryptoGeneratorsToMap(Thing thing)
        {
            if (!criticalCryptoGenerators_InMap.Contains(thing))
            {
                criticalCryptoGenerators_InMap.Add(thing);
            }
        }

        public void RemoveCriticalCryptoGeneratorsFromMap(Thing thing)
        {
            if (criticalCryptoGenerators_InMap.Contains(thing))
            {
                criticalCryptoGenerators_InMap.Remove(thing);
            }

        }

        public void AddRestartableToMap(Thing thing)
        {
            if (!restartables_InMap.Contains(thing))
            {
                restartables_InMap.Add(thing);
            }
        }

        public void RemoveRestartableFromMap(Thing thing)
        {
            if (restartables_InMap.Contains(thing))
            {
                restartables_InMap.Remove(thing);
            }

        }


    }


}
