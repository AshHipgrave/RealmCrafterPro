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
using RCTPlugin;

namespace RCTTest
{
    public partial class AutoTextureDialog : Form
    {
        AutoTextureNode SelectedNode = null;

        public AutoTextureDialog()
        {
            InitializeComponent();

            
            ConfigureTexture0.Text = "Texture 0";
            ConfigureTexture1.Text = "Texture 1";
            ConfigureTexture2.Text = "Texture 2";
            ConfigureTexture3.Text = "Texture 3";
            ConfigureTexture4.Text = "Texture 4";

            ConfigureHeight.ValueChanged += new EventHandler(ConfigureHeight_ValueChanged);
            ConfigureSlope.ValueChanged += new EventHandler(ConfigureSlope_ValueChanged);

//0.80f, 2, 1.0f - 0.37f, 1.0f - 0.0f, -1.0f, 20.0f);
//0.80f, 1, 1.0f - 1.0f, 1.0f - 0.36f, -20.0f, 20.0f);
//0.80f, 0, 1.0f - 0.37f, 1.0f - 0.0f, -20.0f, -1.0f);
//0.80f, 3, 1.0f - 0.37f, 1.0f - 0.0f, 7.0f, 20.0f);


            // Set defaults
            ConfigureTexture0.SlopeMin = 0.0;
            ConfigureTexture0.SlopeMax = 0.372858997354675;
            //ConfigureTexture0.HeightMin = 0;
            //ConfigureTexture0.HeightMax = 400;
            ConfigureTexture0.HeightMin = -1;
            ConfigureTexture0.HeightMax = 20;

            ConfigureTexture1.SlopeMin = 0.36763888866886;
            ConfigureTexture1.SlopeMax = 1.0;
            //ConfigureTexture1.HeightMin = -100;
            //ConfigureTexture1.HeightMax = 500;
            ConfigureTexture1.HeightMin = -20;
            ConfigureTexture1.HeightMax = 20;

            ConfigureTexture2.SlopeMin = 0.0;
            ConfigureTexture2.SlopeMax = 0.372858997354675;
            //ConfigureTexture2.HeightMin = -100;
            //ConfigureTexture2.HeightMax = 0;
            ConfigureTexture2.HeightMin = -20;
            ConfigureTexture2.HeightMax = -1;

            ConfigureTexture3.SlopeMin = 0.0;
            ConfigureTexture3.SlopeMax = 0.372858997354675;
            //ConfigureTexture3.HeightMax = 500;
            //ConfigureTexture3.HeightMin = 400;
            ConfigureTexture3.HeightMax = 20; 
            ConfigureTexture3.HeightMin = 7;
                       

            ConfigureTexture4.SlopeMin = 0.0;
            ConfigureTexture4.SlopeMax = 0.0;
            ConfigureTexture4.HeightMin = 0;
            ConfigureTexture4.HeightMax = 0;

            SelectedNode = ConfigureTexture0;
            ConfigureTexture_Click(ConfigureTexture0, EventArgs.Empty);

        }

        void ConfigureSlope_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedNode == null)
                return;

            double SlopeMin = ConfigureSlope.Min;
            double SlopeMax = ConfigureSlope.Max;

            SelectedNode.SlopeMin = SlopeMin;
            SelectedNode.SlopeMax = SlopeMax;
        }

        void ConfigureHeight_ValueChanged(object sender, EventArgs e)
        {
            if (SelectedNode == null)
                return;

            SelectedNode.HeightMin = ConfigureHeight.Low;
            SelectedNode.HeightMax = ConfigureHeight.High;
        }

        private void ConfigureTexture_Click(object sender, EventArgs e)
        {
            ConfigureTexture0.Selected = false;
            ConfigureTexture1.Selected = false;
            ConfigureTexture2.Selected = false;
            ConfigureTexture3.Selected = false;
            ConfigureTexture4.Selected = false;

            if (sender is AutoTextureNode)
            {
                AutoTextureNode Node = sender as AutoTextureNode;

                SelectedNode = Node;
                Node.Selected = true;
                ConfigureSlope.Min = Node.SlopeMin;
                ConfigureSlope.Max = Node.SlopeMax;
                ConfigureHeight.Low = Node.HeightMin;
                ConfigureHeight.High = Node.HeightMax;
            }
        }

        private void OKBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }
    }
}