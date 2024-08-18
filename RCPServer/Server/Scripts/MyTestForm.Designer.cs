using System;
using System.Collections.Generic;
using System.Text;
using Scripting;

namespace UserScripts
{
    public partial class MyTestForm : Scripting.Forms.Form
    {
        private void InitializeComponent()
        {
            button1 = new Scripting.Forms.Button();
            textBox1 = new Scripting.Forms.TextBox();
            checkBox1 = new Scripting.Forms.CheckBox();
            comboBox1 = new Scripting.Forms.ComboBox();
            label1 = new Scripting.Forms.Label();
            listBox1 = new Scripting.Forms.ListBox();
            panel1 = new Scripting.Forms.Panel();
            progressBarUp = new Scripting.Forms.Button();
            progressBarDown = new Scripting.Forms.Button();
            progressBar1 = new Scripting.Forms.ProgressBar();
            trackBar1 = new Scripting.Forms.TrackBar();
            itemButton1 = new Scripting.Forms.ItemButton();
            ///
            /// button1
            /// 
            button1.Location = new Scripting.Math.Vector2(160.0f, 30.0f);
            button1.Size = new Scripting.Math.Vector2(50.0f, 20.0f);
            button1.Name = "button1";
            button1.Text = "Click";
            button1.Parent = this;
            button1.Click += new Scripting.Forms.EventHandler(this.button1_Click);
            ///
            /// textBox1
            /// 
            textBox1.Location = new Scripting.Math.Vector2(50.0f, 30.0f);
            textBox1.Size = new Scripting.Math.Vector2(100.0f, 20.0f);
            textBox1.Name = "textBox1";
            textBox1.Parent = this;
            ///
            /// checkBox1
            /// 
            checkBox1.Location = new Scripting.Math.Vector2(220.0f, 30.0f);
            checkBox1.Size = new Scripting.Math.Vector2(120.0f, 20.0f);
            checkBox1.Name = "checkBox1";
            checkBox1.Text = "I'm a Checkbox!";
            checkBox1.Parent = this;
            checkBox1.CheckedChange += new Scripting.Forms.EventHandler(this.checkBox1_CheckedChange);
            ///
            /// comboBox1
            /// 
            comboBox1.Location = new Scripting.Math.Vector2(50.0f, 60.0f);
            comboBox1.Size = new Scripting.Math.Vector2(120.0f, 200.0f);
            comboBox1.Name = "comboBox1";
            comboBox1.Parent = this;
            comboBox1.Items.AddRange(new string[] { "Item1", "Item2", "Item3" });
            comboBox1.SelectedIndex = 0;
            comboBox1.SelectedIndexChanged += new Scripting.Forms.EventHandler(this.comboBox1_SelectedIndexChanged);
            ///
            /// label1
            /// 
            label1.Location = new Scripting.Math.Vector2(50.0f, 10.0f);
            label1.Size = new Scripting.Math.Vector2(120.0f, 20.0f);
            label1.Name = "label1";
            label1.Text = "\\c=#ff0000This is a test label";
            label1.Parent = this;
            ///
            /// listBox1
            /// 
            listBox1.Location = new Scripting.Math.Vector2(200.0f, 60.0f);
            listBox1.Size = new Scripting.Math.Vector2(120.0f, 200.0f);
            listBox1.Name = "listBox1";
            listBox1.Parent = this;
            listBox1.Items.AddRange(new string[] { "List1", "List2", "List3", "List4" });
            listBox1.SelectedIndex = 0;
            listBox1.SelectedIndexChanged += new Scripting.Forms.EventHandler(this.listBox1_SelectedIndexChanged);
            ///
            /// panel1
            /// 
            panel1.Location = new Scripting.Math.Vector2(360.0f, 10.0f);
            panel1.Size = new Scripting.Math.Vector2(93.0f, 55.0f);
            panel1.Name = "panel1";
            panel1.Image = "C:\\sglogo.bmp";
            panel1.Parent = this;
            ///
            /// progressBarUp
            /// 
            progressBarUp.Location = new Scripting.Math.Vector2(490.0f, 130.0f);
            progressBarUp.Size = new Scripting.Math.Vector2(20.0f, 20.0f);
            progressBarUp.Name = "progressBarUp";
            progressBarUp.Text = ">";
            progressBarUp.Parent = this;
            progressBarUp.Click += new Scripting.Forms.EventHandler(this.progressBarUp_Click);
            ///
            /// progressBarDown
            /// 
            progressBarDown.Location = new Scripting.Math.Vector2(360.0f, 130.0f);
            progressBarDown.Size = new Scripting.Math.Vector2(20.0f, 20.0f);
            progressBarDown.Name = "progressBarDown";
            progressBarDown.Text = "<";
            progressBarDown.Parent = this;
            progressBarDown.Click += new Scripting.Forms.EventHandler(this.progressBarDown_Click);
            ///
            /// progressBar1
            /// 
            progressBar1.Location = new Scripting.Math.Vector2(385.0f, 130.0f);
            progressBar1.Size = new Scripting.Math.Vector2(100.0f, 20.0f);
            progressBar1.Name = "progressBar1";
            progressBar1.Parent = this;
            ///
            /// trackBar1
            /// 
            trackBar1.Location = new Scripting.Math.Vector2(385.0f, 160.0f);
            trackBar1.Size = new Scripting.Math.Vector2(100.0f, 40.0f);
            trackBar1.Name = "trackBar1";
            trackBar1.Parent = this;
            trackBar1.Minimum = 0;
            trackBar1.Maximum = 100;
            trackBar1.TickFrequency = 10;
            trackBar1.Value = 50;
            trackBar1.ValueChanged += new Scripting.Forms.EventHandler(trackBar1_ValueChanged);
            ///
            /// itemButton1
            /// 
            itemButton1.Location = new Scripting.Math.Vector2(12.0f, 200.0f);
            itemButton1.Size = new Scripting.Math.Vector2(64, 64);
            itemButton1.ItemID = 0;
            itemButton1.ItemAmount = 10;
            itemButton1.Timer = 50;
            itemButton1.CanDragFromInventory = true;
            itemButton1.CanDragFromSpells = true;
            itemButton1.CanDragToInventory = true;
            itemButton1.Click += new Scripting.Forms.EventHandler(itemButton1_Click);
            itemButton1.RightClick += new Scripting.Forms.EventHandler(itemButton1_RightClick);
            itemButton1.DraggedFromInventory += new Scripting.Forms.EventHandler(itemButton1_DraggedFromInventory);
            itemButton1.DraggedFromSpells += new Scripting.Forms.EventHandler(itemButton1_DraggedFromSpells);
            itemButton1.DraggedToInventory += new Scripting.Forms.EventHandler(itemButton1_DraggedToInventory);
            ///
            /// MyTestForm
            /// 
            this.Closed += new Scripting.Forms.EventHandler(MyTestForm_Closed);
            this.Location = new Scripting.Math.Vector2(0.5f, 0.5f);
            this.PositionType = Scripting.Forms.PositionType.Centered;
            this.Size = new Scripting.Math.Vector2(640, 480);
            this.Controls.Add(button1);
            this.Controls.Add(textBox1);
            this.Controls.Add(checkBox1);
            this.Controls.Add(comboBox1);
            this.Controls.Add(label1);
            this.Controls.Add(listBox1);
            this.Controls.Add(panel1);
            this.Controls.Add(progressBarUp);
            this.Controls.Add(progressBarDown);
            this.Controls.Add(progressBar1);
            this.Controls.Add(trackBar1);
            this.controls.Add(itemButton1);
        }

        Scripting.Forms.Button button1;
        Scripting.Forms.TextBox textBox1;
        Scripting.Forms.CheckBox checkBox1;
        Scripting.Forms.ComboBox comboBox1;
        Scripting.Forms.Label label1;
        Scripting.Forms.ListBox listBox1;
        Scripting.Forms.Panel panel1;
        Scripting.Forms.Button progressBarUp;
        Scripting.Forms.Button progressBarDown;
        Scripting.Forms.ProgressBar progressBar1;
        Scripting.Forms.TrackBar trackBar1;
        Scripting.Forms.ItemButton itemButton1;
    }
}
