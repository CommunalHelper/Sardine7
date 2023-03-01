using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste.Mod.Sardine7.Entities;
using Celeste;

namespace Celeste.Mod.Sardine7.Triggers
{
    [CustomEntity("Sardine7/GokuroSpawnTrigger")]
    class GokuroSpawnTrigger : Trigger
    {
        private EntityData data;

        private string flag;

        private string notFlag;

        private float yPosition;

        public GokuroSpawnTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            flag = data.Attr("flag");
            notFlag = data.Attr("notFlag");
            yPosition = data.Float("yPosition", -1f);
            this.data = data;
        }

        public override void OnEnter(Player player)
        {
            if ((string.IsNullOrEmpty(flag) || (base.Scene as Level).Session.GetFlag(flag)) && !(base.Scene as Level).Session.GetFlag(notFlag))
            {
                base.OnEnter(player);
                Level level = SceneAs<Level>();
                Vector2 position = new Vector2(level.Bounds.Left - 32, level.Bounds.Top + (yPosition == -1f ? level.Bounds.Height / 2 : yPosition));
                base.Scene.Add(new Gokuro(position, data));
                RemoveSelf();
            }
        }
    }
}
