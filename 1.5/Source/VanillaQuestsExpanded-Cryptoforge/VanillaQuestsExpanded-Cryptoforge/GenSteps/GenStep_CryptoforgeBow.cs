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
            QuestPart_CryptoforgeBow questPart = null;
            foreach (var quest in Find.QuestManager.QuestsListForReading)
            {
                List<QuestPart> questParts = quest.PartsListForReading;
                for (int i = 0; i < questParts.Count; i++)
                {
                    if (questParts[i] is QuestPart_CryptoforgeBow bowQuestPart && bowQuestPart.site == parms.sitePart.site)
                    {
                        questPart = bowQuestPart;
                        break;
                    }
                }
            }

            if (questPart is null)
            {
                Log.Error("CryptoforgeBow quest generated without corresponding quest part.");
                return;
            }

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
            var siteFaction = questPart.siteFaction;
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
            List<IntVec3> combinedRectCells = GetCombinedRectCells(leftRect, centerRect, rightRect);
            if (questPart.enemyUnitPawns != null && questPart.enemyUnitPawns.Any())
            {
                List<Pawn> enemyPawns = new List<Pawn>();
                foreach (var pawnKindDef in questPart.enemyUnitPawns)
                {
                    Pawn enemyPawn = PawnGenerator.GeneratePawn(pawnKindDef, siteFaction);
                    enemyPawns.Add(enemyPawn);
                    IntVec3 spawnCell = mapCenter;
                    if (combinedRectCells.Where(x => x.Walkable(map)).TryRandomElement(out var randomCell))
                    {
                        spawnCell = randomCell;
                        GenSpawn.Spawn(enemyPawn, spawnCell, map);
                    }
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