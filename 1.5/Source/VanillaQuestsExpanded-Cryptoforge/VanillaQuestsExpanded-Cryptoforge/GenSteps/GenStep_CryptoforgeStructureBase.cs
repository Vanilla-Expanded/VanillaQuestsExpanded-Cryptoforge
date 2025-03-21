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
    public abstract class GenStep_CryptoforgeStructureBase : GenStep
    {
        public override int SeedPart => 46515475;
        protected abstract string LeftDefNamePrefix { get; }
        protected abstract string CenterDefNamePrefix { get; }
        protected abstract string RightDefNamePrefix { get; }

        protected IntVec3 mapCenter; // Added protected mapCenter field

        public override void Generate(Map map, GenStepParams parms)
        {
            string leftDefName = LeftDefNamePrefix + "_LeftSide_" + (Rand.Bool ? "Alpha" : "Beta");
            string centerDefName = CenterDefNamePrefix + "_Center_" + (Rand.Bool ? "Alpha" : "Beta");
            string rightDefName = RightDefNamePrefix + "_RightSide_" + (Rand.Bool ? "Alpha" : "Beta");
            StructureLayoutDef leftDef = DefDatabase<StructureLayoutDef>.GetNamed(leftDefName);
            StructureLayoutDef centerDef = DefDatabase<StructureLayoutDef>.GetNamed(centerDefName);
            StructureLayoutDef rightDef = DefDatabase<StructureLayoutDef>.GetNamed(rightDefName);

            if (leftDef == null || centerDef == null || rightDef == null)
            {
                Log.Error("Failed to load Cryptoforge structure StructureLayoutDefs.");
                return;
            }
            var siteFaction = map.ParentFaction;
            mapCenter = map.Center; // Initialize protected mapCenter field
            var structureSize = leftDef.Sizes;
            var currentPos = mapCenter + new IntVec3(-structureSize.x, 0, 0);
            CellRect leftRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(leftRect, map);
            GenerateLayout(leftDef, leftRect, map, siteFaction);
            currentPos += new IntVec3(leftRect.Width, 0, 0);

            structureSize = centerDef.Sizes;
            CellRect centerRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(centerRect, map);
            GenerateLayout(centerDef, centerRect, map, siteFaction);
            currentPos += new IntVec3(centerRect.Width, 0, 0);

            structureSize = rightDef.Sizes;
            CellRect rightRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(rightRect, map);
            GenerateLayout(rightDef, rightRect, map, siteFaction);

            PostGenerate(map, parms, leftRect, centerRect, rightRect);
        }

        protected virtual void PostGenerate(Map map, GenStepParams parms, CellRect leftRect, CellRect centerRect, CellRect rightRect)
        {
        }

        private void GenerateLayout(StructureLayoutDef structureDef, CellRect rect, Map map, Faction faction)
        {
            structureDef.Generate(rect, map, faction);
        }
    }
}