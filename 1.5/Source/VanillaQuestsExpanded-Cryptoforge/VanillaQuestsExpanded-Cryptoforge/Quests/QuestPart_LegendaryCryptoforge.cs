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
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref inSignal, "inSignal");
        }

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            if (signal.tag == inSignal)
            {
                this.quest.End(QuestEndOutcome.Success, sendLetter: false, playSound: false);
                var key2 = "LetterQuestCompletedLabel";
                var key = "LetterQuestCompletedSuccess".Translate(quest.name.CapitalizeFirst()) + "\n\n" + "VQE_QuestCompletedMessage".Translate();
                var textLetterDef = LetterDefOf.PositiveEvent;
                SoundDefOf.Quest_Succeded.PlayOneShotOnCamera();
                Find.LetterStack.ReceiveLetter(key2.Translate(), key, textLetterDef, null, null,
                quest, null, null, 0, true);
            }
        }
    }
}