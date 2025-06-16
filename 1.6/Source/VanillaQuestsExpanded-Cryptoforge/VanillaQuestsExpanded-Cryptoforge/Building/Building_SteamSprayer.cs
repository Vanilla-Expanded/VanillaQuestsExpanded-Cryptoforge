
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace VanillaQuestsExpandedCryptoforge
{

    public class Building_SteamSprayer : Building
    {

      
        public ConstantSprayer sprayer;
        public CompPowerTrader compPower;


        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            sprayer = new ConstantSprayer(this);
            compPower = this.TryGetComp<CompPowerTrader>();
        }

        protected override void Tick()
        {
            base.Tick();


            if (compPower?.PowerOn==true)
            {
                sprayer.SteamSprayerTick();
              
            }
          
        }


    }


}
