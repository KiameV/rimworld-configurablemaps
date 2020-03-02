using ConfigurableMaps.Settings;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.Noise;

namespace ConfigurableMaps.Maker
{
    /*internal class CMRiverMaker
    {
        private ModuleBase generator;
        private ModuleBase coordinateX;
        private ModuleBase coordinateZ;
        private ModuleBase shallowizer;
        private float surfaceLevel;
        private float shallowFactor = 0.2f;
        private List<IntVec3> lhs = new List<IntVec3>();
        private List<IntVec3> rhs = new List<IntVec3>();
        private float fordability;
        public CMRiverMaker(Vector3 center, float angle, RiverDef riverDef)
        {
#if DEBUG
            Log.Warning("CMRiverMaker Prefix");
#endif
            //
            // Water Level (for Rivers)
            //
            if (riverDef.widthOnMap > 9) { fordability = 0.05f; }
            else if (riverDef.widthOnMap > 7) { fordability = 0.1f; }
            else if (riverDef.widthOnMap > 5) { fordability = 0.15f; }
            else { fordability = 0.2f; }
            this.surfaceLevel = riverDef.widthOnMap / 2f;
            if (TerrainSettings.waterLevel < 0)
            {
                if (TerrainSettings.waterLevel > 5)
                {
                    TerrainSettings.waterLevel = (Rand.Value) * 5;
                }
            }
            if (TerrainSettings.waterLevel < 1)
            {
                this.surfaceLevel = this.surfaceLevel * 0.8f;
                this.fordability = this.fordability * 1.2f;
            }
            else if (TerrainSettings.waterLevel < 2)
            {
                this.surfaceLevel = this.surfaceLevel * 0.9f;
                this.fordability = this.fordability * 1.1f;
            }
            else if (TerrainSettings.waterLevel < 3) { }
            else if (TerrainSettings.waterLevel < 4)
            {
                this.surfaceLevel = this.surfaceLevel * 1.1f;
                this.fordability = this.fordability * 0.9f;
            }
            else
            {
                this.surfaceLevel = this.surfaceLevel * 1.2f;
                this.fordability = this.fordability * 0.8f;
            }
            this.coordinateX = new AxisAsValueX();
            this.coordinateZ = new AxisAsValueZ();
            this.coordinateX = new Rotate(0, (double)(-angle), 0, this.coordinateX);
            this.coordinateZ = new Rotate(0, (double)(-angle), 0, this.coordinateZ);
            this.coordinateX = new Translate((double)(-center.x), 0, (double)(-center.z), this.coordinateX);
            this.coordinateZ = new Translate((double)(-center.x), 0, (double)(-center.z), this.coordinateZ);
            ModuleBase perlin = new Perlin(0.0299999993294477, 2, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
            ModuleBase multiply = new Perlin(0.0299999993294477, 2, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
            ModuleBase @const = new Const(8);
            perlin = new Multiply(perlin, @const);
            multiply = new Multiply(multiply, @const);
            this.coordinateX = new Displace(this.coordinateX, perlin, new Const(0), multiply);
            this.coordinateZ = new Displace(this.coordinateZ, perlin, new Const(0), multiply);
            this.generator = this.coordinateX;
            this.shallowizer = new Perlin(0.0299999993294477, 2, 0.5, 3, Rand.Range(0, 2147483647), QualityMode.Medium);
            this.shallowizer = new Abs(this.shallowizer);
        }
        public TerrainDef TerrainAt(IntVec3 loc, bool recordForValidation = false)
        {
#if DEBUG
            Log.Warning("CMRiverMaker.TerrainAt Prefix");
#endif
            float value = this.generator.GetValue(loc);
            float single = this.surfaceLevel - Mathf.Abs(value);
            if (single > 2f && this.shallowizer.GetValue(loc) > this.fordability)
            {
                return TerrainDefOf.WaterMovingChestDeep;
            }
            if (single <= 0f)
            {
                return null;
            }
            if (recordForValidation)
            {
                if (value >= 0f)
                {
                    this.rhs.Add(loc);
                }
                else
                {
                    this.lhs.Add(loc);
                }
            }
            return TerrainDefOf.WaterMovingShallow;
        }
        public void ValidatePassage(Map map)
        {
            IntVec3 intVec3 = (
            from loc in this.lhs
            where (!loc.InBounds(map) ? false : loc.GetTerrain(map) == TerrainDefOf.WaterMovingShallow)
            select loc).RandomElementWithFallback<IntVec3>(IntVec3.Invalid);
            IntVec3 intVec31 = (
            from loc in this.rhs
            where (!loc.InBounds(map) ? false : loc.GetTerrain(map) == TerrainDefOf.WaterMovingShallow)
            select loc).RandomElementWithFallback<IntVec3>(IntVec3.Invalid);
            if (intVec3 == IntVec3.Invalid || intVec31 == IntVec3.Invalid)
            {
                Log.Error("Failed to find river edges in order to verify passability");
            }
            else
            {
                while (!map.reachability.CanReach(intVec3, intVec31, PathEndMode.OnCell, TraverseMode.PassAllDestroyableThings))
                {
                    if (this.shallowFactor <= 1f)
                    {
                        this.shallowFactor += 0.1f;
                        foreach (IntVec3 allCell in map.AllCells)
                        {
                            if (allCell.GetTerrain(map) == TerrainDefOf.WaterMovingChestDeep && this.shallowizer.GetValue(allCell) <= this.shallowFactor)
                            {
                                map.terrainGrid.SetTerrain(allCell, TerrainDefOf.WaterMovingShallow);
                            }
                        }
                    }
                    else
                    {
                        Log.Error("Failed to make river shallow enough for passability");
                        break;
                    }
                }
            }
        }
        public Vector3 WaterCoordinateAt(IntVec3 loc)
        {
#if DEBUG
            Log.Warning("CMRiverMaker.WaterCoordinateAt Prefix");
#endif
            Vector3 vector3 = new Vector3(this.coordinateX.GetValue(loc), 0f, this.coordinateZ.GetValue(loc));
            return vector3;
        }
    }*/
}
