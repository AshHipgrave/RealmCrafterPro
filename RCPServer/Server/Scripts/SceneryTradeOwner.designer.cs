using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public partial class SceneryTradeOwner : Scripting.Forms.Form
    {
        private void InitializeComponent()
        {
            CancelButton = new Scripting.Forms.Button();
            BuyButton = new Scripting.Forms.Button();
            label1 = new Scripting.Forms.Label();
            label2 = new Scripting.Forms.Label();
            OwnerLabel = new Scripting.Forms.Label();
            CostLabel = new Scripting.Forms.Label();
            ///
            ///CancelButton
            ///
            CancelButton.Location = new Scripting.Math.Vector2(160, 58);
            CancelButton.PositionType = Scripting.Forms.PositionType.Absolute;
            CancelButton.Size = new Scripting.Math.Vector2(75, 23);
            CancelButton.SizeType = Scripting.Forms.SizeType.Absolute;
            CancelButton.Name = "CancelButton";
            CancelButton.Text = "Close";
            CancelButton.Click += new Scripting.Forms.EventHandler(CancelButton_Click);
            ///
            ///BuyButton
            ///
            BuyButton.Location = new Scripting.Math.Vector2(79, 58);
            BuyButton.PositionType = Scripting.Forms.PositionType.Absolute;
            BuyButton.Size = new Scripting.Math.Vector2(75, 23);
            BuyButton.SizeType = Scripting.Forms.SizeType.Absolute;
            BuyButton.Name = "BuyButton";
            BuyButton.Text = "Buy";
            BuyButton.Click += new Scripting.Forms.EventHandler(BuyButton_Click);
            ///
            ///label1
            ///
            label1.Location = new Scripting.Math.Vector2(12, 12);
            label1.PositionType = Scripting.Forms.PositionType.Absolute;
            label1.Size = new Scripting.Math.Vector2(91, 20);
            label1.SizeType = Scripting.Forms.SizeType.Absolute;
            label1.Name = "label1";
            label1.Text = "Current Owner:";
            ///
            ///label2
            ///
            label2.Location = new Scripting.Math.Vector2(12, 30);
            label2.PositionType = Scripting.Forms.PositionType.Absolute;
            label2.Size = new Scripting.Math.Vector2(75, 20);
            label2.SizeType = Scripting.Forms.SizeType.Absolute;
            label2.Name = "label2";
            label2.Text = "Cost:";
            ///
            ///OwnerLabel
            ///
            OwnerLabel.Location = new Scripting.Math.Vector2(114, 12);
            OwnerLabel.PositionType = Scripting.Forms.PositionType.Absolute;
            OwnerLabel.Size = new Scripting.Math.Vector2(75, 20);
            OwnerLabel.SizeType = Scripting.Forms.SizeType.Absolute;
            OwnerLabel.Name = "OwnerLabel";
            OwnerLabel.Text = "None";
            ///
            ///CostLabel
            ///
            CostLabel.Location = new Scripting.Math.Vector2(114, 30);
            CostLabel.PositionType = Scripting.Forms.PositionType.Absolute;
            CostLabel.Size = new Scripting.Math.Vector2(75, 20);
            CostLabel.SizeType = Scripting.Forms.SizeType.Absolute;
            CostLabel.Name = "CostLabel";
            CostLabel.Text = "0";
            ///
            ///this
            ///
            this.Controls.Add(CancelButton);
            this.Controls.Add(BuyButton);
            this.Controls.Add(label1);
            this.Controls.Add(label2);
            this.Controls.Add(OwnerLabel);
            this.Controls.Add(CostLabel);
            this.Location = new Scripting.Math.Vector2(0.5, 0.5);
            this.PositionType = Scripting.Forms.PositionType.Centered;
            this.Size = new Scripting.Math.Vector2(260, 130);
            this.SizeType = Scripting.Forms.SizeType.Absolute;
            this.Text = "Chest Script";
        }
        
        public Scripting.Forms.Button CancelButton;
        public Scripting.Forms.Button BuyButton;
        public Scripting.Forms.Label label1;
        public Scripting.Forms.Label label2;
        public Scripting.Forms.Label OwnerLabel;
        public Scripting.Forms.Label CostLabel;
    }
}

