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

namespace RealmCrafter_GE
{
    public partial class ShaderProfileEdit : Form
    {
        private ShaderManagerEditMode _EditMode = ShaderManagerEditMode.Add;
        private ShaderProfile Profile = null;
        private DialogResult DR = DialogResult.Cancel;

        public ShaderProfileEdit()
        {
            InitializeComponent();
        }

        public ShaderManagerEditMode EditMode
        {
            get { return _EditMode; }
            set { _EditMode = value; }
        }

        public ShaderProfile SelectedProfile
        {
            get { return Profile; }
            set { Profile = value; }
        }

        public void UpdateProfile()
        {
            if (Profile == null)
            {
                return;
            }

            HNCombo.Items.Clear();
            HFCombo.Items.Clear();
            MNCombo.Items.Clear();
            MFCombo.Items.Clear();
            LNCombo.Items.Clear();
            LFCombo.Items.Clear();

            int HNIndex = -1;
            int HFIndex = -1;
            int MNIndex = -1;
            int MFIndex = -1;
            int LNIndex = -1;
            int LFIndex = -1;
            int Index = 0;

            foreach (LoadedEffect Le in ShaderManager.LoadedShaders)
            {
                if (Le != null)
                {
                    if (Le.Effect == Profile.HighNear)
                    {
                        HNIndex = Index;
                    }
                    if (Le.Effect == Profile.HighFar)
                    {
                        HFIndex = Index;
                    }
                    if (Le.Effect == Profile.MediumNear)
                    {
                        MNIndex = Index;
                    }
                    if (Le.Effect == Profile.MediumFar)
                    {
                        MFIndex = Index;
                    }
                    if (Le.Effect == Profile.LowNear)
                    {
                        LNIndex = Index;
                    }
                    if (Le.Effect == Profile.LowFar)
                    {
                        LFIndex = Index;
                    }

                    HNCombo.Items.Add(Le.Name);
                    HFCombo.Items.Add(Le.Name);
                    MNCombo.Items.Add(Le.Name);
                    MFCombo.Items.Add(Le.Name);
                    LNCombo.Items.Add(Le.Name);
                    LFCombo.Items.Add(Le.Name);
                    ++Index;
                }
            }

            HNCombo.SelectedIndex = HNIndex;
            HFCombo.SelectedIndex = HFIndex;
            MNCombo.SelectedIndex = MNIndex;
            MFCombo.SelectedIndex = MFIndex;
            LNCombo.SelectedIndex = LNIndex;
            LFCombo.SelectedIndex = LFIndex;

            RangeBox.Value = Convert.ToDecimal((Profile.Range * 100.0f));
            NameBox.Text = Profile.Name;

            if (Profile == ShaderManager.DefaultStatic)
            {
                DefaultStaticBox.Checked = true;
            }
            else
            {
                DefaultStaticBox.Checked = false;
            }

            if (Profile == ShaderManager.DefaultAnimated)
            {
                DefaultAnimatedBox.Checked = true;
            }
            else
            {
                DefaultAnimatedBox.Checked = false;
            }
        }

        public DialogResult DResult
        {
            get { return DR; }
            set { DR = value; }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            if (_EditMode == ShaderManagerEditMode.Add)
            {
                bool Found = false;

                foreach (ShaderProfile Pr in ShaderManager.LoadedProfiles)
                {
                    if (Pr != null && Pr.Name.ToLower() == Profile.Name.ToLower())
                    {
                        Found = true;
                        break;
                    }
                }

                if (Found)
                {
                    MessageBox.Show("Error: Profile with name '" + Profile.Name + "' already exists!");
                    return;
                }
            }

            // Save defaults
            if (DefaultAnimatedBox.Checked)
            {
                ShaderManager.DefaultAnimated = Profile;
            }

            if (DefaultStaticBox.Checked)
            {
                ShaderManager.DefaultStatic = Profile;
            }

            DR = DialogResult.Yes;
            this.Hide();
        }

        private void DCancelButton_Click(object sender, EventArgs e)
        {
            DR = DialogResult.Cancel;
            this.Hide();
        }

        private void NameBox_TextChanged(object sender, EventArgs e)
        {
            if (Profile != null)
            {
                Profile.Name = NameBox.Text;
            }
        }

        private void RangeBox_ValueChanged(object sender, EventArgs e)
        {
            if (Profile != null)
            {
                Profile.Range = ((float) RangeBox.Value) / 100.0f;
            }
        }

        private void HNCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Profile == null)
            {
                return;
            }

            foreach (LoadedEffect Le in ShaderManager.LoadedShaders)
            {
                if (Le != null)
                {
                    if (Le.Name == HNCombo.SelectedItem as String)
                    {
                        Profile.HighNear = ShaderManager.GetShader(Le.ID);
                    }
                }
            }
        }

        private void HFCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Profile == null)
            {
                return;
            }

            foreach (LoadedEffect Le in ShaderManager.LoadedShaders)
            {
                if (Le != null)
                {
                    if (Le.Name == HFCombo.SelectedItem as String)
                    {
                        Profile.HighFar = ShaderManager.GetShader(Le.ID);
                    }
                }
            }
        }

        private void MNCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Profile == null)
            {
                return;
            }

            foreach (LoadedEffect Le in ShaderManager.LoadedShaders)
            {
                if (Le != null)
                {
                    if (Le.Name == MNCombo.SelectedItem as String)
                    {
                        Profile.MediumNear = ShaderManager.GetShader(Le.ID);
                    }
                }
            }
        }

        private void MFCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Profile == null)
            {
                return;
            }

            foreach (LoadedEffect Le in ShaderManager.LoadedShaders)
            {
                if (Le != null)
                {
                    if (Le.Name == MFCombo.SelectedItem as String)
                    {
                        Profile.MediumFar = ShaderManager.GetShader(Le.ID);
                    }
                }
            }
        }

        private void LNCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Profile == null)
            {
                return;
            }

            foreach (LoadedEffect Le in ShaderManager.LoadedShaders)
            {
                if (Le != null)
                {
                    if (Le.Name == LNCombo.SelectedItem as String)
                    {
                        Profile.LowNear = ShaderManager.GetShader(Le.ID);
                    }
                }
            }
        }

        private void LFCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Profile == null)
            {
                return;
            }

            foreach (LoadedEffect Le in ShaderManager.LoadedShaders)
            {
                if (Le != null)
                {
                    if (Le.Name == LFCombo.SelectedItem as String)
                    {
                        Profile.LowFar = ShaderManager.GetShader(Le.ID);
                    }
                }
            }
        }

        private void DefaultStaticBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DefaultStaticBox.Checked && DefaultAnimatedBox.Checked)
            {
                DefaultAnimatedBox.Checked = false;
            }
        }

        private void DefaultAnimatedBox_CheckedChanged(object sender, EventArgs e)
        {
            if (DefaultStaticBox.Checked && DefaultAnimatedBox.Checked)
            {
                DefaultStaticBox.Checked = false;
            }
        }
    }
}