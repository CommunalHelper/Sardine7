using Celeste.Mod.Entities;
using System;
using System.Collections;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System.Linq;
using FMOD.Studio;

namespace Celeste.Mod.Sardine7.Entities
{
    [CustomEntity("Sardine7/Gokuro")]
    [Tracked]
    public class Gokuro : Entity
    {
        private const int StChase = 0;

        private const int StChargeUp = 1;

        private const int StAttack = 2;

        private const int StDummy = 3;

        private const int StWaiting = 4;

        private const int StHurt = 5;

        private const float HitboxBackRange = 4f;

        public Sprite Sprite;

        private Sprite lightning;

        private bool lightningVisible;

        private VertexLight light;

        private Level level;

        private SineWave sine;

        private float cameraXOffset;

        private StateMachine state;

        private int attackIndex;

        private float targetAnxiety;

        private float anxietySpeed;

        private bool easeBackFromRightEdge;

        private bool doRespawnAnim;

        private bool leaving;

        private Shaker shaker;

        private PlayerCollider bounceCollider;

        private Vector2 colliderTargetPosition;

        private bool canControlTimeRate = true;

        private SoundSource prechargeSfx;

        private SoundSource chargeSfx;

        private bool hasEnteredSfx;

        private const float minCameraOffsetX = -48f;

        private const float yApproachTargetSpeed = 100f;

        private float yApproachSpeed;

        private float[] ChaseWaitTimes;

        private float attackSpeed;

        private float attackMaxSpeed;

        private float chronosBarrier;

        private const float HurtXSpeed = 100f;

        private const float HurtYSpeed = 200f;

        private bool yRespawnApproach;

        private bool extraApproach;

        private bool quickDeath;

        private bool startFollow;

        private float volume;

        private int attackCount;

        private float PitY => quickDeath ? (level.Camera.Bottom + 20) : (level.Bounds.Bottom + 20);

        private float TargetY
        {
            get
            {
                Player entity = level.Tracker.GetEntity<Player>();
                if (entity != null)
                {
                    return MathHelper.Clamp(entity.CenterY, level.Bounds.Top + 8, level.Bounds.Bottom - 8);
                }
                return base.Y;
            }
        }

        public bool DummyMode => state.State == 3;

        public Gokuro(Vector2 position, string chaseWaitString, bool bouncy, bool respawnApproach, float ySpeed, bool dieFast, float attackMax, float timeBarrier, bool luigi, bool extraApp, bool startFollowTarget, float boluumu, int atkCount)
            : base(position)
        {
            attackMaxSpeed = attackMax;
            chronosBarrier = timeBarrier;
            ChaseWaitTimes = chaseWaitString.Split(',').Select(Convert.ToSingle).ToArray();
            yApproachSpeed = ySpeed;
            yRespawnApproach = respawnApproach;
            quickDeath = dieFast;
            startFollow = startFollowTarget;
            volume = boluumu;
            attackCount = atkCount;
            Add(Sprite = luigi ? Sardine7Module.SpriteBank.Create("gokuro_boss") : GFX.SpriteBank.Create("oshiro_boss"));
            extraApproach = extraApp;
            Sprite.Play("idle");
            Add(lightning = GFX.SpriteBank.Create("oshiro_boss_lightning"));
            lightning.Visible = false;
            lightning.OnFinish = delegate
            {
                lightningVisible = false;
            };
            base.Collider = new Circle(14f);
            base.Collider.Position = (colliderTargetPosition = new Vector2(3f, 4f));
            Add(sine = new SineWave(0.5f));
            if (bouncy)
                Add(bounceCollider = new PlayerCollider(OnPlayerBounce, new Hitbox(28f, 6f, -11f, -11f)));
            Add(new PlayerCollider(OnPlayer));
            base.Depth = -12500;
            Visible = false;
            Add(light = new VertexLight(Color.White, 1f, 32, 64));
            Add(shaker = new Shaker(on: false));
            state = new StateMachine();
            state.SetCallbacks(0, ChaseUpdate, ChaseCoroutine, ChaseBegin);
            state.SetCallbacks(1, ChargeUpUpdate, ChargeUpCoroutine, null, ChargeUpEnd);
            state.SetCallbacks(2, AttackUpdate, AttackCoroutine, AttackBegin, AttackEnd);
            state.SetCallbacks(3, null);
            state.SetCallbacks(4, WaitingUpdate);
            state.SetCallbacks(5, HurtUpdate, null, HurtBegin);
            Add(state);
            Add(new TransitionListener
            {
                OnOutBegin = delegate
                {
                    if (base.X > (float)level.Bounds.Left + Sprite.Width / 2f)
                    {
                        Visible = false;
                    }
                    else
                    {
                        easeBackFromRightEdge = true;
                    }
                },
                OnOut = delegate
                {
                    lightning.Update();
                    if (easeBackFromRightEdge)
                    {
                        base.X -= 128f * Engine.RawDeltaTime;
                    }
                }
            });
            Add(prechargeSfx = new SoundSource());
            Add(chargeSfx = new SoundSource());
            Distort.AnxietyOrigin = new Vector2(1f, 0.5f);
        }

