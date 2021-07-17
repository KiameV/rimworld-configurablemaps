using System.Collections.Generic;
using UnityEngine;
using Verse;
using static ConfigurableMaps.WindowUtil;

namespace ConfigurableMaps
{
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
            Settings.DoWindowContents(inRect);
        }
    }

    public class Settings : ModSettings
    {
        public static bool detectedFertileFields = false;
        public static bool detectedCuprosStones = false;

        private enum ToShow { None, World, Map }
        private static ToShow toShow = ToShow.None;
        private static WSFieldValues wsFieldValues;
        private static MSFieldValues msFieldValues;

        public static WorldSettings WorldSettings;
        public static MapSettings MapSettings;

        public static float GetRandomMultiplier(System.Random r, int min = 0, int max = 40000)
        {
            return r.Next(min, max) * 0.01f;
        }

        public static void DoWindowContents(Rect rect)
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
                    new FloatMenuOption(WorldSettings.Name, () =>
                    {
                        toShow = ToShow.World;
                        if (wsFieldValues == null)
                            wsFieldValues = WorldSettings.GetFieldValues();
                    }),
                    new FloatMenuOption(MapSettings.Name, () => {
                        toShow = ToShow.Map;
                        if (msFieldValues == null)
                            msFieldValues = MapSettings.GetFieldValues();
                    })
                };
                Find.WindowStack.Add(new FloatMenu(l));
            }
            rect.y += 30f;
            if (toShow == ToShow.World)
                WorldSettings.DoWindowContents(rect, wsFieldValues);
            else if (toShow == ToShow.Map)
                MapSettings.DoWindowContents(rect, msFieldValues);
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
        public RandomizableFieldValue(string label, OnChange<T> onChange, GetValue<T> getValue, T min, T max, T d, OnChange<bool>  onRandomizableChange, GetValue<bool> getRandomizableValue) : base(label, onChange, getValue, min, max, d)
        {
            this.OnRandomizableChange = onRandomizableChange;
            this.GetRandomizableValue = getRandomizableValue;
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
}
