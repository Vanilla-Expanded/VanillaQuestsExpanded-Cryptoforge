
using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
namespace VanillaQuestsExpandedCryptoforge
{
    public class CompProperties_NonRefuelable : CompProperties
    {
        public float fuelConsumptionRate = 1f;

        public float fuelCapacity = 2f;

        public float initialFuelPercent;

      

        public bool destroyOnNoFuel = true;

    

        public bool showFuelGizmo=true;

    
        public bool drawFuelGaugeInMap=true;

    
        [MustTranslate]
        public string fuelLabel;

        [MustTranslate]
        public string fuelGizmoLabel;

        [MustTranslate]
        public string outOfFuelMessage;

        public string fuelIconPath;

        public bool externalTicking;

        private Texture2D fuelIcon;

        public string FuelLabel
        {
            get
            {
                if (fuelLabel.NullOrEmpty())
                {
                    return "Fuel".TranslateSimple();
                }
                return fuelLabel;
            }
        }

        public string FuelGizmoLabel
        {
            get
            {
                if (fuelGizmoLabel.NullOrEmpty())
                {
                    return "Fuel".TranslateSimple();
                }
                return fuelGizmoLabel;
            }
        }

    

        public CompProperties_NonRefuelable()
        {
            compClass = typeof(CompNonRefuelable);
        }

      
    
    }
}