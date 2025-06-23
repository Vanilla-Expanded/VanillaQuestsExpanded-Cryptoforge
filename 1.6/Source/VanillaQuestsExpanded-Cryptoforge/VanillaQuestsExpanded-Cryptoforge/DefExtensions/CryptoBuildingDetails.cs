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


        public ThingDef buildingLeft = null;
        public SoundDef deconstructSound = null;
        public string gizmoTexture;
        public string gizmoText;
        public string gizmoDesc;
        public List<PawnKindDef> frozenMechanoids;

    }



}
