using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ConfigurableMaps.Maker
{
    public class CMGenStep_Terrain : GenStep_Terrain
    {
		private readonly MethodInfo generateRiverLookupTextureMI = typeof(GenStep_Terrain).GetMethod("GenerateRiverLookupTexture", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		private readonly MethodInfo terrainFromMI = typeof(GenStep_Terrain).GetMethod("TerrainFrom", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

		public override void Generate(Map map, GenStepParams parms)
        {
            BeachMaker.Init(map);
            RiverMaker riverMaker = this.GenerateRiver(map);
			List<IntVec3> list = new List<IntVec3>();
			MapGenFloatGrid elevation = MapGenerator.Elevation;
			MapGenFloatGrid fertility = MapGenerator.Fertility;
			MapGenFloatGrid caves = MapGenerator.Caves;
			TerrainGrid terrainGrid = map.terrainGrid;
			foreach (IntVec3 allCell in map.AllCells)
			{
				Building edifice = allCell.GetEdifice(map);
				TerrainDef terrainDef =
					((edifice == null || edifice.def.Fillage != FillCategory.Full) 
					&& !(caves[allCell] > 0f)) 
					  ? (TerrainDef)terrainFromMI.Invoke(this, new object[] { allCell, map, elevation[allCell], fertility[allCell], riverMaker, false }) 
					  : (TerrainDef)terrainFromMI.Invoke(this, new object[] { allCell, map, elevation[allCell], fertility[allCell], riverMaker, true });
				if (terrainDef.IsRiver && edifice != null)
				{
					list.Add(edifice.Position);
					edifice.Destroy();
				}
				terrainGrid.SetTerrain(allCell, terrainDef);
			}
			riverMaker?.ValidatePassage(map);
			RoofCollapseCellsFinder.RemoveBulkCollapsingRoofs(list, map);
			BeachMaker.Cleanup();
			foreach (TerrainPatchMaker terrainPatchMaker in map.Biome.terrainPatchMakers)
			{
				terrainPatchMaker.Cleanup();
			}
		}

		private RiverMaker GenerateRiver(Map map)
		{
			List<Tile.RiverLink> rivers = Find.WorldGrid[map.Tile].Rivers;
			if (rivers == null || rivers.Count == 0)
			{
				return null;
			}
			float angle = Find.WorldGrid.GetHeadingFromTo(map.Tile, rivers.OrderBy((Tile.RiverLink rl) => -rl.river.degradeThreshold).First().neighbor);
			Rot4 a = Find.World.CoastDirectionAt(map.Tile);
			if (a != Rot4.Invalid)
			{
				angle = a.AsAngle + (float)Rand.RangeInclusive(-30, 30);
			}
			RiverMaker riverMaker = new CMRiverMaker(new Vector3(Rand.Range(0.3f, 0.7f) * (float)map.Size.x, 0f, Rand.Range(0.3f, 0.7f) * (float)map.Size.z), angle, rivers.OrderBy((Tile.RiverLink rl) => -rl.river.degradeThreshold).FirstOrDefault().river);
			generateRiverLookupTextureMI.Invoke(this, new object[] { map, riverMaker });
			return riverMaker;
		}
	}
}