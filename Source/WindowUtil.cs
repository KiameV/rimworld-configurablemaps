using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public static class WindowUtil
    {
        public delegate void OnChange<T>(T t);
        public static void DrawFloatInput(float x, ref float y, FieldValue<float> fv)
        {
            fv.Buffer = DrawLabeledInput(x, y, fv.Label, fv.Buffer, out float nextX);
            if (float.TryParse(fv.Buffer, out float f))
            {
                fv.OnChange(f);
            }
            if (Widgets.ButtonText(new Rect(nextX, y, 100, 28), "CM.Default".Translate()))
            {
                fv.OnChange(fv.Default);
                fv.UpdateBuffer();
            }
            y += 40;
        }
        public static void DrawRandomizableFloatInput(float x, ref float y, RandomizableFieldValue<float> fv)
        {
            if (fv.GetRandomizableValue())
            {
                DrawLabel(x, y, 150, fv.Label);
            }
            else
            {
                fv.Buffer = DrawLabeledInput(x, y, fv.Label, fv.Buffer, out float nextX);
                if (float.TryParse(fv.Buffer, out float f))
                {
                    fv.OnChange(f);
                }
                if (Widgets.ButtonText(new Rect(nextX, y, 100, 28), "CM.Default".Translate()))
                {
                    fv.OnChange(fv.Default);
                    fv.UpdateBuffer();
                }
            }
            y += 30;
            Widgets.Label(new Rect(x, y, 100, 28), "Randomize".Translate());
            var r = fv.GetRandomizableValue();
            Widgets.Checkbox(new Vector2(x + 110, y - 2), ref r);
            fv.OnRandomizableChange(r);
            y += 30;
        }

        public static void DrawInputWithSlider(float x, ref float y, FieldValue<float> fv, string leftLabel = null, string rightLabel = null)
        {
            DrawFloatInput(x, ref y, fv);
            y += 10;

            if (!float.TryParse(fv.Buffer, out float orig))
                orig = -10000;

            if (leftLabel == null)
                leftLabel = fv.Min.ToString("0.0");
            if (rightLabel == null)
                rightLabel = fv.Max.ToString("0.0");
            var result = Widgets.HorizontalSlider(new Rect(x, y, 300, 20), orig, fv.Min, fv.Max, false, null, leftLabel, rightLabel);
            if (orig != result && Math.Abs(orig-result) > 0.001) {
                fv.OnChange(result);
                fv.UpdateBuffer();
            }
            y += 40;
        }

        public static void DrawInputRandomizableWithSlider(float x, ref float y, RandomizableFieldValue<float> fv, string leftLabel = null, string rightLabel = null)
        {
            DrawRandomizableFloatInput(x, ref y, fv);
            y += 10;

            if (!float.TryParse(fv.Buffer, out float orig))
                orig = 0;

            if (leftLabel == null)
                leftLabel = fv.Min.ToString("0.0");

            if (rightLabel == null)
                rightLabel = fv.Max.ToString("0.0");

            if (!fv.GetRandomizableValue())
            {
                var result = Widgets.HorizontalSlider(new Rect(x, y, 300, 20), orig, fv.Min, fv.Max, false, null, leftLabel, rightLabel);
                if (orig != result && Math.Abs(orig - result) > 0.001)
                {
                    fv.OnChange(result);
                    fv.UpdateBuffer();
                }
                y += 40;
            }
        }

        public static void DrawIntInput(float x, ref float y, FieldValue<int> fv)
        {
            fv.Buffer = DrawLabeledInput(x, y, fv.Label, fv.Buffer, out float nextX);
            if (Double.TryParse(fv.Buffer, out double d))
            {
                fv.OnChange((int)d);
            }
            if (Widgets.ButtonText(new Rect(nextX, y, 100, 28), "CM.Default".Translate()))
            {
                fv.OnChange(fv.Default);
                fv.UpdateBuffer();
            }
            y += 40;
        }

        public static bool DrawBoolInput(float x, ref float y, string label, bool value, OnChange<bool> onChange, string tip = "", float buttonWidth = 100f)
        {
            DrawLabel(x, y, 240, label);
            if (tip != "")
            {
                TooltipHandler.TipRegion(new Rect(x, y, 240 + buttonWidth, 32), tip);
            }
            if (Widgets.ButtonText(new Rect(x + 240, y, buttonWidth, 32), value.ToString().ToString()))
            {
                value = !value;
                onChange(value);
            }
            y += 40;
            return value;
        }

        public static string DrawLabeledInput(float x, float y, string label, string buffer, out float nextX)
        {
            DrawLabel(x, y, 150, label);
            var s = Widgets.TextField(new Rect(x + 160, y, 60, 32), buffer);
            nextX = 240;
            return s;
        }

        public static void DrawLabel(float x, ref float y, float width, string label, float yInc = 32)
        {
            DrawLabel(x, y, width, label);
            y += yInc;
        }

        public static void DrawLabel(float x, float y, float width, string label)
        {
            if (label == null)
                return;

            // 0.14 is about the size of each character
            if (label.Length > width * 0.137)
            {
                Text.Font = GameFont.Tiny;
            }

            Widgets.Label(new Rect(x, y + 2, width, 30), label);

            Text.Font = GameFont.Small;
        }

        public delegate string GetLabel<E>(E e) where E : System.Enum;
        public static void DrawEnumSelection<E>(float x, ref float y, string label, E value, GetLabel<E> getLabel, OnChange<E> onChange) where E : System.Enum
        {

            Widgets.Label(new Rect(x, y, 150, 28), label.Translate());
            if (Widgets.ButtonText(new Rect(160, y, 100, 28), getLabel(value)))
            {
                E e;
                var values = typeof(E).GetEnumValues();
                List<FloatMenuOption> l = new List<FloatMenuOption>(values.Length);
                for (int i = 0; i < values.Length; ++i)
                {
                    e = (E)values.GetValue(i);
                    l.Add(new enumFloatMenu<E>(getLabel(e), onChange, e));
                };
                Find.WindowStack.Add(new FloatMenu(l));
            }
            y += 35;
        }
        private class enumFloatMenu<E> : FloatMenuOption where E : System.Enum
        {
            E e;

            public enumFloatMenu(string label, OnChange<E> onChange, E e) : base(label, null)
            {
                this.e = e;
                base.action = () => onChange(this.e);
            }
        }
    }
}
