using Verse;
using RimWorld;
using KCSG;
using System.Collections.Generic;
using System.Linq;

namespace VanillaQuestsExpandedCryptoforge
{
    public class GenStep_CryptoforgeScanningSite : GenStep_CustomStructureGen
    {
        public override int SeedPart => 8675309;

        public override void Generate(Map map, GenStepParams parms)
        {
            var terrainGrid = map.terrainGrid;
            var thingGrid = map.thingGrid;
            var defaultTerrain = TerrainDefOf.Soil;

            foreach (var cell in map.AllCells.ToList())
            {
                var thingsToRemove = new List<Thing>();
                foreach (var thing in thingGrid.ThingsListAt(cell))
                {
                    if (thing.def.mineable || thing.def.IsSmoothable || thing.def.defName.Contains("Chunk") || thing.def.defName.Contains("Mineable"))
                    {
                        thingsToRemove.Add(thing);
                    }
                }
                foreach (var thing in thingsToRemove)
                {
                    thing.Destroy(DestroyMode.Vanish);
                }

                var currentTerrain = terrainGrid.TerrainAt(cell);
                if (currentTerrain.layerable || currentTerrain.smoothedTerrain != null)
                {
                    terrainGrid.SetTerrain(cell, defaultTerrain);
                }
                if (currentTerrain.passability == Traversability.Impassable && !currentTerrain.IsWater)
                {
                    map.terrainGrid.SetTerrain(cell, defaultTerrain);
                }
            }

            foreach (var cell in map.AllCells)
            {
                map.terrainGrid.SetTerrain(cell, map.terrainGrid.TerrainAt(cell).smoothedTerrain ?? map.terrainGrid.TerrainAt(cell));
            }

            base.Generate(map, parms);
        }
    }
}