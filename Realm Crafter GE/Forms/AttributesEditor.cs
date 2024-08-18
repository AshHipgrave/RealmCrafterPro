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

namespace RealmCrafter_GE
{
    public partial class AttributesEditor : Form
    {
        public bool Saved;
        private string[] Names;
        private bool[] IsSkill;
        private bool[] Hidden;
        private ushort HealthStat, EnergyStat, BreathStat, ToughnessStat, StrengthStat, SpeedStat;
        private bool SuppressList;

        // Constructor
        public AttributesEditor()
        {
            Saved = false;
            Names = new string[Attributes.TotalAttributes];
            IsSkill = new bool[Attributes.TotalAttributes];
            Hidden = new bool[Attributes.TotalAttributes];
            SuppressList = false;

            InitializeComponent();
        }

        // Cancel button pressed
        private void AttributesCancel_Click(object sender, EventArgs e)
        {
            // Close window
            Close();
        }

        // Save button pressed
        private void AttributesSave_Click(object sender, EventArgs e)
        {
            // Update attributes
            Attributes.AssignmentPoints = (int) AttributeAssignSpinner.Value;
            for (int i = 0; i < Attributes.TotalAttributes; ++i)
            {
                Attributes.Names[i] = Names[i];
                Attributes.IsSkill[i] = IsSkill[i];
                Attributes.Hidden[i] = Hidden[i];
            }

            // Update fixed attributes
            Attributes.HealthStat = HealthStat;
            Attributes.EnergyStat = EnergyStat;
            Attributes.BreathStat = BreathStat;
            Attributes.ToughnessStat = ToughnessStat;
            Attributes.StrengthStat = StrengthStat;
            Attributes.SpeedStat = SpeedStat;

            Attributes.Save(@"Data\Server Data\Attributes.dat");
            Attributes.SaveFixed();

            // Close window
            Saved = true;
            Close();
        }

        // Form shown
        private void AttributesEditor_Load(object sender, EventArgs e)
        {
            // Load fixed attributes from file
            Attributes.LoadFixed();

            HealthStat = Attributes.HealthStat;
            EnergyStat = Attributes.EnergyStat;
            BreathStat = Attributes.BreathStat;
            ToughnessStat = Attributes.ToughnessStat;
            StrengthStat = Attributes.StrengthStat;
            SpeedStat = Attributes.SpeedStat;

            // Fill lists of attributes
            for (int i = 0; i < Attributes.TotalAttributes; ++i)
            {
                Names[i] = Attributes.Names[i];
                IsSkill[i] = Attributes.IsSkill[i];
                Hidden[i] = Attributes.Hidden[i];

                if (Names[i] != "")
                {
                    ListBoxItem LBI = new ListBoxItem(Names[i], (uint)i);
                    AttributesList.Items.Add(LBI);
                }
            }
            UpdateFixedAttributeLists();

            // Set assignment points
            AttributeAssignSpinner.Value = Attributes.AssignmentPoints;

            // Select first attribute
            if(AttributesList.Items.Count > 0)
                AttributesList.SelectedIndex = 0;
        }

        // Resets the fixed attribute combo boxes
        private void UpdateFixedAttributeLists()
        {
            SuppressList = true;

            // Clear current lists
            AttributeFixedHealthCombo.Items.Clear();
            AttributeFixedEnergyCombo.Items.Clear();
            AttributeFixedBreathCombo.Items.Clear();
            AttributeFixedToughnessCombo.Items.Clear();
            AttributeFixedStrengthCombo.Items.Clear();
            AttributeFixedSpeedCombo.Items.Clear();

            // None options
            AttributeFixedEnergyCombo.Items.Add("(None)");
            AttributeFixedEnergyCombo.SelectedIndex = 0;
            AttributeFixedBreathCombo.Items.Add("(None)");
            AttributeFixedBreathCombo.SelectedIndex = 0;

            // Add new attributes
            for (int i = 0; i < AttributesList.Items.Count; i++)
            {
                // Add attribute
                ListBoxItem LBI = (ListBoxItem) AttributesList.Items[i];
                AttributeFixedHealthCombo.Items.Add(LBI);
                AttributeFixedEnergyCombo.Items.Add(LBI);
                AttributeFixedBreathCombo.Items.Add(LBI);
                AttributeFixedToughnessCombo.Items.Add(LBI);
                AttributeFixedStrengthCombo.Items.Add(LBI);
                AttributeFixedSpeedCombo.Items.Add(LBI);

                // Select if necessary
                if (LBI.Value == HealthStat)
                {
                    AttributeFixedHealthCombo.SelectedIndex = i;
                }
                if (LBI.Value == EnergyStat)
                {
                    AttributeFixedEnergyCombo.SelectedIndex = i + 1;
                }
                if (LBI.Value == BreathStat)
                {
                    AttributeFixedBreathCombo.SelectedIndex = i + 1;
                }
                if (LBI.Value == ToughnessStat)
                {
                    AttributeFixedToughnessCombo.SelectedIndex = i;
                }
                if (LBI.Value == StrengthStat)
                {
                    AttributeFixedStrengthCombo.SelectedIndex = i;
                }
                if (LBI.Value == SpeedStat)
                {
                    AttributeFixedSpeedCombo.SelectedIndex = i;
                }
            }

            SuppressList = false;
        }

