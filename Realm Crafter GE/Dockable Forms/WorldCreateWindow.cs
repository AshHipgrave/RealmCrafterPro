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
// Temporary copapasta of old one.
namespace RealmCrafter_GE.Dockable_Forms
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Text;
    using System.Windows.Forms;
    using WeifenLuo.WinFormsUI.Docking;

    public partial class WorldCreateWindow : DockContent
    {
        public WorldCreateWindow()
        {
            InitializeComponent();

            string[] Zones = System.IO.Directory.GetFiles(@"Data\Areas\", "*.dat");
            foreach (string S in Zones)
            {
                string Name = System.IO.Path.GetFileNameWithoutExtension(S);
                WorldPlacePortalLinkCombo.Items.Add(Name);
            }

            UpdateScripts();

            string[] Emitters = System.IO.Directory.GetFiles(@"Data\Emitter Configs\");
            string Filename;
            foreach (string S in Emitters)
            {
                Filename = System.IO.Path.GetFileNameWithoutExtension(S);
                WorldPlaceEmitterCombo.Items.Add(Filename);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }


        public void UpdateScripts()
        {
            WorldPlaceTriggerScriptCombo.Items.Clear();
            foreach (string S in GE.ScriptsList)
            {
                WorldPlaceTriggerScriptCombo.Items.Add(S);
            }
        }

        private void WorldCreateWindow_Shown(object sender, EventArgs e)
        {
            WorldPlaceObjectTypeCombo.SelectedIndex = 0;
        }

        private void WorldPlaceObjectTypeCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Hide all
            WorldPlaceSceneryLabel.Visible = false;
            WorldPlaceSceneryButton.Visible = false;
            WorldPlaceAlignCheck.Visible = false;
            WorldPlaceGridCheck.Visible = false;
            WorldPlaceEmitterLabel.Visible = false;
            WorldPlaceEmitterCombo.Visible = false;
            WorldPlaceWaterLabel.Visible = false;
            WorldPlaceWaterButton.Visible = false;
            WorldPlaceTriggerScriptCombo.Visible = false;
            WorldPlaceTriggerScriptLabel.Visible = false;
            WorldPlacePortalLinkCombo.Visible = false;
            WorldPlacePortalLinkLabel.Visible = false;
            WorldPlacePortalLinkNameCombo.Visible = false;
            WorldPlacePortalLinkNameLabel.Visible = false;
            WorldPlacePortalNameLabel.Visible = false;
            WorldPlacePortalNameText.Visible = false;
            WorldPlaceTreeCombo.Visible = false;
            WorldTreePlaceLabel.Visible = false;
            TreeRandomRotationCheckbox.Visible = false;
            TreeHeightVarianceLabel.Visible = false;
            TreeHeightVarianceSpinner.Visible = false;

            // Show as required
            switch (WorldPlaceObjectTypeCombo.SelectedIndex)
            {
                    // Zone placement gadgets
                case 0:
                    break;

                    // Scenery object placement gadgets
                case 1:
                    WorldPlaceSceneryLabel.Visible = true;
                    WorldPlaceSceneryButton.Visible = true;
                    WorldPlaceAlignCheck.Visible = true;
                    WorldPlaceGridCheck.Visible = true;
                    break;

                    // Emitter placement gadgets
                case 3:
                    WorldPlaceEmitterLabel.Visible = true;
                    WorldPlaceEmitterCombo.Visible = true;
                    break;

                    // Water placement gadgets
                case 4:
                    WorldPlaceWaterLabel.Visible = true;
                    WorldPlaceWaterButton.Visible = true;
                    break;

                    // Trigger placement gadgets
                case 8:
                    WorldPlaceTriggerScriptCombo.Visible = true;
                    WorldPlaceTriggerScriptLabel.Visible = true;
                    break;

                    // Portal placement gadgets
                case 10:
                    WorldPlacePortalLinkCombo.Visible = true;
                    WorldPlacePortalLinkLabel.Visible = true;
                    WorldPlacePortalLinkNameCombo.Visible = true;
                    WorldPlacePortalLinkNameLabel.Visible = true;
                    WorldPlacePortalNameLabel.Visible = true;
                    WorldPlacePortalNameText.Visible = true;
                    break;
                    
                    // Tree placement gadgets
                case 12:
                    WorldPlaceTreeCombo.Visible = true;
                    WorldTreePlaceLabel.Visible = true;

                    WorldPlaceTreeCombo.Items.Clear();

                    foreach (TreeNode Node in Program.GETreeManager.TreesList.Nodes)
                    {
                        if (Node.Tag is StoredTree)
                        {
                            StoredTree Store = Node.Tag as StoredTree;

                            if (Store.LTType != null)
                            {
                                WorldPlaceTreeCombo.Items.Add(Store);
                            }
                        }
                    }

                    TreeRandomRotationCheckbox.Visible = true;
                    TreeHeightVarianceLabel.Visible = true;
                    TreeHeightVarianceSpinner.Visible = true;

                    break;
            }

            if (WorldPlaceObjectTypeCombo.SelectedIndex > -1)
            {
                Program.GE.SetWorldButtonSelection = (int) RealmCrafter_GE.GE.WorldButtonSelection.CREATE;
                Program.GE.UncheckObjectControls();
            }

            Program.GE.PlaceObject = WorldPlaceObjectTypeCombo.SelectedIndex;
        }

        private void WorldPlaceTriggerScriptCombo_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void WorldPlaceAlignCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (WorldPlaceGridCheck.Checked == true && WorldPlaceAlignCheck.Checked == true)
            {
                WorldPlaceGridCheck.Checked = false;
            }
        }

        private void WorldPlaceWaterButton_Click(object sender, EventArgs e)
        {
            Program.GE.PlaceWaterID = MediaDialogs.GetTexture(false, Program.GE.PlaceWaterID);
            WorldPlaceWaterButton.Text = GE.NiceTextureName(Program.GE.PlaceWaterID);
        }

        private void WorldPlacePortalLinkCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Update list of portals within selected zone
            WorldPlacePortalLinkNameCombo.Items.Clear();
            WorldPlacePortalLinkNameCombo.Enabled = false;
            if (WorldPlacePortalLinkCombo.SelectedIndex > 0)
            {
                List<string> PortalNames = null;

                // Selected zone is current zone
                if (Program.GE.CurrentServerZone != null &&
                    (string) WorldPlacePortalLinkCombo.SelectedItem == Program.GE.CurrentServerZone.Name)
                {
                    PortalNames = Program.GE.ZonePortalNames(Program.GE.CurrentServerZone);
                }
                else
                {
                    // Or another zone (load it temporarily)
                    PortalNames = RealmCrafter.ServerZone.Zone.LoadPortalNames((string)WorldPlacePortalLinkCombo.SelectedItem);
//                     RealmCrafter.ServerZone.Zone LoadedZone =
//                         RealmCrafter.ServerZone.Zone.Load((string) WorldPlacePortalLinkCombo.SelectedItem, null);
//                     if (LoadedZone != null)
//                     {
//                         PortalNames = Program.GE.ZonePortalNames(LoadedZone);
//                         LoadedZone.Delete();
//                     }
                }

                // Found portal names
                if (PortalNames != null)
                {
                    for (int i = 0; i < PortalNames.Count; ++i)
                    {
                        WorldPlacePortalLinkNameCombo.Items.Add(PortalNames[i]);
                    }

                    WorldPlacePortalLinkNameCombo.Enabled = true;
                    if (WorldPlacePortalLinkNameCombo.Items.Count > 0)
                    {
                        WorldPlacePortalLinkNameCombo.SelectedIndex = 0;
                    }
                }
            }
        }

        private void WorldPlaceSceneryButton_Click(object sender, EventArgs e)
        {
            Program.GE.PlaceSceneryID = MediaDialogs.GetMesh(false, Program.GE.PlaceSceneryID);
            WorldPlaceSceneryButton.Text = GE.NiceMeshName(Program.GE.PlaceSceneryID);
        }

        private void WorldPlaceGridCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (WorldPlaceGridCheck.Checked == true && WorldPlaceAlignCheck.Checked == true)
            {
                WorldPlaceAlignCheck.Checked = false;
            }
        }
    }
}