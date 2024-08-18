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
using RealmCrafter;

namespace RealmCrafter_GE.Dockable_Forms.GubbinEditor
{
    public partial class NewActorTemplate : Form
    {
        public NewActorTemplate()
        {
            InitializeComponent();

            foreach (object o in Program.GE.ActorsList.Items)
            {
                ActorsCombo.Items.Add(o);
            }

            if(ActorsCombo.Items.Count > 0)
                ActorsCombo.SelectedIndex = 0;
        }

        public Actor Actor
        {
            get
            {
                ListBoxItem LBI = ActorsCombo.SelectedItem as ListBoxItem;
                if (LBI == null)
                    return null;

                return Actor.Index[LBI.Value];
            }
        }

        public byte Gender
        {
            get
            {
                return (byte)(MaleRadio.Checked ? 0 : 1);
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Hide();
        }

        private void ActorsCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBoxItem LBI = ActorsCombo.SelectedItem as ListBoxItem;
            if (LBI == null)
                return;

            Actor Current = Actor.Index[LBI.Value];

            MaleRadio.Enabled = true;
            FemaleRadio.Enabled = true;

            
            if (Current.Genders == 1)
            {
                MaleRadio.Checked = true;
                FemaleRadio.Checked = false;
                FemaleRadio.Enabled = false;
            }
            else if (Current.Genders == 2)
            {
                MaleRadio.Checked = false;
                FemaleRadio.Checked = true;
                MaleRadio.Enabled = false;
            }
            
        }
    }
}