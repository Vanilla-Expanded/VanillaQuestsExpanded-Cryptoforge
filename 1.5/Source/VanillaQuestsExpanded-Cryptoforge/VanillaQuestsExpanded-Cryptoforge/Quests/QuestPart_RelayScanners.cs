using RimWorld;
using RimWorld.QuestGen;
using Verse;
using RimWorld.Planet;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestPart_RelayScanners : QuestPart_SiteKeepWhileQuestActive
    {
        [NoTranslate]
        public string inSignal;
        private int scanSignalsCount = 0;
        public Map siteMap;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref scanSignalsCount, "scanSignalsCount", 0);
            Scribe_References.Look(ref siteMap, "siteMap");
            Scribe_Values.Look(ref inSignal, "inSignal");
        }

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            if (signal.tag == inSignal)
            {
                if (siteMap == null || siteMap != site.Map)
                {
                    scanSignalsCount = 0;
                    siteMap = site.Map;
                }
                scanSignalsCount++;
                if (scanSignalsCount >= 2)
                {
                    this.quest.End(QuestEndOutcome.Success);
                }
            }
        }
    }
}