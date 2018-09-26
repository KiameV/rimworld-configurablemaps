using ConfigurableMaps.Settings;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace ConfigurableMaps
{
    static class BiomeUtil
    {
        private static readonly Dictionary<BiomeDef, BiomeOriginalValues> BiomeStats = new Dictionary<BiomeDef, BiomeOriginalValues>();

        public static void Init()
        {
            if (BiomeStats.Count == 0)
            {
                foreach (BiomeDef def in DefDatabase<BiomeDef>.AllDefs)
                {
                    BiomeStats.Add(def, new BiomeOriginalValues(def.animalDensity, def.plantDensity));
                }
            }
        }

        public static void UpdateBiomeStatsPerUserSettings()
        {
#if DEBUG
            Log.Warning("Begin UpdateBiomeStatsPerUserSettings");
#endif
            Init();

            float v = ThingsSettings.animalDensityLevel;
            if (v > 4)
            {
                v = Rand.Value * 4;
            }
            float animalModifier;
            if (v < 1) { animalModifier = 0.5f; }
            else if (v < 2) { animalModifier = 1; }
            else if (v < 3) { animalModifier = 2; }
            else { animalModifier = 4; }

            v = ThingsSettings.plantDensityLevel;
            if (v > 4)
            {
                v = Rand.Value * 4;
            }
            float plantModifier;
            if (v < 1) { plantModifier = 0.5f; }
            else if (v < 2) { plantModifier = 1; }
            else if (v < 3) { plantModifier = 2; }
            else { plantModifier = 4; }

            BiomeOriginalValues orig;
            foreach (BiomeDef def in DefDatabase<BiomeDef>.AllDefs)
            {
#if DEBUG
                Log.Message("    " + def.defName);
#endif
                if (BiomeStats.TryGetValue(def, out orig))
                {
#if DEBUG
                    Log.Message("        Animal Density: Orig [" + orig.AnimalDensity + "]   New [" + (orig.AnimalDensity * animalModifier) + "]");
                    Log.Message("        Plant Density: Orig [" + orig.PlantDensity + "]   New [" + (orig.PlantDensity * plantModifier) + "]");
#endif
                    def.animalDensity = orig.AnimalDensity * animalModifier;
                    def.plantDensity = orig.PlantDensity * plantModifier;
                }
#if DEBUG
                else
                {
                    Log.Error("Not Found");
                }
#endif
            }
#if DEBUG
            Log.Warning("End UpdateBiomeStatsPerUserSettings");
#endif
        }

        private struct BiomeOriginalValues
        {
            public readonly float AnimalDensity;
            public readonly float PlantDensity;
            public BiomeOriginalValues(float animalDensity, float plantDensity)
            {
                this.AnimalDensity = animalDensity;
                this.PlantDensity = plantDensity;
            }
        }
    }
}
