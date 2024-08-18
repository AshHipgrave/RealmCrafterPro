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
using System.Xml;

namespace L3DT
{
    public partial class L3DTImportDialog : Form
    {
        public L3DTImportDialog()
        {
            InitializeComponent();
        }

        public string HFZPath { get { return HMPathTextbox.Text; } }
        public string SplatRPath { get { return SplatRPathTextbox.Text; } }
        public string SplatGPath { get { return SplatGPathTextbox.Text; } }
        public string SplatBPath { get { return SplatBPathTextbox.Text; } }
        public string SplatAPath { get { return SplatAPathTextbox.Text; } }
        public string Texture0Path { get { return Tex0PathTextbox.Text; } }
        public string Texture1Path { get { return Tex1PathTextbox.Text; } }
        public string Texture2Path { get { return Tex2PathTextbox.Text; } }
        public string Texture3Path { get { return Tex3PathTextbox.Text; } }
        public string Texture4Path { get { return Tex4PathTextbox.Text; } }

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
                VerifyPath(SplatRPathTextbox.Text);
                VerifyPath(SplatGPathTextbox.Text);
                VerifyPath(SplatBPathTextbox.Text);
                VerifyPath(SplatAPathTextbox.Text);
                VerifyPath(Tex0PathTextbox.Text);
                VerifyPath(Tex1PathTextbox.Text);
                VerifyPath(Tex2PathTextbox.Text);
                VerifyPath(Tex3PathTextbox.Text);
                VerifyPath(Tex4PathTextbox.Text);
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
            if (OpenHFZDialog.ShowDialog() == DialogResult.Cancel)
                return;

            HMPathTextbox.Text = OpenHFZDialog.FileName;
        }

        private void SplatPathButton_Click(object sender, EventArgs e)
        {
            if (OpenSplatDialog.ShowDialog() == DialogResult.Cancel)
                return;

            if (sender == SplatRPathButton)
                SplatRPathTextbox.Text = OpenSplatDialog.FileName;
            if (sender == SplatGPathButton)
                SplatGPathTextbox.Text = OpenSplatDialog.FileName;
            if (sender == SplatBPathButton)
                SplatBPathTextbox.Text = OpenSplatDialog.FileName;
            if (sender == SplatAPathButton)
                SplatAPathTextbox.Text = OpenSplatDialog.FileName;
        }

        private void TexturePathButton_Click(object sender, EventArgs e)
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

        private void ImportXMLButton_Click(object sender, EventArgs e)
        {
            if (OpenXMLDialog.ShowDialog() == DialogResult.Cancel)
                return;

            string path = Path.GetFullPath(OpenXMLDialog.FileName);
            string LocalDirectory = Path.GetDirectoryName(path) + "\\";

            XmlTextReader X = null;

            int SearchIndex = 0;
            string SplatFile = "";
            string TextureFile = "";

            try
            {
                // Open file
                X = new XmlTextReader(path);

                // Read elements
                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("FileName", StringComparison.CurrentCultureIgnoreCase))
                    {
                        String ReadPath = X.ReadString();
                        if (ReadPath.Substring(0, 1).Equals("\"", StringComparison.CurrentCultureIgnoreCase))
                            ReadPath = ReadPath.Substring(1);
                        if (ReadPath.Substring(ReadPath.Length-1, 1).Equals("\"", StringComparison.CurrentCultureIgnoreCase))
                            ReadPath = ReadPath.Substring(0, ReadPath.Length - 1);

                        SplatFile = LocalDirectory + ReadPath;
                    }

                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("TextureFile", StringComparison.CurrentCultureIgnoreCase))
                    {
                        String ReadPath = X.ReadString();
                        if (ReadPath.Substring(0, 1).Equals("\"", StringComparison.CurrentCultureIgnoreCase))
                            ReadPath = ReadPath.Substring(1);
                        if (ReadPath.Substring(ReadPath.Length - 1, 1).Equals("\"", StringComparison.CurrentCultureIgnoreCase))
                            ReadPath = ReadPath.Substring(0, ReadPath.Length - 1);

                        TextureFile = ReadPath;

                        switch (SearchIndex)
                        {
                            case 0:
                                {
                                    SplatRPathTextbox.Text = SplatFile;
                                    Tex0PathTextbox.Text = TextureFile;
                                    break;
                                }
                            case 1:
                                {
                                    SplatGPathTextbox.Text = SplatFile;
                                    Tex1PathTextbox.Text = TextureFile;
                                    break;
                                }
                            case 2:
                                {
                                    SplatBPathTextbox.Text = SplatFile;
                                    Tex2PathTextbox.Text = TextureFile;
                                    break;
                                }
                            case 3:
                                {
                                    SplatAPathTextbox.Text = SplatFile;
                                    Tex3PathTextbox.Text = TextureFile;
                                    break;
                                }
                            case 4:
                                {
                                    Tex4PathTextbox.Text = TextureFile;
                                    break;
                                }
                        }
                        ++SearchIndex;
                        SplatFile = "";
                        TextureFile = "";
                    }

                }
            }
            catch (System.IO.FileNotFoundException Ex)
            {
                // So what?
            }
            catch (Exception Ex)
            {
                throw new Exception("Read XML File failed: \n" + Ex.Message);
            }
            finally
            {
                // Cleanup
                if (X != null)
                    X.Close();
            }
        }





    }
}