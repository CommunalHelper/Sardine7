using Monocle;
using System;
using System.Xml;
using System.Linq;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Sardine7
{
    public class Sardine7Module : EverestModule
    {
        public static SpriteBank SpriteBank;

        public static Atlas Game;

        public static Sardine7Module Instance;

        public Sardine7Module()
        {
            Instance = this;
        }

        public override void Load()
        {
            // On.Celeste.Level.GetFullCameraTargetAt += Sardine7GetFullCameraTargetAt;
        }

        public override void Unload()
        {
            // On.Celeste.Level.GetFullCameraTargetAt -= Sardine7GetFullCameraTargetAt;
        }

        public override void LoadContent(bool firstLoad)
        {
            if (firstLoad)
                SpriteBank = new SpriteBank(GFX.Game, "Graphics/Sardine7/Sprites.xml");
            // Decal Registry Properties
            // pollution
            ParticleType PollutedChimney = new ParticleType
            {
                SourceChooser = new Chooser<MTexture>(GFX.Game["particles/smoke0"], GFX.Game["particles/smoke1"], GFX.Game["particles/smoke2"], GFX.Game["particles/smoke3"]),
                Color = Color.Black,
                Color2 = new Color(44, 44, 44),
                ColorMode = ParticleType.ColorModes.Choose,
                Acceleration = new Vector2(-4f, 1f),
                LifeMin = 2f,
                LifeMax = 4f,
                Size = 1f,
                SizeRange = 0.25f,
                Direction = (float)Math.PI / 2f,
                DirectionRange = 0.5f,
                SpeedMin = 4f,
                SpeedMax = 12f,
                RotationMode = ParticleType.RotationModes.Random,
                ScaleOut = true
            };
            ParticleType PollutedSmallChimney = new ParticleType
            {
                SourceChooser = new Chooser<MTexture>(GFX.Game["particles/smoke0"], GFX.Game["particles/smoke1"], GFX.Game["particles/smoke2"], GFX.Game["particles/smoke3"]),
                Color = Color.Black,
                Color2 = new Color(44, 44, 44),
                ColorMode = ParticleType.ColorModes.Choose,
                Acceleration = new Vector2(-4f, 0.5f),
                LifeMin = 2f,
                LifeMax = 4f,
                Size = 0.5f,
                SizeRange = 0.125f,
                Direction = (float)Math.PI / 2f,
                DirectionRange = 0.25f,
                SpeedMin = 4f,
                SpeedMax = 12f,
                RotationMode = ParticleType.RotationModes.Random,
                ScaleOut = true
            };
            DecalRegistry.AddPropertyHandler("Sardine7_pollution",
                            delegate (Decal decal, XmlAttributeCollection attrs)
                            {
                                float x2 = (attrs["offsetX"] != null) ? float.Parse(attrs["offsetX"].Value) : 0f;
                                float y2 = (attrs["offsetY"] != null) ? float.Parse(attrs["offsetY"].Value) : 0f;
                                Vector2 offset2 = new Vector2(x2, y2);
                                bool inbg = attrs["inbg"] != null && bool.Parse(attrs["inbg"].Value);
                                bool small = attrs["small"] != null && bool.Parse(attrs["small"].Value);
                                Level level = decal.Scene as Level;
                                ParticleSystem system = inbg ? level.ParticlesBG : level.ParticlesFG;
                                ParticleEmitter particleEmitter = new ParticleEmitter(system, small ? PollutedSmallChimney : PollutedChimney, offset2, new Vector2(4f, 1f), -(float)Math.PI / 2f, 1, 0.2f);
                                decal.Add(particleEmitter);
                                particleEmitter.SimulateCycle();
                            });
            DecalRegistry.AddPropertyHandler("makeSolids",
                delegate (Decal decal, XmlAttributeCollection attrs)
                {
                    float[] x = (attrs["offsetX"] != null) ? attrs["offsetX"].Value.Split(',').Select(Convert.ToSingle).ToArray() : new[] { 0f };
                    float[] y = (attrs["offsetY"] != null) ? attrs["offsetY"].Value.Split(',').Select(Convert.ToSingle).ToArray() : new[] { 0f };
                    float[] w = (attrs["width"] != null) ? attrs["width"].Value.Split(',').Select(Convert.ToSingle).ToArray() : new[] { decal.Width };
                    float[] h = (attrs["height"] != null) ? attrs["height"].Value.Split(',').Select(Convert.ToSingle).ToArray() : new[] { decal.Height };
                    bool safe = attrs["safe"] != null && bool.Parse(attrs["safe"].Value);
                    bool blockWaterfalls = attrs["blockWaterfalls"] != null && bool.Parse(attrs["blockWaterfalls"].Value);
                    int surfaceSoundIndex = (attrs["surfaceSoundIndex"] != null) ? int.Parse(attrs["surfaceSoundIndex"].Value) : 0;
                    for (int i = 0; i < x.Length; i++)
                    {
                        Solid solid = new Solid(decal.Position + new Vector2(x[i], y[i]), w[i], h[i], safe: safe);
                        solid.BlockWaterfalls = blockWaterfalls;
                        solid.SurfaceSoundIndex = surfaceSoundIndex;
                        decal.Scene.Add(solid);
                    }
                });
        }

        /*
        public Vector2 Sardine7GetFullCameraTargetAt(On.Celeste.Level.orig_GetFullCameraTargetAt orig, Level self, Player player, Vector2 at)
        {
            Vector2 position = player.Position;
            Logger.Log("Sardine7", (self.Tracker == null).ToString());
            foreach (Entity entity in self.Tracker.GetEntities<SmoothieCameraTargetTrigger>())
            {
                SmoothieCameraTargetTrigger smoothie = entity as SmoothieCameraTargetTrigger;
                if (smoothie != null && player.CollideCheck(entity))
                {
                    smoothie.OnStay(player);
                }
            }
            player.Position = position;
            return orig(self, player, at);
        }
        */
    }
}
