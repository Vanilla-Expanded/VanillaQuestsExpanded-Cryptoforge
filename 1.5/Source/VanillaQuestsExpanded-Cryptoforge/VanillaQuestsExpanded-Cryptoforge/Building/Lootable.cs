using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;
using Verse.Sound;



namespace VanillaQuestsExpandedCryptoforge
{
    public class Lootable : Building
    {


        MapComponent_LootablesInMap comp;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            comp = Map.GetComponent<MapComponent_LootablesInMap>();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {

            foreach (Gizmo c in base.GetGizmos())
            {
                yield return c;
            }
            Command_Action command_Action = new Command_Action();

            if (comp?.lootables_InMap.Contains(this) == false)
            {
                command_Action.defaultDesc = "VQE_ScavengeDesc".Translate(this.LabelCap);
                
                command_Action.defaultLabel = "VQE_Scavenge".Translate(this.LabelCap);
                command_Action.icon = ContentFinder<Texture2D>.Get("UI/ScavengeJunk_Gizmo", true);
                command_Action.hotKey = KeyBindingDefOf.Misc1;
                command_Action.action = delegate
                {
                    Map.GetComponent<MapComponent_LootablesInMap>()?.AddLootableToMap(this);
                };
            }
            else
            {
                command_Action.defaultDesc = "VQE_ScavengeDesc".Translate(this.LabelCap);

                command_Action.defaultLabel = "VQE_Scavenge".Translate(this.LabelCap);
                command_Action.icon = ContentFinder<Texture2D>.Get("UI/ScavengeJunk_Gizmo", true);
                command_Action.Disabled = true;
            }

            yield return command_Action;

        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
          
            if (comp != null)
            {
                comp.RemoveLootableFromMap(this);
            }
            base.Destroy(mode);

        }

        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {

            if (comp != null)
            {
                comp.RemoveLootableFromMap(this);
            }
            base.Kill(dinfo, exactCulprit);

        }

        protected override void DrawAt(Vector3 drawLoc, bool flip = false)
        {
            base.DrawAt(drawLoc, flip);

            if (comp?.lootables_InMap.Contains(this) == true)
            {
                Vector3 drawPos = DrawPos;
                drawPos.y = AltitudeLayer.MetaOverlays.AltitudeFor() + 0.181818187f;
                float num = ((float)Math.Sin((double)((Time.realtimeSinceStartup + 397f * (float)(thingIDNumber % 571)) * 4f)) + 1f) * 0.5f;
                num = 0.3f + num * 0.7f;
                Material material = FadedMaterialPool.FadedVersionOf(GraphicsCache.ScavengeOverlay, num);
                Graphics.DrawMesh(MeshPool.plane08, drawPos, Quaternion.identity, material, 0);
            }
        }




        public void Open()
        {
            CryptoBuildingDetails contentDetails = this.def.GetModExtension<CryptoBuildingDetails>();
            if (contentDetails != null)
            {
                foreach (ThingAndCount thingDefCount in contentDetails.contents)
                {
                    Thing thingToMake = ThingMaker.MakeThing(thingDefCount.thing, null);
                    thingToMake.stackCount = thingDefCount.count;
                    GenPlace.TryPlaceThing(thingToMake, Position, Map, ThingPlaceMode.Near);


                }
                if(contentDetails.buildingLeft != null)
                {
                    Thing palletToMake = GenSpawn.Spawn(ThingMaker.MakeThing(contentDetails.buildingLeft), Position, Map);

                    if (palletToMake.def.CanHaveFaction)
                    {
                        palletToMake.SetFaction(this.Faction);
                    }
                }
                if(contentDetails.deconstructSound!= null)
                {
                    contentDetails.deconstructSound.PlayOneShot(this);
                }
                
                if (this.Spawned)
                {
                    this.Destroy();
                }

            }


        }

    }
}
