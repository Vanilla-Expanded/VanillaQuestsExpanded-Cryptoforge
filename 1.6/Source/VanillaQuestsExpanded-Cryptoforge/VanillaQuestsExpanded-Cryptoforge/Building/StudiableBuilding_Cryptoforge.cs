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

using VEF.Buildings;


namespace VanillaQuestsExpandedCryptoforge
{
    public class StudiableBuilding_Cryptoforge : StudiableBuilding
    {
        public override void Study(Pawn pawn)
        {
            var site = Map.Parent;
            var signal = "Studied_" + this.def.defName;
            Find.SignalManager.SendSignal(new Signal(signal, site.Named("SUBJECT")));
            QuestUtility.SendQuestTargetSignals(site.questTags, signal, site.Named("SUBJECT"));
            base.Study(pawn);
            
        }
    }
}
