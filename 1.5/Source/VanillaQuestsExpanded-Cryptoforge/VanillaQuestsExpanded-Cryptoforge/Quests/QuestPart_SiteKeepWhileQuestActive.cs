using RimWorld;
using RimWorld.Planet;
using Verse;
using System.Linq;

namespace VanillaQuestsExpandedCryptoforge
{
    public class QuestPart_SiteKeepWhileQuestActive : QuestPartActivable
    {
        public MapParent mapParent;
        public Faction siteFaction;
        private int lastTileChecked = -1;
        public Map Map => mapParent?.Map;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look(ref mapParent, "mapParent");
            Scribe_References.Look(ref siteFaction, "siteFaction");
            Scribe_Values.Look(ref lastTileChecked, "lastTileChecked", -1);

            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                if (mapParent != null)
                {
                    lastTileChecked = mapParent.Tile;
                }
            }
        }

        public override void QuestPartTick()
        {
            base.QuestPartTick();

            if (mapParent == null || mapParent.Destroyed)
            {
                if (lastTileChecked != -1)
                {
                    var newMapParent = Find.WorldObjects.MapParentAt(lastTileChecked);
                    if (newMapParent != null && newMapParent != mapParent)
                    {
                        mapParent = newMapParent;
                    }
                    else
                    {
                        mapParent = null;
                    }
                }
                else
                {
                    mapParent = null;
                }
            }
            else if (lastTileChecked == -1)
            {
                lastTileChecked = mapParent.Tile;
            }
        }
    }
}