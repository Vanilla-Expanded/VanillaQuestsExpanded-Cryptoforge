using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;

namespace VanillaQuestsExpandedCryptoforge
{

	[DefOf]
	public static class InternalDefOf
	{
		static InternalDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(InternalDefOf));
		}

		public static ThingDef VQE_ForcedAncientAirlock;
		public static ThingDef VQE_AncientAirlock;
		public static ThingDef VQE_FrozenShipHull;

		public static JobDef VQE_Loot;
        public static JobDef VQE_StudyBlueprints;
        public static JobDef VQE_InitiateScan;


    }
}