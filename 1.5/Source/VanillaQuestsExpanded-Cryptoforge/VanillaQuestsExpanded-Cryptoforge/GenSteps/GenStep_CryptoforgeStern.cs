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
    public class GenStep_CryptoforgeStern : GenStep
    {
        public override int SeedPart => 46515475;
        public override void Generate(Map map, GenStepParams parms)
        {
            QuestPart_CryptoforgeSternEnemies questPart = null;
            foreach (var quest in Find.QuestManager.QuestsListForReading)
            {
                List<QuestPart> questParts = quest.PartsListForReading;
                for (int i = 0; i < questParts.Count; i++)
                {
                    if (questParts[i] is QuestPart_CryptoforgeSternEnemies sternEnemiesPart && sternEnemiesPart.site == parms.sitePart.site)
                    {
                        questPart = sternEnemiesPart;
                        break;
                    }
                }
            }

            if (questPart is null)
            {
                Log.Error("CryptoforgeStern quest generated without corresponding quest part.");
                return;
            }

            string leftSternDefName = "VQE_Cryptoforge_Stern_LeftSide_" + (Rand.Bool ? "Alpha" : "Beta");
            string centerSternDefName = "VQE_Cryptoforge_Stern_Center_" + (Rand.Bool ? "Alpha" : "Beta");
            string rightSternDefName = "VQE_Cryptoforge_Stern_RightSide_" + (Rand.Bool ? "Alpha" : "Beta");
            StructureLayoutDef leftSternDef = DefDatabase<StructureLayoutDef>.GetNamed(leftSternDefName);
            StructureLayoutDef centerSternDef = DefDatabase<StructureLayoutDef>.GetNamed(centerSternDefName);
            StructureLayoutDef rightSternDef = DefDatabase<StructureLayoutDef>.GetNamed(rightSternDefName);

            if (leftSternDef == null || centerSternDef == null || rightSternDef == null)
            {
                Log.Error("Failed to load Cryptoforge Stern StructureLayoutDefs.");
                return;
            }
            var siteFaction = questPart.siteFaction;
            var mapCenter = map.Center;
            var structureSize = leftSternDef.Sizes;
            var currentPos = mapCenter + new IntVec3(-structureSize.x, 0, 0);
            CellRect leftRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(leftRect, map);
            leftSternDef.Generate(leftRect, map, siteFaction);
            currentPos += new IntVec3(leftRect.Width, 0, 0);

            structureSize = centerSternDef.Sizes;
            CellRect centerRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(centerRect, map);
            centerSternDef.Generate(centerRect, map, siteFaction);
            currentPos += new IntVec3(centerRect.Width, 0, 0);

            structureSize = rightSternDef.Sizes;
            CellRect rightRect = CellRect.CenteredOn(currentPos, structureSize);
            GenOption.GetAllMineableIn(rightRect, map);
            rightSternDef.Generate(rightRect, map, siteFaction);
            List<IntVec3> combinedRectCells = GetCombinedRectCells(leftRect, centerRect, rightRect);
            if (questPart.enemyUnitPawns != null && questPart.enemyUnitPawns.Any())
            {
                List<Pawn> enemyPawns = new List<Pawn>();
                foreach (var pawnKindDef in questPart.enemyUnitPawns)
                {
                    Pawn enemyPawn = PawnGenerator.GeneratePawn(pawnKindDef, siteFaction);
                    enemyPawns.Add(enemyPawn);
                    IntVec3 spawnCell = mapCenter;
                    if (combinedRectCells.TryRandomElement(out var randomCell))
                    {
                        if (CellFinder.TryRandomClosewalkCellNear(randomCell, map, 5, out var walkableCell))
                        {
                            spawnCell = walkableCell;
                        }
                    }
                    GenSpawn.Spawn(enemyPawn, spawnCell, map);
                }
                LordMaker.MakeNewLord(siteFaction, new LordJob_DefendBaseNoEat(siteFaction, mapCenter), map, enemyPawns);
            }
        }

        private List<IntVec3> GetCombinedRectCells(CellRect leftRect, CellRect centerRect, CellRect rightRect)
        {
            var combinedCells = new List<IntVec3>();
            combinedCells.AddRange(leftRect.Cells);
            combinedCells.AddRange(centerRect.Cells);
            combinedCells.AddRange(rightRect.Cells);
            return combinedCells;
        }
    }
}