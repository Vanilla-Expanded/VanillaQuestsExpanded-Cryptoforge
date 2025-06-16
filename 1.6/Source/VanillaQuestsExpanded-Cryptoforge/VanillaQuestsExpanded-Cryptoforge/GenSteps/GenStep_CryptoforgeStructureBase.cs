using RimWorld;
using Verse;
using KCSG;
using VEF;
using System.Collections.Generic;
using System.Linq;
using Verse.AI.Group;
using UnityEngine;

namespace VanillaQuestsExpandedCryptoforge
{
    [HotSwappable]
    public abstract class GenStep_CryptoforgeStructureBase : GenStep
    {
        public override int SeedPart => 46515475;
        protected abstract string LeftDefNamePrefix { get; }
        protected abstract string CenterDefNamePrefix { get; }
        protected abstract string RightDefNamePrefix { get; }
        public override void Generate(Map map, GenStepParams parms)
        {
            string leftDefName = LeftDefNamePrefix + "_" + (Rand.Bool ? "Alpha" : "Beta");
            string centerDefName = CenterDefNamePrefix + "_" + (Rand.Bool ? "Alpha" : "Beta");
            string rightDefName = RightDefNamePrefix + "_" + (Rand.Bool ? "Alpha" : "Beta");
            KCSG.StructureLayoutDef leftDef = DefDatabase<KCSG.StructureLayoutDef>.GetNamed(leftDefName);
            KCSG.StructureLayoutDef centerDef = DefDatabase<KCSG.StructureLayoutDef>.GetNamed(centerDefName);
            KCSG.StructureLayoutDef rightDef = DefDatabase<KCSG.StructureLayoutDef>.GetNamed(rightDefName);

            if (leftDef == null || centerDef == null || rightDef == null)
            {
                Log.Error("Failed to load Cryptoforge structure StructureLayoutDefs.");
                return;
            }
            var siteFaction = map.ParentFaction;
            var mapCenter = map.Center;
            var structureSize = leftDef.Sizes;
            var currentPos = mapCenter + new IntVec3(-structureSize.x, 0, 0);
            CellRect leftRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(leftRect, map);
            leftDef.Generate(leftRect, map, siteFaction);
            currentPos += new IntVec3(leftRect.Width, 0, 0);

            structureSize = centerDef.Sizes;
            CellRect centerRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(centerRect, map);
            centerDef.Generate(centerRect, map, siteFaction);
            currentPos += new IntVec3(centerRect.Width, 0, 0);

            structureSize = rightDef.Sizes;
            CellRect rightRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(rightRect, map);
            rightDef.Generate(rightRect, map, siteFaction);

            PostGenerate(map, parms, leftRect, centerRect, rightRect);
        }

        protected virtual void PostGenerate(Map map, GenStepParams parms, CellRect leftRect, CellRect centerRect, CellRect rightRect)
        {
            ScatterScrap(map, leftRect, centerRect, rightRect);
        }

        private void ScatterScrap(Map map, CellRect leftRect, CellRect centerRect, CellRect rightRect)
        {
            var mapCells = map.AllCells.Where(x => x.GetThingList(map).Count(x => x is not Filth or Plant) <= 0).ToList();
            var shipCells = GetCombinedRectCells(leftRect, centerRect, rightRect);
            var validCells = mapCells.Except(shipCells).ToList();

            var thingsToScatter = new List<ThingDef>()
            {
                ThingDefOf.ChunkSlagSteel,
                ThingDefOf.ChunkSlagSteel,
                ThingDefOf.ChunkSlagSteel,
                InternalDefOf.VQE_TwistedMetal,
                InternalDefOf.VQE_LargeTwistedMetal,
                InternalDefOf.VQE_HugeTwistedMetal
            };

            var countToScatter = Mathf.RoundToInt(validCells.Count * 0.03f);
            for (int i = 0; i < countToScatter; i++)
            {
                if (!validCells.TryRandomElement(out var cell))
                {
                    break;
                }
                validCells.Remove(cell);

                var thingDef = thingsToScatter.RandomElement();
                GenPlace.TryPlaceThing(ThingMaker.MakeThing(thingDef), cell, map, ThingPlaceMode.Near);
            }
        }

        protected List<IntVec3> GetCombinedRectCells(CellRect leftRect, CellRect centerRect, CellRect rightRect)
        {
            var combinedCells = new List<IntVec3>();
            combinedCells.AddRange(leftRect.Cells);
            combinedCells.AddRange(centerRect.Cells);
            combinedCells.AddRange(rightRect.Cells);
            return combinedCells;
        }
    }
}