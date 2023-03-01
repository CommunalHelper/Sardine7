using Celeste.Mod.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Sardine7.Entities
{
    // Token: 0x020001B7 RID: 439
    [CustomEntity("Sardine7/SokobanBlock")]
    public class SokobanBlock : Solid
    {
        // Token: 0x06000E01 RID: 3585 RVA: 0x00035168 File Offset: 0x00033368
        public SokobanBlock(Vector2 position, float width, float height, SokobanBlock.Axes axes, bool chillOut = false) : base(position, width, height, false)
        {
            this.fill = Calc.HexToColor("62222b");
            this.idleImages = new List<Image>();
            this.activeTopImages = new List<Image>();
            this.activeRightImages = new List<Image>();
            this.activeLeftImages = new List<Image>();
            this.activeBottomImages = new List<Image>();
            this.OnDashCollide = new DashCollision(this.OnDashed);
            this.returnStack = new List<SokobanBlock.MoveState>();
            this.chillOut = chillOut;
            this.giant = (base.Width >= 48f && base.Height >= 48f && chillOut);
            this.canActivate = true;
            this.attackCoroutine = new Coroutine(true);
            this.attackCoroutine.RemoveOnComplete = false;
            base.Add(this.attackCoroutine);
            List<MTexture> atlasSubtextures = GFX.Game.GetAtlasSubtextures("objects/Sardine7/SokobanBlock/block");
            MTexture idle;
            switch (axes)
            {
                default:
                    idle = atlasSubtextures[3];
                    this.canMoveHorizontally = (this.canMoveVertically = true);
                    break;
                case SokobanBlock.Axes.Horizontal:
                    idle = atlasSubtextures[1];
                    this.canMoveHorizontally = true;
                    this.canMoveVertically = false;
                    break;
                case SokobanBlock.Axes.Vertical:
                    idle = atlasSubtextures[2];
                    this.canMoveHorizontally = false;
                    this.canMoveVertically = true;
                    break;
            }
            base.Add(this.face = Sardine7Module.SpriteBank.Create(this.giant ? "giant_sokobanblock_face" : "sokobanblock_face"));
            this.face.Position = new Vector2(base.Width, base.Height) / 2f;
            this.face.Play("idle", false, false);
            this.face.OnLastFrame = delegate (string f)
            {
                bool flag = f == "hit";
                if (flag)
                {
                    this.face.Play(this.nextFaceDirection, false, false);
                }
            };
            int num = (int)(base.Width / 8f) - 1;
            int num2 = (int)(base.Height / 8f) - 1;
            this.AddImage(idle, 0, 0, 0, 0, -1, -1);
            this.AddImage(idle, num, 0, 3, 0, 1, -1);
            this.AddImage(idle, 0, num2, 0, 3, -1, 1);
            this.AddImage(idle, num, num2, 3, 3, 1, 1);
            for (int i = 1; i < num; i++)
            {
                this.AddImage(idle, i, 0, Calc.Random.Choose(1, 2), 0, 0, -1);
                this.AddImage(idle, i, num2, Calc.Random.Choose(1, 2), 3, 0, 1);
            }
            for (int j = 1; j < num2; j++)
            {
                this.AddImage(idle, 0, j, 0, Calc.Random.Choose(1, 2), -1, 0);
                this.AddImage(idle, num, j, 3, Calc.Random.Choose(1, 2), 1, 0);
            }
            base.Add(new LightOcclude(0.2f));
            base.Add(this.returnLoopSfx = new SoundSource());
            base.Add(new WaterInteraction(() => this.crushDir != Vector2.Zero));
        }

        // Token: 0x06000E02 RID: 3586 RVA: 0x0003546F File Offset: 0x0003366F
        public SokobanBlock(EntityData data, Vector2 offset) : this(data.Position + offset, (float)data.Width, (float)data.Height, data.Enum<SokobanBlock.Axes>("axes", SokobanBlock.Axes.Both), data.Bool("chillout", false))
        {
        }

        // Token: 0x06000E03 RID: 3587 RVA: 0x000354AB File Offset: 0x000336AB
        public override void Added(Scene scene)
        {
            base.Added(scene);
            this.level = base.SceneAs<Level>();
        }

        // Token: 0x06000E04 RID: 3588 RVA: 0x000354C4 File Offset: 0x000336C4
        public override void Update()
        {
            base.Update();
            bool flag = this.crushDir == Vector2.Zero;
            if (flag)
            {
                this.face.Position = new Vector2(base.Width, base.Height) / 2f;
                bool flag2 = base.CollideCheck<Player>(this.Position + new Vector2(-1f, 0f));
                if (flag2)
                {
                    this.face.X -= 1f;
                }
                else
                {
                    bool flag3 = base.CollideCheck<Player>(this.Position + new Vector2(1f, 0f));
                    if (flag3)
                    {
                        this.face.X += 1f;
                    }
                    else
                    {
                        bool flag4 = base.CollideCheck<Player>(this.Position + new Vector2(0f, -1f));
                        if (flag4)
                        {
                            this.face.Y -= 1f;
                        }
                    }
                }
            }
            bool flag5 = this.currentMoveLoopSfx != null;
            if (flag5)
            {
                this.currentMoveLoopSfx.Param("submerged", (float)(this.Submerged ? 1 : 0));
            }
            bool flag6 = this.returnLoopSfx != null;
            if (flag6)
            {
                this.returnLoopSfx.Param("submerged", (float)(this.Submerged ? 1 : 0));
            }
        }

        // Token: 0x06000E05 RID: 3589 RVA: 0x00035630 File Offset: 0x00033830
        public override void Render()
        {
            Vector2 position = this.Position;
            this.Position += base.Shake;
            Draw.Rect(base.X + 2f, base.Y + 2f, base.Width - 4f, base.Height - 4f, this.fill);
            base.Render();
            this.Position = position;
        }

        // Token: 0x170001B7 RID: 439
        // (get) Token: 0x06000E06 RID: 3590 RVA: 0x000356A8 File Offset: 0x000338A8
        private bool Submerged
        {
            get
            {
                return base.Scene.CollideCheck<Water>(new Rectangle((int)(base.Center.X - 4f), (int)base.Center.Y, 8, 4));
            }
        }

        // Token: 0x06000E07 RID: 3591 RVA: 0x000356EC File Offset: 0x000338EC
        private void AddImage(MTexture idle, int x, int y, int tx, int ty, int borderX = 0, int borderY = 0)
        {
            MTexture subtexture = idle.GetSubtexture(tx * 8, ty * 8, 8, 8, null);
            Vector2 vector = new Vector2((float)(x * 8), (float)(y * 8));
            bool flag = borderX != 0;
            if (flag)
            {
                base.Add(new Image(subtexture)
                {
                    Color = Color.Black,
                    Position = vector + new Vector2((float)borderX, 0f)
                });
            }
            bool flag2 = borderY != 0;
            if (flag2)
            {
                base.Add(new Image(subtexture)
                {
                    Color = Color.Black,
                    Position = vector + new Vector2(0f, (float)borderY)
                });
            }
            Image image = new Image(subtexture);
            image.Position = vector;
            base.Add(image);
            this.idleImages.Add(image);
            bool flag3 = borderX != 0 || borderY != 0;
            if (flag3)
            {
                bool flag4 = borderX < 0;
                if (flag4)
                {
                    Image image2 = new Image(GFX.Game["objects/Sardine7/SokobanBlock/lit_left"].GetSubtexture(0, ty * 8, 8, 8, null));
                    this.activeLeftImages.Add(image2);
                    image2.Position = vector;
                    image2.Visible = false;
                    base.Add(image2);
                }
                else
                {
                    bool flag5 = borderX > 0;
                    if (flag5)
                    {
                        Image image3 = new Image(GFX.Game["objects/Sardine7/SokobanBlock/lit_right"].GetSubtexture(0, ty * 8, 8, 8, null));
                        this.activeRightImages.Add(image3);
                        image3.Position = vector;
                        image3.Visible = false;
                        base.Add(image3);
                    }
                }
                bool flag6 = borderY < 0;
                if (flag6)
                {
                    Image image4 = new Image(GFX.Game["objects/Sardine7/SokobanBlock/lit_top"].GetSubtexture(tx * 8, 0, 8, 8, null));
                    this.activeTopImages.Add(image4);
                    image4.Position = vector;
                    image4.Visible = false;
                    base.Add(image4);
                }
                else
                {
                    bool flag7 = borderY > 0;
                    if (flag7)
                    {
                        Image image5 = new Image(GFX.Game["objects/Sardine7/SokobanBlock/lit_bottom"].GetSubtexture(tx * 8, 0, 8, 8, null));
                        this.activeBottomImages.Add(image5);
                        image5.Position = vector;
                        image5.Visible = false;
                        base.Add(image5);
                    }
                }
            }
        }

        // Token: 0x06000E08 RID: 3592 RVA: 0x0003593C File Offset: 0x00033B3C
        private void TurnOffImages()
        {
            foreach (Image image in this.activeLeftImages)
            {
                image.Visible = false;
            }
            foreach (Image image2 in this.activeRightImages)
            {
                image2.Visible = false;
            }
            foreach (Image image3 in this.activeTopImages)
            {
                image3.Visible = false;
            }
            foreach (Image image4 in this.activeBottomImages)
            {
                image4.Visible = false;
            }
        }

        // Token: 0x06000E09 RID: 3593 RVA: 0x00035A64 File Offset: 0x00033C64
        private DashCollisionResults OnDashed(Player player, Vector2 direction)
        {
            bool flag = this.CanActivate(direction);
            DashCollisionResults result;
            if (flag)
            {
                this.Attack(direction);
                result = DashCollisionResults.Rebound;
            }
            else
            {
                result = DashCollisionResults.NormalCollision;
            }
            return result;
        }

        // Token: 0x06000E0A RID: 3594 RVA: 0x00035A9C File Offset: 0x00033C9C
        private bool CanActivate(Vector2 direction)
        {
            bool flag = this.giant && direction.X <= 0f;
            bool result;
            if (flag)
            {
                result = false;
            }
            else
            {
                bool flag2 = this.canActivate && this.crushDir != direction;
                if (flag2)
                {
                    bool flag3 = direction.X != 0f && !this.canMoveHorizontally;
                    if (flag3)
                    {
                        result = false;
                    }
                    else
                    {
                        bool flag4 = direction.Y != 0f && !this.canMoveVertically;
                        result = !flag4;
                    }
                }
                else
                {
                    result = false;
                }
            }
            return result;
        }

        // Token: 0x06000E0B RID: 3595 RVA: 0x00035B3C File Offset: 0x00033D3C
        private void Attack(Vector2 direction)
        {
            Audio.Play("event:/game/06_reflection/crushblock_activate", base.Center);
            bool flag = this.currentMoveLoopSfx != null;
            if (flag)
            {
                this.currentMoveLoopSfx.Param("end", 1f);
                SoundSource sfx = this.currentMoveLoopSfx;
                Alarm.Set(this, 0.5f, delegate
                {
                    sfx.RemoveSelf();
                }, Alarm.AlarmMode.Oneshot);
            }
            base.Add(this.currentMoveLoopSfx = new SoundSource());
            this.currentMoveLoopSfx.Position = new Vector2(base.Width, base.Height) / 2f;
            bool flag2 = SaveData.Instance != null && SaveData.Instance.Name != null && SaveData.Instance.Name.StartsWith("fishtank", StringComparison.InvariantCultureIgnoreCase);
            if (flag2)
            {
                this.currentMoveLoopSfx.Play("event:/game/06_reflection/crushblock_move_loop_covert", null, 0f);
            }
            else
            {
                this.currentMoveLoopSfx.Play("event:/game/06_reflection/crushblock_move_loop", null, 0f);
            }
            this.face.Play("hit", false, false);
            this.crushDir = direction;
            this.canActivate = false;
            this.attackCoroutine.Replace(this.AttackSequence());
            base.ClearRemainder();
            this.TurnOffImages();
            this.ActivateParticles(-this.crushDir);
            bool flag3 = this.crushDir.X < 0f;
            if (flag3)
            {
                foreach (Image image in this.activeLeftImages)
                {
                    image.Visible = true;
                }
                this.nextFaceDirection = "left";
            }
            else
            {
                bool flag4 = this.crushDir.X > 0f;
                if (flag4)
                {
                    foreach (Image image2 in this.activeRightImages)
                    {
                        image2.Visible = true;
                    }
                    this.nextFaceDirection = "right";
                }
                else
                {
                    bool flag5 = this.crushDir.Y < 0f;
                    if (flag5)
                    {
                        foreach (Image image3 in this.activeTopImages)
                        {
                            image3.Visible = true;
                        }
                        this.nextFaceDirection = "up";
                    }
                    else
                    {
                        bool flag6 = this.crushDir.Y > 0f;
                        if (flag6)
                        {
                            foreach (Image image4 in this.activeBottomImages)
                            {
                                image4.Visible = true;
                            }
                            this.nextFaceDirection = "down";
                        }
                    }
                }
            }
            bool flag7 = true;
            bool flag8 = this.returnStack.Count > 0;
            if (flag8)
            {
                SokobanBlock.MoveState moveState = this.returnStack[this.returnStack.Count - 1];
                bool flag9 = moveState.Direction == direction || moveState.Direction == -direction;
                if (flag9)
                {
                    flag7 = false;
                }
            }
            bool flag10 = flag7;
            if (flag10)
            {
                this.returnStack.Add(new SokobanBlock.MoveState(this.Position, this.crushDir));
            }
        }

        // Token: 0x06000E0C RID: 3596 RVA: 0x00035ED8 File Offset: 0x000340D8
        private void ActivateParticles(Vector2 dir)
        {
            bool flag = dir == Vector2.UnitX;
            float direction;
            Vector2 position;
            Vector2 positionRange;
            int num;
            if (flag)
            {
                direction = 0f;
                position = base.CenterRight - Vector2.UnitX;
                positionRange = Vector2.UnitY * (base.Height - 2f) * 0.5f;
                num = (int)(base.Height / 8f) * 4;
            }
            else
            {
                bool flag2 = dir == -Vector2.UnitX;
                if (flag2)
                {
                    direction = 3.14159274f;
                    position = base.CenterLeft + Vector2.UnitX;
                    positionRange = Vector2.UnitY * (base.Height - 2f) * 0.5f;
                    num = (int)(base.Height / 8f) * 4;
                }
                else
                {
                    bool flag3 = dir == Vector2.UnitY;
                    if (flag3)
                    {
                        direction = 1.57079637f;
                        position = base.BottomCenter - Vector2.UnitY;
                        positionRange = Vector2.UnitX * (base.Width - 2f) * 0.5f;
                        num = (int)(base.Width / 8f) * 4;
                    }
                    else
                    {
                        direction = -1.57079637f;
                        position = base.TopCenter + Vector2.UnitY;
                        positionRange = Vector2.UnitX * (base.Width - 2f) * 0.5f;
                        num = (int)(base.Width / 8f) * 4;
                    }
                }
            }
            num += 2;
            this.level.Particles.Emit(SokobanBlock.P_Activate, num, position, positionRange, direction);
        }

        // Token: 0x06000E0D RID: 3597 RVA: 0x00036070 File Offset: 0x00034270
        private IEnumerator AttackSequence()
        {
            Input.Rumble(RumbleStrength.Strong, RumbleLength.Medium);
            this.StartShaking(0.4f);
            yield return 0.4f;
            bool flag = !this.chillOut;
            if (flag)
            {
                this.canActivate = true;
            }
            this.StopPlayerRunIntoAnimation = false;
            bool slowing = false;
            float speed = 0f;
            Action something = null;  //dnspy freaking out
            for (; ; )
            {
                bool flag2 = !this.chillOut;
                if (flag2)
                {
                    speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);
                }
                else
                {
                    bool flag3 = slowing || this.CollideCheck<SolidTiles>(this.Position + this.crushDir * 256f);
                    if (flag3)
                    {
                        speed = Calc.Approach(speed, 24f, 500f * Engine.DeltaTime * 0.25f);
                        bool flag4 = !slowing;
                        if (flag4)
                        {
                            slowing = true;
                            float duration = 0.5f;
                            Action onComplete;
                            if ((onComplete = something) == null)
                            {
                                onComplete = (something = delegate ()
                                {
                                    this.face.Play("hurt", false, false);
                                    this.currentMoveLoopSfx.Stop(true);
                                    this.TurnOffImages();
                                });
                            }
                            Alarm.Set(this, duration, onComplete, Alarm.AlarmMode.Oneshot);
                        }
                    }
                    else
                    {
                        speed = Calc.Approach(speed, 240f, 500f * Engine.DeltaTime);
                    }
                }
                bool flag5 = this.crushDir.X != 0f;
                bool hit;
                if (flag5)
                {
                    hit = this.MoveHCheck(speed * this.crushDir.X * Engine.DeltaTime);
                }
                else
                {
                    hit = this.MoveVCheck(speed * this.crushDir.Y * Engine.DeltaTime);
                }
                bool flag6 = this.Top >= (float)(this.level.Bounds.Bottom + 32);
                if (flag6)
                {
                    break;
                }
                bool flag7 = hit;
                if (flag7)
                {
                    goto Block_9;
                }
                bool flag8 = this.Scene.OnInterval(0.02f);
                if (flag8)
                {
                    bool flag9 = this.crushDir == Vector2.UnitX;
                    Vector2 at;
                    float dir;
                    if (flag9)
                    {
                        at = new Vector2(this.Left + 1f, Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
                        dir = 3.14159274f;
                    }
                    else
                    {
                        bool flag10 = this.crushDir == -Vector2.UnitX;
                        if (flag10)
                        {
                            at = new Vector2(this.Right - 1f, Calc.Random.Range(this.Top + 3f, this.Bottom - 3f));
                            dir = 0f;
                        }
                        else
                        {
                            bool flag11 = this.crushDir == Vector2.UnitY;
                            if (flag11)
                            {
                                at = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f), this.Top + 1f);
                                dir = -1.57079637f;
                            }
                            else
                            {
                                at = new Vector2(Calc.Random.Range(this.Left + 3f, this.Right - 3f), this.Bottom - 1f);
                                dir = 1.57079637f;
                            }
                        }
                    }
                    this.level.Particles.Emit(SokobanBlock.P_Crushing, at, dir);
                    at = default(Vector2);
                }
                yield return null;
            }
            this.RemoveSelf();
            yield break;
        Block_9:
            FallingBlock fallingBlock = this.CollideFirst<FallingBlock>(this.Position + this.crushDir);
            bool flag12 = fallingBlock != null;
            if (flag12)
            {
                fallingBlock.Triggered = true;
            }
            bool flag13 = this.crushDir == -Vector2.UnitX;
            if (flag13)
            {
                Vector2 add = new Vector2(0f, 2f);
                int i = 0;
                while ((float)i < this.Height / 8f)
                {
                    Vector2 at2 = new Vector2(this.Left - 1f, this.Top + 4f + (float)(i * 8));
                    bool flag14 = !this.Scene.CollideCheck<Water>(at2) && this.Scene.CollideCheck<Solid>(at2);
                    if (flag14)
                    {
                        this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at2 + add, 0f);
                        this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at2 - add, 0f);
                    }
                    at2 = default(Vector2);
                    int num = i;
                    i = num + 1;
                }
                add = default(Vector2);
            }
            else
            {
                bool flag15 = this.crushDir == Vector2.UnitX;
                if (flag15)
                {
                    Vector2 add2 = new Vector2(0f, 2f);
                    int j = 0;
                    while ((float)j < this.Height / 8f)
                    {
                        Vector2 at3 = new Vector2(this.Right + 1f, this.Top + 4f + (float)(j * 8));
                        bool flag16 = !this.Scene.CollideCheck<Water>(at3) && this.Scene.CollideCheck<Solid>(at3);
                        if (flag16)
                        {
                            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at3 + add2, 3.14159274f);
                            this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at3 - add2, 3.14159274f);
                        }
                        at3 = default(Vector2);
                        int num = j;
                        j = num + 1;
                    }
                    add2 = default(Vector2);
                }
                else
                {
                    bool flag17 = this.crushDir == -Vector2.UnitY;
                    if (flag17)
                    {
                        Vector2 add3 = new Vector2(2f, 0f);
                        int k = 0;
                        while ((float)k < this.Width / 8f)
                        {
                            Vector2 at4 = new Vector2(this.Left + 4f + (float)(k * 8), this.Top - 1f);
                            bool flag18 = !this.Scene.CollideCheck<Water>(at4) && this.Scene.CollideCheck<Solid>(at4);
                            if (flag18)
                            {
                                this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at4 + add3, 1.57079637f);
                                this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at4 - add3, 1.57079637f);
                            }
                            at4 = default(Vector2);
                            int num = k;
                            k = num + 1;
                        }
                        add3 = default(Vector2);
                    }
                    else
                    {
                        bool flag19 = this.crushDir == Vector2.UnitY;
                        if (flag19)
                        {
                            Vector2 add4 = new Vector2(2f, 0f);
                            int l = 0;
                            while ((float)l < this.Width / 8f)
                            {
                                Vector2 at5 = new Vector2(this.Left + 4f + (float)(l * 8), this.Bottom + 1f);
                                bool flag20 = !this.Scene.CollideCheck<Water>(at5) && this.Scene.CollideCheck<Solid>(at5);
                                if (flag20)
                                {
                                    this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at5 + add4, -1.57079637f);
                                    this.SceneAs<Level>().ParticlesFG.Emit(CrushBlock.P_Impact, at5 - add4, -1.57079637f);
                                }
                                at5 = default(Vector2);
                                int num = l;
                                l = num + 1;
                            }
                            add4 = default(Vector2);
                        }
                    }
                }
            }
            Audio.Play("event:/game/06_reflection/crushblock_impact", this.Center);
            this.level.DirectionalShake(this.crushDir, 0.3f);
            Input.Rumble(RumbleStrength.Medium, RumbleLength.Medium);
            this.StartShaking(0.4f);
            this.StopPlayerRunIntoAnimation = true;
            SoundSource sfx = this.currentMoveLoopSfx;
            this.currentMoveLoopSfx.Param("end", 1f);
            this.currentMoveLoopSfx = null;
            Alarm.Set(this, 0.5f, delegate
            {
                sfx.RemoveSelf();
            }, Alarm.AlarmMode.Oneshot);
            this.crushDir = Vector2.Zero;
            this.TurnOffImages();
            bool flag21 = !this.chillOut;
            if (flag21)
            {
                this.face.Play("hurt", false, false);
                this.returnLoopSfx.Play("event:/game/06_reflection/crushblock_return_loop", null, 0f);
                yield return 0.4f;
                float speed2 = 0f;
                float waypointSfxDelay = 0f;
                while (this.returnStack.Count > 0)
                {
                    yield return null;
                    this.StopPlayerRunIntoAnimation = false;
                    SokobanBlock.MoveState ret = this.returnStack[this.returnStack.Count - 1];
                    speed2 = Calc.Approach(speed2, 60f, 160f * Engine.DeltaTime);
                    waypointSfxDelay -= Engine.DeltaTime;
                    bool flag22 = ret.Direction.X != 0f;
                    if (flag22)
                    {
                        this.MoveTowardsX(ret.From.X, speed2 * Engine.DeltaTime);
                    }
                    bool flag23 = ret.Direction.Y != 0f;
                    if (flag23)
                    {
                        this.MoveTowardsY(ret.From.Y, speed2 * Engine.DeltaTime);
                    }
                    bool atTarget = (ret.Direction.X == 0f || this.ExactPosition.X == ret.From.X) && (ret.Direction.Y == 0f || this.ExactPosition.Y == ret.From.Y);
                    bool flag24 = atTarget;
                    if (flag24)
                    {
                        speed2 = 0f;
                        this.returnStack.RemoveAt(this.returnStack.Count - 1);
                        this.StopPlayerRunIntoAnimation = true;
                        bool flag25 = this.returnStack.Count <= 0;
                        if (flag25)
                        {
                            this.face.Play("idle", false, false);
                            this.returnLoopSfx.Stop(true);
                            bool flag26 = waypointSfxDelay <= 0f;
                            if (flag26)
                            {
                                Audio.Play("event:/game/06_reflection/crushblock_rest", this.Center);
                            }
                        }
                        else
                        {
                            bool flag27 = waypointSfxDelay <= 0f;
                            if (flag27)
                            {
                                Audio.Play("event:/game/06_reflection/crushblock_rest_waypoint", this.Center);
                            }
                        }
                        waypointSfxDelay = 0.1f;
                        this.StartShaking(0.2f);
                        yield return 0.2f;
                    }
                    ret = default(SokobanBlock.MoveState);
                }
            }
            yield break;
        }

        // Token: 0x06000E0E RID: 3598 RVA: 0x00036080 File Offset: 0x00034280
        private bool MoveHCheck(float amount)
        {
            bool flag = base.MoveHCollideSolidsAndBounds(this.level, amount, true, null);
            bool result;
            if (flag)
            {
                bool flag2 = amount < 0f && base.Left <= (float)this.level.Bounds.Left;
                if (flag2)
                {
                    result = true;
                }
                else
                {
                    bool flag3 = amount > 0f && base.Right >= (float)this.level.Bounds.Right;
                    if (flag3)
                    {
                        result = true;
                    }
                    else
                    {
                        for (int i = 1; i <= 4; i++)
                        {
                            for (int j = 1; j >= -1; j -= 2)
                            {
                                Vector2 value = new Vector2((float)Math.Sign(amount), (float)(i * j));
                                bool flag4 = !base.CollideCheck<Solid>(this.Position + value);
                                if (flag4)
                                {
                                    this.MoveVExact(i * j);
                                    this.MoveHExact(Math.Sign(amount));
                                    return false;
                                }
                            }
                        }
                        result = true;
                    }
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x06000E0F RID: 3599 RVA: 0x000361A4 File Offset: 0x000343A4
        private bool MoveVCheck(float amount)
        {
            bool flag = base.MoveVCollideSolidsAndBounds(this.level, amount, true, null, false);
            bool result;
            if (flag)
            {
                bool flag2 = amount < 0f && base.Top <= (float)this.level.Bounds.Top;
                if (flag2)
                {
                    result = true;
                }
                else
                {
                    for (int i = 1; i <= 4; i++)
                    {
                        for (int j = 1; j >= -1; j -= 2)
                        {
                            Vector2 value = new Vector2((float)(i * j), (float)Math.Sign(amount));
                            bool flag3 = !base.CollideCheck<Solid>(this.Position + value);
                            if (flag3)
                            {
                                this.MoveHExact(i * j);
                                this.MoveVExact(Math.Sign(amount));
                                return false;
                            }
                        }
                    }
                    result = true;
                }
            }
            else
            {
                result = false;
            }
            return result;
        }

        // Token: 0x040008BB RID: 2235
        public static ParticleType P_Impact;

        // Token: 0x040008BC RID: 2236
        public static ParticleType P_Crushing = new ParticleType
        {
            Source = GFX.Game["particles/rect"],
            Color = Calc.HexToColor("e266ff"),
            Color2 = Calc.HexToColor("fffc68"),
            ColorMode = ParticleType.ColorModes.Blink,
            RotationMode = ParticleType.RotationModes.SameAsDirection,
            Size = 0.5f,
            SizeRange = 0.2f,
            DirectionRange = (float)Math.PI / 6f,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.5f,
            LifeMax = 1.2f,
            SpeedMin = 30f,
            SpeedMax = 50f,
            SpeedMultiplier = 0.4f,
            Acceleration = new Vector2(0f, 10f)
        };

        // Token: 0x040008BD RID: 2237
        public static ParticleType P_Activate = new ParticleType
        {
            Source = GFX.Game["particles/rect"],
            Color = Calc.HexToColor("e4cd5f"),
            Color2 = Color.White,
            ColorMode = ParticleType.ColorModes.Blink,
            RotationMode = ParticleType.RotationModes.SameAsDirection,
            Size = 0.5f,
            SizeRange = 0.2f,
            DirectionRange = (float)Math.PI / 6f,
            FadeMode = ParticleType.FadeModes.Late,
            LifeMin = 0.5f,
            LifeMax = 1.1f,
            SpeedMin = 60f,
            SpeedMax = 100f,
            SpeedMultiplier = 0.2f
        };

        // Token: 0x040008BE RID: 2238
        private const float CrushSpeed = 240f;

        // Token: 0x040008BF RID: 2239
        private const float CrushAccel = 500f;

        // Token: 0x040008C0 RID: 2240
        private const float ReturnSpeed = 60f;

        // Token: 0x040008C1 RID: 2241
        private const float ReturnAccel = 160f;

        // Token: 0x040008C2 RID: 2242
        private Color fill;

        // Token: 0x040008C3 RID: 2243
        private Level level;

        // Token: 0x040008C4 RID: 2244
        private bool canActivate;

        // Token: 0x040008C5 RID: 2245
        private Vector2 crushDir;

        // Token: 0x040008C6 RID: 2246
        private List<SokobanBlock.MoveState> returnStack;

        // Token: 0x040008C7 RID: 2247
        private Coroutine attackCoroutine;

        // Token: 0x040008C8 RID: 2248
        private bool canMoveVertically;

        // Token: 0x040008C9 RID: 2249
        private bool canMoveHorizontally;

        // Token: 0x040008CA RID: 2250
        private bool chillOut;

        // Token: 0x040008CB RID: 2251
        private bool giant;

        // Token: 0x040008CC RID: 2252
        private Sprite face;

        // Token: 0x040008CD RID: 2253
        private string nextFaceDirection;

        // Token: 0x040008CE RID: 2254
        private List<Image> idleImages;

        // Token: 0x040008CF RID: 2255
        private List<Image> activeTopImages;

        // Token: 0x040008D0 RID: 2256
        private List<Image> activeRightImages;

        // Token: 0x040008D1 RID: 2257
        private List<Image> activeLeftImages;

        // Token: 0x040008D2 RID: 2258
        private List<Image> activeBottomImages;

        // Token: 0x040008D3 RID: 2259
        private SoundSource currentMoveLoopSfx;

        // Token: 0x040008D4 RID: 2260
        private SoundSource returnLoopSfx;

        // Token: 0x020001B8 RID: 440
        public enum Axes
        {
            // Token: 0x040008D6 RID: 2262
            Both,
            // Token: 0x040008D7 RID: 2263
            Horizontal,
            // Token: 0x040008D8 RID: 2264
            Vertical
        }

        // Token: 0x020001B9 RID: 441
        private struct MoveState
        {
            // Token: 0x06000E12 RID: 3602 RVA: 0x000362E6 File Offset: 0x000344E6
            public MoveState(Vector2 from, Vector2 direction)
            {
                this.From = from;
                this.Direction = direction;
            }

            // Token: 0x040008D9 RID: 2265
            public Vector2 From;

            // Token: 0x040008DA RID: 2266
            public Vector2 Direction;
        }
    }
}
