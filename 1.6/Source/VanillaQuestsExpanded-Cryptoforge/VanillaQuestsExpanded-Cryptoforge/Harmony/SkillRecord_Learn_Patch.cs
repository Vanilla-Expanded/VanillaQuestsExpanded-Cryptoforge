using HarmonyLib;
using RimWorld;
using Verse;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Reflection;

namespace VanillaQuestsExpandedCryptoforge
{
    [HarmonyPatch(typeof(SkillRecord), "Learn")]
    public static class SkillRecord_Learn_Patch
    {
        public static void RegisterXP(SkillRecord skillRecord, float xp)
        {
            var guiltHediff = skillRecord.Pawn.health.hediffSet.GetFirstHediffOfDef(InternalDefOf.VQE_Guilt) as Hediff_Guilt;
            if (guiltHediff != null)
            {
                guiltHediff.Notify_SkillGained(skillRecord.def, xp);
            }
        }

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool insertedHelper = false;
            var xpSinceLastLevelField = AccessTools.Field(typeof(SkillRecord), "xpSinceLastLevel");
            foreach (var instruction in instructions)
            {
                yield return instruction;
                if (!insertedHelper && instruction.StoresField(xpSinceLastLevelField))
                {
                    insertedHelper = true;
                    MethodInfo helperMethodInfo = AccessTools.Method(typeof(SkillRecord_Learn_Patch), "RegisterXP");
                    yield return new CodeInstruction(OpCodes.Ldarg_0);
                    yield return new CodeInstruction(OpCodes.Ldarg_1);
                    yield return new CodeInstruction(OpCodes.Call, helperMethodInfo);
                }
            }

            if (!insertedHelper)
            {
                Log.Error("Failed to find insertion point for SkillRecord.Learn transpiler.");
            }
        }
    }
}