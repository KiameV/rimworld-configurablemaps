using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ConfigurableMaps
{
    public static class WindowUtil
    {
        public delegate void OnChange<T>(T t);
        public static void DrawFloatInput(float x, ref float y, string label, OnChange<float> onChange, ref string valueBuffer, float defaultValue = 0)
        {
            valueBuffer = DrawLabeledInput(x, y, label, valueBuffer, out float nextX);
            if (float.TryParse(valueBuffer, out float f) && f > 0)
                onChange(f);
            if (defaultValue != 0 && Widgets.ButtonText(new Rect(nextX, y, 100, 28), "CM.Default".Translate()))
            {
                onChange(defaultValue);
            }
            y += 40;
        }
        public static void DrawRandomizableFloatInput(float x, ref float y, string label, OnChange<float> onChange, ref string valueBuffer, ref bool randomize, float defaultValue = 0)
        {
            if (randomize)
            {
                DrawLabel(x, y, 150, label);
            }
            else
            {
                valueBuffer = DrawLabeledInput(x, y, label, valueBuffer, out float nextX);
                if (float.TryParse(valueBuffer, out float f) && f > 0)
                    onChange(f);
                if (defaultValue != 0 && Widgets.ButtonText(new Rect(nextX, y, 100, 28), "CM.Default".Translate()))
                {
                    onChange(defaultValue);
                }
            }
            y += 30;
            Widgets.Label(new Rect(x, y, 100, 28), "Randomize".Translate());
            Widgets.Checkbox(new Vector2(x + 110, y - 2), ref randomize);
            y += 30;
        }

        public static void DrawInputWithSlider(float x, ref float y, string label, float min, float max, OnChange<float> onChange, ref string valueBuffer, float defaultValue = 0)
        {
            DrawFloatInput(x, ref y, label, onChange, ref valueBuffer, defaultValue);
            y += 10;

            if (!float.TryParse(valueBuffer, out float orig))
                orig = 0;

            var result = Widgets.HorizontalSlider(new Rect(x, y, 300, 20), orig, min, max, false, null, min.ToString("0.0"), max.ToString("0.0"));
            if (orig != result && Math.Abs(orig-result) > 0.001) {
                onChange(result);
                valueBuffer = result.ToString("0.00");
            }
            y += 40;
        }

        public static void DrawInputRandomizableWithSlider(float x, ref float y, string label, float min, float max, OnChange<float> onChange, ref string valueBuffer, ref bool randomize, float defaultValue = 0)
        {
            DrawRandomizableFloatInput(x, ref y, label, onChange, ref valueBuffer, ref randomize, defaultValue);
            y += 10;

            if (!float.TryParse(valueBuffer, out float orig))
                orig = 0;

            if (!randomize)
            {
                var result = Widgets.HorizontalSlider(new Rect(x, y, 300, 20), orig, min, max, false, null, min.ToString("0.0"), max.ToString("0.0"));
                if (orig != result && Math.Abs(orig - result) > 0.001)
                {
                    onChange(result);
                    valueBuffer = result.ToString("0.00");
                }
                y += 40;
            }
        }

        public static void DrawIntInput(float x, ref float y, string label, OnChange<int> onChange, ref string valueBuffer, int defaultValue)
        {
            valueBuffer = DrawLabeledInput(x, y, label, valueBuffer, out float nextX);
            if (Double.TryParse(valueBuffer, out double d) && d > 0)
                onChange((int)d);
            if (defaultValue != 0 && Widgets.ButtonText(new Rect(nextX, y, 100, 28), "CM.Default".Translate()))
            {
                onChange(defaultValue);
            }
            y += 40;
        }

        public static bool DrawBoolInput(float x, ref float y, float width, string label, bool value, OnChange<bool> onChange, float buttonWidth = 60f)
        {
            DrawLabel(x, y, 240, label);

            if (Widgets.ButtonText(new Rect(width - buttonWidth, y, buttonWidth, 32), value.ToString()))
            {
                value = !value;
                onChange(value);
            }
            y += 40;
            return value;
        }

        public static String DrawLabeledInput(float x, float y, string label, string buffer, out float nextX)
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
    }
}
