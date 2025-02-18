﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;
using static HarmonyLib.Code;
using Verse.Noise;
using Verse.AI;
using ItemProcessor;
using KCSG;



namespace VanillaQuestsExpandedCryptoforge
{
    public class Scannable : Building
    {

        MapComponent_LootablesInMap comp;
        CryptoBuildingDetails contentDetails;
        public bool scanning = false;
        public int tickCounter = 0;
        public const int totalTicks = 7500; // 3 ingame hours
                                           
      
        private static readonly Vector2 BarSize = new Vector2(0.55f, 0.1f);


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref this.scanning, "scanning");
            Scribe_Values.Look(ref this.tickCounter, "tickCounter");

        }

     

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            comp = Map.GetComponent<MapComponent_LootablesInMap>();
            contentDetails = this.def.GetModExtension<CryptoBuildingDetails>();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {

            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }
            if (!scanning) {

                Command_Action command_Action = new Command_Action();

                if (comp?.scannables_InMap.Contains(this) == false)
                {
                    command_Action.defaultDesc = contentDetails.gizmoDesc.Translate();

                    command_Action.defaultLabel = contentDetails.gizmoText.Translate();
                    command_Action.icon = ContentFinder<Texture2D>.Get(contentDetails.gizmoTexture, true);
                    command_Action.hotKey = KeyBindingDefOf.Misc1;
                    command_Action.action = delegate
                    {
                        Map.GetComponent<MapComponent_LootablesInMap>()?.AddScannablesToMap(this);
                    };
                }
                else
                {
                    command_Action.defaultDesc = contentDetails.gizmoDesc.Translate();

                    command_Action.defaultLabel = contentDetails.gizmoText.Translate();
                    command_Action.icon = ContentFinder<Texture2D>.Get(contentDetails.gizmoTexture, true);
                    command_Action.Disabled = true;
                }

                yield return command_Action;

            }
            

        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {

            if (comp != null)
            {
                comp.RemoveScannablesFromMap(this);
            }
            base.Destroy(mode);

        }

        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {

            if (comp != null)
            {
                comp.RemoveScannablesFromMap(this);
            }
            base.Kill(dinfo, exactCulprit);

        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);

            if (!scanning && comp?.scannables_InMap.Contains(this) == true)
            {
                Vector3 drawPos = DrawPos;
                drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.181818187f;
                float num = ((float)Math.Sin((double)((Time.realtimeSinceStartup + 397f * (float)(thingIDNumber % 571)) * 4f)) + 1f) * 0.5f;
                num = 0.3f + num * 0.7f;
                Material material = FadedMaterialPool.FadedVersionOf(GraphicsCache.EnableOverlay, num);
                Graphics.DrawMesh(MeshPool.plane08, drawPos, Quaternion.identity, material, 0);
            }
            if (scanning)
            {
                Vector3 drawPos = this.DrawPos;
                drawPos.y += 0.0454545468f;
                drawPos.z += 0.25f;
                float CalculatedFillPercent = (float)tickCounter/totalTicks;

               
                GenDraw.DrawFillableBar(new GenDraw.FillableBarRequest
                {
                    center = drawPos,
                    size = BarSize,
                    fillPercent = CalculatedFillPercent,
                    filledMat = GraphicsCache.BarFilledMat,
                    unfilledMat = GraphicsCache.BarUnfilledMat,
                    margin = 0.1f,
                    rotation = Rot4.North
                });

            }
        }

        public void Notify_BeginScan()
        {
            scanning = true;
            if (comp != null)
            {
                comp.RemoveScannablesFromMap(this);
            }
        }

        public void Notify_BegingMechRaid()
        {
            float points = StorytellerUtility.DefaultThreatPointsNow(this.Map);

            Faction faction = Find.FactionManager.OfMechanoids;

            StorytellerComp storytellerComp = Find.Storyteller.storytellerComps.First((StorytellerComp x) => x is StorytellerComp_OnOffCycle || x is StorytellerComp_RandomMain);
            IncidentParms parms = storytellerComp.GenerateParms(IncidentCategoryDefOf.ThreatBig, this.Map);
            parms.forced = true;
            parms.target = this.Map;
            parms.points = points;
            parms.faction = faction;
            List<RaidStrategyDef> source = DefDatabase<RaidStrategyDef>.AllDefs.Where((RaidStrategyDef s) => s.Worker.CanUseWith(parms, PawnGroupKindDefOf.Combat)).ToList();
            if (source.Count > 1)
            {
                parms.raidStrategy = source.RandomElement();
            }
            else
            {
                parms.raidStrategy = RaidStrategyDefOf.ImmediateAttack;
            }
            parms.raidArrivalMode = PawnsArrivalModeDefOf.EdgeWalkIn;

            IncidentDefOf.RaidEnemy.Worker.TryExecute(parms);

        }

        public override void Tick()
        {
            base.Tick();

            if (scanning)
            {
                if (tickCounter == 0)
                {
                    Notify_BegingMechRaid();

                }

                tickCounter++;



                if(tickCounter >= totalTicks)
                {
                    scanning = false;
                    Notify_EndScan();
                }
            }
        }


        public void Notify_EndScan()
        {

            if (contentDetails != null)
            {

                if (contentDetails.buildingLeft != null)
                {


                    Thing palletToMake = GenSpawn.Spawn(ThingMaker.MakeThing(contentDetails.buildingLeft), Position, Map, Rotation);

                    if (palletToMake.def.CanHaveFaction)
                    {
                        palletToMake.SetFaction(this.Faction);
                    }
                }
                if (contentDetails.deconstructSound != null)
                {
                    contentDetails.deconstructSound.PlayOneShot(this);
                }
                          

                if (this.Spawned)
                {
                    this.DeSpawn();
                }

            }


        }

        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
        {
            foreach (FloatMenuOption floatMenuOption in base.GetFloatMenuOptions(selPawn))
            {
                yield return floatMenuOption;
            }
            if (!scanning&&selPawn.CanReserve(this) && selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)
                && !selPawn.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
            {
                if (!selPawn.CanReach(this, PathEndMode.OnCell, Danger.Deadly))
                {
                    yield return new FloatMenuOption("CannotUseReason".Translate("NoPath".Translate().CapitalizeFirst()), null);
                }
                else
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(contentDetails.gizmoText.Translate().CapitalizeFirst(), delegate
                    {
                        selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.VQE_StudyBlueprints, this), JobTag.Misc);
                    }), selPawn, this);
                }



            }
        }


    }
}
