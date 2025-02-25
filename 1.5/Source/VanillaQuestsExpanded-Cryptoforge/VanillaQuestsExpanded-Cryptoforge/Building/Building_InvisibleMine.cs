using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    public class Building_InvisibleMine : Building_Trap
    {
        public bool trapRevealed = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.trapRevealed, "trapRevealed");
            
        }
        protected override void SpringSub(Pawn p)
        {
            GetComp<CompExplosive>().StartWick(p);
            trapRevealed = true;
        }
        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);

            if (trapRevealed)
            {
                var vector = DrawPos + Altitudes.AltIncVect;

                vector.y += Altitudes.AltInc;
                Vector3 vectorOffset = Vector3.zero;

                GraphicsCache.VisibleMine.DrawFromDef(vector + vectorOffset, Rot4.North, null);

            }
        }

    }
}
