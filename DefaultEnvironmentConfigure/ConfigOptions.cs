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
using RealmCrafter.SDK;

namespace DefaultEnvironmentConfigure
{
    public partial class ConfigOptions : Form
    {
        public ConfigOptions()
        {
            InitializeComponent();
        }

        string starsPath = "";
        string cloudsPath = "";
        string gradientPath = "";
        string flarePath = "";

        public string StarsPath
        {
            get { return starsPath; }
            set { starsPath = value; StarsButton.Text = Path.GetFileName(value); }
        }
        
        public string CloudsPath
        {
            get { return cloudsPath; }
            set { cloudsPath = value; CloudsButton.Text = Path.GetFileName(value); }
        }
        
        public string GradientPath
        {
            get { return gradientPath; }
            set { gradientPath = value; GradientButton.Text = Path.GetFileName(value); }
        }
        

        public string FlarePath
        {
            get { return flarePath; }
            set { flarePath = value; FlareButton.Text = Path.GetFileName(value); }
        }

        private void OKButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void CnlButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void StarsButton_Click(object sender, EventArgs e)
        {
            StarsPath = MediaDialogs.GetTexture();
        }

        private void CloudsButton_Click(object sender, EventArgs e)
        {
            CloudsPath = MediaDialogs.GetTexture();
        }

        private void GradientButton_Click(object sender, EventArgs e)
        {
            GradientPath = MediaDialogs.GetTexture();
        }

        private void FlareButton_Click(object sender, EventArgs e)
        {
            FlarePath = MediaDialogs.GetTexture();
        }
    }
}