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
    public partial class GrassTypesEditor : Form
    {
        public RealmCrafter.RCT.TerrainManager TManager;
        protected List<RealmCrafter.RCT.GrassType> DeleteTypes = new List<RealmCrafter.RCT.GrassType>();

        public GrassTypesEditor()
        {
            TManager = RealmCrafter_GE.GE.TerrainManager;
            InitializeComponent();
        }

        public void Reset()
        {
            TypesList.Items.Clear();
            TypesGrid.SelectedObject = null;

            RealmCrafter.RCT.GrassType[] Ts = TManager.FetchGrassTypes();
            TypesList.Items.AddRange(Ts);
            TypesList.SelectedIndex = TypesList.Items.Count - 1;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            foreach (RealmCrafter.RCT.GrassType T in DeleteTypes)
            {
                TManager.RemoveGrassType(T);
            }

            DeleteTypes.Clear();
            TManager.SaveGrassTypes(@".\Data\Game Data\GrassTypes.xml");
            DialogResult = DialogResult.OK;
            Close();
        }

        private void CnlButton_Click(object sender, EventArgs e)
        {
            DeleteTypes.Clear();
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            RealmCrafter.RCT.GrassType T = TManager.CreateGrassType();
            T.Name = "Rename Me (" + (new Random(Environment.TickCount)).Next() + ")";
            int ID = TypesList.Items.Add(T);
            TypesList.SelectedIndex = ID;
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            int ID = TypesList.SelectedIndex;
            if (ID == -1)
                return;

            RealmCrafter.RCT.GrassType T = TypesList.SelectedItem as RealmCrafter.RCT.GrassType;
            TypesList.Items.Remove(T);
            TypesGrid.SelectedObject = null;
            //Program.GE.TManager.RemoveGrassType(T);
            DeleteTypes.Add(T);
        }

        private void TypesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            RealmCrafter.RCT.GrassType T = TypesList.SelectedItem as RealmCrafter.RCT.GrassType;
            TypesGrid.SelectedObject = new GrassTypePropertyInterface(T, new EventHandler(PropertyUpdate));
        }

        private void PropertyUpdate(object sender, EventArgs e)
        {
            RealmCrafter.RCT.GrassType T = sender as RealmCrafter.RCT.GrassType;
            if (T == null)
                return;

            int Index = TypesList.SelectedIndex;
            if (Index == -1)
                return;

            TypesGrid.SelectedObject = null;
            TypesList.Items[TypesList.SelectedIndex] = T;
            TypesList.SelectedIndex = Index;
        }
    }
}