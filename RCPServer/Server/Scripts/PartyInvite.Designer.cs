using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public partial class PartyInvite : Scripting.Forms.Form
    {
        private void InitializeComponent()
        {
            label1 = new Scripting.Forms.Label();
            PlayerName = new Scripting.Forms.Label();
            AcceptButton = new Scripting.Forms.Button();
            DeclineButton = new Scripting.Forms.Button();
            ///
            ///label1
            ///
            label1.Location = new Scripting.Math.Vector2(14, 20);
            label1.PositionType = Scripting.Forms.PositionType.Absolute;
            label1.Size = new Scripting.Math.Vector2(75, 20);
            label1.SizeType = Scripting.Forms.SizeType.Absolute;
            label1.Name = "label1";
            label1.Text = "You have been invited to join a party by ";
            ///
            ///PlayerName
            ///
            PlayerName.Location = new Scripting.Math.Vector2(20, 48);
            PlayerName.PositionType = Scripting.Forms.PositionType.Absolute;
            PlayerName.Size = new Scripting.Math.Vector2(75, 20);
            PlayerName.SizeType = Scripting.Forms.SizeType.Absolute;
            PlayerName.Name = "PlayerName";
            PlayerName.Text = "PlayerName";
            ///
            ///AcceptButton
            ///
            AcceptButton.Location = new Scripting.Math.Vector2(20, 78);
            AcceptButton.PositionType = Scripting.Forms.PositionType.Absolute;
            AcceptButton.Size = new Scripting.Math.Vector2(75, 20);
            AcceptButton.SizeType = Scripting.Forms.SizeType.Absolute;
            AcceptButton.Name = "AcceptButton";
            AcceptButton.Text = "Accept";
            ///
            ///DeclineButton
            ///
            DeclineButton.Location = new Scripting.Math.Vector2(118, 76);
            DeclineButton.PositionType = Scripting.Forms.PositionType.Absolute;
            DeclineButton.Size = new Scripting.Math.Vector2(75, 20);
            DeclineButton.SizeType = Scripting.Forms.SizeType.Absolute;
            DeclineButton.Name = "DeclineButton";
            DeclineButton.Text = "Decline";
            ///
            ///this
            ///
            this.Controls.Add(label1);
            this.Controls.Add(PlayerName);
            this.Controls.Add(AcceptButton);
            this.Controls.Add(DeclineButton);
            this.Location = new Scripting.Math.Vector2(0.5, 0.5);
            this.PositionType = Scripting.Forms.PositionType.Centered;
            this.Size = new Scripting.Math.Vector2(300, 150);
            this.SizeType = Scripting.Forms.SizeType.Absolute;
            this.Text = "Party Invite";
        }
        
        public Scripting.Forms.Label label1;
        public Scripting.Forms.Label PlayerName;
        public Scripting.Forms.Button AcceptButton;
        public Scripting.Forms.Button DeclineButton;
    }
}

