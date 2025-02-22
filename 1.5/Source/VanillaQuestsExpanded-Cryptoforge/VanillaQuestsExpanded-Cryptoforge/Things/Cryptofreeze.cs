﻿
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace VanillaQuestsExpandedCryptoforge
{
    public class Cryptofreeze : AttachableThing, ISizeReporter
    {
        private int ticksSinceSpawn;

        public float fireSize = 0.1f;

        private int ticksSinceSpread;

        private float flammabilityMax = 0.5f;

        public Thing instigator;

        private int ticksUntilSmoke;

        private Sustainer sustainer;

        private static List<Thing> flammableList = new List<Thing>();

        private static int fireCount;

        private static int lastFireCountUpdateTick;

        public const float MinFireSize = 0.1f;

        private const float MinSizeForSpark = 1f;

        private const float TicksBetweenSparksBase = 150f;

        private const float TicksBetweenSparksReductionPerFireSize = 40f;

        private const float MinTicksBetweenSparks = 75f;

        public const float MaxFireSize = 1.75f;

        private const int TicksToBurnFloor = 7500;

        private const int ComplexCalcsInterval = 150;

        private const float MinSizeForIgniteMovables = 0.4f;

        private const float FireBaseGrowthPerTick = 0.00055f;

        private static readonly IntRange SmokeIntervalRange = new IntRange(130, 200);

        private const int SmokeIntervalRandomAddon = 10;

        private const float BaseSkyExtinguishChance = 0.04f;

        private const int BaseSkyExtinguishDamage = 10;

        private const float HeatPerFireSizePerInterval = 160f;

        private const float HeatFactorWhenDoorPresent = 0.15f;

        private const float SnowClearRadiusPerFireSize = 3f;

        private const float SnowClearDepthFactor = 0.1f;

        private const int FireCountParticlesOff = 15;

        public int TicksSinceSpawn => ticksSinceSpawn;

        public override string Label
        {
            get
            {
                if (parent != null)
                {
                    return "FireOn".Translate(parent.LabelCap, parent);
                }
                return def.label;
            }
        }

        public override string InspectStringAddon => "Burning".Translate() + " (" + "FireSizeLower".Translate((fireSize * 100f).ToString("F0")) + ")";

        private float SpreadInterval
        {
            get
            {
                float num = 150f - (fireSize - 1f) * 40f;
                if (num < 75f)
                {
                    num = 75f;
                }
                return num;
            }
        }

        public override bool ShouldBeLitNow()
        {
            return true;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksSinceSpawn, "ticksSinceSpawn", 0);
            Scribe_Values.Look(ref fireSize, "fireSize", 0f);
            Scribe_References.Look(ref instigator, "instigator");
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            RecalcPathsOnAndAroundMe(map);
            LessonAutoActivator.TeachOpportunity(ConceptDefOf.HomeArea, this, OpportunityType.Important);
            ticksSinceSpread = (int)(SpreadInterval * Rand.Value);
        }

        public float CurrentSize()
        {
            return fireSize;
        }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            if (sustainer != null)
            {
                if (sustainer.externalParams.sizeAggregator == null)
                {
                    sustainer.externalParams.sizeAggregator = new SoundSizeAggregator();
                }
                sustainer.externalParams.sizeAggregator.RemoveReporter(this);
            }
            Map map = base.Map;
            base.DeSpawn(mode);
            RecalcPathsOnAndAroundMe(map);
        }

        private void RecalcPathsOnAndAroundMe(Map map)
        {
            IntVec3[] adjacentCellsAndInside = GenAdj.AdjacentCellsAndInside;
            for (int i = 0; i < adjacentCellsAndInside.Length; i++)
            {
                IntVec3 c = base.Position + adjacentCellsAndInside[i];
                if (c.InBounds(map))
                {
                    map.pathing.RecalculatePerceivedPathCostAt(c);
                }
            }
        }

        public override void AttachTo(Thing newParent)
        {
            base.AttachTo(newParent);
            Pawn pawn;
            if ((pawn = newParent as Pawn) != null)
            {
                TaleRecorder.RecordTale(TaleDefOf.WasOnFire, pawn);
            }
        }

        public override void Tick()
        {
            ticksSinceSpawn++;
            if (lastFireCountUpdateTick != Find.TickManager.TicksGame)
            {
                fireCount = base.Map.listerThings.ThingsOfDef(def).Count;
                lastFireCountUpdateTick = Find.TickManager.TicksGame;
            }
            if (sustainer != null && !sustainer.Ended)
            {
                sustainer.Maintain();
            }
            else if (!base.Position.Fogged(base.Map))
            {
                SoundInfo info = SoundInfo.InMap(new TargetInfo(base.Position, base.Map), MaintenanceType.PerTick);
                sustainer = SustainerAggregatorUtility.AggregateOrSpawnSustainerFor(this, InternalDefOf.VQE_Freezing, info);
            }
           
           
            if (fireSize > 1f)
            {
                ticksSinceSpread++;
                if ((float)ticksSinceSpread >= SpreadInterval)
                {
                    TrySpread();
                    ticksSinceSpread = 0;
                }
            }
            if (this.IsHashIntervalTick(150))
            {
                DoComplexCalcs();
            }
            if (ticksSinceSpawn >= 7500)
            {
                TryBurnFloor();
            }
        }

        private void SpawnSmokeParticles()
        {
            if (fireCount < 15)
            {
                FleckMaker.ThrowSmoke(DrawPos, base.Map, fireSize);
            }
            if (fireSize > 0.5f && parent == null)
            {
                FleckMaker.ThrowFireGlow(base.Position.ToVector3Shifted(), base.Map, fireSize);
            }
            float num = fireSize / 2f;
            if (num > 1f)
            {
                num = 1f;
            }
            num = 1f - num;
            ticksUntilSmoke = SmokeIntervalRange.Lerped(num) + (int)(10f * Rand.Value);
        }

        private void DoComplexCalcs()
        {
            bool flag = false;
            flammableList.Clear();
            flammabilityMax = 0f;
            if (!base.Position.GetTerrain(base.Map).extinguishesFire)
            {
                if (parent == null)
                {
                    if (base.Position.TerrainFlammableNow(base.Map))
                    {
                        flammabilityMax = base.Position.GetTerrain(base.Map).GetStatValueAbstract(StatDefOf.Flammability);
                    }
                    List<Thing> list = base.Map.thingGrid.ThingsListAt(base.Position);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Thing thing = list[i];
                        if (thing is Building_Door)
                        {
                            flag = true;
                        }
                        float statValue = thing.GetStatValue(StatDefOf.Flammability);
                        if (!(statValue < 0.01f))
                        {
                            flammableList.Add(list[i]);
                            if (statValue > flammabilityMax)
                            {
                                flammabilityMax = statValue;
                            }
                            if (parent == null && fireSize > 0.4f && list[i].def.category == ThingCategory.Pawn && Rand.Chance(FireUtility.ChanceToAttachFireCumulative(list[i], 150f)))
                            {
                                TryAttachCryptofreeze(list[i],fireSize * 0.2f, instigator);
                            }
                        }
                    }
                }
                else
                {
                    flammableList.Add(parent);
                    flammabilityMax = parent.GetStatValue(StatDefOf.Flammability);
                }
            }
            if (flammabilityMax < 0.01f)
            {
                Destroy();
                return;
            }
            Thing thing2 = ((parent != null) ? parent : ((flammableList.Count <= 0) ? null : flammableList.RandomElement()));
            if (thing2 != null && (!(fireSize < 0.4f) || thing2 == parent || thing2.def.category != ThingCategory.Pawn))
            {
                DoFireDamage(thing2);
            }
            if (base.Spawned)
            {
                float num = fireSize * 160f;
                if (flag)
                {
                    num *= 0.15f;
                }
                GenTemperature.PushHeat(base.Position, base.Map,-num);
                if (Rand.Value < 0.4f)
                {
                    float radius = fireSize * 3f;
                    SnowUtility.AddSnowRadial(base.Position, base.Map, radius, fireSize * 0.1f);
                }
                fireSize += 0.00055f * flammabilityMax * 150f;
                if (fireSize > 1.75f)
                {
                    fireSize = 1.75f;
                }
                
            }
        }

        private void TryBurnFloor()
        {
            if (parent == null && base.Spawned && base.Position.TerrainFlammableNow(base.Map))
            {
                base.Map.terrainGrid.Notify_TerrainBurned(base.Position);
            }
        }

        private bool VulnerableToRain()
        {
            if (!base.Spawned)
            {
                return false;
            }
            RoofDef roofDef = base.Map.roofGrid.RoofAt(base.Position);
            if (roofDef == null)
            {
                return true;
            }
            if (roofDef.isThickRoof)
            {
                return false;
            }
            return base.Position.GetEdifice(base.Map)?.def.holdsRoof ?? false;
        }

        private void DoFireDamage(Thing targ)
        {
            int num = GenMath.RoundRandom(Mathf.Clamp(0.0125f + 0.0036f * fireSize, 0.0125f, 0.05f) * 150f);
            if (num < 1)
            {
                num = 1;
            }
            Pawn pawn;
            if ((pawn = targ as Pawn) != null)
            {
                BattleLogEntry_DamageTaken battleLogEntry_DamageTaken = new BattleLogEntry_DamageTaken(pawn, InternalDefOf.VQE_DamageEvent_CryptoFreeze);
                Find.BattleLog.Add(battleLogEntry_DamageTaken);
                DamageInfo dinfo = new DamageInfo(DamageDefOf.Frostbite, num, 0f, -1f, instigator ?? this);
                dinfo.SetBodyRegion(BodyPartHeight.Undefined, BodyPartDepth.Outside);
                targ.TakeDamage(dinfo).AssociateWithLog(battleLogEntry_DamageTaken);
               
            }
            else
            {
                targ.TakeDamage(new DamageInfo(DamageDefOf.Frostbite, num, 0f, -1f, instigator ?? this));
            }
        }

        protected void TrySpread()
        {
            IntVec3 intVec;
            bool flag;
            if (Rand.Chance(0.8f))
            {
                intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(1, 8)];
                flag = true;
            }
            else
            {
                intVec = base.Position + GenRadial.ManualRadialPattern[Rand.RangeInclusive(10, 20)];
                flag = false;
            }
            if (!intVec.InBounds(base.Map) || !Rand.Chance(ChanceToStartCryptoFreezeIn(intVec, base.Map)))
            {
                return;
            }
            if (!flag)
            {
                CellRect startRect = CellRect.SingleCell(base.Position);
                CellRect endRect = CellRect.SingleCell(intVec);
                if (GenSight.LineOfSight(base.Position, intVec, base.Map, startRect, endRect))
                {
                    ((CryptoSpark)GenSpawn.Spawn(InternalDefOf.VQE_CryptoSpark, base.Position, base.Map)).Launch(this, intVec, intVec, ProjectileHitFlags.All);
                }
            }
            else
            {
                TryStartCryptoFreezeIn(intVec, base.Map, 0.1f, instigator);
            }
        }
        public static bool TryStartCryptoFreezeIn(IntVec3 c, Map map, float fireSize, Thing instigator, SimpleCurve flammabilityChanceCurve = null)
        {
            if (ChanceToStartCryptoFreezeIn(c, map, flammabilityChanceCurve) <= 0f)
            {
                return false;
            }
            Cryptofreeze obj = (Cryptofreeze)ThingMaker.MakeThing(InternalDefOf.VQE_Cryptofreeze);
            obj.fireSize = fireSize;
            obj.instigator = instigator;
            GenSpawn.Spawn(obj, c, map, Rot4.North);
            return true;
        }
        public static float ChanceToStartCryptoFreezeIn(IntVec3 c, Map map, SimpleCurve flammabilityChanceCurve = null)
        {
            List<Thing> thingList = c.GetThingList(map);
            float num = (c.TerrainFlammableNow(map) ? c.GetTerrain(map).GetStatValueAbstract(StatDefOf.Flammability) : 0f);
            for (int i = 0; i < thingList.Count; i++)
            {
                Thing thing = thingList[i];
                if (thing is Cryptofreeze)
                {
                    return 0f;
                }
                if (thing.def.category != ThingCategory.Pawn && thingList[i].FlammableNow)
                {
                    num = Mathf.Max(num, thing.GetStatValue(StatDefOf.Flammability));
                }
            }
            if (flammabilityChanceCurve != null)
            {
                num = flammabilityChanceCurve.Evaluate(num);
            }
            if (num > 0f)
            {
                Building edifice = c.GetEdifice(map);
                if (edifice != null && edifice.def.passability == Traversability.Impassable && edifice.OccupiedRect().ContractedBy(1).Contains(c))
                {
                    return 0f;
                }
                List<Thing> thingList2 = c.GetThingList(map);
                for (int j = 0; j < thingList2.Count; j++)
                {
                    if (thingList2[j].def.category == ThingCategory.Filth && !thingList2[j].def.filth.allowsFire)
                    {
                        return 0f;
                    }
                }
            }
            return num;
        }
        public static void TryAttachCryptofreeze(Thing t, float fireSize, Thing instigator)
        {
            if (t.CanEverAttachFire() && !t.HasAttachment(InternalDefOf.VQE_Cryptofreeze))
            {
                Cryptofreeze obj = (Cryptofreeze)ThingMaker.MakeThing(InternalDefOf.VQE_Cryptofreeze);
                obj.fireSize = fireSize;
                obj.instigator = instigator;
                obj.AttachTo(t);
                GenSpawn.Spawn(obj, t.Position, t.Map, Rot4.North);
                Pawn pawn = t as Pawn;
                if (pawn != null)
                {
                    pawn.jobs.StopAll();
                    pawn.records.Increment(RecordDefOf.TimesOnFire);
                }
            }
        }
    }
}