        public Gokuro(Vector2 position, EntityData data) : this(position, data.Attr("chaseWaitTimes", "1.0,2.0,3.0,2.0,3.0"), data.Bool("bouncy", true), data.Bool("yApproachDuringRespawn", false), data.Float("yApproachSpeed", 100f), data.Bool("dieFast", false), data.Float("attackMaxSpeed", 500f), data.Float("chronosBarrier", 200f), data.Bool("luigi", false), data.Bool("extraApproach", true), data.Float("yPosition", -1f) == -1f, data.Float("volume", 1f), data.Int("attackCount", -1))
        {
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            level = SceneAs<Level>();
            if (state.State != 3)
            {
                state.State = 4;
            }
            if (startFollow)
                base.Y = TargetY;
            cameraXOffset = -48f;
        }

        private void OnPlayer(Player player)
        {
            if (state.State != 5 && (base.CenterX < player.CenterX + 4f || Sprite.CurrentAnimationID != "respawn"))
            {
                player.Die((player.Center - base.Center).SafeNormalize(Vector2.UnitX));
            }
        }

        private void OnPlayerBounce(Player player)
        {
            if (state.State == 2 && player.Bottom <= base.Top + 6f)
            {
                Audio.Play("event:/game/general/thing_booped", Position);
                Celeste.Freeze(0.2f);
                player.Bounce(base.Top + 2f);
                state.State = 5;
                prechargeSfx.Stop();
                chargeSfx.Stop();
            }
        }

        public override void Update()
        {
            base.Update();
            Sprite.Scale.X = Calc.Approach(Sprite.Scale.X, 1f, 0.6f * Engine.DeltaTime);
            Sprite.Scale.Y = Calc.Approach(Sprite.Scale.Y, 1f, 0.6f * Engine.DeltaTime);
            if (!doRespawnAnim)
            {
                Visible = (base.X > (float)level.Bounds.Left - base.Width / 2f);
            }
            if (state.State != 3 && canControlTimeRate)
            {
                if (state.State == 2 && attackSpeed > chronosBarrier)
                {
                    Player entity = base.Scene.Tracker.GetEntity<Player>();
                    if (entity != null && !entity.Dead && (base.CenterX < entity.CenterX + 4f))
                    {
                        float value = Calc.ClampedMap((entity.CenterX - base.CenterX), 30f, 80f, 0.5f);
                        value = (Engine.TimeRate = MathHelper.Lerp(value, 1f, Calc.ClampedMap(Math.Abs(entity.CenterY - base.CenterY), 32f, 48f)));
                    }
                    else
                    {
                        Engine.TimeRate = 1f;
                    }
                }
                else
                {
                    Engine.TimeRate = 1f;
                }
                Distort.GameRate = Calc.Approach(Distort.GameRate, Calc.Map(Engine.TimeRate, 0.5f, 1f), Engine.DeltaTime * 8f);
                Distort.Anxiety = Calc.Approach(Distort.Anxiety, targetAnxiety, anxietySpeed * Engine.DeltaTime);
            }
            else
            {
                Distort.GameRate = 1f;
                Distort.Anxiety = 0f;
            }
        }

        public void StopControllingTime()
        {
            canControlTimeRate = false;
        }

        public override void Render()
        {
            if (lightningVisible)
            {
                lightning.RenderPosition = new Vector2(level.Camera.Left - 2f, base.Top + 16f);
                lightning.Render();
            }
            Sprite.Position = shaker.Value * 2f;
            base.Render();
        }

        public void Leave()
        {
            leaving = true;
        }

        public void Squish()
        {
            Sprite.Scale = new Vector2(1.3f, 0.5f);
            shaker.ShakeFor(0.5f, removeOnFinish: false);
        }

        private void ChaseBegin()
        {
            Sprite.Play("idle");
        }

        private int ChaseUpdate()
        {
            if (!hasEnteredSfx && cameraXOffset >= -16f && !doRespawnAnim)
            {
                Audio.Play("event:/char/oshiro/boss_enter_screen", Position).setVolume(volume);
                hasEnteredSfx = true;
            }
            if (doRespawnAnim && cameraXOffset >= 0f)
            {
                base.Collider.Position.X = -48f;
                Visible = true;
                Sprite.Play("respawn");
                doRespawnAnim = false;
                if (base.Scene.Tracker.GetEntity<Player>() != null)
                {
                    Audio.Play("event:/char/oshiro/boss_reform", Position).setVolume(volume);
                }
            }
            cameraXOffset = Calc.Approach(cameraXOffset, 20f, 80f * Engine.DeltaTime);
            base.X = level.Camera.Left + cameraXOffset;
            base.Collider.Position.X = Calc.Approach(base.Collider.Position.X, colliderTargetPosition.X, Engine.DeltaTime * 128f);
            Collidable = Visible;
            Player entity = level.Tracker.GetEntity<Player>();
            if (entity != null && (Sprite.CurrentAnimationID != "respawn" || yRespawnApproach))
            {
                base.CenterY = Calc.Approach(base.CenterY, TargetY, yApproachSpeed * Engine.DeltaTime);
            }
            return 0;
        }

