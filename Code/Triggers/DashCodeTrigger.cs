using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Celeste.Mod.Entities;
using Microsoft.Xna.Framework;

namespace Celeste.Mod.Sardine7.Triggers
{
    [CustomEntity("Sardine7/DashCodeTrigger")]
    class DashCodeTrigger : Trigger
    {
        public bool ResetOnLeave;

        public bool DisableOnLeave;

        private readonly string[] code;

        private string flag;

        private bool flagValue;

        private bool enabled = false;

        private List<string> currentInputs = new List<string>();

        private DashListener dashListener;

        public DashCodeTrigger(EntityData data, Vector2 offset) : base(data, offset)
        {
            code = data.Attr("code").Split(',').Select(Convert.ToString).ToArray();
            flag = data.Attr("flag");
            flagValue = data.Bool("flagValue", true);
            ResetOnLeave = data.Bool("resetOnLeave");
            DisableOnLeave = data.Bool("disableOnLeave", true);
            Add(dashListener = new DashListener());
            dashListener.OnDash = delegate (Vector2 dir)
            {
                string text = "";
                if (dir.Y < 0f)
                {
                    text = "U";
                }
                else if (dir.Y > 0f)
                {
                    text = "D";
                }
                if (dir.X < 0f)
                {
                    text += "L";
                }
                else if (dir.X > 0f)
                {
                    text += "R";
                }
                currentInputs.Add(text);
                if (currentInputs.Count > code.Length)
                    currentInputs.RemoveAt(0);
                if (enabled && currentInputs.Count == code.Length)
                {
                    bool flag2 = true;
                    for (int j = 0; j < code.Length; j++)
                    {
                        if (!currentInputs[j].Equals(code[j]))
                        {
                            flag2 = false;
                        }
                    }
                    if (flag2)
                    {
                        (base.Scene as Level).Session.SetFlag(flag, flagValue);
                        RemoveSelf();
                    }
                }
            };
        }

        public override void OnEnter(Player player)
        {
            enabled = true;
        }

        public override void OnLeave(Player player)
        {
            if (DisableOnLeave)
                enabled = false;
            if (ResetOnLeave)
                currentInputs = new List<string>();
        }
    }
}
