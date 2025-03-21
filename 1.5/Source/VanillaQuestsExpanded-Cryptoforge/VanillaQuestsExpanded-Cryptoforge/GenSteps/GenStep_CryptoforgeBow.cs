using RimWorld;
using Verse;
using KCSG;
using VFECore;
using System.Collections.Generic;
using System.Linq;
using Verse.AI.Group;

namespace VanillaQuestsExpandedCryptoforge
{
    [HotSwappable]
    public class GenStep_CryptoforgeBow : GenStep
    {
        public override int SeedPart => 46515475;
        public override void Generate(Map map, GenStepParams parms)
        {
            string leftBowDefName = "VQE_Cryptoforge_Bow_LeftSide_" + (Rand.Bool ? "Alpha" : "Beta");
            string centerBowDefName = "VQE_Cryptoforge_Bow_Center_" + (Rand.Bool ? "Alpha" : "Beta");
            string rightBowDefName = "VQE_Cryptoforge_Bow_RightSide_" + (Rand.Bool ? "Alpha" : "Beta");
            StructureLayoutDef leftBowDef = DefDatabase<StructureLayoutDef>.GetNamed(leftBowDefName);
            StructureLayoutDef centerBowDef = DefDatabase<StructureLayoutDef>.GetNamed(centerBowDefName);
            StructureLayoutDef rightBowDef = DefDatabase<StructureLayoutDef>.GetNamed(rightBowDefName);

            if (leftBowDef == null || centerBowDef == null || rightBowDef == null)
            {
                Log.Error("Failed to load Cryptoforge Bow StructureLayoutDefs.");
                return;
            }
            var siteFaction = Faction.OfInsects;
            var mapCenter = map.Center;
            var structureSize = leftBowDef.Sizes;
            var currentPos = mapCenter + new IntVec3(-structureSize.x, 0, 0);
            CellRect leftRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(leftRect, map);
            leftBowDef.Generate(leftRect, map, siteFaction);
            currentPos += new IntVec3(leftRect.Width, 0, 0);

            structureSize = centerBowDef.Sizes;
            CellRect centerRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(centerRect, map);
            centerBowDef.Generate(centerRect, map, siteFaction);
            currentPos += new IntVec3(centerRect.Width, 0, 0);

            structureSize = rightBowDef.Sizes;
            CellRect rightRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(rightRect, map);
            rightBowDef.Generate(rightRect, map, siteFaction);
        }
    }
}