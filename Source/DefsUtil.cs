using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace ConfigurableMaps
{
    static class DefsUtil
    {
        private static readonly List<Pair<BiomeDef, BiomeOriginalValues>> BiomeDefs = new List<Pair<BiomeDef, BiomeOriginalValues>>();
        private static readonly List<Pair<GenStep_Scatterer, ScattererValues>> Scatterers = new List<Pair<GenStep_Scatterer, ScattererValues>>();
        private static readonly List<Pair<ThingDef, float>> Minability = new List<Pair<ThingDef, float>>();

        public static void Update()
        {
            var animalMultiplier = MapSettings.AnimalDensity.IsRandom ? Settings.GetRandomMultiplier() : MapSettings.AnimalDensity.Multiplier;
            var plantMultiplier = MapSettings.PlantDensity.IsRandom ? Settings.GetRandomMultiplier() : MapSettings.PlantDensity.Multiplier;
            float m;
            ThingDef td;

            // Biome
            BiomeDefs.Clear();
            foreach (BiomeDef def in DefDatabase<BiomeDef>.AllDefs)
            {
                BiomeDefs.Add(new Pair<BiomeDef, BiomeOriginalValues>(def, new BiomeOriginalValues(def.animalDensity, def.plantDensity)));
                def.animalDensity *= animalMultiplier;
                def.plantDensity *= plantMultiplier;
            }

            Scatterers.Clear();
            // Ruins
            UpdateGenStepScatterer("ScatterRuinsSimple", MapSettings.Ruins, Scatterers);
            // Shrines
            UpdateGenStepScatterer("ScatterShrines", MapSettings.Shrines, Scatterers);
            // SteamGeysers
            UpdateGenStepScatterer("SteamGeysers", MapSettings.Geysers, Scatterers);
            // PreciousLump
            UpdateGenStepScatterer("PreciousLump", MapSettings.OreLevel, Scatterers);

            // Minable
            Minability.Clear();
            td = DefDatabase<ThingDef>.GetNamed("MineablePlasteel", false);
            if (td != null)
            {
                m = MapSettings.MinablePlasteel.IsRandom ? Settings.GetRandomMultiplier() : MapSettings.MinablePlasteel.Multiplier;
                Minability.Add(new Pair<ThingDef, float>(td, td.building.mineableScatterCommonality));
                td.building.mineableScatterCommonality *= m;
            }
            else
                Log.Warning("[Configurable Maps] unable to patch MineablePlasteel.");


            td = ThingDefOf.MineableSteel;
            m = MapSettings.MinableSteel.IsRandom ? Settings.GetRandomMultiplier() : MapSettings.MinableSteel.Multiplier;
            Minability.Add(new Pair<ThingDef, float>(td, td.building.mineableScatterCommonality));
            td.building.mineableScatterCommonality *= m;

            td = ThingDefOf.MineableComponentsIndustrial;
            m = MapSettings.MinableComponentsIndustrial.IsRandom ? Settings.GetRandomMultiplier() : MapSettings.MinableComponentsIndustrial.Multiplier;
            Minability.Add(new Pair<ThingDef, float>(td, td.building.mineableScatterCommonality));
            td.building.mineableScatterCommonality *= m;
        }

        private static void UpdateGenStepScatterer(string genStepDefName, RandomizableMultiplier ruins, List<Pair<GenStep_Scatterer, ScattererValues>> scatterers)
        {
            GenStepDef d = DefDatabase<GenStepDef>.GetNamed(genStepDefName, false);
            if (d?.genStep is GenStep_Scatterer rs)
            {
                float m = MapSettings.Ruins.IsRandom ? Settings.GetRandomMultiplier() : MapSettings.Ruins.Multiplier;
                Scatterers.Add(new Pair<GenStep_Scatterer, ScattererValues>(rs, new ScattererValues(rs.countPer10kCellsRange)));
                rs.countPer10kCellsRange *= m;
            }
            else
                Log.Warning($"[Configurable Maps] unable to patch {genStepDefName}");
        }

        public static void Restore()
        {
            foreach (var p in BiomeDefs)
            {
                p.First.animalDensity = p.Second.AnimalDensity;
                p.First.plantDensity = p.Second.PlantDensity;
            }
            BiomeDefs.Clear();

            foreach (var p in Scatterers)
            {
                p.First.countPer10kCellsRange = p.Second.CountPer10kCellsRange;
            }
            Scatterers.Clear();

            foreach (var p in Minability)
            {
                p.First.building.mineableScatterCommonality = p.Second;
            }
            Minability.Clear();
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

        private struct ScattererValues
        {
            public readonly FloatRange CountPer10kCellsRange;
            public ScattererValues(FloatRange countPer10kCellsRange)
            {
                this.CountPer10kCellsRange = countPer10kCellsRange;
            }
        }
    }
}
