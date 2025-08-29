
using RimWorld;
using System.Collections.Generic;
using VEF;
using Verse;
using Verse.AI;
namespace VanillaQuestsExpandedCryptoforge
{
    public static class CryptofreezeUtility
    {

        private static readonly List<Cryptofreeze> fireList = new List<Cryptofreeze>();

        public static bool IsCryptofreezeBurning(this TargetInfo t)
        {
            if (t.HasThing)
            {
                return t.Thing.IsCryptofreezeBurning();
            }
            return t.Cell.ContainsStaticCryptofreeze(t.Map);
        }

        public static bool IsCryptofreezeBurning(this Thing t)
        {
            if (t.Destroyed || !t.Spawned)
            {
                return false;
            }
            if (t.def.size == IntVec2.One)
            {
                if (t is Pawn)
                {
                    return t.HasAttachment(InternalDefOf.VQE_Cryptofreeze);
                }
                return t.Position.ContainsStaticCryptofreeze(t.Map);
            }
            foreach (IntVec3 item in t.OccupiedRect())
            {
                if (item.ContainsStaticCryptofreeze(t.Map))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool ContainsStaticCryptofreeze(this IntVec3 c, Map map)
        {
            List<Thing> list = map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                Cryptofreeze fire = list[i] as Cryptofreeze;
                if (fire != null && fire.parent == null)
                {
                    return true;
                }
            }
            return false;
        }

        public static List<Cryptofreeze> GetCryptofreezeNearCell(this IntVec3 cell, Map map)
        {
            fireList.Clear();
            Room room = RegionAndRoomQuery.RoomAt(cell, map);
            if (room == null || room.Dereferenced || room.Fogged || room.IsHuge || room.TouchesMapEdge)
            {
                Region region = cell.GetRegion(map);
                if (region == null)
                {
                    List<Thing> list = map.thingGrid.ThingsListAt(cell);
                    for (int i = 0; i < list.Count; i++)
                    {
                        Cryptofreeze fire = list[i] as Cryptofreeze;
                        if (fire != null && fire.parent == null)
                        {
                            fireList.Add(fire);
                        }
                    }
                }
                else
                {
                    region.ListerThings.GetThingsOfType(fireList);
                }
            }
            else
            {
                List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
                for (int j = 0; j < containedAndAdjacentThings.Count; j++)
                {
                    Cryptofreeze fire2 = containedAndAdjacentThings[j] as Cryptofreeze;
                    if (fire2 != null)
                    {
                        fireList.Add(fire2);
                    }
                }
            }
            fireList.Shuffle();
            fireList.Swap(0, fireList.FindIndex(0, (Cryptofreeze f) => f.Position == cell));
            return fireList;
        }
    }


}
