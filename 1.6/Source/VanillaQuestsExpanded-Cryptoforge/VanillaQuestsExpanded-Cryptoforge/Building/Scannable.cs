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




namespace VanillaQuestsExpandedCryptoforge
{
    public class Scannable : Building
    {

        MapComponent_CryptoBuildingsInMap comp;
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
            comp = Map.GetComponent<MapComponent_CryptoBuildingsInMap>();
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
                        Map.GetComponent<MapComponent_CryptoBuildingsInMap>()?.AddScannablesToMap(this);
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
            Messages.Message("VQE_ScanStarted".Translate(), this, MessageTypeDefOf.NeutralEvent);
        }

        public void Notify_BegingMechRaid()
        {
            Utils.CreateMechRaid(this.Map, 1);

        }

        protected override void Tick()
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
            Messages.Message("VQE_ScanComplete".Translate(), this, MessageTypeDefOf.PositiveEvent);

            var site = Map.Parent;
            var signal = "Scanned_" + this.def.defName;
            Find.SignalManager.SendSignal(new Signal(signal, site.Named("SUBJECT")));
            QuestUtility.SendQuestTargetSignals(site.questTags, signal, site.Named("SUBJECT"));
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
                )
            {
                if (!selPawn.CanReach(this, PathEndMode.OnCell, Danger.Deadly))
                {
                    yield return new FloatMenuOption("CannotUseReason".Translate("NoPath".Translate().CapitalizeFirst()), null);
                }
                else
                {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(contentDetails.gizmoText.Translate().CapitalizeFirst(), delegate
                    {
                        selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.VQE_InitiateScan, this), JobTag.Misc);
                    }), selPawn, this);
                }



            }
        }


    }
}
