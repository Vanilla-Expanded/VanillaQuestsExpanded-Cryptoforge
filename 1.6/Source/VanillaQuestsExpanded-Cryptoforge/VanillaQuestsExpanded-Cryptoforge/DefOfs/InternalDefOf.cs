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
		public static ThingDef VQE_FrozenCryptogenerator;
		public static ThingDef VQE_FrozenCryptogenerator_Off;
		public static ThingDef VQE_Cryptofreeze;
		public static ThingDef VQE_CryptoSpark;


		public static DamageDef VQE_ExtinguishCryptofreeze;

		public static JobDef VQE_Loot;
		public static JobDef VQE_StudyBlueprints;
		public static JobDef VQE_InitiateScan;
		public static JobDef VQE_BeatCryptofreeze;
		public static JobDef VQE_RestartGenerator;
		public static SoundDef VQE_BlaringSiren;
		public static SoundDef VQE_MeltdownExplosion_Cryo;
		public static SoundDef VQE_Freezing;
		public static SoundDef VQE_Freezing_Single;
		public static SoundDef VQE_CrystalShatter;
		public static EffecterDef VQE_Effect_IceShatter;
		public static RulePackDef VQE_DamageEvent_CryptoFreeze;
		public static HediffDef VQE_CryptoSlowdown;
		public static HediffDef VQE_IceCrawlerHediff;
		public static HediffDef VQE_MegamidgeHediff;

		public static SitePartDef VQE_CryptoforgeChapter1Site;
		public static SitePartDef VQE_CryptoforgeChapter2Site;
		public static SitePartDef VQE_CryptoforgeChapter3Site;
		public static SitePartDef VQE_CryptoforgeChapter4Site;
		public static ThingDef VQE_AncientCryptoforge, VQE_AncientBlackBox, VQE_FrozenScanningRelay;
		public static ThingDef VQE_TwistedMetal;
		public static ThingDef VQE_LargeTwistedMetal;
		public static ThingDef VQE_HugeTwistedMetal;
		public static RaidStrategyDef ImmediateAttackBreaching;
		public static HediffDef VQE_Guilt;
		public static PawnKindDef VQE_Hero;
		public static ThingDef VQE_FrozenEmptyCryptosleepPod;
		public static ThingDef VQE_AncientLandmine;
		public static RulePackDef VQE_HeroMaleNames, VQE_HeroFemaleNames, VQE_HeroLastNames;
		public static ThingDef VQE_Apparel_CryptoHeavyHelmet;
		public static ThingDef VQE_CryptoHeavyArmor;
	}
}