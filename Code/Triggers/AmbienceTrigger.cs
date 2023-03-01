using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Sardine7.Triggers
{
    [CustomEntity("Sardine7/AmbienceTrigger")]
    class AmbienceTrigger : Trigger
    {
        public string Track;

        public bool ResetOnLeave;

        private string oldTrack;

        public AmbienceTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            Track = data.Attr("track");
            ResetOnLeave = data.Bool("resetOnLeave", defaultValue: false);
        }

        public override void OnEnter(Player player)
        {
            if (ResetOnLeave)
            {
                oldTrack = Audio.GetEventName(Audio.CurrentAmbienceEventInstance);
            }
            Session session = SceneAs<Level>().Session;
            session.Audio.Ambience.Event = SFX.EventnameByHandle(Track);
            session.Audio.Apply();
        }

        public override void OnLeave(Player player)
        {
            if (ResetOnLeave)
            {
                Session session = SceneAs<Level>().Session;
                session.Audio.Ambience.Event = oldTrack;
                session.Audio.Apply();
            }
        }
    }
}