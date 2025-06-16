using RimWorld;
using Verse;
using KCSG;
using VEF.Buildings;
using VEF;
using System.Collections.Generic;
using System.Linq;
using Verse.AI.Group;

namespace VanillaQuestsExpandedCryptoforge
{
    [HotSwappable]
    public class GenStep_LegendaryCryptoforge : GenStep_CryptoforgeStructureBase
    {
        protected override string LeftDefNamePrefix => "VQE_Cryptoforge_LeftSide";
        protected override string CenterDefNamePrefix => "VQE_Cryptoforge_Center";
        protected override string RightDefNamePrefix => "VQE_Cryptoforge_RightSide";
        protected override void PostGenerate(Map map, GenStepParams parms, CellRect leftRect, CellRect centerRect, CellRect rightRect)
        {
            base.PostGenerate(map, parms, leftRect, centerRect, rightRect);
            map.Parent.SetFaction(Faction.OfMechanoids);
            var cryptoGenerator = map.listerThings.ThingsOfDef(InternalDefOf.VQE_FrozenCryptogenerator_Off).FirstOrDefault();
            cryptoGenerator.TryGetComp<CompBouncingArrow>().doBouncingArrow = true;
        }
    }
}