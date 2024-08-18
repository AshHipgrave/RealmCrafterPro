using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public partial class SceneryTradeWindow : Scripting.Forms.Form
    {
        private void InitializeComponent()
        {
            itemButton1 = new Scripting.Forms.ItemButton();
            itemButton2 = new Scripting.Forms.ItemButton();
            itemButton3 = new Scripting.Forms.ItemButton();
            itemButton4 = new Scripting.Forms.ItemButton();
            itemButton5 = new Scripting.Forms.ItemButton();
            CancelButton = new Scripting.Forms.Button();
            ///
            ///itemButton1
            ///
            itemButton1.Location = new Scripting.Math.Vector2(12, 12);
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
            ///itemButton2
            ///
            itemButton2.Location = new Scripting.Math.Vector2(70, 12);
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
            itemButton3.Location = new Scripting.Math.Vector2(128, 12);
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
            itemButton4.Location = new Scripting.Math.Vector2(186, 12);
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
            itemButton5.Location = new Scripting.Math.Vector2(244, 12);
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
            ///CancelButton
            ///
            CancelButton.Location = new Scripting.Math.Vector2(217, 72);
            CancelButton.PositionType = Scripting.Forms.PositionType.Absolute;
            CancelButton.Size = new Scripting.Math.Vector2(75, 23);
            CancelButton.SizeType = Scripting.Forms.SizeType.Absolute;
            CancelButton.Name = "CancelButton";
            CancelButton.Text = "Close";
            CancelButton.Click += new Scripting.Forms.EventHandler(CancelButton_Click);
            ///
            ///this
            ///
            this.Controls.Add(itemButton1);
            this.Controls.Add(itemButton2);
            this.Controls.Add(itemButton3);
            this.Controls.Add(itemButton4);
            this.Controls.Add(itemButton5);
            this.Controls.Add(CancelButton);
            this.Location = new Scripting.Math.Vector2(0.5, 0.5);
            this.PositionType = Scripting.Forms.PositionType.Centered;
            this.Size = new Scripting.Math.Vector2(320, 140);
            this.SizeType = Scripting.Forms.SizeType.Absolute;
            this.Text = "Chest Script";
        }
        
        public Scripting.Forms.ItemButton itemButton1;
        public Scripting.Forms.ItemButton itemButton2;
        public Scripting.Forms.ItemButton itemButton3;
        public Scripting.Forms.ItemButton itemButton4;
        public Scripting.Forms.ItemButton itemButton5;
        public Scripting.Forms.Button CancelButton;
    }
}