        private IEnumerator ChaseCoroutine()
        {
            yield return ChaseWaitTimes[attackIndex];
            attackIndex++;
            attackIndex %= ChaseWaitTimes.Length;
            prechargeSfx.Play("event:/char/oshiro/boss_precharge");
            DynData<SoundSource> prechargeData = new DynData<SoundSource>(prechargeSfx);
            EventInstance preInstance = prechargeData.Get<EventInstance>("instance");
            preInstance.setVolume(volume);
            Sprite.Play("charge");
            yield return 0.7f;
            if (base.Scene.Tracker.GetEntity<Player>() != null)
            {
                Alarm.Set(this, 0.216f, delegate
                {
                    chargeSfx.Play("event:/char/oshiro/boss_charge");
                    DynData<SoundSource> chargeData = new DynData<SoundSource>(chargeSfx);
                    EventInstance instance = chargeData.Get<EventInstance>("instance");
                    instance.setVolume(volume);
                });
                state.State = 1;
            }
            else
            {
                Sprite.Play("idle");
            }
        }

        private int ChargeUpUpdate()
        {
            if (level.OnInterval(0.05f))
            {
                Sprite.Position = Calc.Random.ShakeVector();
            }
            cameraXOffset = Calc.Approach(cameraXOffset, 0f, 40f * Engine.DeltaTime);
            base.X = level.Camera.Left + cameraXOffset;
            Player entity = level.Tracker.GetEntity<Player>();
            if (entity != null && extraApproach)
            {
                base.CenterY = Calc.Approach(base.CenterY, MathHelper.Clamp(entity.CenterY, level.Bounds.Top + 8, level.Bounds.Bottom - 8), 30f * Engine.DeltaTime);
            }
            return 1;
        }

        private void ChargeUpEnd()
        {
            Sprite.Position = Vector2.Zero;
        }

        private IEnumerator ChargeUpCoroutine()
        {
            Celeste.Freeze(0.05f);
            Distort.Anxiety = 0.3f;
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            lightningVisible = true;
            lightning.Play("once", restart: true);
            yield return 0.3f;
            Player player = Scene.Tracker.GetEntity<Player>();
            state.State = (player != null) ? 2 : 0;
        }

        private void AttackBegin()
        {
            attackSpeed = 0f;
            targetAnxiety = 0.3f;
            anxietySpeed = 4f;
            level.DirectionalShake(Vector2.UnitX);
        }

        private void AttackEnd()
        {
            targetAnxiety = 0f;
            anxietySpeed = 0.5f;
        }

        private int AttackUpdate()
        {
            base.X += attackSpeed * Engine.DeltaTime;
            attackSpeed = Calc.Approach(attackSpeed, attackMaxSpeed, 2000f * Engine.DeltaTime);
            if (base.X >= level.Camera.Right + 48f)
            {
                attackCount -= 1;
                if (attackCount == 0)
                    Leave();
                if (leaving)
                {
                    RemoveSelf();
                    return 2;
                }
                base.X = level.Camera.Left - 48f;
                cameraXOffset = -48f;
                doRespawnAnim = true;
                Visible = false;
                return 0;
            }
            Input.Rumble(RumbleStrength.Light, RumbleLength.Short);
            if (base.Scene.OnInterval(0.05f))
            {
                TrailManager.Add(this, Color.Red * 0.6f, 0.5f);
            }
            return 2;
        }

        private IEnumerator AttackCoroutine()
        {
            yield return 0.1f;
            targetAnxiety = 0f;
            anxietySpeed = 0.5f;
        }

        public void EnterDummyMode()
        {
            state.State = 3;
        }

        public void LeaveDummyMode()
        {
            state.State = 0;
        }

        private int WaitingUpdate()
        {
            Player entity = base.Scene.Tracker.GetEntity<Player>();
            if (entity != null && entity.Speed != Vector2.Zero && entity.X > (float)level.Bounds.Left + 48)
            {
                return 0;
            }
            return 4;
        }

        private void HurtBegin()
        {
            Sprite.Play("hurt", restart: true);
        }

        private int HurtUpdate()
        {
            base.X += 100f * Engine.DeltaTime;
            base.Y += 200f * Engine.DeltaTime;
            if (base.Top > PitY)
            {
                if (leaving)
                {
                    RemoveSelf();
                    return 5;
                }
                base.X = level.Camera.Left - 48f;
                cameraXOffset = -48f;
                doRespawnAnim = true;
                Visible = false;
                return 0;
            }
            return 5;
        }
    }
}
