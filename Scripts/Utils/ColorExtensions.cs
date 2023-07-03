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
        public static Color SetS(this Color color, float newValue)
        {

            Color.RGBToHSV(color, out float H, out float S, out float V);
            var c = Color.HSVToRGB(H, newValue, V);
            c.a = color.a;
            return c;
        }
        
        public static Color SetH(this Color color, float newValue)
        {

            Color.RGBToHSV(color, out float H, out float S, out float V);
            var c = Color.HSVToRGB(newValue, S, V);
            c.a = color.a;
            return c;
        }
        
        public static Color RandomBrightColor( int seed)
        {
            Random.InitState(seed);
            return Color.HSVToRGB(Random.value, 1, 15, true);
        }
    }
}