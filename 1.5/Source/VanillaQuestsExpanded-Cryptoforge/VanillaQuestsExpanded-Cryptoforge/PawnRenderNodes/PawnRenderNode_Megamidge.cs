using RimWorld;
using UnityEngine;
using Verse;
using VFECore;

namespace VanillaQuestsExpandedCryptoforge
{
    public class PawnRenderNode_Megamidge : PawnRenderNode_AnimalPart
    {
        public PawnRenderNode_Megamidge(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VQE_MegamidgeHediff) != null)
            {
                Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
                return GraphicDatabase.Get<Graphic_Multi>(graphic.path + "_Cold", ShaderDatabase.Cutout, graphic.drawSize, graphic.color);
            }

            return base.GraphicFor(pawn);
        }
    }
}
