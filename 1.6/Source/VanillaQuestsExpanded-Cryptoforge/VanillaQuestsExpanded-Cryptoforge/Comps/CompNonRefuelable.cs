
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using System.Security.Cryptography;
using RimWorld;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;
using Verse.Sound;
using UnityEngine.Analytics;
namespace VanillaQuestsExpandedCryptoforge
{
    [StaticConstructorOnStartup]
    public class CompNonRefuelable : ThingComp, IThingGlower
    {
        private float fuel;

        private Sustainer sustainerAmbient;

        private CompFlickable flickComp;

        public const string RefueledSignal = "Refueled";

        public const string RanOutOfFuelSignal = "RanOutOfFuel";

        private static readonly Texture2D SetTargetFuelLevelCommand = ContentFinder<Texture2D>.Get("UI/Commands/SetTargetFuelLevel");

        private static readonly Vector2 FuelBarSize = new Vector2(1f, 0.2f);

        private static readonly Material FuelBarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.6f, 0.56f, 0.13f));

        private static readonly Material FuelBarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));

        public MapComponent_CryptoBuildingsInMap comp;

        public CompProperties_NonRefuelable Props => (CompProperties_NonRefuelable)props;

        public float Fuel => fuel;

        public const int firstRaidPeriod = 7500; // 3 hours
        public const int secondRaidPeriod = 15000; // 6 hours
        public const int thirdRaidPeriod = 22500; // 9 hours

        public int raidTickCounter = 0;

        public bool inConstantRaids = false;


        public float FuelPercentOfMax => fuel / Props.fuelCapacity;

       

        public bool HasFuel
        {
            get
            {
               
                return fuel > 0f;
            }
        }

        private float ConsumptionRatePerTick => Props.fuelConsumptionRate / 60000f;

       


        public bool ShouldBeLitNow()
        {
            return HasFuel;
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
          
            fuel = Props.fuelCapacity * Props.initialFuelPercent;
         
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref fuel, "fuel", 0f);
            Scribe_Values.Look(ref raidTickCounter, "raidTickCounter");
            Scribe_Values.Look(ref inConstantRaids, "inConstantRaids");


        }

        public override void PostDraw()
        {
            base.PostDraw();
           
           
            if (Props.drawFuelGaugeInMap)
            {
                GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
                r.center = parent.DrawPos + Vector3.up * 0.1f;
                r.size = FuelBarSize;
                r.fillPercent = FuelPercentOfMax;
                r.filledMat = FuelBarFilledMat;
                r.unfilledMat = FuelBarUnfilledMat;
                r.margin = 0.15f;
                Rot4 rotation = parent.Rotation;
                rotation.Rotate(RotationDirection.Clockwise);
                r.rotation = rotation;
                GenDraw.DrawFillableBar(r);
            }
        }

       

      

    

        public override void CompTick()
        {
            base.CompTick();

            if(comp is null)
            {
                comp = this.parent.Map.GetComponent<MapComponent_CryptoBuildingsInMap>();

            }
            raidTickCounter++;

            if(raidTickCounter == firstRaidPeriod)
            {
                Utils.CreateMechRaid(this.parent.Map, 0.25f);
            }
            if (raidTickCounter == secondRaidPeriod)
            {
                Utils.CreateMechRaid(this.parent.Map, 0.5f);
            }
            if (raidTickCounter == thirdRaidPeriod)
            {
                Utils.CreateMechRaid(this.parent.Map, 1f);
                inConstantRaids = true;
                raidTickCounter++;
            }
            if(inConstantRaids && raidTickCounter % 7500 == 0)
            {
                Utils.CreateMechRaid(this.parent.Map, 1f);
            }


            ConsumeFuel(ConsumptionRatePerTick);
            if(Fuel< Props.fuelCapacity / 3)
            {
                
                comp?.AddCriticalCryptoGeneratorsToMap(this.parent);
               

                if(sustainerAmbient is null)
                {
                    SoundInfo info = SoundInfo.InMap(this.parent, MaintenanceType.PerTick);
                    sustainerAmbient = InternalDefOf.VQE_BlaringSiren.TrySpawnSustainer(info);

                }
                else
                {
                    sustainerAmbient.Maintain();
                }

            }
           
        }

        public void ConsumeFuel(float amount)
        {
         
            fuel -= amount;
            if (fuel <= 0f)
            {
                fuel = 0f;
                if (Props.destroyOnNoFuel)
                {

                    InternalDefOf.VQE_MeltdownExplosion_Cryo.PlayOneShotOnCamera();


                    int radius = 13;
                    int numCells = GenRadial.NumCellsInRadius(radius);                               

                    Find.CameraDriver.shaker.DoShake(mag: 20f);

                    AccessTools.FieldRef<MoteCounter, int> moteCount = AccessTools.FieldRefAccess<MoteCounter, int>(fieldName: "moteCount");

                    DamageInfo destroyInfo = new DamageInfo(DamageDefOf.Bomb, float.MaxValue, float.MaxValue, instigator: this.parent);
                    GenExplosion.DoExplosion(this.parent.PositionHeld, this.parent.Map, radius, DamageDefOf.Flame, damAmount: 500, applyDamageToExplosionCellsNeighbors: true, chanceToStartFire: 1f, instigator: this.parent);

                    int x = 0;
                    for (int i = 0; i < numCells; i++)
                    {
                        IntVec3 intVec = this.parent.PositionHeld + GenRadial.RadialPattern[i];
                        if (intVec.InBounds(this.parent.Map))
                        {
                            x++;
                            if (x % 50 == 0)
                            {
                                moteCount(this.parent.Map.moteCounter) = 0;
                                Vector3 vc = intVec.ToVector3();
                                FleckMaker.ThrowLightningGlow(vc, this.parent.Map, size: 10f);
                                FleckMaker.ThrowMetaPuff(vc, this.parent.Map);
                            }
                            List<Thing> things = this.parent.Map.thingGrid.ThingsListAtFast(intVec);

                            for (int j = 0; j < things.Count; j++)
                            {
                                Thing thing = things[j];
                                if (thing.def.filth is null && thing.def != InternalDefOf.VQE_Cryptofreeze && thing != this.parent && thing.def != InternalDefOf.VQE_FrozenCryptogenerator_Off && !(thing.def.building?.isNaturalRock ?? false))
                                    thing.TakeDamage(destroyInfo);
                            }

                        }
                    }



                    FloodFillerFog.FloodUnfog(this.parent.PositionHeld, this.parent.Map);

                    GenExplosion.DoExplosion(this.parent.PositionHeld , this.parent.Map, 50, DamageDefOf.Frostbite, damAmount: 15, applyDamageToExplosionCellsNeighbors: true, chanceToStartFire: 0f, instigator: this.parent, postExplosionSpawnThingDef: InternalDefOf.VQE_Cryptofreeze, postExplosionSpawnChance: 1f,explosionSound: InternalDefOf.VQE_Freezing_Single);
    
                    comp?.RemoveCriticalCryptoGeneratorsFromMap(this.parent);
                  
                    if (this.parent.Spawned)
                    {
                        this.parent.DeSpawn();
                    }



                }
                parent.BroadcastCompSignal("RanOutOfFuel");
            }
        }

    

       

        public void Notify_UsedThisTick()
        {
            ConsumeFuel(ConsumptionRatePerTick);
        }

      

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            
            if (Props.showFuelGizmo && Find.Selector.SingleSelectedThing == parent)
            {
                Gizmo_NonRefuelableFuelStatus gizmo_RefuelableFuelStatus = new Gizmo_NonRefuelableFuelStatus();
                gizmo_RefuelableFuelStatus.refuelable = this;
                yield return gizmo_RefuelableFuelStatus;
            }
           
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "DEV: Set fuel to 0";
                command_Action.action = delegate
                {
                    fuel = 0f;
                    parent.BroadcastCompSignal("Refueled");
                };
                yield return command_Action;
                Command_Action command_Action2 = new Command_Action();
                command_Action2.defaultLabel = "DEV: Set fuel to 0.1";
                command_Action2.action = delegate
                {
                    fuel = 0.1f;
                    parent.BroadcastCompSignal("Refueled");
                };
                yield return command_Action2;
                Command_Action command_Action3 = new Command_Action();
                command_Action3.defaultLabel = "DEV: Fuel -20%";
                command_Action3.action = delegate
                {
                    ConsumeFuel(Props.fuelCapacity * 0.2f);
                };
                yield return command_Action3;
                Command_Action command_Action4 = new Command_Action();
                command_Action4.defaultLabel = "DEV: Set fuel to max";
                command_Action4.action = delegate
                {
                    fuel = Props.fuelCapacity;
                    parent.BroadcastCompSignal("Refueled");
                };
                yield return command_Action4;
            }
        }
    }
}