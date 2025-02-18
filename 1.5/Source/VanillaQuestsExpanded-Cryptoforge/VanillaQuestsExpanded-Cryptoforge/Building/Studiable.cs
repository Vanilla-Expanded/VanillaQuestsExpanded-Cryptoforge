using System;
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
    public class Studiable : Building
    {


        MapComponent_LootablesInMap comp;
        CryptoBuildingDetails contentDetails;

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
            Command_Action command_Action = new Command_Action();

            if (comp?.studiables_InMap.Contains(this) == false)
            {
                command_Action.defaultDesc = contentDetails.gizmoDesc.Translate();

                command_Action.defaultLabel = contentDetails.gizmoText.Translate();
                command_Action.icon = ContentFinder<Texture2D>.Get(contentDetails.gizmoTexture, true);
                command_Action.hotKey = KeyBindingDefOf.Misc1;
                command_Action.action = delegate
                {
                    Map.GetComponent<MapComponent_LootablesInMap>()?.AddStudiablesToMap(this);
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

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {

            if (comp != null)
            {
                comp.RemoveStudiablesFromMap(this);
            }
            base.Destroy(mode);

        }

        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {

            if (comp != null)
            {
                comp.RemoveStudiablesFromMap(this);
            }
            base.Kill(dinfo, exactCulprit);

        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);

            if (comp?.studiables_InMap.Contains(this) == true)
            {
                Vector3 drawPos = DrawPos;
                drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.181818187f;
                float num = ((float)Math.Sin((double)((Time.realtimeSinceStartup + 397f * (float)(thingIDNumber % 571)) * 4f)) + 1f) * 0.5f;
                num = 0.3f + num * 0.7f;
                Material material = FadedMaterialPool.FadedVersionOf(GraphicsCache.StudyOverlay, num);
                Graphics.DrawMesh(MeshPool.plane08, drawPos, Quaternion.identity, material, 0);
            }
        }

        public void Study(Pawn pawn)
        {
            
            if (contentDetails != null)
            {
              
                if (contentDetails.buildingLeft != null)
                {
                 

                    Thing palletToMake = GenSpawn.Spawn(ThingMaker.MakeThing(contentDetails.buildingLeft), Position, Map,Rotation);

                    if (palletToMake.def.CanHaveFaction)
                    {
                        palletToMake.SetFaction(this.Faction);
                    }
                }
                if (contentDetails.deconstructSound != null)
                {
                    contentDetails.deconstructSound.PlayOneShot(this);
                }

                if (contentDetails.craftingInspiration)
                {
                    pawn.mindState.inspirationHandler.TryStartInspiration(InspirationDefOf.Inspired_Creativity);
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
            if (selPawn.CanReserve(this) && selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation)
                && !selPawn.skills.GetSkill(SkillDefOf.Intellectual).TotallyDisabled)
            {
                if (!selPawn.CanReach(this, PathEndMode.OnCell, Danger.Deadly))
                {
                    yield return new FloatMenuOption("CannotUseReason".Translate("NoPath".Translate().CapitalizeFirst()), null);
                }
                else {
                    yield return FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(contentDetails.gizmoText.Translate().CapitalizeFirst(), delegate
                    {
                        selPawn.jobs.TryTakeOrderedJob(JobMaker.MakeJob(InternalDefOf.VQE_StudyBlueprints, this), JobTag.Misc);
                    }), selPawn, this);
                }
                
                   
                
            }
        }


    }
}
