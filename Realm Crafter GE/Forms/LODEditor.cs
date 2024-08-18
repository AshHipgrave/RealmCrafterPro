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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealmCrafter_GE.Forms
{
    public partial class LODEditor : Form
    {
        ushort HighMesh = 65535;
        ushort MedMesh = 65535;
        ushort LowMesh = 65535;

        public LODEditor()
        {
            InitializeComponent();

            HighMeshButton.Text = "No Mesh Selected";
            MediumMeshButton.Text = "No Mesh Selected";
            LowMeshButton.Text = "No Mesh Selected";
        }

        private void GOButton_Click(object sender, EventArgs e)
        {
            if (HighMesh == 65535)
            {
                MessageBox.Show("No mesh selected to modify!");
                return;
            }

            float DistToMed = (float)DistToMedSpinner.Value;
            float DistToLow = (float)DistToLowSpinner.Value;
            float DistToHide = (float)DistToHideSpinner.Value;

            if (DistToLow < DistToMed)
            {
                MessageBox.Show("Error: 'Distance To Low' must be further than 'Distance To Medium'.");
                return;
            }

            if (DistToHide < DistToLow)
            {
                MessageBox.Show("Error: 'Distance To Hide' must be further than 'Distance To Low'.");
                return;
            }

            if (Program.GE.CurrentClientZone == null)
                return;

            int Updated = 0;

            foreach (RealmCrafter.ClientZone.ZoneObject Obj in Program.GE.CurrentClientZone.Sceneries)
            {
                if (Obj != null && Obj is RealmCrafter.ClientZone.Scenery)
                {
                    RealmCrafter.ClientZone.Scenery Scn = Obj as RealmCrafter.ClientZone.Scenery;

                    if(Scn.EN == null)
                        continue;

                    if (Scn.MeshID == HighMesh)
                    {
                        Scn.EN._MeshLOD_Medium = MedMesh;
                        Scn.EN._MeshLOD_Low = LowMesh;

                        Scn.EN.distLOD_High = DistToMed;
                        Scn.EN.distLOD_Medium = DistToLow;
                        Scn.EN.distLOD_Low = DistToHide;

                        ++Updated;
                    }
                }
            }

            if (Updated == 0)
                MessageBox.Show("Couldn't find any instances of the selected mesh.");
            else
                MessageBox.Show("Completed, " + Updated + " meshes updated!");
            
        }

        private void LODEditor_Load(object sender, EventArgs e)
        {
            if (Program.GE.CurrentClientZone == null)
            {
                Close();
            }
        }

        private void HighMeshButton_Click(object sender, EventArgs e)
        {
            HighMesh = MediaDialogs.GetMesh(true, HighMesh);

            if (HighMesh == 65535)
                HighMeshButton.Text = "No Mesh Selected";
            else
                HighMeshButton.Text = GE.NiceMeshName(HighMesh);
        }

        private void MediumMeshButton_Click(object sender, EventArgs e)
        {
            MedMesh = MediaDialogs.GetMesh(true, MedMesh);

            if (MedMesh == 65535)
                MediumMeshButton.Text = "No Mesh Selected";
            else
                MediumMeshButton.Text = GE.NiceMeshName(MedMesh);
        }

        private void LowMeshButton_Click(object sender, EventArgs e)
        {
            LowMesh = MediaDialogs.GetMesh(true, LowMesh);

            if (LowMesh == 65535)
                LowMeshButton.Text = "No Mesh Selected";
            else
                LowMeshButton.Text = GE.NiceMeshName(LowMesh);
        }

        

        
    }
}
