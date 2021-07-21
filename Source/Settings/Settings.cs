using System.Collections.Generic;
using UnityEngine;
using Verse;
using static ConfigurableMaps.WindowUtil;

namespace ConfigurableMaps
{
    public static class Consts
    {
        public const float DEFAULT_MULTIPLIER = 1f;
    }

    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "ConfigurableMaps".Translate();
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            base.GetSettings<Settings>().DoWindowContents(inRect);
        }
    }

    public class Settings : ModSettings
    {
        public static bool detectedImpassableMaps = false;
        public static bool detectedFertileFields = false;
        public static bool detectedCuprosStones = false;

        private enum ToShow { None, World, Map }
        private static ToShow toShow = ToShow.None;
        private WSFieldValues wsFieldValues;
        private MSFieldValues msFieldValues;

        public static WorldSettings WorldSettings;
        public static MapSettings MapSettings;

        public static float GetRandomMultiplier(int min = 0, int max = 40000)
        {
            return Rand.RangeInclusive(min, max) * 0.01f;
        }

        public void DoWindowContents(Rect rect)
        {
            if (WorldSettings == null)
                WorldSettings = new WorldSettings();
            if (MapSettings == null)
                MapSettings = new MapSettings();

            string label;
            if (toShow == ToShow.None)
                label = "WorldChooseButton".Translate();
            else if (toShow == ToShow.World)
                label = WorldSettings.Name;
            else
                label = MapSettings.Name;

            if (Widgets.ButtonText(new Rect(rect.x, rect.y, 300, 28), label))
            {
                List<FloatMenuOption> l = new List<FloatMenuOption>()
                {
                    new FloatMenuOption(WorldSettings.Name, () => { toShow = ToShow.World; }),
                    new FloatMenuOption(MapSettings.Name, () => { toShow = ToShow.Map; })
                };
                Find.WindowStack.Add(new FloatMenu(l));
            }
            rect.y += 30f;
            if (toShow == ToShow.World)
            {
                if (wsFieldValues == null)
                    wsFieldValues = WorldSettings.GetFieldValues();
                WorldSettings.DoWindowContents(rect, wsFieldValues);
            }
            else if (toShow == ToShow.Map)
            {
                if (msFieldValues == null)
                    msFieldValues = MapSettings.GetFieldValues();
                MapSettings.DoWindowContents(rect, msFieldValues);
            }
        }

        public override void ExposeData()
        {
            if (WorldSettings == null)
                WorldSettings = new WorldSettings();
            if (MapSettings == null)
                MapSettings = new MapSettings();

            base.ExposeData();
            Scribe_Deep.Look(ref WorldSettings, "WorldSettings");
            Scribe_Deep.Look(ref MapSettings, "MapSettings");
        }
    }

    public delegate T GetValue<T>();
    public class FieldValue<T>
    {
        public OnChange<T> OnChange;
        public GetValue<T> GetValue;
        public T Min, Max, Default;
        public string Buffer;
        public string Label;
        public FieldValue(string label, OnChange<T> onChange, GetValue<T> getValue, T min, T max, T d)
        {
            this.Label = label;
            this.OnChange = onChange;
            this.GetValue = getValue;
            this.Min = min;
            this.Max = max;
            this.Default = d;
            this.UpdateBuffer();
        }
        public void UpdateBuffer()
        {
            var v = this.GetValue();
            if (v is float f)
                this.Buffer = f.ToString("0.00");
            else
                this.Buffer = v.ToString();
        }
    }
    
    public class RandomizableFieldValue<T> : FieldValue<T>
    {
        public OnChange<bool> OnRandomizableChange;
        public GetValue<bool> GetRandomizableValue;
        public RandomizableFieldValue(string label, OnChange<T> onChange, GetValue<T> getValue, T min, T max, T d, OnChange<bool>  onRandomizableChange, GetValue<bool> getRandomizableValue) 
            : base(label, onChange, getValue, min, max, d)
        {
            this.OnRandomizableChange = onRandomizableChange;
            this.GetRandomizableValue = getRandomizableValue;
        }
    }

    public class RandomizableMultiplierFieldValue : RandomizableFieldValue<float>
    {
        public RandomizableMultiplierFieldValue(string label, RandomizableMultiplier rm, float min = 0, float max = 4, float d = Consts.DEFAULT_MULTIPLIER) : 
            base(label, rm.SetMultiplier, rm.GetMultiplier, min, max, d, rm.SetIsRandom, rm.GetIsRandom)
        {
            // Empty
        }
    }

    public interface IWindow
    {
        string Name { get; }
    }

    public interface IWindow<T> : IWindow
    {
        void DoWindowContents(Rect inRect, T t);
        T GetFieldValues();
    }

    public class RandomizableMultiplier : IExposable
    {
        public float Multiplier;
        public bool IsRandom = false;
        private float max = 0;
        private float defaultValue;

        public RandomizableMultiplier(float defaultValue = Consts.DEFAULT_MULTIPLIER)
        {
            this.defaultValue = defaultValue;
            this.max = 0;
            this.Multiplier = defaultValue;
        }
        public RandomizableMultiplier(float max, float defaultValue = Consts.DEFAULT_MULTIPLIER) {
            this.max = max;
            this.defaultValue = defaultValue;
            this.Multiplier = defaultValue;
        }

        public void ExposeData()
        {
            Scribe_Values.Look(ref this.Multiplier, "multiplier", Consts.DEFAULT_MULTIPLIER);
            Scribe_Values.Look(ref this.IsRandom, "isRandom", false);
        }

        public float GetMultiplier() => this.Multiplier;
        public void SetMultiplier(float v)
        {
            if (this.max != 0 && this.Multiplier > max)
                this.Multiplier = max;
            else
                this.Multiplier = v;
        }
        public bool GetIsRandom() => this.IsRandom;
        public void SetIsRandom(bool b) => this.IsRandom = b;
    }
}
