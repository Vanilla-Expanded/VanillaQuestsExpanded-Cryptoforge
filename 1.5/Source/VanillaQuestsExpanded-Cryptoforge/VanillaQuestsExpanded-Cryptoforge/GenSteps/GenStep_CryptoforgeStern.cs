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
    public class GenStep_CryptoforgeStern : GenStep_CryptoforgeStructureBase
    {
        protected override string LeftDefNamePrefix => "VQE_Cryptoforge_Stern_LeftSide";
        protected override string CenterDefNamePrefix => "VQE_Cryptoforge_Stern_Center";
        protected override string RightDefNamePrefix => "VQE_Cryptoforge_Stern_RightSide";
        protected override void PostGenerate(Map map, GenStepParams parms, CellRect leftRect, CellRect centerRect, CellRect rightRect)
        {
            base.PostGenerate(map, parms, leftRect, centerRect, rightRect);
            QuestPart_CryptoforgeStern questPart = null;
            foreach (var quest in Find.QuestManager.QuestsListForReading)
            {
                List<QuestPart> questParts = quest.PartsListForReading;
                for (int i = 0; i < questParts.Count; i++)
                {
                    if (questParts[i] is QuestPart_CryptoforgeStern sternEnemiesPart && sternEnemiesPart.site == parms.sitePart.site)
                    {
                        questPart = sternEnemiesPart;
                        break;
                    }
                }
            }

            if (questPart.enemyUnitPawns != null && questPart.enemyUnitPawns.Any())
            {
                List<Pawn> enemyPawns = new List<Pawn>();
                foreach (var pawnKindDef in questPart.enemyUnitPawns)
                {
                    Pawn enemyPawn = PawnGenerator.GeneratePawn(pawnKindDef, questPart.siteFaction);
                    enemyPawns.Add(enemyPawn);
                    IntVec3 spawnCell = map.Center;
                    if (GetCombinedRectCells(leftRect, centerRect, rightRect).Where(x => x.Walkable(map)).TryRandomElement(out var randomCell))
                    {
                        spawnCell = randomCell;
                        GenSpawn.Spawn(enemyPawn, spawnCell, map);
                    }
                }
                LordMaker.MakeNewLord(questPart.siteFaction, new LordJob_DefendBaseNoEat(questPart.siteFaction, centerRect.CenterCell), map, enemyPawns);
            }

        }


    }
}