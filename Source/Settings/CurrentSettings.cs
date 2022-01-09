using RimWorld;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public class CurrentSettings : IWindow<List<FieldValue<float>>>
    {
        public static readonly List<OriginalAnimalPlant> Biomes = new List<OriginalAnimalPlant>();

        public string Name => "CM.CurrentSettings".Translate();

        public void DoWindowContents(Rect inRect, List<FieldValue<float>> fvs)
        {
            float y = inRect.y;
            foreach (var fv in fvs)
            {
                WindowUtil.DrawInputWithSlider(inRect.x, ref y, fv);
            }
        }

        public List<FieldValue<float>> GetFieldValues()
        {
            return new List<FieldValue<float>>(2)
            {
                new FieldValue<float>("CM.AnimalDensity".Translate(), (float v) => WorldComp.AnimalMultiplier = v, () => WorldComp.AnimalMultiplier, 0, 6, 1),
                new FieldValue<float>("CM.PlantDensity".Translate(), (float v) => WorldComp.PlantMultiplier = v, () => WorldComp.PlantMultiplier, 0, 6, 1),
            };
        }

        public void ApplySettings(List<FieldValue<float>> fvs)
        {
            if (fvs != null)
                ApplySettings(fvs[0].GetValue(), fvs[1].GetValue());
            else
                Log.Warning("[Configurable Maps] No values to apply");
        }

        public static void ApplySettings(float animalMultiplier, float plantMultiplier)
        {
            if (animalMultiplier <= 0)
            {
                animalMultiplier = MapSettings.AnimalDensity.GetMultiplier();
                Log.Warning($"[Configurable Maps] No map comp animal, now using {animalMultiplier}");
            }
            if (plantMultiplier <= 0)
            {
                plantMultiplier = MapSettings.PlantDensity.GetMultiplier();
                Log.Warning($"[Configurable Maps] No map comp plant, now using {plantMultiplier}");
            }

            foreach (var b in Biomes)
            {
                b.ApplyMultipliers(animalMultiplier, plantMultiplier);
            }
        }
    }
}

public struct OriginalAnimalPlant
{
    const float MAX_ANIMAL = 40;
    const float MAX_PLANT = 100;

    public BiomeDef Def;
    public readonly float Animal, Plant;

    public OriginalAnimalPlant(BiomeDef def)
    {
        this.Def = def;
        this.Animal = def.animalDensity;
        this.Plant = def.plantDensity;
    }
    public void ApplyMultipliers(float animal, float plant)
    {
        this.Def.animalDensity = this.Animal * animal;
        if (this.Def.animalDensity < 0)
            this.Def.animalDensity = 0;
        else if (this.Def.animalDensity > MAX_ANIMAL)
            this.Def.animalDensity = MAX_ANIMAL;

        this.Def.plantDensity = this.Plant * plant;
        if (this.Def.plantDensity < 0)
            this.Def.plantDensity = 0;
        else if (this.Def.plantDensity > MAX_PLANT)
            this.Def.plantDensity = MAX_PLANT;
    }
}
