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
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace HeightMap
{
    public partial class HeightMapImportDialog : Form
    {
        public HeightMapImportDialog()
        {
            InitializeComponent();
        }

        public String HeightMapPath
        {
            get { return HMPathTextbox.Text; }
        }

        public String Texture0Path
        {
            get { return Tex0PathTextbox.Text; }
        }

        public String Texture1Path
        {
            get { return Tex1PathTextbox.Text; }
        }

        public String Texture2Path
        {
            get { return Tex2PathTextbox.Text; }
        }

        public String Texture3Path
        {
            get { return Tex3PathTextbox.Text; }
        }

        public String Texture4Path
        {
            get { return Tex4PathTextbox.Text; }
        }

        public float MaximumHeight
        {
            get { return Convert.ToSingle(ConfigMaxTextbox.Text); }
        }

        public float MinimumHeight
        {
            get { return Convert.ToSingle(ConfigMinTextbox.Text); }
        }

        private void GradientBox_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new LinearGradientBrush(new Point(0, 0), new Point(0, GradientBox.Height), Color.White, Color.Black);
            e.Graphics.FillRectangle(Grad, GradientBox.ClientRectangle);
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void HMPathButton_Click(object sender, EventArgs e)
        {
            if (OpenHeightmapDialog.ShowDialog() == DialogResult.Cancel)
                return;

            HMPathTextbox.Text = OpenHeightmapDialog.FileName;
        }

        private void Tex0PathButton_Click(object sender, EventArgs e)
        {
            if (OpenTextureDialog.ShowDialog() == DialogResult.Cancel)
                return;

            Tex0PathTextbox.Text = OpenTextureDialog.FileName;
        }

        private void Tex1PathButton_Click(object sender, EventArgs e)
        {
            if (OpenTextureDialog.ShowDialog() == DialogResult.Cancel)
                return;

            Tex1PathTextbox.Text = OpenTextureDialog.FileName;
        }

        private void Tex2PathButton_Click(object sender, EventArgs e)
        {
            if (OpenTextureDialog.ShowDialog() == DialogResult.Cancel)
                return;

            Tex2PathTextbox.Text = OpenTextureDialog.FileName;
        }

        private void Tex3PathButton_Click(object sender, EventArgs e)
        {
            if (OpenTextureDialog.ShowDialog() == DialogResult.Cancel)
                return;

            Tex3PathTextbox.Text = OpenTextureDialog.FileName;
        }

        private void Tex4PathButton_Click(object sender, EventArgs e)
        {
            if (OpenTextureDialog.ShowDialog() == DialogResult.Cancel)
                return;

            Tex4PathTextbox.Text = OpenTextureDialog.FileName;
        }

        private void ConfigMaxTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(ConfigMaxTextbox.Text);
            }
            catch
            {
                ConfigMaxTextbox.Text = "0.00";
            }
        }

        private void ConfigMinTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(ConfigMinTextbox.Text);
            }
            catch
            {
                ConfigMinTextbox.Text = "0.00";
            }
        }
    }
}