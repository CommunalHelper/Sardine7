using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste;
using Monocle;
using System.Xml;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using Celeste.Mod;

namespace Sardine7.Triggers
{
    [TrackedAs(typeof(MiniTextboxTrigger))]
    [CustomEntity("Sardine7/SecondTextboxTrigger")]
    class SecondTextboxTrigger : Trigger
    {
        [TrackedAs(typeof(MiniTextbox))]
        private class SecondTextbox : Entity
        {
            public const float TextScale = 0.75f;

            public const float BoxWidth = 1688f;

            public const float BoxHeight = 144f;

            public const float HudElementHeight = 180f;

            private int index;

            private FancyText.Text[] text;

            private MTexture[] box;

            private float ease;

            public bool closing;

            private float[] lifespan;

            private Coroutine routine;

            private Sprite[] portrait;

            private FancyText.Portrait[] portraitData;

            private float portraitSize;

            private float[] portraitScale;

            private SoundSource talkerSfx;

            private int dialogIndex = 0;

            private int ceiling;

            public static bool Displayed
            {
                get
                {
                    foreach (SecondTextbox entity in Engine.Scene.Tracker.GetEntities<SecondTextbox>())
                    {
                        if (!entity.closing && entity.ease > 0.25f)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            public SecondTextbox(string[][] dialogId, float[] lifespan)
            {
                this.lifespan = lifespan;
                base.Tag = ((int)Tags.HUD | (int)Tags.TransitionUpdate);
                portraitSize = 112f;
                text = dialogId.Select(s => FancyText.Parse(Dialog.Get(Calc.Random.Choose(s).Trim()), (int)(1688f - portraitSize - 32f), 2)).ToArray();
                ceiling = text.Length;
                portraitData = new FancyText.Portrait[ceiling];
                portrait = new Sprite[ceiling];
                portraitScale = new float[ceiling];
                box = Enumerable.Repeat(GFX.Portraits["textbox/default_mini"], ceiling).ToArray();
                for (int i=0;i<ceiling;i++)
                {
                    foreach (FancyText.Node node in text[i].Nodes)
                    {
                        if (node is FancyText.Portrait)
                        {
                            FancyText.Portrait port = portraitData[i] = (node as FancyText.Portrait);
                            this.portrait[i] = GFX.PortraitsSpriteBank.Create("portrait_" + port.Sprite);
                            XmlElement xML = GFX.PortraitsSpriteBank.SpriteData["portrait_" + port.Sprite].Sources[0].XML;
                            portraitScale[i] = portraitSize / xML.AttrFloat("size", 160f);
                            string id = "textbox/" + xML.Attr("textbox", "default") + "_mini";
                            if (GFX.Portraits.Has(id))
                            {
                                box[i] = GFX.Portraits[id];
                            }
                        }
                    }
                }
                Add(routine = new Coroutine(Routine()));
                routine.UseRawDeltaTime = true;
                Add(new TransitionListener
                {
                    OnOutBegin = delegate
                    {
                        if (!closing)
                        {
                            routine.Replace(Close());
                        }
                    }
                });
                if (Level.DialogSnapshot == null)
                {
                    Level.DialogSnapshot = Audio.CreateSnapshot("snapshot:/dialogue_in_progress", start: false);
                }
                Audio.ResumeSnapshot(Level.DialogSnapshot);
            }

            private IEnumerator Routine()
            {
                while(dialogIndex < ceiling)
                {
                    int i = dialogIndex;
                    List<Entity> entities = Scene.Tracker.GetEntities<MiniTextbox>();
                    foreach (SecondTextbox item in entities)
                    {
                        if (item != this)
                        {
                            item.Add(new Coroutine(item.Close()));
                        }
                    }
                    if (entities.Count > 0)
                    {
                        yield return 0.3f;
                    }
                    while ((ease += Engine.DeltaTime * 4f) < 1f)
                    {
                        yield return null;
                    }
                    ease = 1f;
                    if (portrait[i] != null)
                    {
                        Add(portrait);
                        string beginAnim = "begin_" + portraitData[i].Animation;
                        if (portrait[i].Has(beginAnim))
                        {
                            portrait[i].Play(beginAnim);
                            while (portrait[i].CurrentAnimationID == beginAnim && portrait[i].Animating)
                            {
                                yield return null;
                            }
                        }
                        portrait[i].Play("talk_" + portraitData[i].Animation);
                        talkerSfx = new SoundSource().Play(portraitData[i].SfxEvent);
                        talkerSfx.Param("dialogue_portrait", portraitData[i].SfxExpression);
                        talkerSfx.Param("dialogue_end", 0f);
                        Add(talkerSfx);
                    }
                    float num = 0f;
                    while (index < text[i].Nodes.Count)
                    {
                        if (text[i].Nodes[index] is FancyText.Char)
                        {
                            num += (text[i].Nodes[index] as FancyText.Char).Delay;
                        }
                        index++;
                        if (num > 0.016f)
                        {
                            yield return num;
                            num = 0f;
                        }
                    }
                    if (portrait[i] != null)
                    {
                        portrait[i].Play("idle_" + portraitData[i].Animation);
                    }
                    if (talkerSfx != null)
                    {
                        talkerSfx.Param("dialogue_portrait", 0f);
                        talkerSfx.Param("dialogue_end", 1f);
                    }
                    Audio.EndSnapshot(Level.DialogSnapshot);
                    yield return lifespan[i];
                    while ((ease -= Engine.DeltaTime * 4f) > 0f)
                    {
                        yield return null;
                    }
                    ease = 0f;
                    dialogIndex += 1;
                }
                yield return Close();
            }

            private IEnumerator Close()
            {
                if (!closing)
                {
                    closing = true;
                    RemoveSelf();
                }
                return null;
            }

            public override void Update()
            {
                if ((base.Scene as Level).RetryPlayerCorpse != null && !closing)
                {
                    routine.Replace(Close());
                }
                base.Update();
            }

            public override void Render()
            {
                int i = dialogIndex < ceiling ? dialogIndex : ceiling - 1;
                if (ease <= 0f)
                {
                    return;
                }
                Level level = base.Scene as Level;
                if (!level.FrozenOrPaused && level.RetryPlayerCorpse == null && !level.SkippingCutscene)
                {
                    Vector2 vector = new Vector2(Engine.Width / 2, 72f + ((float)Engine.Width - 1688f) / 4f);
                    Vector2 vector2 = vector + new Vector2(-828f, -56f);
                    box[i].DrawCentered(vector, Color.White, new Vector2(1f, ease));
                    if (portrait[i] != null)
                    {
                        portrait[i].Scale = new Vector2(1f, ease) * portraitScale[i];
                        portrait[i].RenderPosition = vector2 + new Vector2(portraitSize / 2f, portraitSize / 2f);
                        portrait[i].Render();
                    }
                    text[i].Draw(new Vector2(vector2.X + portraitSize + 32f, vector.Y), new Vector2(0f, 0.5f), new Vector2(1f, ease) * 0.75f, 1f, 0, index);
                }
            }

            public override void Removed(Scene scene)
            {
                Audio.EndSnapshot(Level.DialogSnapshot);
                base.Removed(scene);
            }

            public override void SceneEnd(Scene scene)
            {
                Audio.EndSnapshot(Level.DialogSnapshot);
                base.SceneEnd(scene);
            }
        }

        private enum Modes
        {
            OnPlayerEnter,
            OnLevelStart,
            OnTheoEnter
        }

        private EntityID id;

        private string[][] dialogOptions;

        private Modes mode;

        private bool triggered;

        private bool onlyOnce;

        private int deathCount;

        private float[] lifespan;

        public SecondTextboxTrigger(EntityData data, Vector2 offset, EntityID id)
            : base(data, offset)
        {
            this.id = id;
            mode = data.Enum("mode", Modes.OnPlayerEnter);
            dialogOptions = data.Attr("dialog_id").Split(';').Select(s => s.Split(',')).ToArray();
            onlyOnce = data.Bool("only_once");
            deathCount = data.Int("death_count", -1);
            lifespan = data.Attr("lifespan").Split(',').Select(Convert.ToSingle).ToArray();
            if (mode == Modes.OnTheoEnter)
            {
                Add(new HoldableCollider((Action<Holdable>)delegate
                {
                    Trigger();
                }, (Collider)null));
            }
        }

        public override void Awake(Scene scene)
        {
            base.Awake(scene);
            if (mode == Modes.OnLevelStart)
            {
                Trigger();
            }
        }

        public override void OnEnter(Player player)
        {
            if (mode == Modes.OnPlayerEnter)
            {
                Trigger();
            }
        }

        private void Trigger()
        {
            if (!triggered && (deathCount < 0 || (base.Scene as Level).Session.DeathsInCurrentLevel == deathCount))
            {
                triggered = true;
                base.Scene.Add(new SecondTextbox(dialogOptions, lifespan));
                if (onlyOnce)
                {
                    (base.Scene as Level).Session.DoNotLoad.Add(id);
                }
            }
        }
    }
}
