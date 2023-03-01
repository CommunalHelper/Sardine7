using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.Sardine7.Entities {
    [CustomEntity("Sardine7/Bunny")]
    public class Bunny : Entity {

        private static readonly Color[] colors = new Color[] {
            Calc.HexToColor("ffffff"),
            Calc.HexToColor("eeeeee"),
        };

        private Sprite sprite;
        private Vector2 start;
        private Coroutine routine;

        private bool moving;
        private int hops;

        private bool hiding;

        public Bunny(EntityData data, Vector2 offset)
            : base(data.Position + offset) {
            Depth = -9999;
            start = Position;
            Add(sprite = Sardine7Module.SpriteBank.Create("bunny"));
            sprite.Color = Calc.Random.Choose(colors);
            Add(routine = new Coroutine(IdleRoutine()));
            Collider = new Hitbox(4, 4, -2f, -4f);
        }

        public override void Update() {
            sprite.Scale.X = Calc.Approach(sprite.Scale.X, Math.Sign(sprite.Scale.X), 4f * Engine.DeltaTime);
            sprite.Scale.Y = Calc.Approach(sprite.Scale.Y, 1f, 4f * Engine.DeltaTime);

            base.Update();
        }

        private IEnumerator IdleRoutine() {
            while (true) {
                Player player = Scene.Tracker.GetEntity<Player>();
                if (player != null && Math.Abs(player.X - X) < 32f && player.Y > Y - 48f && player.Y < Y + 24f) {
                    if (!hiding) {
                        hiding = true;
                        sprite.Play("hide");
                    }
                    yield return null;
                    continue;
                }

                if (hiding) {
                    hiding = false;
                    sprite.Play("show");
                }

                if (Calc.Random.Chance(0.13f * hops)) {
                    moving = !moving;
                    if (!moving)
                        hops = 0;
                }

                float delay = 0.25f + Calc.Random.NextFloat(1f);
                if (moving)
                    delay *= 0.2f;
                for (float t = 0f; t < delay; t += Engine.DeltaTime) {
                    yield return null;
                }

                Vector2 start = Position;
                Vector2 target = GetNextTarget(start);
                if (target == start) {
                    yield return null;
                    continue;
                }

                Audio.Play("event:/game/general/birdbaby_hop", Position);
                sprite.Scale.X = Math.Sign(target.X - Position.X);
                SimpleCurve bezier = new SimpleCurve(Position, target, (Position + target) / 2f - Vector2.UnitY * (7f + Calc.Random.NextFloat(8f)));
                sprite.Play("jump", false, true);
                float speed = 4f + Calc.Random.NextFloat(1f);
                for (float t = 0f; t < 1f; t += Engine.DeltaTime * speed) {
                    Position = bezier.GetPoint(t);
                    yield return null;
                }
                sprite.Play("idle");
                sprite.Scale.X = Math.Sign(sprite.Scale.X) * 1.4f;
                sprite.Scale.Y = 0.6f;
                Position = target;
                hops++;
            }
        }

        private Vector2 GetNextTarget(Vector2 start) {
            for (int attempt = 0; attempt < 4; attempt++) {
                float offs;
                if (moving) {
                    offs = 8f + Calc.Random.NextFloat(16f);
                } else {
                    offs = 4f + Calc.Random.NextFloat(4f);
                }
                offs *= Calc.Random.Next(2) == 0 ? -1 : 1;

                Vector2 next = start + new Vector2(offs, 0f);

                if (Check(next)) {
                    next = next - new Vector2(0f, 8f);
                    if (Check(next))
                        next = next - new Vector2(0f, 8f);

                } else if (!Check(next + new Vector2(0f, 1f))) {
                    next = next + new Vector2(0f, 8f);
                    if (!Check(next + new Vector2(0f, 1f)))
                        next = next + new Vector2(0f, 8f);
                }

                if (!Check(next) && Check(next + new Vector2(0f, 1f), true))
                    return next;
            }

            return start;
        }

        private bool Check(Vector2 at, bool onlyGround = false) {
            return CollideCheck<Solid>(at) || (!onlyGround && CollideCheck<BunnyBlockField>(at));
        }

    }
}
