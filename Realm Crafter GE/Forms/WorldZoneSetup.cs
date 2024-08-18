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
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using RealmCrafter;
using RealmCrafter.ServerZone;
using RenderingServices;
using Environment=RealmCrafter.Environment;

namespace RealmCrafter_GE
{
    public partial class WorldZoneSetup : Form
    {
        public WorldZoneSetup()
        {
            InitializeComponent();
        }

        private void ZoneSetup_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void ZoneSetup_Load(object sender, EventArgs e)
        {
            UpdateZoneSetup(true);
        }

        public void UpdateZoneSetup(bool Scripts)
        {
            //Scripts
            if (Scripts)
            {
                WorldZoneEntryScriptCombo.BeginUpdate();
                WorldZoneEntryScriptCombo.Items.Clear();

                WorldZoneEntryScriptCombo.Items.Add("(None)");
                foreach (string S in GE.ScriptsList)
                {
                    WorldZoneEntryScriptCombo.Items.Add(S);
                }
                WorldZoneEntryScriptCombo.EndUpdate();
            }

            #region Initialise zone setup gadgets
            if (Program.GE.CurrentClientZone != null)
            {
                string EnvironmentName = Program.GE.CurrentClientZone.EnvironmentName;
                if (WorldZoneEnvironmentList.Items.Count > 0)
                    WorldZoneEnvironmentList.SelectedIndex = -1;

                for (int i = 0; i < WorldZoneEnvironmentList.Items.Count; ++i)
                {
                    if (WorldZoneEnvironmentList.Items[i].ToString().Equals(EnvironmentName))
                    {
                        WorldZoneEnvironmentList.SelectedIndex = i;
                        break;
                    }
                }


                WorldZoneOutdoorsCheck.Checked = Program.GE.CurrentClientZone.Outdoors;
                //WorldZoneStormCloudsButton.Text = GE.NiceTextureName(65535);
                WorldZoneAmbientColourButton.BackColor = Color.FromArgb(Program.GE.CurrentClientZone.AmbientR,
                                                                        Program.GE.CurrentClientZone.AmbientG,
                                                                        Program.GE.CurrentClientZone.AmbientB);
                WorldZoneFogColourButton.BackColor = Color.FromArgb(Program.GE.CurrentClientZone.FogR,
                                                                    Program.GE.CurrentClientZone.FogG,
                                                                    Program.GE.CurrentClientZone.FogB);


                if (Program.GE.CurrentClientZone.FogNear < -100f)
                {
                    Program.GE.CurrentClientZone.FogNear = -100f;
                }
                else if (Program.GE.CurrentClientZone.FogNear > 3000f)
                {
                    Program.GE.CurrentClientZone.FogNear = 3000f;
                }
                if (Program.GE.CurrentClientZone.FogFar < 250f)
                {
                    Program.GE.CurrentClientZone.FogFar = 250f;
                }
                if (Program.GE.CurrentClientZone.FogFar > 3000f)
                {
                    Program.GE.CurrentClientZone.FogFar = 3000f;
                }
                WorldZoneFogNearSlider.Value = (int) Program.GE.CurrentClientZone.FogNear;
                WorldZoneFogFarSlider.Value = (int) Program.GE.CurrentClientZone.FogFar;

                WorldZoneGravitySpinner.Value = Program.GE.CurrentServerZone.Gravity;
                WorldZoneSlopeRestrictSpinner.Value = (decimal) Program.GE.CurrentClientZone.SlopeRestrict;
                WorldZonePvPCheck.Checked = Program.GE.CurrentServerZone.PvP;
                if (string.IsNullOrEmpty(Program.GE.CurrentServerZone.Script))
                {
                    WorldZoneEntryScriptCombo.SelectedIndex = 0;
                }
                else
                {
                    int Idx = WorldZoneEntryScriptCombo.Items.IndexOf(Program.GE.CurrentServerZone.Script);
                    if (Idx > 0)
                    {
                        WorldZoneEntryScriptCombo.SelectedIndex = Idx;
                    }
                }
                WorldZoneLoadImageButton.Text = GE.NiceTextureName(Program.GE.CurrentClientZone.LoadingTexID);
                WorldZoneLoadMusicButton.Text = GE.NiceMusicName(Program.GE.CurrentClientZone.LoadingMusicID);
                //WorldZoneMapButton.Text = NiceTextureName(CurrentClientZone.MapTexID);
                WorldZoneSetupZoneMapMarkerTextureButton.Text =
                    GE.NiceTextureName(Program.GE.CurrentClientZone.MapMarkerTexID);
                WorldZoneSetupZoneMapMapTextureButton.Text = GE.NiceTextureName(Program.GE.CurrentClientZone.MapTexID);

                //WorldScaleAllSpinner.Value = 1;
            }
            #endregion
        }

