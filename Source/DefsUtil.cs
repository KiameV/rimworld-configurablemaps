using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Verse;

namespace ConfigurableMaps
{
    static class DefsUtil
    {
        private static bool applied = false;
        private static readonly List<Pair<BiomeDef, BiomeOriginalValues>> BiomeDefs = new List<Pair<BiomeDef, BiomeOriginalValues>>();
        private static readonly List<Pair<GenStep_Scatterer, ScattererValues>> Scatterers = new List<Pair<GenStep_Scatterer, ScattererValues>>();
        private static readonly List<Pair<ThingDef, float>> Minability = new List<Pair<ThingDef, float>>();
        private static Pair<GenStep_PreciousLump, FloatRange> PreciousLump;

        public static void Update()
        {
            if (applied)
                return;
            applied = true;
            MapSettings.Initialize();
            var animalMultiplier = MapSettings.AnimalDensity.GetMultiplier();
            var plantMultiplier = MapSettings.PlantDensity.GetMultiplier();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Configurable Maps] Using Map Settings:");

            // Biome
            BiomeDefs.Clear();
            foreach (BiomeDef def in DefDatabase<BiomeDef>.AllDefs)
            {
                try
                {
                    BiomeDefs.Add(new Pair<BiomeDef, BiomeOriginalValues>(def, new BiomeOriginalValues(def.animalDensity, def.plantDensity)));
                    def.animalDensity *= animalMultiplier;
                    def.plantDensity *= plantMultiplier;
                }
                catch
                {
                    Log.Error("[Configurable Maps] failed to update biome " + def.defName);
                }
            }

            Scatterers.Clear();
            // Ruins
            UpdateGenStepScatterer("ScatterRuinsSimple", MapSettings.Ruins, Scatterers, sb);
            // Shrines
            UpdateGenStepScatterer("ScatterShrines", MapSettings.Shrines, Scatterers, sb);
            // SteamGeysers
            UpdateGenStepScatterer("SteamGeysers", MapSettings.Geysers, Scatterers, sb);
            // Minable
            UpdateMineable("MineableGold", MapSettings.MineableGold, Minability, sb);
            UpdateMineable("MineableSilver", MapSettings.MineableSilver, Minability, sb);
            UpdateMineable("MineableUranium", MapSettings.MineableUranium, Minability, sb);
            UpdateMineable("MineableJade", MapSettings.MineableJade, Minability, sb);
            UpdateMineable("MineablePlasteel", MapSettings.MineablePlasteel, Minability, sb);
            UpdateMineable("MinableSteel", MapSettings.MineableSteel, Minability, sb);
            UpdateMineable("MineableComponentsIndustrial", MapSettings.MineableComponentsIndustrial, Minability, sb);

            Log.Message(sb.ToString());
        }

        private static void UpdateMineable(string mineableDefName, RandomizableMultiplier rm, List<Pair<ThingDef, float>> minability, StringBuilder sb)
        {
            try
            {
                var td = DefDatabase<ThingDef>.GetNamed(mineableDefName, false);
                if (td != null)
                {
                    Minability.Add(new Pair<ThingDef, float>(td, td.building.mineableScatterCommonality));
                    td.building.mineableScatterCommonality *= rm.GetMultiplier();
                    sb.AppendLine($"- {mineableDefName}.mineableScatterCommonality = {td.building.mineableScatterCommonality}");
                }
                else
                    Log.Warning($"$[Configurable Maps] unable to patch {mineableDefName}.");
            }
            catch
            {
                Log.Error($"[Configurable Maps] failed to find and patch {mineableDefName}");
            }
        }

