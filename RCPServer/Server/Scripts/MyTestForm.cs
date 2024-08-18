using System;
using System.Collections.Generic;
using System.Text;
using Scripting.Forms;
using Scripting.Math;
using Scripting;

namespace UserScripts
{
    /// <summary>
    /// My custom form for testing
    /// </summary>
    public partial class MyTestForm : Form
    {
        // A counter variable. Its value needs to remain constant even when zoning!
        int MyCounter = 0;
        Timer T = new Timer(100, true);

        public MyTestForm()
        {
            InitializeComponent();

            T.Tick += new Scripting.Forms.EventHandler(T_Tick);
        }

        void T_Tick(object sender, FormEventArgs e)
        {
            MyCounter += 1;
            progressBar1.Value = MyCounter;

            if (MyCounter >= 100)
            {
                T.Stop();
                this.Text = "CLOSED";
            }
        }

        /// <summary>
        /// Overridden serialize command, so we can store local data
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(PacketWriter writer)
        {
            // Write counter
            writer.Write(MyCounter);

            base.Serialize(writer);
        }

        /// <summary>
        /// Overridden Deserialize command, read-back local data
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="master"></param>
        /// <param name="actor"></param>
        /// <param name="clientControls"></param>
        public override void Deserialize(PacketReader reader, Control master, Actor actor, LinkedList<Control> clientControls)
        {
            // Read counter
            MyCounter = reader.ReadInt32();

            base.Deserialize(reader, master, actor, clientControls);
        }

        void itemButton1_DraggedToInventory(object sender, Scripting.Forms.FormEventArgs e)
        {
            this.Actor.Output("itemButton1_DraggedToInventory");
        }

        void itemButton1_DraggedFromSpells(object sender, Scripting.Forms.FormEventArgs e)
        {
            this.Actor.Output("itemButton1_DraggedFromSpells");
        }

        void itemButton1_DraggedFromInventory(object sender, Scripting.Forms.FormEventArgs e)
        {
            this.Actor.Output("itemButton1_DraggedFromInventory");
        }

        void itemButton1_RightClick(object sender, Scripting.Forms.FormEventArgs e)
        {
            this.Actor.Output("itemButton1_RightClick");
        }

        void itemButton1_Click(object sender, Scripting.Forms.FormEventArgs e)
        {
            this.Actor.Output("itemButton1_Click");
        }

        /// <summary>
        /// User click the 'X' button on the forum
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void MyTestForm_Closed(object sender, FormEventArgs e)
        {
            // Show a console message and close/cleanup the dialog
            RCScript.Log("CLOSED");

            Actor.CloseDialog(this);
        }

        /// <summary>
        /// User clicked button1
        /// </summary>
        void button1_Click(object sender, FormEventArgs e)
        {
            button1.Text = "Hello!";
            T.Start();

            label1.InlineStringProcessing = !label1.InlineStringProcessing;
        }

        /// <summary>
        /// User checked checkBox1
        /// </summary>
        void checkBox1_CheckedChange(object sender, FormEventArgs e)
        {
            this.Text = "checkBox1 was " + (checkBox1.Checked ? "CHECKED!" : "UNCHECKED!");
        }

        /// <summary>
        /// User changed an item in comboBox1
        /// </summary>
        void comboBox1_SelectedIndexChanged(object sender, FormEventArgs e)
        {
            this.Text = "comboBox1 index changed to: " + comboBox1.SelectedIndex + ": " + comboBox1.SelectedValue;
        }

        /// <summary>
        /// User changed an item in listBox1
        /// </summary>
        void listBox1_SelectedIndexChanged(object sender, FormEventArgs e)
        {
            this.Text = "listBox1 index changed to: " + listBox1.SelectedIndex + ": " + listBox1.SelectedValue;
        }

        /// <summary>
        /// Use click the '>' button
        /// </summary>
        void progressBarUp_Click(object sender, FormEventArgs e)
        {
            progressBar1.Value = progressBar1.Value + 10;
            ++MyCounter;
            this.Text = "Counter = " + MyCounter;
        }

        /// <summary>
        /// User clicked the '<' button
        /// </summary>
        void progressBarDown_Click(object sender, FormEventArgs e)
        {
            progressBar1.Value = progressBar1.Value - 10;
            --MyCounter;
            this.Text = "Counter = " + MyCounter;
        }

        /// <summary>
        /// User moved the trackBar1
        /// </summary>
        void trackBar1_ValueChanged(object sender, FormEventArgs e)
        {
            this.Text = "trackBar1 changed to: " + trackBar1.Value;
        }

    }
}
