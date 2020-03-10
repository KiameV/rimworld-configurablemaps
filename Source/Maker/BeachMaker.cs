using RimWorld;
using Verse;

namespace ConfigurableMaps.Maker
{
    internal static class CMBeachMaker
    {
		public static void Init(Map map)
		{
			BeachMaker.Init(map);
		}

		public static void Cleanup()
		{
			BeachMaker.Cleanup();
		}

		public static TerrainDef BeachTerrainAt(IntVec3 loc, BiomeDef biome)
		{
			return BeachMaker.BeachTerrainAt(loc, biome);
		}
	}
}
