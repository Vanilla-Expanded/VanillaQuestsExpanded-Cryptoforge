using RimWorld;
using RimWorld.QuestGen;
using Verse;
using RimWorld.Planet;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestPart_EndQuestOnScanSignals : QuestPart_SiteKeepWhileQuestActive
    {
        [NoTranslate]
        public string inSignal;
        private int scanSignalsCount = 0;
        public int maxSignalsCount;
        public Map siteMap;
        public ThingDef scanningBuilding;
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref scanSignalsCount, "scanSignalsCount");
            Scribe_Values.Look(ref maxSignalsCount, "maxSignalsCount");
            Scribe_References.Look(ref siteMap, "siteMap");
            Scribe_Values.Look(ref inSignal, "inSignal");
            Scribe_Defs.Look(ref scanningBuilding, "scanningBuilding");
        }

        public override void QuestPartTick()
        {
            base.QuestPartTick();
            CheckMapSite();
            if (siteMap != null)
            {
                var count = siteMap.listerThings.ThingsOfDef(scanningBuilding).Count;
                if (count < maxSignalsCount - scanSignalsCount)
                {
                    this.quest.End(QuestEndOutcome.Fail);
                }
            }
        }

        public override void Notify_QuestSignalReceived(Signal signal)
        {
            base.Notify_QuestSignalReceived(signal);
            CheckMapSite();
            if (signal.tag == inSignal)
            {
                scanSignalsCount++;
                if (scanSignalsCount >= maxSignalsCount)
                {
                    this.quest.End(QuestEndOutcome.Success);
                }
            }
        }

        private void CheckMapSite()
        {
            if (Map != null)
            {
                if (siteMap == null || siteMap != Map)
                {
                    scanSignalsCount = 0;
                    siteMap = Map;
                }
            }
            else if (siteMap != null)
            {
                scanSignalsCount = 0;
                siteMap = null;
            }
        }
    }
}