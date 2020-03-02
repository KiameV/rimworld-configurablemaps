using ConfigurableMaps.Settings;
using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ConfigurableMaps.Maker
{
    /*
    public class CMGenStep_Terrain : GenStep
    {
        public CMGenStep_Terrain() { }

        public override int SeedPart
        {
            get
            {
#if DEBUG
            Log.Warning("CMGenStep_Terrain.SeedPart Prefix");
#endif
                return "ConfigurableMaps.CMGenStep_Terrain".GetHashCode();
            }
        }

        public override void Generate(Map map, GenStepParams parms)
        {
#if DEBUG
            Log.Warning("CMGenStep_Terrain.Generate Prefix");
#endif
            CMBeachMaker.Init(map);
            CMRiverMaker riverMaker = this.GenerateRiver(map);
            List<IntVec3> intVec3s = new List<IntVec3>();
            MapGenFloatGrid elevation = MapGenerator.Elevation;
            MapGenFloatGrid fertility = MapGenerator.Fertility;
            MapGenFloatGrid caves = MapGenerator.Caves;
            TerrainGrid terrainGrid = map.terrainGrid;
            foreach (IntVec3 allCell in map.AllCells)
            {
                Building edifice = allCell.GetEdifice(map);
                TerrainDef terrainDef = null;
                terrainDef = ((edifice == null || edifice.def.Fillage != FillCategory.Full) && caves[allCell] <= 0f ? this.TerrainFrom(allCell, map, elevation[allCell], fertility[allCell], riverMaker, false) : this.TerrainFrom(allCell, map, elevation[allCell], fertility[allCell], riverMaker, true));
                if (terrainDef == TerrainDefOf.WaterMovingShallow || terrainDef == TerrainDefOf.WaterMovingChestDeep)
                {
                    if (edifice != null)
                    {
                        intVec3s.Add(edifice.Position);
                        edifice.Destroy(DestroyMode.Vanish);
                    }
                }
                terrainGrid.SetTerrain(allCell, terrainDef);
            }
            if (riverMaker != null)
            {
                riverMaker.ValidatePassage(map);
            }
            RoofCollapseCellsFinder.RemoveBulkCollapsingRoofs(intVec3s, map);
            CMBeachMaker.Cleanup();
            foreach (TerrainPatchMaker terrainPatchMaker in map.Biome.terrainPatchMakers)
            {
                terrainPatchMaker.Cleanup();
            }
        }

        private CMRiverMaker GenerateRiver(Map map)
        {
            CMRiverMaker riverMaker;
            List<Tile.RiverLink> visibleRivers = Find.WorldGrid[map.Tile].potentialRivers;
            if (visibleRivers == null || visibleRivers.Count == 0)
            {
                riverMaker = null;
            }
            else
            {
                WorldGrid worldGrid = Find.WorldGrid;
                int tile = map.Tile;
                Tile.RiverLink riverLink = (
                    from rl in visibleRivers
                    orderby -rl.river.degradeThreshold
                    select rl).First<Tile.RiverLink>();
                float headingFromTo = worldGrid.GetHeadingFromTo(tile, riverLink.neighbor);
                float single = Rand.Range(0.3f, 0.7f) * (float)map.Size.x;
                float single1 = Rand.Range(0.3f, 0.7f);
                IntVec3 size = map.Size;
                Vector3 vector3 = new Vector3(single, 0f, single1 * (float)size.z);
                Vector3 vector31 = vector3;
                float single2 = headingFromTo;
                Tile.RiverLink riverLink1 = (
                from rl in visibleRivers
                orderby -rl.river.degradeThreshold
                select rl).FirstOrDefault<Tile.RiverLink>();
                CMRiverMaker riverMaker1 = new CMRiverMaker(vector31, single2, riverLink1.river);
                this.GenerateRiverLookupTexture(map, riverMaker1);
                riverMaker = riverMaker1;
            }
            return riverMaker;
        }
        private void GenerateRiverLookupTexture(Map map, CMRiverMaker riverMaker)
        {
            int num = Mathf.CeilToInt((
            from rd in DefDatabase<RiverDef>.AllDefs
            select rd.widthOnMap / 2f + 5f).Max());
            int num1 = Mathf.Max(4, num) * 2;
            Dictionary<int, GRLT_Entry> nums = new Dictionary<int, GRLT_Entry>();
            Dictionary<int, GRLT_Entry> nums1 = new Dictionary<int, GRLT_Entry>();
            Dictionary<int, GRLT_Entry> nums2 = new Dictionary<int, GRLT_Entry>();
            for (int i = -num1; i < map.Size.z + num1; i++)
            {
                for (int j = -num1; j < map.Size.x + num1; j++)
                {
                    IntVec3 intVec3 = new IntVec3(j, 0, i);
                    Vector3 vector3 = riverMaker.WaterCoordinateAt(intVec3);
                    int num2 = Mathf.FloorToInt(vector3.z / 4f);
                    this.UpdateRiverAnchorEntry(nums, intVec3, num2, (vector3.z + Mathf.Abs(vector3.x)) / 4f);
                    this.UpdateRiverAnchorEntry(nums1, intVec3, num2, (vector3.z + Mathf.Abs(vector3.x - (float)num)) / 4f);
                    this.UpdateRiverAnchorEntry(nums2, intVec3, num2, (vector3.z + Mathf.Abs(vector3.x + (float)num)) / 4f);
                }
            }
            int num3 = Mathf.Max(new int[] { nums.Keys.Min(), nums1.Keys.Min(), nums2.Keys.Min() });
            int num4 = Mathf.Min(new int[] { nums.Keys.Max(), nums1.Keys.Max(), nums2.Keys.Max() });
            for (int k = num3; k < num4; k++)
            {
                WaterInfo waterInfo = map.waterInfo;
                if (nums1.ContainsKey(k) && nums1.ContainsKey(k + 1))
                {
                    waterInfo.riverDebugData.Add(nums1[k].bestNode.ToVector3Shifted());
                    List<Vector3> vector3s = waterInfo.riverDebugData;
                    GRLT_Entry item = nums1[k + 1];
                    vector3s.Add(item.bestNode.ToVector3Shifted());
                }
                if (nums.ContainsKey(k) && nums.ContainsKey(k + 1))
                {
                    waterInfo.riverDebugData.Add(nums[k].bestNode.ToVector3Shifted());
                    List<Vector3> vector3s1 = waterInfo.riverDebugData;
                    GRLT_Entry gRLTEntry = nums[k + 1];
                    vector3s1.Add(gRLTEntry.bestNode.ToVector3Shifted());
                }
                if (nums2.ContainsKey(k) && nums2.ContainsKey(k + 1))
                {
                    waterInfo.riverDebugData.Add(nums2[k].bestNode.ToVector3Shifted());
                    List<Vector3> vector3s2 = waterInfo.riverDebugData;
                    GRLT_Entry item1 = nums2[k + 1];
                    vector3s2.Add(item1.bestNode.ToVector3Shifted());
                }
                if (nums1.ContainsKey(k) && nums.ContainsKey(k))
                {
                    waterInfo.riverDebugData.Add(nums1[k].bestNode.ToVector3Shifted());
                    waterInfo.riverDebugData.Add(nums[k].bestNode.ToVector3Shifted());
                }
                if (nums.ContainsKey(k) && nums2.ContainsKey(k))
                {
                    waterInfo.riverDebugData.Add(nums[k].bestNode.ToVector3Shifted());
                    waterInfo.riverDebugData.Add(nums2[k].bestNode.ToVector3Shifted());
                }
            }
            IntVec3 size = map.Size;
            IntVec3 size1 = map.Size;
            CellRect cellRect = new CellRect(-2, -2, size.x + 4, size1.z + 4);
            float[] singleArray = new float[cellRect.Area * 2];
            int num5 = 0;
            for (int l = cellRect.minZ; l <= cellRect.maxZ; l++)
            {
                for (int m = cellRect.minX; m <= cellRect.maxX; m++)
                {
                    IntVec3 intVec31 = new IntVec3(m, 0, l);
                    bool flag = true;
                    int num6 = 0;
                    while (num6 < (int)GenAdj.AdjacentCellsAndInside.Length)
                    {
                        if (riverMaker.TerrainAt(intVec31 + GenAdj.AdjacentCellsAndInside[num6], false) == null)
                        {
                            num6++;
                        }
                        else
                        {
                            flag = false;
                            break;
                        }
                    }
                    if (!flag)
                    {
                        Vector2 vector2 = intVec31.ToIntVec2.ToVector2();
                        int num7 = -2147483648;
                        Vector2 vector21 = Vector2.zero;
                        int num8 = num3;
                        while (num8 < num4)
                        {
                            Vector2 vector22 = nums1[num8].bestNode.ToIntVec2.ToVector2();
                            GRLT_Entry gRLTEntry1 = nums1[num8 + 1];
                            Vector2 vector23 = gRLTEntry1.bestNode.ToIntVec2.ToVector2();
                            Vector2 vector24 = nums[num8].bestNode.ToIntVec2.ToVector2();
                            GRLT_Entry item2 = nums[num8 + 1];
                            Vector2 vector25 = item2.bestNode.ToIntVec2.ToVector2();
                            Vector2 vector26 = nums2[num8].bestNode.ToIntVec2.ToVector2();
                            GRLT_Entry gRLTEntry2 = nums2[num8 + 1];
                            Vector2 vector27 = gRLTEntry2.bestNode.ToIntVec2.ToVector2();
                            Vector2 vector28 = GenGeo.InverseQuadBilinear(vector2, vector24, vector22, vector25, vector23);
                            if (vector28.x < -0.0001f || vector28.x > 1.0001f || vector28.y < -0.0001f || vector28.y > 1.0001f)
                            {
                                Vector2 vector29 = GenGeo.InverseQuadBilinear(vector2, vector24, vector26, vector25, vector27);
                                if (vector29.x < -0.0001f || vector29.x > 1.0001f || vector29.y < -0.0001f || vector29.y > 1.0001f)
                                {
                                    num8++;
                                }
                                else
                                {
                                    vector21 = new Vector2(vector29.x * (float)num, (vector29.y + (float)num8) * 4f);
                                    num7 = num8;
                                    break;
                                }
                            }
                            else
                            {
                                vector21 = new Vector2(-vector28.x * (float)num, (vector28.y + (float)num8) * 4f);
                                num7 = num8;
                                break;
                            }
                        }
                        if (num7 == -2147483648)
                        {
                            Log.ErrorOnce("Failed to find all necessary river flow data", 5273133);
                        }
                        singleArray[num5] = vector21.x;
                        singleArray[num5 + 1] = vector21.y;
                    }
                    num5 += 2;
                }
            }
            float[] singleArray1 = new float[cellRect.Area * 2];
            float[] singleArray2 = new float[] { 0.123317f, 0.123317f, 0.123317f, 0.123317f, 0.077847f, 0.077847f, 0.077847f, 0.077847f, 0.195346f };
            int num9 = 0;
            for (int n = cellRect.minZ; n <= cellRect.maxZ; n++)
            {
                for (int o = cellRect.minX; o <= cellRect.maxX; o++)
                {
                    IntVec3 intVec32 = new IntVec3(o, 0, n);
                    float single = 0f;
                    float single1 = 0f;
                    float single2 = 0f;
                    for (int p = 0; p < (int)GenAdj.AdjacentCellsAndInside.Length; p++)
                    {
                        if (cellRect.Contains(intVec32 + GenAdj.AdjacentCellsAndInside[p]))
                        {
                            int adjacentCellsAndInside = num9 + (GenAdj.AdjacentCellsAndInside[p].x + GenAdj.AdjacentCellsAndInside[p].z * cellRect.Width) * 2;
                            if ((int)singleArray.Length <= adjacentCellsAndInside + 1 || adjacentCellsAndInside < 0)
                            {
                                Log.Message("you wut");
                            }
                            if (singleArray[adjacentCellsAndInside] != 0f || singleArray[adjacentCellsAndInside + 1] != 0f)
                            {
                                single = single + singleArray[adjacentCellsAndInside] * singleArray2[p];
                                single1 = single1 + singleArray[adjacentCellsAndInside + 1] * singleArray2[p];
                                single2 += singleArray2[p];
                            }
                        }
                    }
                    if (single2 > 0f)
                    {
                        singleArray1[num9] = single / single2;
                        singleArray1[num9 + 1] = single1 / single2;
                    }
                    num9 += 2;
                }
            }
            singleArray = singleArray1;
            for (int q = 0; q < (int)singleArray.Length; q += 2)
            {
                if (singleArray[q] != 0f || singleArray[q + 1] != 0f)
                {
                    Vector3 pointOnDisc = PointOnDisc() * 0.4f;
                    singleArray[q] += pointOnDisc.x;
                    singleArray[q + 1] += pointOnDisc.z;
                }
            }
            byte[] numArray = new byte[(int)singleArray.Length * 4];
            Buffer.BlockCopy(singleArray, 0, numArray, 0, (int)singleArray.Length * 4);
            map.waterInfo.riverOffsetMap = numArray;
            map.waterInfo.GenerateRiverFlowMap();
        }
        private static Vector3 PointOnDisc()
        {
            Vector3 result;
            do
            {
                result = new Vector3(Rand.Value - 0.5f, 0f, Rand.Value - 0.5f) * 2f;
            }
            while (result.sqrMagnitude > 1f);
            return result;
        }
        private TerrainDef TerrainFrom(IntVec3 c, Map map, float elevation, float fertility, CMRiverMaker river, bool preferSolid)
        {
            if (map.Biome.defName == "RWBCavern")
            {
                TerrainDef terrainDefCavern;
                for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)
                {
                    terrainDefCavern = map.Biome.terrainPatchMakers[i].TerrainAt(c, map, fertility);
                    if (terrainDefCavern != null)
                    {
                        if (c.GetFirstBuilding(map) != null)
                        {
                            c.GetFirstBuilding(map).Destroy();
                        }
                        map.roofGrid.SetRoof(c, RoofDefOf.RoofRockThick);
                        return terrainDefCavern;
                    }
                }
            }
            if (map.Biome.defName.Contains("Archipelago"))
            {
                // *
                // Coast Level (for Islands)
                //
                float adjustment = 0.0f;
                if (TerrainSettings.coastLevel < 1) { }
                else if (TerrainSettings.coastLevel < 2) { adjustment = 0.02f; }
                else if (TerrainSettings.coastLevel < 3) { adjustment = 0.04f; }
                else if (TerrainSettings.coastLevel < 4) { adjustment = 0.06f; }
                else if (TerrainSettings.coastLevel < 5) { adjustment = 0.08f; }
                else if (TerrainSettings.coastLevel < 6) { adjustment = 0.10f; }
                else { adjustment = 0.12f; }
                bool lakeIsles = false;
                if (map.Biome.defName.Contains("_Fresh")) { lakeIsles = true; }
                if (elevation < 0.75f)
                {
                    Building edifice = c.GetEdifice(map);
                    if (edifice != null) { edifice.Destroy(DestroyMode.Vanish); }
                    map.roofGrid.SetRoof(c, null);
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            int x = c.x + i;
                            int z = c.z + j;
                            if (x >= 0 && z >= 0 && x < map.Size.x && z < map.Size.z)
                            {
                                IntVec3 newSpot = new IntVec3(x, 0, z);
                                if (map.roofGrid.RoofAt(newSpot) != null)
                                {
                                    if (map.roofGrid.RoofAt(newSpot).isThickRoof)
                                    {
                                        map.roofGrid.SetRoof(newSpot, RoofDefOf.RoofRockThin);
                                    }
                                    else
                                    {
                                        if ((i == 0 && j != 0) || (i != 0 && j == 0))
                                        {
                                            if (Rand.Value < 0.33)
                                            {
                                                map.roofGrid.SetRoof(newSpot, null);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }* /
                if (elevation > 0.65f && elevation <= 0.69f)
                {
                    return TerrainDefOf.Gravel;
                }
                if (elevation > 0.69f & elevation < 0.71f)
                {
                    if (HarmonyPatches.detectedFertileFields == true)
                    {
                        return TerrainDef.Named("RockySoil");
                    }
                    return TerrainDefOf.Gravel;
                }
                if (elevation >= 0.71f)
                {
                    return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
                }
                TerrainDef deepWater = TerrainDefOf.WaterOceanDeep;
                TerrainDef shallowWater = TerrainDefOf.WaterOceanShallow;
                /*if (lakeIsles.Equals(true))
                {
                    deepWater = TerrainDefOf.WaterDeep;
                    shallowWater = TerrainDefOf.WaterShallow;
                }
                if (elevation < (0.40f + adjustment))
                {
                    return deepWater;
                }
                if (elevation < (0.45f + adjustment))
                {
                    return shallowWater;
                }
                TerrainDef borderTerrainL = TerrainDefOf.Sand;
                TerrainDef borderTerrainH = TerrainDefOf.Sand;
                if (lakeIsles.Equals(true))
                {
                    if (map.Biome.defName.Contains("Boreal") || map.Biome.defName.Contains("Tundra"))
                    {
                        borderTerrainL = TerrainDef.Named("Mud");
                        borderTerrainH = TerrainDef.Named("MossyTerrain");
                    }
                    else if (map.Biome.defName.Contains("ColdBog") || map.Biome.defName.Contains("Swamp"))
                    {
                        borderTerrainL = TerrainDef.Named("Marsh");
                        borderTerrainH = TerrainDef.Named("MarshyTerrain");
                    }
                    else if (map.Biome.defName.Contains("Temperate") || map.Biome.defName.Contains("Tropical"))
                    {
                        borderTerrainL = TerrainDef.Named("Mud");
                        borderTerrainH = TerrainDef.Named("SoilRich");
                    }
                }
                if (elevation < 0.47f)
                {
                    return borderTerrainL;
                }
                if (elevation < 0.50f)
                {
                    return borderTerrainH;
                }
                TerrainDef terrainDefA = TerrainThreshold.TerrainAtValue(map.Biome.terrainsByFertility, fertility);
                if (terrainDefA != null)
                {
                    return terrainDefA;
                }* /
                return null;// borderTerrainH;
            }
            TerrainDef terrainDef = null;
            if (river != null)
            {
                terrainDef = river.TerrainAt(c, true);
            }
            TerrainDef terrainDef1 = CMBeachMaker.BeachTerrainAt(c, map, map.Biome);
            TerrainDef terrainDef2 = null;
            for (int i = 0; i < map.Biome.terrainPatchMakers.Count; i++)
            {
                terrainDef2 = map.Biome.terrainPatchMakers[i].TerrainAt(c, map, fertility);
                if (terrainDef2 != null)
                {
                    break;
                }
            }
            if (terrainDef == null && preferSolid)
            {
                if (!TerrainSettings.disallowIslands ||
                    (terrainDef1 == null && terrainDef2 == null))
                {
                    return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
                }
                Building edifice = c.GetEdifice(map);
                if (edifice != null) { edifice.Destroy(DestroyMode.Vanish); }
                map.roofGrid.SetRoof(c, null);
                for (int i = -1; i < 2; i++)
                {
                    for (int j = -1; j < 2; j++)
                    {
                        int x = c.x + i;
                        int z = c.z + j;
                        if (x >= 0 && z >= 0 && x < map.Size.x && z < map.Size.z)
                        {
                            IntVec3 newSpot = new IntVec3(x, 0, z);
                            if (map.roofGrid.RoofAt(newSpot) != null)
                            {
                                if (map.roofGrid.RoofAt(newSpot).isThickRoof)
                                {
                                    map.roofGrid.SetRoof(newSpot, RoofDefOf.RoofRockThin);
                                }
                                else
                                {
                                    if ((i == 0 && j != 0) || (i != 0 && j == 0))
                                    {
                                        if (Rand.Value < 0.33)
                                        {
                                            map.roofGrid.SetRoof(newSpot, null);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (terrainDef1 == TerrainDefOf.WaterOceanDeep)
            {
                return terrainDef1;
            }
            if (terrainDef == TerrainDefOf.WaterMovingShallow || terrainDef == TerrainDefOf.WaterMovingChestDeep)
            {
                return terrainDef;
            }
            if (terrainDef1 != null)
            {
                //
                // Lake water has priority over beach sand
                //
                if (terrainDef1.defName.Contains("Sand"))
                {
                    if ((terrainDef2 != null) && terrainDef2.defName.Contains("Water")) { }
                    else
                    {
                        return terrainDef1;
                    }
                }
                else
                {
                    return terrainDef1;
                }
            }
            if (terrainDef != null)
            {
                return terrainDef;
            }
            if (terrainDef2 != null)
            {
                return terrainDef2;
            }
            if (elevation > 0.55f && elevation <= 0.59f)
            {
                return TerrainDefOf.Gravel;
            }
            if (elevation > 0.59f & elevation < 0.61f)
            {
                if (HarmonyPatches.detectedFertileFields)
                {
                    return TerrainDef.Named("RockySoil");
                }
                return TerrainDefOf.Gravel;
            }
            if (elevation >= 0.61f)
            {
                return GenStep_RocksFromGrid.RockDefAt(c).building.naturalTerrain;
            }
            terrainDef1 = TerrainThreshold.TerrainAtValue(map.Biome.terrainsByFertility, fertility);
            if (terrainDef1 != null)
            {
                return terrainDef1;
            }
#if WarnedMissingTerrain
            Log.Error(string.Concat(new object[] { "No terrain found in biome ", map.Biome.defName, " for elevation=", elevation, ", fertility=", fertility }));
#endif
            return TerrainDefOf.Sand;
        }
        private void UpdateRiverAnchorEntry(Dictionary<int, GRLT_Entry> entries, IntVec3 center, int entryId, float zValue)
        {
            float single = zValue - (float)entryId;
            if (single <= 2f)
            {
                if (!entries.ContainsKey(entryId) || entries[entryId].bestDistance > single)
                {
                    GRLT_Entry gRLTEntry = new GRLT_Entry()
                    {
                        bestDistance = single,
                        bestNode = center
                    };
                    entries[entryId] = gRLTEntry;
                }
            }
        }
        private struct GRLT_Entry
        {
            public float bestDistance;
            public IntVec3 bestNode;
        }
    }*/
}