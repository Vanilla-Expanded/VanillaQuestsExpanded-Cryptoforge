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
    public class Building_TurretGun_Armoured : Building_TurretGun
    {


        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            if (Rand.Chance(0.35f))
            {
                Effecter effecter = EffecterDefOf.Deflect_Metal_Bullet.Spawn();
                effecter.Trigger(this, this);

                absorbed = true;
            }
            else { absorbed = false; }
            

        }
    }
}
