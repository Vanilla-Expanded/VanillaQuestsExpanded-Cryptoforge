using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaQuestsExpandedCryptoforge
{

    public class LootableContents : DefModExtension
    {

        public List<ThingAndCount> contents = null;
        public ThingDef buildingLeft = null;
        public SoundDef deconstructSound = null;

    }

    public class ThingAndCount
    {
        public ThingDef thing;
        public int count;

    }


}
