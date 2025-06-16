using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Building_FloorHeater : Building
    {

        CompPowerTrader compPower;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
           
            compPower = this.TryGetComp<CompPowerTrader>();
        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);

            if (compPower?.PowerOn==true)
            {
                var vector = DrawPos + Altitudes.AltIncVect;

                vector.y += Altitudes.AltInc;
                Vector3 vectorOffset = Vector3.zero;

                GraphicsCache.FloorHeater_On.DrawFromDef(vector + vectorOffset, Rot4.North, null);

            }
        }

    }
}