        private static FieldInfo GetTotalValueRange()
        {
            return typeof(SitePartDef).GetField("totalValueRange", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        private static void UpdateGenStepScatterer(string genStepDefName, RandomizableMultiplier rm, List<Pair<GenStep_Scatterer, ScattererValues>> scatterers, StringBuilder sb)
        {
            try
            {
                float m = rm.GetMultiplier();
                GenStepDef d = DefDatabase<GenStepDef>.GetNamed(genStepDefName, false);
                if (d?.genStep is GenStep_PreciousLump pl)
                {
                    PreciousLump = new Pair<GenStep_PreciousLump, FloatRange>(pl, new FloatRange(pl.totalValueRange.min, pl.totalValueRange.max));
                    pl.totalValueRange.min *= m;
                    pl.totalValueRange.max *= m;
                    sb.AppendLine($"- {genStepDefName}.totalValueRange = {pl.totalValueRange} -- {pl.forcedLumpSize}");
                }
                else if (d?.genStep is GenStep_Scatterer rs)
                {
                    Scatterers.Add(new Pair<GenStep_Scatterer, ScattererValues>(rs, new ScattererValues(rs.countPer10kCellsRange)));
                    rs.countPer10kCellsRange.min *= m;
                    rs.countPer10kCellsRange.max *= m;
                    sb.AppendLine($"- {genStepDefName}.countPer10kCellsRange = {rs.countPer10kCellsRange}");
                }
                else
                    Log.Warning($"[Configurable Maps] unable to patch {genStepDefName}");
            }
            catch
            {
                Log.Error("[Configurable Maps] failed to update scatterer " + genStepDefName);
            }
        }

        private static void UpdateGenStepScatterer(string genStepDefName, float multiplier, List<Pair<GenStep_Scatterer, ScattererValues>> scatterers, StringBuilder sb)
        {
            try
            {
                GenStepDef d = DefDatabase<GenStepDef>.GetNamed(genStepDefName, false);
                if (d?.genStep is GenStep_PreciousLump pl)
                {
                    PreciousLump = new Pair<GenStep_PreciousLump, FloatRange>(pl, new FloatRange(pl.totalValueRange.min, pl.totalValueRange.max));
                    pl.totalValueRange.min *= multiplier;
                    pl.totalValueRange.max *= multiplier;
                    sb.AppendLine($"- {genStepDefName}.totalValueRange = {pl.totalValueRange}");
                }
                else if (d?.genStep is GenStep_Scatterer rs)
                {
                    Scatterers.Add(new Pair<GenStep_Scatterer, ScattererValues>(rs, new ScattererValues(rs.countPer10kCellsRange)));
                    rs.countPer10kCellsRange.min *= multiplier;
                    rs.countPer10kCellsRange.max *= multiplier;
                    sb.AppendLine($"- {genStepDefName}.countPer10kCellsRange = {rs.countPer10kCellsRange}");
                }
                else
                    Log.Warning($"[Configurable Maps] unable to patch {genStepDefName}");
            }
            catch
            {
                Log.Error("[Configurable Maps] failed to update scatterer " + genStepDefName);
            }
        }


        public static void Restore()
        {
            if (applied)
            {
                foreach (var p in BiomeDefs)
                {
                    p.First.animalDensity = p.Second.AnimalDensity;
                    p.First.plantDensity = p.Second.PlantDensity;
                }
                BiomeDefs.Clear();

                foreach (var p in Scatterers)
                {
                    p.First.countPer10kCellsRange.min = p.Second.CountPer10kCellsRange.min;
                    p.First.countPer10kCellsRange.max = p.Second.CountPer10kCellsRange.max;
                }
                Scatterers.Clear();

                foreach (var p in Minability)
                {
                    p.First.building.mineableScatterCommonality = p.Second;
                }
                Minability.Clear();

                if (PreciousLump.First != null)
                {
                    PreciousLump.First.totalValueRange.min = PreciousLump.Second.min;
                    PreciousLump.First.totalValueRange.max = PreciousLump.Second.max;
                }
                applied = false;
            }
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
            public ScattererValues(FloatRange fr)
            {
                this.CountPer10kCellsRange = new FloatRange(fr.min, fr.max);
            }
        }

        private struct SitePartValues
        {
            public readonly float Min, Max;
            public SitePartValues(float min, float max)
            {
                this.Min = min;
                this.Max = max;
            }
        }
    }
}
