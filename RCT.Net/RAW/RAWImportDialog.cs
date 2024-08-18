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
using System.IO;

namespace RAW
{
    public partial class RAWImportDialog : Form
    {
        public RAWImportDialog()
        {
            InitializeComponent();

            FormatCombobox.SelectedIndex = 0;
        }

        public string RAWPath { get { return HMPathTextbox.Text; } }
        public string Texture0Path { get { return Tex0PathTextbox.Text; } }
        public string Texture1Path { get { return Tex1PathTextbox.Text; } }
        public string Texture2Path { get { return Tex2PathTextbox.Text; } }
        public string Texture3Path { get { return Tex3PathTextbox.Text; } }
        public string Texture4Path { get { return Tex4PathTextbox.Text; } }
        public int RAWFormat { get { return FormatCombobox.SelectedIndex; } }
        public float YScale { get { return Convert.ToSingle(YScaleTextbox.Text); } }
        public int HeaderLength { get { return Convert.ToInt32(HeaderSizeTextbox.Text); } }
        public int TerrainWidth { get { return Convert.ToInt32(TerrainWidthTextbox.Text); } }
        public bool AutoDetectWidth { get { return AutoDetectCheckbox.Checked; } }

        private void VerifyPath(string path)
        {
            if (path.Length > 0)
                if (File.Exists(path))
                    return;
                else
                    throw new Exception("File does not exist: " + path);
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            try
            {
                if (HMPathTextbox.Text.Length == 0)
                    throw new Exception("Error: You must import a heightmap!");
                VerifyPath(HMPathTextbox.Text);
                VerifyPath(Tex0PathTextbox.Text);
                VerifyPath(Tex1PathTextbox.Text);
                VerifyPath(Tex2PathTextbox.Text);
                VerifyPath(Tex3PathTextbox.Text);
                VerifyPath(Tex4PathTextbox.Text);

                int UserWidth = TerrainWidth;
                if (!AutoDetectWidth)
                    if (UserWidth < 32 || UserWidth > 2048 || UserWidth % 32 != 0)
                        throw new Exception("Error: User specified width was invalid!");
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return;
            }
            DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void HMPathButton_Click(object sender, EventArgs e)
        {
            if (OpenRAWDialog.ShowDialog() == DialogResult.Cancel)
                return;

            HMPathTextbox.Text = OpenRAWDialog.FileName;
        }

        private void TexPathButton_Click(object sender, EventArgs e)
        {
            if (OpenTextureDialog.ShowDialog() == DialogResult.Cancel)
                return;

            if (sender == Tex0PathButton)
                Tex0PathTextbox.Text = OpenTextureDialog.FileName;
            if (sender == Tex1PathButton)
                Tex1PathTextbox.Text = OpenTextureDialog.FileName;
            if (sender == Tex2PathButton)
                Tex2PathTextbox.Text = OpenTextureDialog.FileName;
            if (sender == Tex3PathButton)
                Tex3PathTextbox.Text = OpenTextureDialog.FileName;
            if (sender == Tex4PathButton)
                Tex4PathTextbox.Text = OpenTextureDialog.FileName;
        }

        private void AutoDetectCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            if (AutoDetectCheckbox.Checked)
                TerrainWidthTextbox.ReadOnly = true;
            else
                TerrainWidthTextbox.ReadOnly = false;
        }

        private void YScaleTextbox_TextChanged(object sender, EventArgs e)
        {
            float V = 0.0f;
            try
            {
                V = Convert.ToSingle(YScaleTextbox.Text);
            }
            catch
            {
                YScaleTextbox.Text = "0.00";
            }
        }

        private void HeaderSizeTextbox_TextChanged(object sender, EventArgs e)
        {
            int V = 0;
            try
            {
                V = Convert.ToInt32(HeaderSizeTextbox.Text);
                if (V < 0)
                    throw new Exception();
            }
            catch
            {
                HeaderSizeTextbox.Text = "0";
            }
        }

        private void TerrainWidthTextbox_TextChanged(object sender, EventArgs e)
        {
            int V = 0;
            try
            {
                V = Convert.ToInt32(TerrainWidthTextbox.Text);
                if (V < 0)
                    throw new Exception();
            }
            catch
            {
                TerrainWidthTextbox.Text = "0";
            }
        }





    }
}