        // Gets the current fixed attribute settings
        private void GetFixedAttributes()
        {
            ListBoxItem LBI;
            if (AttributeFixedHealthCombo.SelectedIndex >= 0)
            {
                LBI = (ListBoxItem) AttributeFixedHealthCombo.Items[AttributeFixedHealthCombo.SelectedIndex];
                HealthStat = (ushort) LBI.Value;
            }
            if (AttributeFixedToughnessCombo.SelectedIndex >= 0)
            {
                LBI = (ListBoxItem) AttributeFixedToughnessCombo.Items[AttributeFixedToughnessCombo.SelectedIndex];
                ToughnessStat = (ushort) LBI.Value;
            }
            if (AttributeFixedStrengthCombo.SelectedIndex >= 0)
            {
                LBI = (ListBoxItem) AttributeFixedStrengthCombo.Items[AttributeFixedStrengthCombo.SelectedIndex];
                StrengthStat = (ushort) LBI.Value;
            }
            if (AttributeFixedSpeedCombo.SelectedIndex >= 0)
            {
                LBI = (ListBoxItem) AttributeFixedSpeedCombo.Items[AttributeFixedSpeedCombo.SelectedIndex];
                SpeedStat = (ushort) LBI.Value;
            }
            if (AttributeFixedEnergyCombo.SelectedIndex == 0)
            {
                EnergyStat = 65535;
            }
            else if (AttributeFixedEnergyCombo.SelectedIndex > 0)
            {
                LBI = (ListBoxItem) AttributeFixedEnergyCombo.Items[AttributeFixedEnergyCombo.SelectedIndex];
                EnergyStat = (ushort) (LBI.Value);
            }
            if (AttributeFixedBreathCombo.SelectedIndex == 0)
            {
                BreathStat = 65535;
            }
            else if (AttributeFixedBreathCombo.SelectedIndex > 0)
            {
                LBI = (ListBoxItem) AttributeFixedBreathCombo.Items[AttributeFixedBreathCombo.SelectedIndex];
                BreathStat = (ushort) (LBI.Value);
            }
        }

        // Fixed attributes changed
        private void AttributeFixedHealthCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressList)
            {
                GetFixedAttributes();
            }
        }

        private void AttributeFixedEnergyCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressList)
            {
                GetFixedAttributes();
            }
        }

        private void AttributeFixedBreathCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressList)
            {
                GetFixedAttributes();
            }
        }

        private void AttributeFixedToughnessCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressList)
            {
                GetFixedAttributes();
            }
        }

        private void AttributeFixedStrengthCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressList)
            {
                GetFixedAttributes();
            }
        }

        private void AttributeFixedSpeedCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressList)
            {
                GetFixedAttributes();
            }
        }

        // Selected attribute changed
        private void AttributesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressList && AttributesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) AttributesList.Items[AttributesList.SelectedIndex];
                AttributeNameText.Text = Names[LBI.Value];
                AttributeSkillCheck.Checked = IsSkill[LBI.Value];
                AttributeHideCheck.Checked = Hidden[LBI.Value];
            }
        }

        // Attribute settings changed
        private void AttributeNameText_TextChanged(object sender, EventArgs e)
        {
            if (AttributesList.SelectedIndex >= 0)
            {
                // Update name
                ListBoxItem LBI = (ListBoxItem) AttributesList.Items[AttributesList.SelectedIndex];
                Names[LBI.Value] = AttributeNameText.Text;

                // Update lists
                SuppressList = true;
                int Idx = AttributesList.SelectedIndex;
                LBI = new ListBoxItem(AttributeNameText.Text, LBI.Value);
                AttributesList.Items.RemoveAt(Idx);
                AttributesList.Items.Insert(Idx, LBI);
                AttributesList.SelectedIndex = Idx;
                SuppressList = false;
                UpdateFixedAttributeLists();
            }
        }

        private void AttributeSkillCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (AttributesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) AttributesList.Items[AttributesList.SelectedIndex];
                IsSkill[LBI.Value] = AttributeSkillCheck.Checked;
            }
        }

        private void AttributeHideCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (AttributesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) AttributesList.Items[AttributesList.SelectedIndex];
                Hidden[LBI.Value] = AttributeHideCheck.Checked;
            }
        }

        // Add attribute
        private void AttributeAdd_Click(object sender, EventArgs e)
        {
            // Find free slot
            bool Found = false;
            for (int i = 0; i < Attributes.TotalAttributes; ++i)
            {
                if (Names[i] == "")
                {
                    Names[i] = "New attribute";
                    ListBoxItem LBI = new ListBoxItem(Names[i], (uint)i);
                    int Idx = AttributesList.Items.Add(LBI);
                    UpdateFixedAttributeLists();
                    AttributesList.SelectedIndex = Idx;
                    Found = true;
                    break;
                }
            }

            // No free slot found
            if (!Found)
            {
                MessageBox.Show("Maximum number of attributes already created!", "Error");
            }
        }

        // Remove attribute
        private void AttributeRemove_Click(object sender, EventArgs e)
        {
            if (AttributesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) AttributesList.Items[AttributesList.SelectedIndex];
                Names[LBI.Value] = "";
                IsSkill[LBI.Value] = false;
                Hidden[LBI.Value] = false;
                AttributesList.Items.RemoveAt(AttributesList.SelectedIndex);
                UpdateFixedAttributeLists();
                AttributeNameText.Text = "";
            }
        }
    }
}