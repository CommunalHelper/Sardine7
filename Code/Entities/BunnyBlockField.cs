using Microsoft.Xna.Framework;
using Monocle;
using Celeste.Mod.Entities;

namespace Celeste.Mod.Sardine7.Entities
{
    [CustomEntity("Sardine7/BunnyBlockField")]
    [Tracked]
    public class BunnyBlockField : Entity {

        public BunnyBlockField(Vector2 position, int width, int height)
            : base(position) {
            Collider = new Hitbox(width, height);
        }

        public BunnyBlockField(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.Width, data.Height) {
        }

    }
}
