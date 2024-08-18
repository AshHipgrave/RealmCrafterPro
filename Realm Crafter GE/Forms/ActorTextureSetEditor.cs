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
    public partial class ActorTextureSetEditor : Form
    {
        ActorTextureSet[] DisplayedSet = new ActorTextureSet[256];

        public ActorTextureSetEditor()
        {
            InitializeComponent();
        }

        private void ActorTextureSetEditor_Load(object sender, EventArgs e)
        {



        }

        public ActorTextureSet[] TextureSet
        {
            get
            {
                int MaxLen = 0;
                for (int i = 0; i < DisplayedSet.Length; ++i)
                {
                    if (DisplayedSet[i].Tex0 < 65535
                        || DisplayedSet[i].Tex1 < 65535
                        || DisplayedSet[i].Tex2 < 65535
                        || DisplayedSet[i].Tex3 < 65535)
                        MaxLen = i;
                }

                ActorTextureSet[] Set = new ActorTextureSet[MaxLen + 1];
                for (int i = 0; i < MaxLen + 1; ++i)
                    Set[i] = DisplayedSet[i];

                return Set;
            }
            set
            {
                DisplayedSet = new ActorTextureSet[256];
                for (int i = 0; i < value.Length; ++i)
                    DisplayedSet[i] = value[i];

                if (value.Length < 256)
                {
                    for (int i = value.Length; i < 256; ++i)
                        DisplayedSet[i] = new ActorTextureSet(65535, 65535, 65535, 65535);
                }

                TextureGrid.Rows.Clear();
                for (int i = 0; i < DisplayedSet.Length; ++i)
                {
                    DataGridViewButtonCell Tex0Cell = new DataGridViewButtonCell();
                    DataGridViewButtonCell Tex1Cell = new DataGridViewButtonCell();
                    DataGridViewButtonCell Tex2Cell = new DataGridViewButtonCell();
                    DataGridViewButtonCell Tex3Cell = new DataGridViewButtonCell();
                    Tex0Cell.Value = DisplayedSet[i].Tex0 < 65535 ? GE.NiceTextureName(DisplayedSet[i].Tex0) : "NONE";
                    Tex1Cell.Value = DisplayedSet[i].Tex1 < 65535 ? GE.NiceTextureName(DisplayedSet[i].Tex1) : "NONE";
                    Tex2Cell.Value = DisplayedSet[i].Tex2 < 65535 ? GE.NiceTextureName(DisplayedSet[i].Tex2) : "NONE";
                    Tex3Cell.Value = DisplayedSet[i].Tex3 < 65535 ? GE.NiceTextureName(DisplayedSet[i].Tex3) : "NONE";

                    DataGridViewRow R = new DataGridViewRow();
                    R.Cells.Add(Tex0Cell);
                    R.Cells.Add(Tex1Cell);
                    R.Cells.Add(Tex2Cell);
                    R.Cells.Add(Tex3Cell);

                    TextureGrid.Rows.Add(R);
                    TextureGrid.Rows[i].HeaderCell.Value = i.ToString();
                }
            }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        private void TextureGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            ushort Sel = 65535;
            switch (e.ColumnIndex)
            {
                case 0:
                    {
                        Sel = MediaDialogs.GetTexture(true, "Actors", DisplayedSet[e.RowIndex].Tex0);
                        DisplayedSet[e.RowIndex].Tex0 = Sel;
                        break;
                    }
                case 1:
                    {
                        Sel = MediaDialogs.GetTexture(true, "Actors", DisplayedSet[e.RowIndex].Tex1);
                        DisplayedSet[e.RowIndex].Tex1 = Sel;
                        break;
                    }
                case 2:
                    {
                        Sel = MediaDialogs.GetTexture(true, "Actors", DisplayedSet[e.RowIndex].Tex2);
                        DisplayedSet[e.RowIndex].Tex2 = Sel;
                        break;
                    }
                case 3:
                    {
                        Sel = MediaDialogs.GetTexture(true, "Actors", DisplayedSet[e.RowIndex].Tex3);
                        DisplayedSet[e.RowIndex].Tex3 = Sel;
                        break;
                    }
            }

            TextureGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = GE.NiceTextureName(Sel);

        }
    }
}