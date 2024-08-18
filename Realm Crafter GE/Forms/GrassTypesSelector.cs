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

namespace RCTTest
{
    public partial class GrassTypesSelector : Form
    {
        protected string selectedType = "";
        public RealmCrafter.RCT.TerrainManager TManager;

        public string SelectedType
        {
            get { return selectedType; }
            set { selectedType = value; }
        }

        public GrassTypesSelector()
        {
            TManager = RealmCrafter_GE.GE.TerrainManager;
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CnlButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void GrassTypesSelector_Shown(object sender, EventArgs e)
        {
            TypesList.Items.Clear();

            RealmCrafter.RCT.GrassType[] Ts = TManager.FetchGrassTypes();
            TypesList.Items.AddRange(Ts);

            TypesList.SelectedItem = TManager.FindGrassType(selectedType);
        }

        private void TypesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TypesList.SelectedItem is RealmCrafter.RCT.GrassType)
                selectedType = (TypesList.SelectedItem as RealmCrafter.RCT.GrassType).Name;
        }
    }
}