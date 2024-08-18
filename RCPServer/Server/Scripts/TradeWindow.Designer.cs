using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public partial class TradeWindow : Scripting.Forms.Form
    {
        private void InitializeComponent()
        {
            itemButton1 = new Scripting.Forms.ItemButton();
            label1 = new Scripting.Forms.Label();
            itemButton2 = new Scripting.Forms.ItemButton();
            itemButton3 = new Scripting.Forms.ItemButton();
            itemButton4 = new Scripting.Forms.ItemButton();
            itemButton5 = new Scripting.Forms.ItemButton();
            itemButton6 = new Scripting.Forms.ItemButton();
            itemButton7 = new Scripting.Forms.ItemButton();
            itemButton8 = new Scripting.Forms.ItemButton();
            itemButton9 = new Scripting.Forms.ItemButton();
            itemButton10 = new Scripting.Forms.ItemButton();
            itemButton11 = new Scripting.Forms.ItemButton();
            MoneyText = new Scripting.Forms.TextBox();
            label2 = new Scripting.Forms.Label();
            label3 = new Scripting.Forms.Label();
            itemButton12 = new Scripting.Forms.ItemButton();
            itemButton13 = new Scripting.Forms.ItemButton();
            itemButton14 = new Scripting.Forms.ItemButton();
            itemButton15 = new Scripting.Forms.ItemButton();
            itemButton16 = new Scripting.Forms.ItemButton();
            itemButton17 = new Scripting.Forms.ItemButton();
            itemButton18 = new Scripting.Forms.ItemButton();
            itemButton19 = new Scripting.Forms.ItemButton();
            itemButton20 = new Scripting.Forms.ItemButton();
            CancelButton = new Scripting.Forms.Button();
            TradeButton = new Scripting.Forms.Button();
            label4 = new Scripting.Forms.Label();
            MoneyLabel = new Scripting.Forms.Label();
            ///
            ///itemButton1
            ///
            itemButton1.Location = new Scripting.Math.Vector2(12, 32);
            itemButton1.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton1.Size = new Scripting.Math.Vector2(48, 48);
            itemButton1.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton1.Name = "itemButton1";
            itemButton1.Text = "button";
            itemButton1.CanDragToInventory = true;
            itemButton1.CanDragFromInventory = true;
            itemButton1.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton1.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton1.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///label1
            ///
            label1.Location = new Scripting.Math.Vector2(12, 12);
            label1.PositionType = Scripting.Forms.PositionType.Absolute;
            label1.Size = new Scripting.Math.Vector2(94, 20);
            label1.SizeType = Scripting.Forms.SizeType.Absolute;
            label1.Name = "label1";
            label1.Text = "Items to Trade:";
            label1.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            label1.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///itemButton2
            ///
            itemButton2.Location = new Scripting.Math.Vector2(70, 32);
            itemButton2.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton2.Size = new Scripting.Math.Vector2(48, 48);
            itemButton2.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton2.Name = "itemButton2";
            itemButton2.Text = "button";
            itemButton2.CanDragToInventory = true;
            itemButton2.CanDragFromInventory = true;
            itemButton2.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton2.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton2.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton3
            ///
            itemButton3.Location = new Scripting.Math.Vector2(128, 32);
            itemButton3.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton3.Size = new Scripting.Math.Vector2(48, 48);
            itemButton3.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton3.Name = "itemButton3";
            itemButton3.Text = "button";
            itemButton3.CanDragToInventory = true;
            itemButton3.CanDragFromInventory = true;
            itemButton3.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton3.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton3.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton4
            ///
            itemButton4.Location = new Scripting.Math.Vector2(186, 32);
            itemButton4.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton4.Size = new Scripting.Math.Vector2(48, 48);
            itemButton4.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton4.Name = "itemButton4";
            itemButton4.Text = "button";
            itemButton4.CanDragToInventory = true;
            itemButton4.CanDragFromInventory = true;
            itemButton4.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton4.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton4.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton5
            ///
            itemButton5.Location = new Scripting.Math.Vector2(244, 32);
            itemButton5.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton5.Size = new Scripting.Math.Vector2(48, 48);
            itemButton5.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton5.Name = "itemButton5";
            itemButton5.Text = "button";
            itemButton5.CanDragToInventory = true;
            itemButton5.CanDragFromInventory = true;
            itemButton5.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton5.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton5.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton6
            ///
            itemButton6.Location = new Scripting.Math.Vector2(12, 90);
            itemButton6.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton6.Size = new Scripting.Math.Vector2(48, 48);
            itemButton6.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton6.Name = "itemButton6";
            itemButton6.Text = "button";
            itemButton6.CanDragToInventory = true;
            itemButton6.CanDragFromInventory = true;
            itemButton6.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton6.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton6.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton7
            ///
            itemButton7.Location = new Scripting.Math.Vector2(70, 90);
            itemButton7.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton7.Size = new Scripting.Math.Vector2(48, 48);
            itemButton7.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton7.Name = "itemButton7";
            itemButton7.Text = "button";
            itemButton7.CanDragToInventory = true;
            itemButton7.CanDragFromInventory = true;
            itemButton7.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton7.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton7.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton8
            ///
            itemButton8.Location = new Scripting.Math.Vector2(128, 90);
            itemButton8.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton8.Size = new Scripting.Math.Vector2(48, 48);
            itemButton8.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton8.Name = "itemButton8";
            itemButton8.Text = "button";
            itemButton8.CanDragToInventory = true;
            itemButton8.CanDragFromInventory = true;
            itemButton8.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton8.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton8.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton9
            ///
            itemButton9.Location = new Scripting.Math.Vector2(186, 90);
            itemButton9.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton9.Size = new Scripting.Math.Vector2(48, 48);
            itemButton9.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton9.Name = "itemButton9";
            itemButton9.Text = "button";
            itemButton9.CanDragToInventory = true;
            itemButton9.CanDragFromInventory = true;
            itemButton9.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton9.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton9.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton10
            ///
            itemButton10.Location = new Scripting.Math.Vector2(244, 90);
            itemButton10.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton10.Size = new Scripting.Math.Vector2(48, 48);
            itemButton10.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton10.Name = "itemButton10";
            itemButton10.Text = "button";
            itemButton10.CanDragToInventory = true;
            itemButton10.CanDragFromInventory = true;
            itemButton10.RightClick += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton10.DraggedToInventory += new Scripting.Forms.EventHandler(Item_RightClick);
            itemButton10.DraggedFromInventory += new Scripting.Forms.EventHandler(Item_DraggedFromInventory);
            ///
            ///itemButton11
            ///
            itemButton11.Location = new Scripting.Math.Vector2(12, 198);
            itemButton11.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton11.Size = new Scripting.Math.Vector2(48, 48);
            itemButton11.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton11.Name = "itemButton11";
            itemButton11.Text = "button";
            ///
            ///MoneyText
            ///
            MoneyText.Location = new Scripting.Math.Vector2(69, 148);
            MoneyText.PositionType = Scripting.Forms.PositionType.Absolute;
            MoneyText.Size = new Scripting.Math.Vector2(75, 20);
            MoneyText.SizeType = Scripting.Forms.SizeType.Absolute;
            MoneyText.Name = "MoneyText";
            ///
            ///label2
            ///
            label2.Location = new Scripting.Math.Vector2(12, 150);
            label2.PositionType = Scripting.Forms.PositionType.Absolute;
            label2.Size = new Scripting.Math.Vector2(75, 20);
            label2.SizeType = Scripting.Forms.SizeType.Absolute;
            label2.Name = "label2";
            label2.Text = "Money: ";
            label2.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            label2.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///label3
            ///
            label3.Location = new Scripting.Math.Vector2(12, 178);
            label3.PositionType = Scripting.Forms.PositionType.Absolute;
            label3.Size = new Scripting.Math.Vector2(175, 20);
            label3.SizeType = Scripting.Forms.SizeType.Absolute;
            label3.Name = "label3";
            label3.Text = "Items ACTOR is Trading:";
            label3.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            label3.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///itemButton12
            ///
            itemButton12.Location = new Scripting.Math.Vector2(70, 198);
            itemButton12.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton12.Size = new Scripting.Math.Vector2(48, 48);
            itemButton12.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton12.Name = "itemButton12";
            itemButton12.Text = "button";
            ///
            ///itemButton13
            ///
            itemButton13.Location = new Scripting.Math.Vector2(128, 198);
            itemButton13.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton13.Size = new Scripting.Math.Vector2(48, 48);
            itemButton13.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton13.Name = "itemButton13";
            itemButton13.Text = "button";
            ///
            ///itemButton14
            ///
            itemButton14.Location = new Scripting.Math.Vector2(186, 198);
            itemButton14.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton14.Size = new Scripting.Math.Vector2(48, 48);
            itemButton14.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton14.Name = "itemButton14";
            itemButton14.Text = "button";
            ///
            ///itemButton15
            ///
            itemButton15.Location = new Scripting.Math.Vector2(244, 198);
            itemButton15.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton15.Size = new Scripting.Math.Vector2(48, 48);
            itemButton15.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton15.Name = "itemButton15";
            itemButton15.Text = "button";
            ///
            ///itemButton16
            ///
            itemButton16.Location = new Scripting.Math.Vector2(12, 256);
            itemButton16.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton16.Size = new Scripting.Math.Vector2(48, 48);
            itemButton16.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton16.Name = "itemButton16";
            itemButton16.Text = "button";
            ///
            ///itemButton17
            ///
            itemButton17.Location = new Scripting.Math.Vector2(70, 256);
            itemButton17.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton17.Size = new Scripting.Math.Vector2(48, 48);
            itemButton17.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton17.Name = "itemButton17";
            itemButton17.Text = "button";
            ///
            ///itemButton18
            ///
            itemButton18.Location = new Scripting.Math.Vector2(128, 256);
            itemButton18.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton18.Size = new Scripting.Math.Vector2(48, 48);
            itemButton18.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton18.Name = "itemButton18";
            itemButton18.Text = "button";
            ///
            ///itemButton19
            ///
            itemButton19.Location = new Scripting.Math.Vector2(186, 256);
            itemButton19.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton19.Size = new Scripting.Math.Vector2(48, 48);
            itemButton19.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton19.Name = "itemButton19";
            itemButton19.Text = "button";
            ///
            ///itemButton20
            ///
            itemButton20.Location = new Scripting.Math.Vector2(244, 256);
            itemButton20.PositionType = Scripting.Forms.PositionType.Absolute;
            itemButton20.Size = new Scripting.Math.Vector2(48, 48);
            itemButton20.SizeType = Scripting.Forms.SizeType.Absolute;
            itemButton20.Name = "itemButton20";
            itemButton20.Text = "button";
            ///
            ///CancelButton
            ///
            CancelButton.Location = new Scripting.Math.Vector2(136, 344);
            CancelButton.PositionType = Scripting.Forms.PositionType.Absolute;
            CancelButton.Size = new Scripting.Math.Vector2(75, 20);
            CancelButton.SizeType = Scripting.Forms.SizeType.Absolute;
            CancelButton.Name = "CancelButton";
            CancelButton.Text = "Cancel";
            CancelButton.Click += new Scripting.Forms.EventHandler(CancelButton_Click);
            ///
            ///TradeButton
            ///
            TradeButton.Location = new Scripting.Math.Vector2(218, 344);
            TradeButton.PositionType = Scripting.Forms.PositionType.Absolute;
            TradeButton.Size = new Scripting.Math.Vector2(75, 20);
            TradeButton.SizeType = Scripting.Forms.SizeType.Absolute;
            TradeButton.Name = "TradeButton";
            TradeButton.Text = "Trade";
            TradeButton.Click += new Scripting.Forms.EventHandler(TradeButton_Click);
            ///
            ///label4
            ///
            label4.Location = new Scripting.Math.Vector2(12, 314);
            label4.PositionType = Scripting.Forms.PositionType.Absolute;
            label4.Size = new Scripting.Math.Vector2(46, 20);
            label4.SizeType = Scripting.Forms.SizeType.Absolute;
            label4.Name = "label4";
            label4.Text = "Money: ";
            label4.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            label4.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///MoneyLabel
            ///
            MoneyLabel.Location = new Scripting.Math.Vector2(70, 314);
            MoneyLabel.PositionType = Scripting.Forms.PositionType.Absolute;
            MoneyLabel.Size = new Scripting.Math.Vector2(75, 20);
            MoneyLabel.SizeType = Scripting.Forms.SizeType.Absolute;
            MoneyLabel.Name = "MoneyLabel";
            MoneyLabel.Text = "";
            MoneyLabel.ScissorRegion = new Scripting.Math.Vector2(0, 0);
            MoneyLabel.ScrollOffset = new Scripting.Math.Vector2(0, 0);
            ///
            ///this
            ///
            this.Controls.Add(itemButton1);
            this.Controls.Add(label1);
            this.Controls.Add(itemButton2);
            this.Controls.Add(itemButton3);
            this.Controls.Add(itemButton4);
            this.Controls.Add(itemButton5);
            this.Controls.Add(itemButton6);
            this.Controls.Add(itemButton7);
            this.Controls.Add(itemButton8);
            this.Controls.Add(itemButton9);
            this.Controls.Add(itemButton10);
            this.Controls.Add(itemButton11);
            this.Controls.Add(MoneyText);
            this.Controls.Add(label2);
            this.Controls.Add(label3);
            this.Controls.Add(itemButton12);
            this.Controls.Add(itemButton13);
            this.Controls.Add(itemButton14);
            this.Controls.Add(itemButton15);
            this.Controls.Add(itemButton16);
            this.Controls.Add(itemButton17);
            this.Controls.Add(itemButton18);
            this.Controls.Add(itemButton19);
            this.Controls.Add(itemButton20);
            this.Controls.Add(CancelButton);
            this.Controls.Add(TradeButton);
            this.Controls.Add(label4);
            this.Controls.Add(MoneyLabel);
            this.Location = new Scripting.Math.Vector2(0.5, 0.5);
            this.PositionType = Scripting.Forms.PositionType.Centered;
            this.Size = new Scripting.Math.Vector2(320, 410);
            this.SizeType = Scripting.Forms.SizeType.Absolute;
            this.Text = "Trading With:";
        }
        
        public Scripting.Forms.ItemButton itemButton1;
        public Scripting.Forms.Label label1;
        public Scripting.Forms.ItemButton itemButton2;
        public Scripting.Forms.ItemButton itemButton3;
        public Scripting.Forms.ItemButton itemButton4;
        public Scripting.Forms.ItemButton itemButton5;
        public Scripting.Forms.ItemButton itemButton6;
        public Scripting.Forms.ItemButton itemButton7;
        public Scripting.Forms.ItemButton itemButton8;
        public Scripting.Forms.ItemButton itemButton9;
        public Scripting.Forms.ItemButton itemButton10;
        public Scripting.Forms.ItemButton itemButton11;
        public Scripting.Forms.TextBox MoneyText;
        public Scripting.Forms.Label label2;
        public Scripting.Forms.Label label3;
        public Scripting.Forms.ItemButton itemButton12;
        public Scripting.Forms.ItemButton itemButton13;
        public Scripting.Forms.ItemButton itemButton14;
        public Scripting.Forms.ItemButton itemButton15;
        public Scripting.Forms.ItemButton itemButton16;
        public Scripting.Forms.ItemButton itemButton17;
        public Scripting.Forms.ItemButton itemButton18;
        public Scripting.Forms.ItemButton itemButton19;
        public Scripting.Forms.ItemButton itemButton20;
        public Scripting.Forms.Button CancelButton;
        public Scripting.Forms.Button TradeButton;
        public Scripting.Forms.Label label4;
        public Scripting.Forms.Label MoneyLabel;
    }
}

