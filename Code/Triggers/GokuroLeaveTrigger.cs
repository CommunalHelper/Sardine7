using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Celeste.Mod.Sardine7.Entities;
using System.Linq;

namespace Celeste.Mod.Sardine7.Triggers
{
    [CustomEntity("Sardine7/GokuroLeaveTrigger")]
    class GokuroLeaveTrigger : Trigger
    {
        private readonly int count;

        public GokuroLeaveTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            this.count = data.Int("count", 1);
        }

        public override void OnEnter(Player player)
        {
            base.OnEnter(player);
            var gokuroList = base.Scene.Tracker.GetEntities<Gokuro>();
            foreach (Gokuro kuroko in gokuroList.Take(count))
                kuroko.Leave();
            RemoveSelf();
        }
    }
}
