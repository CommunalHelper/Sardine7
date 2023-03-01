using Celeste.Mod.Entities;
using System.Reflection;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Sardine7.Entities
{
    // Stolen from SpringCollab2020 with a few changes in .cs and .jl
    // Original cs code is written by WoofWoofDoggo and 0x0ade
    // Original jl code is written by WoofWoofDoggo and JaThePlayer

    // What I did: move light source point to the center of the tile, and change graphics accordingly
    // This allows a much better experience when placing these entities in Ahorn
    // And I also removed the bloom point that comes with the light source entity

    [CustomEntity("Sardine7/LightSource")]
    class LightSource : Entity
    {
        public LightSource(EntityData data, Vector2 offset) : base(data.Position + offset + new Vector2(4, 4))
        {
            alpha = data.Float("alpha", 1f);
            radius = data.Float("radius", 48f);
            color = ColorHelper.GetColor(data.Attr("color", "White"));
            
            Add(light = new VertexLight(color, alpha, data.Int("startFade", 24), data.Int("endFade", 48)));
        }

        private VertexLight light;

        private float alpha;

        private float radius;

        private Color color;
    }

    // Cruor made this
    class ColorHelper
    {
        public static Color GetColor(string color)
        {
            foreach (PropertyInfo c in colorProps)
            {
                if (color.Equals(c.Name, System.StringComparison.OrdinalIgnoreCase))
                    return (Color)c.GetValue(new Color(), null);
            }

            try
            {
                return Calc.HexToColor(color.Replace("#", ""));
            }
            catch
            {
                Logger.Log("ColorHelper", "Failed to transform color " + color + ", returning Color.White");
            }

            return Color.White;
        }

        private static PropertyInfo[] colorProps = typeof(Color).GetProperties();
    }
}