using RimWorld;
using RimWorld.QuestGen;
using Verse;
using RimWorld.Planet;
using Verse.Sound;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestPart_LegendaryCryptoforge : QuestPart_SiteKeepWhileQuestActive
    {
        [NoTranslate]
        public string inSignal;
        public Map siteMap;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref siteMap, "siteMap");
            Scribe_Values.Look(ref inSignal, "inSignal");
        }

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (signal.tag == inSignal)
            {
                this.quest.End(QuestEndOutcome.Success, sendLetter: false, playSound: false);
                var key2 = "LetterQuestCompletedLabel";
                var key = "LetterQuestCompletedSuccess" + "\n" + "VQE_QuestCompletedMessage";
                var textLetterDef = LetterDefOf.PositiveEvent;
                SoundDefOf.Quest_Succeded.PlayOneShotOnCamera();
                Find.LetterStack.ReceiveLetter(key2.Translate(), 
                key.Translate(quest.name.CapitalizeFirst()), textLetterDef, null, null, 
                quest, null, null, 0, true);

            }
        }

        public override void QuestPartTick()
        {
            base.QuestPartTick();
            CheckMapSite();
            if (site.Map != null)
            {
                if (siteMap.listerThings.ThingsOfDef(InternalDefOf.VQE_FrozenCryptogenerator_Off).Any() is false
                    && siteMap.listerThings.ThingsOfDef(InternalDefOf.VQE_FrozenCryptogenerator).Any() is false)
                {
                    this.quest.End(QuestEndOutcome.Fail);
                }
            }
        }

        private void CheckMapSite()
        {
            if (site.Map != null)
            {
                if (siteMap == null || siteMap != site.Map)
                {
                    siteMap = site.Map;
                }
            }
            else if (siteMap != null)
            {
                siteMap = null;
            }
        }
    }
}