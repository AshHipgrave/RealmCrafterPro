using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public partial class BankWindow : Scripting.Forms.Form
    {
        private void InitializeComponent()
        {
            label1 = new Scripting.Forms.Label();
            MoneyAmount = new Scripting.Forms.TextBox();
            MoneyCount = new Scripting.Forms.Label();
            DepositButton = new Scripting.Forms.Button();
            WithdrawButton = new Scripting.Forms.Button();
            Previous = new Scripting.Forms.Button();
            Next = new Scripting.Forms.Button();
            PageLabel = new Scripting.Forms.Label();
            ///
            ///label1
            ///
            label1.Location = new Scripting.Math.Vector2(19, 14);
            label1.PositionType = Scripting.Forms.PositionType.Absolute;
            label1.Size = new Scripting.Math.Vector2(48, 20);
            label1.SizeType = Scripting.Forms.SizeType.Absolute;
            label1.Name = "label1";
            label1.Text = "Money:";
            label1.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            label1.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///MoneyAmount
            ///
            MoneyAmount.Location = new Scripting.Math.Vector2(139, 12);
            MoneyAmount.PositionType = Scripting.Forms.PositionType.Absolute;
            MoneyAmount.Size = new Scripting.Math.Vector2(87, 20);
            MoneyAmount.SizeType = Scripting.Forms.SizeType.Absolute;
            MoneyAmount.Name = "MoneyAmount";
            MoneyAmount.Text = "0";
            ///
            ///MoneyCount
            ///
            MoneyCount.Location = new Scripting.Math.Vector2(76, 14);
            MoneyCount.PositionType = Scripting.Forms.PositionType.Absolute;
            MoneyCount.Size = new Scripting.Math.Vector2(57, 20);
            MoneyCount.SizeType = Scripting.Forms.SizeType.Absolute;
            MoneyCount.Name = "MoneyCount";
            MoneyCount.Text = "0";
            MoneyCount.Align = Scripting.Forms.TextAlign.Top;
            MoneyCount.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            MoneyCount.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///DepositButton
            ///
            DepositButton.Location = new Scripting.Math.Vector2(240, 11);
            DepositButton.PositionType = Scripting.Forms.PositionType.Absolute;
            DepositButton.Size = new Scripting.Math.Vector2(75, 20);
            DepositButton.SizeType = Scripting.Forms.SizeType.Absolute;
            DepositButton.Name = "DepositButton";
            DepositButton.Text = "Deposit";
            DepositButton.Click += new Scripting.Forms.EventHandler(DepositMoney);
            ///
            ///WithdrawButton
            ///
            WithdrawButton.Location = new Scripting.Math.Vector2(325, 12);
            WithdrawButton.PositionType = Scripting.Forms.PositionType.Absolute;
            WithdrawButton.Size = new Scripting.Math.Vector2(75, 20);
            WithdrawButton.SizeType = Scripting.Forms.SizeType.Absolute;
            WithdrawButton.Name = "WithdrawButton";
            WithdrawButton.Text = "Withdraw";
            WithdrawButton.Align = Scripting.Forms.TextAlign.Middle;
            WithdrawButton.Click += new Scripting.Forms.EventHandler(WithdrawMoney);
            ///
            ///Previous
            ///
            Previous.Location = new Scripting.Math.Vector2(97, 42);
            Previous.PositionType = Scripting.Forms.PositionType.Absolute;
            Previous.Size = new Scripting.Math.Vector2(49, 20);
            Previous.SizeType = Scripting.Forms.SizeType.Absolute;
            Previous.Name = "Previous";
            Previous.Text = "<<";
            Previous.Click += new Scripting.Forms.EventHandler(Previous_Click);
            ///
            ///Next
            ///
            Next.Location = new Scripting.Math.Vector2(159, 42);
            Next.PositionType = Scripting.Forms.PositionType.Absolute;
            Next.Size = new Scripting.Math.Vector2(49, 20);
            Next.SizeType = Scripting.Forms.SizeType.Absolute;
            Next.Name = "Next";
            Next.Text = ">>";
            Next.Click += new Scripting.Forms.EventHandler(Next_Click);
            ///
            ///PageLabel
            ///
            PageLabel.Location = new Scripting.Math.Vector2(18, 42);
            PageLabel.PositionType = Scripting.Forms.PositionType.Absolute;
            PageLabel.Size = new Scripting.Math.Vector2(75, 20);
            PageLabel.SizeType = Scripting.Forms.SizeType.Absolute;
            PageLabel.Name = "PageLabel";
            PageLabel.Text = "Pages";
            PageLabel.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            PageLabel.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///this
            ///
            this.Controls.Add(label1);
            this.Controls.Add(MoneyAmount);
            this.Controls.Add(MoneyCount);
            this.Controls.Add(DepositButton);
            this.Controls.Add(WithdrawButton);
            this.Controls.Add(Previous);
            this.Controls.Add(Next);
            this.Controls.Add(PageLabel);
            this.Location = new Scripting.Math.Vector2(0.5, 0.5);
            this.PositionType = Scripting.Forms.PositionType.Centered;
            this.Size = new Scripting.Math.Vector2(500, 360);
            this.SizeType = Scripting.Forms.SizeType.Absolute;
            this.Name = "BankWindow";
            this.Text = "Bank";
            this.BackColor = System.Drawing.Color.FromArgb(255, 242, 242, 242);
        }

        public Scripting.Forms.Label label1;
        public Scripting.Forms.TextBox MoneyAmount;
        public Scripting.Forms.Label MoneyCount;
        public Scripting.Forms.Button DepositButton;
        public Scripting.Forms.Button WithdrawButton;
        public Scripting.Forms.Button Previous;
        public Scripting.Forms.Button Next;
        public Scripting.Forms.Label PageLabel;
    }
}