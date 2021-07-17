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

        public static void Update(Random r)
        {
            var animalMultiplier = MapSettings.IsAnimalDensityMultiplierRandom ? Settings.GetRandomMultiplier(r) : MapSettings.AnimalDensityMultiplier;
            var plantMultiplier = MapSettings.IsPlantDensityMultiplierRandom ? Settings.GetRandomMultiplier(r) : MapSettings.PlantDensityMultiplier;
            float m;

            BiomeDefs.Clear();
            foreach (BiomeDef def in DefDatabase<BiomeDef>.AllDefs)
            {
                BiomeDefs.Add(new Pair<BiomeDef, BiomeOriginalValues>(def, new BiomeOriginalValues(def.animalDensity, def.plantDensity)));
                def.animalDensity *= animalMultiplier;
                def.plantDensity *= plantMultiplier;
            }

            Scatterers.Clear();
            GenStepDef gsDef = DefDatabase<GenStepDef>.GetNamed("ScatterRuinsSimple", false);
            if (gsDef?.genStep is GenStep_Scatterer rs)
            {
                m = MapSettings.IsRuinsMultiplierRandom ? Settings.GetRandomMultiplier(r) : MapSettings.RuinsMultiplier;
                Scatterers.Add(new Pair<GenStep_Scatterer, ScattererValues>(rs, new ScattererValues(rs.countPer10kCellsRange)));
                rs.countPer10kCellsRange *= m;
            }
            else
                Log.Warning("[Configurable Maps] unable to patch ScatterRuinsSimple, genStep was not a GenStep_Scatterer.");

            gsDef = DefDatabase<GenStepDef>.GetNamed("ScatterShrines", false);
            if (gsDef?.genStep is GenStep_Scatterer ss)
            {
                m = MapSettings.IsShrinesMultiplierRandom ? Settings.GetRandomMultiplier(r) : MapSettings.ShrinesMultiplier;
                Scatterers.Add(new Pair<GenStep_Scatterer, ScattererValues>(ss, new ScattererValues(ss.countPer10kCellsRange)));
                ss.countPer10kCellsRange *= m;
            }
            else
                Log.Warning("[Configurable Maps] unable to patch ScatterShrines, genStep was not a GenStep_Scatterer.");
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
