using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaQuestsExpandedCryptoforge
{

    public class CryptoBuildingDetails : DefModExtension
    {

        public List<ThingAndCount> contents = null;
        public ThingDef buildingLeft = null;
        public SoundDef deconstructSound = null;
        public string gizmoTexture;
        public string gizmoText;
        public string gizmoDesc;
        public bool craftingInspiration;
        public SkillDef skillForStudying;
        public List<PawnKindDef> frozenMechanoids;

    }

    public class ThingAndCount
    {
        public ThingDef thing;
        public int count;

    }


}
