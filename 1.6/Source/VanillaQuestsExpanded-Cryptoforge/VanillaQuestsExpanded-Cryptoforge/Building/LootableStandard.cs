using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;
using RimWorld;


namespace VanillaQuestsExpandedCryptoforge
{
    public class LootableStandard : Building, IOpenable
    {

        public virtual int OpenTicks => 300;

        public virtual bool CanOpen => true;

        public virtual void Open()
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
                if (contentDetails.buildingLeft != null)
                {
                    Thing palletToMake = GenSpawn.Spawn(ThingMaker.MakeThing(contentDetails.buildingLeft), Position, Map);

                    if (palletToMake.def.CanHaveFaction)
                    {
                        palletToMake.SetFaction(this.Faction);
                    }
                }
                if (this.Spawned)
                {
                    this.Destroy();
                }

            }


        }

    }
}
