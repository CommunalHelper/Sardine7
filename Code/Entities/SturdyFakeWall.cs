﻿using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Sardine7.Entities
{
    [Tracked(false)]
    [CustomEntity("Sardine7/SturdyFakeWall")]
    public class SturdyFakeWall : Entity
    {
        private char fillTile;

        private TileGrid tiles;

        private bool fade;

        private EffectCutout cutout;

        private float transitionStartAlpha;

        private bool transitionFade;

        private EntityID eid;

        private bool playRevealWhenTransitionedInto;

        private bool playRevealNormal;

        private bool permanent;

        private bool faded = false;

        private bool blendin;

        private string revealSound;

        public SturdyFakeWall(Vector2 position, char tile, float width, float height, EntityID eid)
            : base(position)
        {
            this.eid = eid;
            fillTile = tile;
            base.Collider = new Hitbox(width, height);
            base.Depth = -13000;
            Add(cutout = new EffectCutout());
        }

        public SturdyFakeWall(EntityData data, Vector2 offset, EntityID eid)
            : this(data.Position + offset, data.Char("tiletype", '3'), data.Width, data.Height, eid)
        {
            playRevealWhenTransitionedInto = data.Bool("playTransitionReveal");
            playRevealNormal = data.Bool("playNormalReveal", true);
            permanent = data.Bool("permanent", true);
            blendin = data.Bool("blendin", true);
            revealSound = data.Attr("revealSound", "event:/game/general/secret_revealed");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            int tilesX = (int)base.Width / 8;
            int tilesY = (int)base.Height / 8;
            if (blendin)
            {
                Level level = SceneAs<Level>();
                Rectangle tileBounds = level.Session.MapData.TileBounds;
                VirtualMap<char> solidsData = level.SolidsData;
                int x = (int)base.X / 8 - tileBounds.Left;
                int y = (int)base.Y / 8 - tileBounds.Top;
                tiles = GFX.FGAutotiler.GenerateOverlay(fillTile, x, y, tilesX, tilesY, solidsData).TileGrid;
            }
            else
            {
                tiles = GFX.FGAutotiler.GenerateBox(fillTile, tilesX, tilesY).TileGrid;
            }
            Add(tiles);
            Add(new TileInterceptor(tiles, highPriority: false));
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (CollideCheck<Player>())
            {
                tiles.Alpha = 0f;
                fade = true;
                cutout.Visible = false;
                if (playRevealWhenTransitionedInto)
                {
                    Audio.Play(revealSound, base.Center);
                }
                if (permanent)
                {
                    SceneAs<Level>().Session.DoNotLoad.Add(eid);
                }
            }
            else
            {
                TransitionListener transitionListener = new TransitionListener();
                transitionListener.OnOut = OnTransitionOut;
                transitionListener.OnOutBegin = OnTransitionOutBegin;
                transitionListener.OnIn = OnTransitionIn;
                transitionListener.OnInBegin = OnTransitionInBegin;
                Add(transitionListener);
            }
        }

        private void OnTransitionOutBegin()
        {
            if (Collide.CheckRect(this, SceneAs<Level>().Bounds))
            {
                transitionFade = true;
                transitionStartAlpha = tiles.Alpha;
            }
            else
            {
                transitionFade = false;
            }
        }

        private void OnTransitionOut(float percent)
        {
            if (transitionFade)
            {
                tiles.Alpha = transitionStartAlpha * (1f - percent);
            }
        }

        private void OnTransitionInBegin()
        {
            Level level = SceneAs<Level>();
            if (level.PreviousBounds.HasValue && Collide.CheckRect(this, level.PreviousBounds.Value))
            {
                transitionFade = true;
                tiles.Alpha = 0f;
            }
            else
            {
                transitionFade = false;
            }
        }

        private void OnTransitionIn(float percent)
        {
            if (transitionFade)
            {
                tiles.Alpha = percent;
            }
        }

        public override void Update()
        {
            base.Update();
            if (faded) return;
            if (fade)
            {
                tiles.Alpha = Calc.Approach(tiles.Alpha, 0f, 2f * Engine.DeltaTime);
                cutout.Alpha = tiles.Alpha;
                if (tiles.Alpha <= 0f)
                {
                    if (permanent)
                    {
                        RemoveSelf();
                    }
                    else
                    {
                        faded = true;
                    }
                }
                return;
            }
            Player player = CollideFirst<Player>();
            if (player != null && player.StateMachine.State != 9)
            {
                if (permanent)
                {
                    SceneAs<Level>().Session.DoNotLoad.Add(eid);
                }
                fade = true;
                if (playRevealNormal)
                {
                    Audio.Play(revealSound, base.Center);
                }
            }
        }

        public override void Render()
        {
            if (blendin)
            {
                Level level = base.Scene as Level;
                if (level.ShakeVector.X < 0f && level.Camera.X <= (float)level.Bounds.Left && base.X <= (float)level.Bounds.Left)
                {
                    tiles.RenderAt(Position + new Vector2(-3f, 0f));
                }
                if (level.ShakeVector.X > 0f && level.Camera.X + 320f >= (float)level.Bounds.Right && base.X + base.Width >= (float)level.Bounds.Right)
                {
                    tiles.RenderAt(Position + new Vector2(3f, 0f));
                }
                if (level.ShakeVector.Y < 0f && level.Camera.Y <= (float)level.Bounds.Top && base.Y <= (float)level.Bounds.Top)
                {
                    tiles.RenderAt(Position + new Vector2(0f, -3f));
                }
                if (level.ShakeVector.Y > 0f && level.Camera.Y + 180f >= (float)level.Bounds.Bottom && base.Y + base.Height >= (float)level.Bounds.Bottom)
                {
                    tiles.RenderAt(Position + new Vector2(0f, 3f));
                }
            }
            base.Render();
        }
    }
}
