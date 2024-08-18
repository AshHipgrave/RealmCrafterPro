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
    public partial class GubbinSetEditor : Form
    {
        List<List<GubbinTemplate>> InternalList = new List<List<GubbinTemplate>>();
        List<List<GubbinTemplate>> UserList = new List<List<GubbinTemplate>>();

        public GubbinSetEditor()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            UserList.Clear();
            UserList.AddRange(InternalList.ToArray());

            DialogResult = DialogResult.OK;
            Close();
        }

        public List<List<GubbinTemplate>> GubbinSets
        {
            set
            {
                UserList = value;
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            List<GubbinTemplate> Li = new List<GubbinTemplate>();
            SetsList.Items.Add(SetToString(Li));
            InternalList.Add(Li);

            SetsList.SelectedIndex = SetsList.Items.Count - 1;
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            if (SetsList.SelectedIndex < 0 || SetsList.SelectedIndex >= InternalList.Count)
                return;

            GubbinSelector Sel = new GubbinSelector();
            Sel.CurrentSaved = InternalList[SetsList.SelectedIndex];

            if (Sel.ShowDialog() == DialogResult.OK)
            {
                int Idx = SetsList.SelectedIndex;

                SetsList.Items.Clear();

                foreach (List<GubbinTemplate> Li in InternalList)
                {
                    SetsList.Items.Add(SetToString(Li));
                }

                SetsList.SelectedIndex = Idx;
            }
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (SetsList.SelectedIndex < 0 || SetsList.SelectedIndex >= InternalList.Count)
                return;

            InternalList.RemoveAt(SetsList.SelectedIndex);
            SetsList.Items.RemoveAt(SetsList.SelectedIndex);

        }

        private void SetsList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void GubbinSetEditor_Load(object sender, EventArgs e)
        {
            InternalList.Clear();
            InternalList.AddRange(UserList.ToArray());
            SetsList.Items.Clear();

            foreach (List<GubbinTemplate> Li in InternalList)
            {
                SetsList.Items.Add(SetToString(Li));
            }
        }

        private string SetToString(List<GubbinTemplate> li)
        {
            if (li.Count == 0)
                return "No Templates";

            string O = "";

            for (int i = 0; i < ((li.Count < 2) ? li.Count : 2); ++i)
            {
                O += "{ " + li[i].Name + "} ";
            }

            return O;
        }
    }
}