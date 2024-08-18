//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################
using System;
using System.Collections.Generic;
using System.Text;

namespace RealmCrafter_GE.Dockable_Forms.PostProcess.ParamTypes
{
    class cParam_Int
    {
        OnParamChangeCallback ParamChangeCallback;

        System.Windows.Forms.GroupBox gb;
        System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        System.Windows.Forms.TextBox textB;
        System.Windows.Forms.TrackBar tb;
        
        string Value;

        // 3, 5, 7, 9
        public cParam_Int(System.Windows.Forms.Control Parent, 
                        string Name, string value, OnParamChangeCallback paramChangeCallback) 
        {
            Value = value;
            ParamChangeCallback = paramChangeCallback;
            
            gb = new System.Windows.Forms.GroupBox();
            tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            textB = new System.Windows.Forms.TextBox();
            tb = new System.Windows.Forms.TrackBar();

            Parent.Controls.Add(this.gb);
            // 
            // gb
            // 
            this.gb.Controls.Add(this.tableLayoutPanel4);
            this.gb.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb.Location = new System.Drawing.Point(3, 18);
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(326, 49);
            this.gb.TabIndex = 1;
            this.gb.TabStop = false;
            this.gb.Text = Name;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 2;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel4.Controls.Add(this.textB, 1, 0);
            this.tableLayoutPanel4.Controls.Add(this.tb, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 18);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(320, 28);
            this.tableLayoutPanel4.TabIndex = 0;
            // 
            // textB
            // 
            this.textB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textB.Location = new System.Drawing.Point(259, 3);
            this.textB.Name = "textB";
            this.textB.ReadOnly = true;
            this.textB.Size = new System.Drawing.Size(58, 22);
            this.textB.TabIndex = 0;
            // 
            // tb
            // 
            this.tb.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tb.Location = new System.Drawing.Point(3, 3);
            this.tb.LargeChange = 2;
            this.tb.Maximum = 9;
            this.tb.Minimum = 3;
            this.tb.SmallChange = 2;
            this.tb.TickFrequency = 2;
            this.tb.Value = 3;
            this.tb.Name = "tb";
            this.tb.Size = new System.Drawing.Size(250, 22);
            this.tb.TabIndex = 1;
            this.tb.Scroll += new System.EventHandler(this.tb_Scroll);

            this.tb.Value = System.Convert.ToInt16(Value);
            textB.Text = Value;
        }

        private void tb_Scroll(object sender, EventArgs e)
        {
            if (tb.Value % 2 == 0) tb.Value += (System.Convert.ToInt16(Value) - tb.Value);
            textB.Text = tb.Value.ToString();
            Value = textB.Text;

            ParamChangeCallback(Value, gb.Text);
        }
    }
}
