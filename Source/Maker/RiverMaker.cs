using ConfigurableMaps.Settings;
using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;

namespace ConfigurableMaps.Maker
{
    internal class CMRiverMaker : RiverMaker
    {
        private readonly FieldInfo surfaceLevelFI = typeof(RiverMaker).GetField("surfaceLevel", BindingFlags.NonPublic | BindingFlags.Instance);

        public CMRiverMaker(Vector3 center, float angle, RiverDef riverDef) : base(center, angle, riverDef)
        {
            // Water Level (for Rivers)
            /*if (riverDef.widthOnMap > 9) { fordability = 0.05f; }
            else if (riverDef.widthOnMap > 7) { fordability = 0.1f; }
            else if (riverDef.widthOnMap > 5) { fordability = 0.15f; }
            else { fordability = 0.2f; }*/
/*
            float surfaceLevel = riverDef.widthOnMap * 0.5f;

            if (TerrainSettings.waterLevel < 0 ||
                TerrainSettings.waterLevel > 5)
            {
                TerrainSettings.waterLevel = (Rand.Value) * 5;
            }
            if (TerrainSettings.waterLevel < 1)
            {
                surfaceLevel *= 0.8f;
                //this.fordability = this.fordability * 1.2f;
            }
            else if (TerrainSettings.waterLevel < 2)
            {
                surfaceLevel *= 0.9f;
                //this.fordability = this.fordability * 1.1f;
            }
            else if (TerrainSettings.waterLevel < 3) { }
            else if (TerrainSettings.waterLevel < 4)
            {
                surfaceLevel *= 1.1f;
                //this.fordability = this.fordability * 0.9f;
            }
            else
            {
                surfaceLevel *= 1.2f;
                //this.fordability = this.fordability * 0.8f;
            }

            this.surfaceLevelFI.SetValue(this, surfaceLevel);*/
        }
    }
}