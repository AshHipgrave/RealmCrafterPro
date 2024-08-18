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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Design;

namespace RealmCrafter
{
    public partial class LightFunctionDialog : Form
    {
        LightFunction Function;
        int TimeH = 0, TimeM = 0, TimeS = 0;

        public LightFunctionDialog()
        {
            InitializeComponent();
        }

        //void Application_Idle(object sender, EventArgs e)
        //{
        //    ++TimeS;
        //    if (TimeS > 59)
        //    {
        //        TimeS = 0;
        //        ++TimeM;
        //        if (TimeM > 59)
        //        {
        //            TimeM = 0;
        //            ++TimeH;
        //            if (TimeH > 23)
        //                TimeH = 0;
        //        }
        //    }

        //    Text = TimeH.ToString() + ":" + TimeM.ToString() + ":" + TimeS.ToString();

        //    Function.Update(TimeH, TimeM, TimeS, button1);
        //}

        public LightFunction LightFunction
        {
            get { return Function; }
            set
            {
                Function = value;
                if(Function == null)
                    return;

                Function.DeCompile(ref EventsList);
            }
        }

        private void LightFunctionDialog_Load(object sender, EventArgs e)
        {

        }

        private void EventsList_SelectionChanged(object sender, EventArgs e)
        {
            if (EventsList.SelectedRows.Count == 0)
            {
                return;
            }

            EventProperties.SelectedObject = new LightFunctionProperty(EventsList.SelectedRows[0]);
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            EventsList.Rows.Add(new string[] { "0", "False", "", "100" });
            EventsList.Rows[EventsList.Rows.Count - 1].Cells[0].Tag = new LightFunctionTime();
            EventsList.Rows[EventsList.Rows.Count - 1].Cells[1].Tag = "false";
            EventsList.Rows[EventsList.Rows.Count - 1].Cells[2].Tag = new RenderingServices.Vector3(0, 0, 0);
            EventsList.Rows[EventsList.Rows.Count - 1].Cells[3].Tag = "100";

            EventsList.Rows[EventsList.Rows.Count - 1].Cells[2].Style.BackColor = Color.Black;

        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            if (Function != null)
                Function.Compile(EventsList);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CnlButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void RemoveButton_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in EventsList.SelectedRows)
            {
                EventsList.Rows.Remove((row));
            }
        }
    }




}