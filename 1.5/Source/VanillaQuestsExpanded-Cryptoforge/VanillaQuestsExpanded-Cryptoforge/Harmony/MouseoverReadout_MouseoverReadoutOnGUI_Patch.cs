using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{
    [HarmonyPatch(typeof(MouseoverReadout), nameof(MouseoverReadout.MouseoverReadoutOnGUI))]
    public static class MouseoverReadout_MouseoverReadoutOnGUI_Patch
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var codes = instructions.ToList();
            var getThingListMethod = AccessTools.Method(typeof(GridsUtility), nameof(GridsUtility.GetThingList), new[] { typeof(IntVec3), typeof(Map) });
            var filterLandminesMethod = AccessTools.Method(typeof(MouseoverReadout_MouseoverReadoutOnGUI_Patch), nameof(FilterLandmines));
            bool patched = false;

            for (int i = 0; i < codes.Count; i++)
            {
                var code = codes[i];
                yield return code;

                if (code.Calls(getThingListMethod))
                {
                    yield return new CodeInstruction(OpCodes.Call, filterLandminesMethod);
                    patched = true;
                }
            }

            if (!patched)
            {
                Log.Error("[VQE Cryptoforge] Failed to patch MouseoverReadout.MouseoverReadoutOnGUI to filter landmines.");
            }
        }

        public static List<Thing> FilterLandmines(List<Thing> things)
        {
            return things.Where(t => t.def != InternalDefOf.VQE_AncientLandmine).ToList();
        }
    }
}