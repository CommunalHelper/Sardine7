using System.Collections;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using static Celeste.TalkComponent;

namespace Celeste.Mod.Sardine7.Entities
{
    // SeekerInTheDark
    [CustomEntity("Sardine7/HumbleLookout")]
    [TrackedAs(typeof(Lookout))]
    public class HumbleLookout : Lookout
    {
        private class UnsociableTalkComponentUI : TalkComponentUI
        {
            public UnsociableTalkComponentUI(TalkComponent handler)
                : base(handler)
            {}

            public new void Render()
            { }
        }

        private bool talkative;

        private bool attractive;

        public HumbleLookout(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            attractive = data.Bool("attractive");
            talkative = data.Bool("talkative");
        }

        public override void Added(Scene scene)
        {
            base.Added(scene);
            DynData<Lookout> baseData = new DynData<Lookout>((Lookout)this);
            if (!attractive)
            {
                Sprite sprite = baseData.Get<Sprite>("sprite");
                sprite = Sardine7Module.SpriteBank.CreateOn(sprite, "humble_lookout");
                Tween lightTween = baseData.Get<Tween>("lightTween");
                sprite.OnFrameChange = delegate (string s)
                {
                    if ((s == "idle" || s == "badeline_idle" || s == "nobackpack_idle") && sprite.CurrentAnimationFrame == sprite.CurrentAnimationTotalFrames - 1)
                    {
                        lightTween.Start();
                    }
                };
            }
            if (!talkative)
            {
                TalkComponent talk = baseData.Get<TalkComponent>("talk");
                talk.UI = new UnsociableTalkComponentUI(talk);
            }
        }

        private new IEnumerator LookRoutine(Player player)
        {
            yield return player.DummyWalkToExact((int)X, walkBackwards: false, 1f, cancelOnFall: true);
        }
    }
}
