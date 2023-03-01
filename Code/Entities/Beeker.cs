using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Sardine7.Entities
{
    // SeekerInTheDark
    [CustomEntity("Sardine7/Beeker")]
    [TrackedAs(typeof(Seeker))]
    public class Beeker : Seeker
    {
        public Beeker(Vector2 position, Vector2[] patrolPoints)
            : base(position, patrolPoints) => this.Light.RemoveSelf();  // Lights Off!

        public Beeker(EntityData data, Vector2 offset)
            : this(data.Position + offset, data.NodesOffset(offset))
        {
        }
    }
}