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
    public partial class ProjectilesEditor : Form
    {
        // Members
        public int Selected = 10001;
        public GE GEInstance;
        private bool SuppressSelection = false;
        private bool DeliberateClose = false;

        // Constructor
        public ProjectilesEditor()
        {
            InitializeComponent();
        }

        // Form created
        private void ProjectilesEditor_Load(object sender, EventArgs e)
        {
            // List emitters
            Emitter1Combo.Items.Add("(None)");
            Emitter2Combo.Items.Add("(None)");
            string File;
            string[] Emitters = System.IO.Directory.GetFiles(@"Data\Emitter Configs\");
            foreach (string S in Emitters)
            {
                File = System.IO.Path.GetFileNameWithoutExtension(S);
                Emitter1Combo.Items.Add(File);
                Emitter2Combo.Items.Add(File);
            }

            // Fill list
            ProjectilesList.BeginUpdate();
            Projectile P = Projectile.FirstProjectile;
            while (P != null)
            {
                ProjectilesList.Items.Add(new ListBoxItem(P.Name, (uint)P.ID));
                P = P.NextProjectile;
            }
            ProjectilesList.EndUpdate();

            if (ProjectilesList.Items.Count > 0)
            {
                ProjectilesList.SelectedIndex = 0;
            }
        }

        // Projectile selected
        private void ProjectilesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!SuppressSelection && ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile P = Projectile.Index[LBI.Value];

                NameText.Text = P.Name;
                MeshButton.Text = GE.NiceMeshName(P.MeshID);
                int Idx = Emitter1Combo.Items.IndexOf(P.Emitter1);
                if (Idx >= 0)
                {
                    Emitter1Combo.SelectedIndex = Idx;
                }
                else
                {
                    Emitter1Combo.SelectedIndex = 0;
                }
                Idx = Emitter2Combo.Items.IndexOf(P.Emitter2);
                if (Idx >= 0)
                {
                    Emitter2Combo.SelectedIndex = Idx;
                }
                else
                {
                    Emitter2Combo.SelectedIndex = 0;
                }
                Emitter1TextureButton.Text = GE.NiceTextureName(P.Emitter1TexID);
                Emitter2TextureButton.Text = GE.NiceTextureName(P.Emitter2TexID);
                HomingCheck.Checked = P.Homing;
                ChanceSpinner.Value = P.HitChance;
                SpeedSpinner.Value = P.Speed;
                DamageSpinner.Value = P.Damage;
                DamageTypeButton.Text = Item.DamageTypes[P.DamageType];
            }
        }

        // Projectile settings changed
        private void NameText_TextChanged(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].Name = NameText.Text;

                // Update the list
                SuppressSelection = true;
                int Idx = ProjectilesList.SelectedIndex;
                LBI = new ListBoxItem(Projectile.Index[LBI.Value].Name, LBI.Value);
                ProjectilesList.Items.RemoveAt(Idx);
                Idx = ProjectilesList.Items.Add(LBI);
                ProjectilesList.SelectedIndex = Idx;
                SuppressSelection = false;
            }
        }

        private void MeshButton_Click(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].MeshID = MediaDialogs.GetMesh(true, "", Projectile.Index[LBI.Value].MeshID);
                MeshButton.Text = GE.NiceMeshName(Projectile.Index[LBI.Value].MeshID);
            }
        }

        private void Emitter1Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                if (Emitter1Combo.SelectedIndex >= 0)
                {
                    Projectile.Index[LBI.Value].Emitter1 = Emitter1Combo.SelectedItem.ToString();
                }
                else
                {
                    Projectile.Index[LBI.Value].Emitter1 = "";
                }
            }
        }

        private void Emitter2Combo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                if (Emitter2Combo.SelectedIndex >= 0)
                {
                    Projectile.Index[LBI.Value].Emitter2 = Emitter2Combo.SelectedItem.ToString();
                }
                else
                {
                    Projectile.Index[LBI.Value].Emitter2 = "";
                }
            }
        }

        private void Emitter1TextureButton_Click(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].Emitter1TexID = MediaDialogs.GetTexture(true, "",
                                                                                    Projectile.Index[LBI.Value].
                                                                                        Emitter1TexID);
                Emitter1TextureButton.Text = GE.NiceTextureName(Projectile.Index[LBI.Value].Emitter1TexID);
            }
        }

        private void Emitter2TextureButton_Click(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].Emitter2TexID = MediaDialogs.GetTexture(true, "",
                                                                                    Projectile.Index[LBI.Value].
                                                                                        Emitter2TexID);
                Emitter2TextureButton.Text = GE.NiceTextureName(Projectile.Index[LBI.Value].Emitter2TexID);
            }
        }

        private void HomingCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].Homing = HomingCheck.Checked;
            }
        }

        private void SpeedSpinner_ValueChanged(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].Speed = (byte) SpeedSpinner.Value;
            }
        }

        private void ChanceSpinner_ValueChanged(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].HitChance = (byte) ChanceSpinner.Value;
            }
        }

        private void DamageSpinner_ValueChanged(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Projectile.Index[LBI.Value].Damage = (ushort) DamageSpinner.Value;
            }
        }

        private void DamageTypeButton_Click(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                int Result = GEInstance.GetDamageType(Projectile.Index[LBI.Value].DamageType);
                Projectile.Index[LBI.Value].DamageType = (ushort) Result;
                DamageTypeButton.Text = Item.DamageTypes[Projectile.Index[LBI.Value].DamageType];
            }
        }

        // New
        private void New_Click(object sender, EventArgs e)
        {
            try
            {
                Projectile P = new Projectile();
                ProjectilesList.SelectedIndex = ProjectilesList.Items.Add(new ListBoxItem(P.Name, (uint)P.ID));
            }
            catch (ProjectileException)
            {
                MessageBox.Show("Limit of 10000 projectiles already reached!", "Error");
            }
        }

        // Duplicate
        private void Duplicate_Click(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;

                try
                {
                    Projectile P = new Projectile();
                    P.Name = Projectile.Index[LBI.Value].Name + " (copy)";
                    P.MeshID = Projectile.Index[LBI.Value].MeshID;
                    P.Emitter1 = Projectile.Index[LBI.Value].Emitter1;
                    P.Emitter2 = Projectile.Index[LBI.Value].Emitter2;
                    P.Emitter1TexID = Projectile.Index[LBI.Value].Emitter1TexID;
                    P.Emitter2TexID = Projectile.Index[LBI.Value].Emitter2TexID;
                    P.Homing = Projectile.Index[LBI.Value].Homing;
                    P.HitChance = Projectile.Index[LBI.Value].HitChance;
                    P.Speed = Projectile.Index[LBI.Value].Speed;
                    P.Damage = Projectile.Index[LBI.Value].Damage;
                    P.DamageType = Projectile.Index[LBI.Value].DamageType;
                    ProjectilesList.SelectedIndex = ProjectilesList.Items.Add(new ListBoxItem(P.Name, (uint)P.ID));
                }
                catch (ProjectileException)
                {
                    MessageBox.Show("Limit of 10000 projectiles already reached!", "Error");
                }
            }
        }

        // Delete
        private void Delete_Click(object sender, EventArgs e)
        {
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;

                // Delete projectile
                Projectile.Index[LBI.Value].Delete();

                // Remove from main list
                SuppressSelection = true;
                ProjectilesList.Items.RemoveAt(ProjectilesList.SelectedIndex);
                SuppressSelection = false;
            }
        }

        // Cancel button
        private void Cancel_Click(object sender, EventArgs e)
        {
            // Reload projectiles from file
            for (int i = 0; i < 10000; ++i)
            {
                if (Projectile.Index[i] != null)
                {
                    Projectile.Index[i].Delete();
                }
            }
            Projectile.Load(@"Data\Server Data\Projectiles.dat");

            // Return cancel
            Selected = 10001;
            DeliberateClose = true;
            Close();
        }

        // Cancel (by X button)
        private void ProjectilesEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && DeliberateClose == false)
            {
                DialogResult Result = MessageBox.Show("Really lose changes to projectiles?", "Realm Crafter",
                                                      MessageBoxButtons.OKCancel);
                if (Result == DialogResult.OK)
                {
                    // Reload projectiles from file
                    for (int i = 0; i < 10000; ++i)
                    {
                        if (Projectile.Index[i] != null)
                        {
                            Projectile.Index[i].Delete();
                        }
                    }
                    Projectile.Load(@"Data\Server Data\Projectiles.dat");

                    // Return cancel
                    Selected = 10001;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        // Save button (with select)
        private void Save_Click(object sender, EventArgs e)
        {
            // Save projectiles to file
            Projectile.Save(@"Data\Server Data\Projectiles.dat");

            // Return selected
            if (ProjectilesList.SelectedIndex >= 0)
            {
                ListBoxItem LBI = (ListBoxItem) ProjectilesList.SelectedItem;
                Selected = (int)LBI.Value;
            }
            else
            {
                Selected = 10000;
            }

            DeliberateClose = true;
            Close();
        }

        // Save (without select)
        private void SaveNoSelect_Click(object sender, EventArgs e)
        {
            // Save projectiles to file
            Projectile.Save(@"Data\Server Data\Projectiles.dat");

            // Return no selection
            Selected = 10000;
            DeliberateClose = true;
            Close();
        }
    }
}