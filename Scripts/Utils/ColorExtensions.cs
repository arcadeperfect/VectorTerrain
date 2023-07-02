using Unity.VisualScripting;
using UnityEngine;


namespace VectorTerrain.Scripts.Utils
{
    public static class ColorExtensions
    {
        public static Color SetV(this Color color, float newValue)
        {

            Color.RGBToHSV(color, out float H, out float S, out float V);
            var c = Color.HSVToRGB(H, S, newValue);
            c.a = color.a;
            return c;
        }
    }
}