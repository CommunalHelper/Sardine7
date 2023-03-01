using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;
using Monocle;

namespace Celeste.Mod.Sardine7.Triggers
{
    [Tracked]
    [CustomEntity("Sardine7/SmoothieCameraTargetTrigger")]
    class SmoothieCameraTargetTrigger : Trigger
    {

        public Vector2 Target;

        public float xLerpStrength;

        public float yLerpStrength;

        public PositionModes PositionMode;

        public bool XOnly;

        public bool YOnly;

        public string DeleteFlag;

        public SmoothieCameraTargetTrigger(EntityData data, Vector2 offset)
            : base(data, offset)
        {
            Target = data.Nodes[0] + offset - new Vector2(320f, 180f) * 0.5f;
            xLerpStrength = data.Float("xLerpStrength");
            yLerpStrength = data.Float("yLerpStrength");
            PositionMode = data.Enum("positionMode", PositionModes.NoEffect);
            XOnly = data.Bool("xOnly");
            YOnly = data.Bool("yOnly");
            DeleteFlag = data.Attr("deleteFlag");
        }

        public override void OnStay(Player player)
        {
            if (string.IsNullOrEmpty(DeleteFlag) || !SceneAs<Level>().Session.GetFlag(DeleteFlag))
            {
                player.CameraAnchor = Target;
                player.CameraAnchorLerp = Vector2.UnitX * MathHelper.Clamp(xLerpStrength * GetPositionLerp(player, PositionMode), 0f, 1f)
                    + Vector2.UnitY * MathHelper.Clamp(yLerpStrength * GetPositionLerp(player, PositionMode), 0f, 1f);
                player.CameraAnchorIgnoreX = YOnly;
                player.CameraAnchorIgnoreY = XOnly;
            }
        }

        public override void OnLeave(Player player)
        {
            base.OnLeave(player);
            bool flag = false;
            foreach (CameraTargetTrigger entity in base.Scene.Tracker.GetEntities<CameraTargetTrigger>())
            {
                if (entity.PlayerIsInside)
                {
                    flag = true;
                    break;
                }
            }
            if (!flag)
            {
                foreach (CameraAdvanceTargetTrigger entity2 in base.Scene.Tracker.GetEntities<CameraAdvanceTargetTrigger>())
                {
                    if (entity2.PlayerIsInside)
                    {
                        flag = true;
                        break;
                    }
                }
            }
            if (!flag)
            {
                player.CameraAnchorLerp = Vector2.Zero;
            }
        }
    }
}
