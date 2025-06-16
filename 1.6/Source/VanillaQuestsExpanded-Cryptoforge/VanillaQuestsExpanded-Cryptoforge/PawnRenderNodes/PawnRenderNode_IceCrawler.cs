using RimWorld;
using UnityEngine;
using Verse;
using VEF;

namespace VanillaQuestsExpandedCryptoforge
{
    public class PawnRenderNode_IceCrawler : PawnRenderNode_AnimalPart
    {
        public PawnRenderNode_IceCrawler(Pawn pawn, PawnRenderNodeProperties props, PawnRenderTree tree)
            : base(pawn, props, tree)
        {
        }

        public override Graphic GraphicFor(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VQE_IceCrawlerHediff) != null)
            {
                Graphic graphic = pawn.ageTracker.CurKindLifeStage.bodyGraphicData.Graphic;
                return GraphicDatabase.Get<Graphic_Multi>(graphic.path + "_Cold", ShaderDatabase.Cutout, graphic.drawSize, graphic.color);
            }

            return base.GraphicFor(pawn);
        }
    }
}