        public void MinimalUpdateZoneSetup()
        {
            UpdateZoneSetup(false);
        }

        private void WorldZoneOutdoorsCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                if (Program.GE.CurrentClientZone.Outdoors != WorldZoneOutdoorsCheck.Checked)
                {
                    Program.GE.CurrentClientZone.Outdoors = WorldZoneOutdoorsCheck.Checked;
                    Program.GE.SetWorldSavedStatus(false);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneAmbientColourButton_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                DialogResult D = Program.GE.ColourDialog.ShowDialog();
                if (D == DialogResult.OK)
                {
                    Program.GE.CurrentClientZone.AmbientR = Program.GE.ColourDialog.Color.R;
                    Program.GE.CurrentClientZone.AmbientG = Program.GE.ColourDialog.Color.G;
                    Program.GE.CurrentClientZone.AmbientB = Program.GE.ColourDialog.Color.B;
                    WorldZoneAmbientColourButton.BackColor = Program.GE.ColourDialog.Color;
                    Program.GE.SetWorldSavedStatus(false);
                }
                if (Program.GE.CurrentClientZone != null)
                {
                    Render.FogMode(1);
                    Program.GE.CurrentClientZone.SetViewDistance(Program.GE.Camera, Program.GE.CurrentClientZone.FogNear,
                                                                 Program.GE.CurrentClientZone.FogFar);
                    Program.GE.Camera.CameraClsColor(Program.GE.CurrentClientZone.FogR,
                                                     Program.GE.CurrentClientZone.FogG,
                                                     Program.GE.CurrentClientZone.FogB);
                    Render.FogColor(Program.GE.CurrentClientZone.FogR, Program.GE.CurrentClientZone.FogG,
                                    Program.GE.CurrentClientZone.FogB);
                    Render.AmbientLight(Program.GE.CurrentClientZone.AmbientR, Program.GE.CurrentClientZone.AmbientG,
                                        Program.GE.CurrentClientZone.AmbientB);
                    //DefaultLight.Direction(???);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneFogColourButton_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                DialogResult D = Program.GE.ColourDialog.ShowDialog();
                if (D == DialogResult.OK)
                {
                    Program.GE.CurrentClientZone.FogR = Program.GE.ColourDialog.Color.R;
                    Program.GE.CurrentClientZone.FogG = Program.GE.ColourDialog.Color.G;
                    Program.GE.CurrentClientZone.FogB = Program.GE.ColourDialog.Color.B;
                    WorldZoneFogColourButton.BackColor = Program.GE.ColourDialog.Color;
                    Program.GE.SetWorldSavedStatus(false);
                }

                if (Program.GE.CurrentClientZone != null)
                {
                    Render.FogMode(1);
                    Program.GE.CurrentClientZone.SetViewDistance(Program.GE.Camera, Program.GE.CurrentClientZone.FogNear,
                                                                 Program.GE.CurrentClientZone.FogFar);
                    Program.GE.Camera.CameraClsColor(Program.GE.CurrentClientZone.FogR,
                                                     Program.GE.CurrentClientZone.FogG,
                                                     Program.GE.CurrentClientZone.FogB);
                    Render.FogColor(Program.GE.CurrentClientZone.FogR, Program.GE.CurrentClientZone.FogG,
                                    Program.GE.CurrentClientZone.FogB);
                    Render.AmbientLight(Program.GE.CurrentClientZone.AmbientR, Program.GE.CurrentClientZone.AmbientG,
                                        Program.GE.CurrentClientZone.AmbientB);
                    //DefaultLight.Direction(???);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneFogNearSlider_Scroll(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                if (Program.GE.CurrentClientZone.FogNear != WorldZoneFogNearSlider.Value)
                {
                    Program.GE.CurrentClientZone.FogNear = WorldZoneFogNearSlider.Value;
                    Program.GE.SetWorldSavedStatus(false);
                }

                if (Program.GE.CurrentClientZone != null)
                {
                    Render.FogMode(1);
                    Program.GE.CurrentClientZone.SetViewDistance(Program.GE.Camera, Program.GE.CurrentClientZone.FogNear,
                                                                 Program.GE.CurrentClientZone.FogFar);
                    Program.GE.Camera.CameraClsColor(Program.GE.CurrentClientZone.FogR,
                                                     Program.GE.CurrentClientZone.FogG,
                                                     Program.GE.CurrentClientZone.FogB);
                    Render.FogColor(Program.GE.CurrentClientZone.FogR, Program.GE.CurrentClientZone.FogG,
                                    Program.GE.CurrentClientZone.FogB);
                    Render.AmbientLight(Program.GE.CurrentClientZone.AmbientR, Program.GE.CurrentClientZone.AmbientG,
                                        Program.GE.CurrentClientZone.AmbientB);
                    //DefaultLight.Direction(???);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneFogFarSlider_Scroll(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                if (Program.GE.CurrentClientZone.FogFar != WorldZoneFogFarSlider.Value)
                {
                    Program.GE.CurrentClientZone.FogFar = WorldZoneFogFarSlider.Value;
                    Program.GE.SetWorldSavedStatus(false);
                    if (Program.GE.CurrentClientZone.FogFar < Program.GE.CurrentClientZone.FogNear + 10f)
                    {
                        Program.GE.CurrentClientZone.FogNear = Program.GE.CurrentClientZone.FogFar - 10f;
                        WorldZoneFogNearSlider.Value = (int) Program.GE.CurrentClientZone.FogNear;
                    }
                    if (Program.GE.CurrentClientZone != null)
                    {
                        Render.FogMode(1);
                        Program.GE.CurrentClientZone.SetViewDistance(Program.GE.Camera,
                                                                     Program.GE.CurrentClientZone.FogNear,
                                                                     Program.GE.CurrentClientZone.FogFar);
                        Program.GE.Camera.CameraClsColor(Program.GE.CurrentClientZone.FogR,
                                                         Program.GE.CurrentClientZone.FogG,
                                                         Program.GE.CurrentClientZone.FogB);
                        Render.FogColor(Program.GE.CurrentClientZone.FogR, Program.GE.CurrentClientZone.FogG,
                                        Program.GE.CurrentClientZone.FogB);
                        Render.AmbientLight(Program.GE.CurrentClientZone.AmbientR, Program.GE.CurrentClientZone.AmbientG,
                                            Program.GE.CurrentClientZone.AmbientB);
                        //DefaultLight.Direction(???);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void ZoneSetupAccept_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                Program.GE.CurrentClientZone.EnvironmentName = WorldZoneEnvironmentList.SelectedItem.ToString();
                Environment3D.SetCurrentZone(Program.GE.CurrentClientZone, Program.GE.CurrentClientZone.EnvironmentName);
            }

            Hide();

            Program.GE.UpdateRenderingPanel((int)RealmCrafter_GE.GE.GETab.NEWWORLD);
        }

        private void worldZoneSetupZoneMapShowPlayerPositionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            // Do nothing?
        }

        private void WorldZoneSetupZoneMapMarkerTextureButton_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                ushort Result = MediaDialogs.GetTexture(true, "", Program.GE.CurrentClientZone.MapMarkerTexID);

                Program.GE.CurrentClientZone.MapMarkerTexID = Result;
                WorldZoneSetupZoneMapMarkerTextureButton.Text = GE.NiceTextureName(Result);
                Program.GE.SetWorldSavedStatus(false);
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneSetupZoneMapMapTextureButton_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                ushort Result = MediaDialogs.GetTexture(false, "", Program.GE.CurrentClientZone.MapTexID);

                Program.GE.CurrentClientZone.MapTexID = Result;
                //WorldZoneMapButton.Text = NiceTextureName(Result);
                ((Button) sender).Text = GE.NiceTextureName(Result);
                Program.GE.SetWorldSavedStatus(false);
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneGravitySpinner_ValueChanged(object sender, EventArgs e)
        {
            if (Program.GE.CurrentServerZone != null)
            {
                if (Program.GE.CurrentServerZone.Gravity != WorldZoneGravitySpinner.Value)
                {
                    Program.GE.CurrentServerZone.Gravity = (ushort) WorldZoneGravitySpinner.Value;
                    Program.GE.SetWorldSavedStatus(false);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneSlopeRestrictSpinner_ValueChanged(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                if ((decimal) Program.GE.CurrentClientZone.SlopeRestrict != WorldZoneSlopeRestrictSpinner.Value)
                {
                    Program.GE.CurrentClientZone.SlopeRestrict = (float) WorldZoneSlopeRestrictSpinner.Value;
                    Program.GE.SetWorldSavedStatus(false);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZonePvPCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (Program.GE.CurrentServerZone != null)
            {
                if (Program.GE.CurrentServerZone.PvP != WorldZonePvPCheck.Checked)
                {
                    Program.GE.CurrentServerZone.PvP = WorldZonePvPCheck.Checked;
                    Program.GE.SetWorldSavedStatus(false);
                }
            }
        }

        private void WorldZoneEntryScriptCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.GE.CurrentServerZone != null)
            {
                if (WorldZoneEntryScriptCombo.SelectedIndex > 0)
                {
                    if (Program.GE.CurrentServerZone.Script != (string) WorldZoneEntryScriptCombo.SelectedItem)
                    {
                        Program.GE.CurrentServerZone.Script = (string) WorldZoneEntryScriptCombo.SelectedItem;
                        Program.GE.SetWorldSavedStatus(false);
                    }
                }
                else if (!string.IsNullOrEmpty(Program.GE.CurrentServerZone.Script))
                {
                    Program.GE.CurrentServerZone.Script = "";
                    Program.GE.SetWorldSavedStatus(false);
                }
            }
        }

        private void WorldZoneLoadImageButton_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                ushort Result = MediaDialogs.GetTexture(true, "", Program.GE.CurrentClientZone.LoadingTexID);

                Program.GE.CurrentClientZone.LoadingTexID = Result;
                WorldZoneLoadImageButton.Text = GE.NiceTextureName(Result);
                Program.GE.SetWorldSavedStatus(false);
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldZoneLoadMusicButton_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null)
            {
                ushort Result = MediaDialogs.GetMusic(true, "", Program.GE.CurrentClientZone.LoadingMusicID);
                if (Result < 65535)
                {
                    Program.GE.CurrentClientZone.LoadingMusicID = Result;
                    WorldZoneLoadMusicButton.Text = GE.NiceMusicName(Result);
                    Program.GE.SetWorldSavedStatus(false);
                }
                else
                {
                    Program.GE.CurrentClientZone.LoadingMusicID = 65535;
                    WorldZoneLoadMusicButton.Text = "No music set";
                    Program.GE.SetWorldSavedStatus(false);
                }
            }
            else
            {
                MessageBox.Show("Please select a zone first!", "No zone selected", MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
            }
        }

        private void WorldScaleAllPerformButton_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null && Program.GE.CurrentServerZone != null)
            {
                Program.GE.ScaleEntireZone((float) WorldScaleAllSpinner.Value);
            }
        }

        private void WorldZoneSetup_VisibleChanged(object sender, EventArgs e)
        {
        }

        public void WorldZoneSetupTimeTrackbar_Scroll(object sender, EventArgs e)
        {
            decimal Time = WorldZoneSetupTimeTrackbar.Value;
            decimal TimeM = Time % 60;
            decimal TimeH = (Time - TimeM) / 60;

            Environment3D.SetTime(Convert.ToInt32(TimeH), Convert.ToInt32(TimeM));

            String TimeHS = TimeH.ToString();
            String TimeMS = TimeM.ToString();

            if (TimeH < 10) TimeHS = "0" + TimeHS;
            if (TimeM < 10) TimeMS = "0" + TimeMS;

            WorldZoneSetupTimeLabel.Text = "Preview Time: " + TimeHS + ":" + TimeMS;
        }

        private void ZoneSetupPanel_Paint(object sender, PaintEventArgs e)
        {

        }

        private void WorldZoneEnvironmentConfigure_Click(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone != null && WorldZoneEnvironmentList.SelectedIndex > -1)
            {
                string EnvName = WorldZoneEnvironmentList.SelectedItem.ToString();
                if (!EnvName.Equals(Program.GE.CurrentClientZone.EnvironmentName))
                {
                    Environment3D.SetCurrentZone(Program.GE.CurrentClientZone, EnvName);
                    Program.GE.CurrentClientZone.EnvironmentName = EnvName;
                }

                foreach (RealmCrafter.SDK.ISDKEnvironmentConfigurator Co in RealmCrafter.SDK.SDKPlugins.EnvironmentConfigurators)
                {
                    if (Co.GetName().Equals(EnvName))
                    {
                        Co.Show(Program.GE.CurrentClientZone.Name);
                        Environment3D.SetCurrentZone(Program.GE.CurrentClientZone, EnvName);
                        return;
                    }
                }
            }
        }
      
    }
}