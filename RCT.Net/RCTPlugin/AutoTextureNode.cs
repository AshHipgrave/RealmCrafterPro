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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RCTPlugin
{
    public partial class AutoTextureNode : UserControl
    {
        protected bool _Selected = false;
        protected String _TexturePath;

        public AutoTextureNode()
        {
            InitializeComponent();

            MainGroup.MouseClick += new MouseEventHandler(MainGroup_MouseClick);
        }

        void MainGroup_MouseClick(object sender, MouseEventArgs e)
        {
            this.OnClick(EventArgs.Empty);
        }

        public bool Selected
        {
            get { return _Selected; }
            set
            {
                _Selected = value;

                if (_Selected == false)
                    MainGroup.BackColor = SystemColors.Control;
                else
                    MainGroup.BackColor = Color.White;

                MainGroup.Invalidate();
            }
        }

        public bool AllowTextureChange
        {
            get { return TexturePathButton.Visible; }
            set { TexturePathButton.Visible = value; }
        }

        public String TexturePath
        {
            get { return _TexturePath; }
            set
            {
                _TexturePath = value;
                TexturePathLabel.Text = "Texture Path: " + Path.GetFileName(_TexturePath);
            }
        }

        public double SlopeMin
        {
            get { return Convert.ToDouble(SlopeMinTextbox.Text); }
            set
            {
                SlopeMinTextbox.TextChanged -= new EventHandler(SlopeMinTextbox_TextChanged);
                SlopeMinTextbox.Text = value.ToString();
                SlopeMinTextbox.TextChanged += new EventHandler(SlopeMinTextbox_TextChanged);
            }
        }

        public double SlopeMax
        {
            get { return Convert.ToDouble(SlopeMaxTextbox.Text); }
            set
            {
                SlopeMaxTextbox.TextChanged -= new EventHandler(SlopeMaxTextbox_TextChanged);
                SlopeMaxTextbox.Text = value.ToString();
                SlopeMaxTextbox.TextChanged += new EventHandler(SlopeMaxTextbox_TextChanged);
            }
        }

        public int HeightMin
        {
            get { return Convert.ToInt32(HeightMinTextbox.Text); }
            set { HeightMinTextbox.Text = value.ToString(); }
        }

        public int HeightMax
        {
            get { return Convert.ToInt32(HeightMaxTextbox.Text); }
            set { HeightMaxTextbox.Text = value.ToString(); }
        }

        public override string Text
        {
            get
            {
                return MainGroup.Text;
            }
            set
            {
                MainGroup.Text = value;
            }
        }

        private void MainGroup_Paint(object sender, PaintEventArgs e)
        {
            if (_Selected)
            {
                Rectangle SelectedEdge = new Rectangle(MainGroup.ClientRectangle.Location, new Size(MainGroup.ClientRectangle.Width - 1, MainGroup.ClientRectangle.Height - 1));
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(51, 153, 255)), SelectedEdge);
            }
        }

        private void SlopeMinTextbox_TextChanged(object sender, EventArgs e)
        {
            double V = 0.0f;
            try
            {
                V = Convert.ToDouble(SlopeMinTextbox.Text);
                MainGroup_MouseClick(null, null);
            }
            catch
            {
                SlopeMinTextbox.Text = "0.00";
            }
        }

        private void SlopeMaxTextbox_TextChanged(object sender, EventArgs e)
        {
            double V = 0.0f;
            try
            {
                V = Convert.ToDouble(SlopeMaxTextbox.Text);
                MainGroup_MouseClick(null, null);
            }
            catch
            {
                SlopeMaxTextbox.Text = "0.00";
            }
        }

        private void HeightMinTextbox_TextChanged(object sender, EventArgs e)
        {
            int V = 0;
            try
            {
                V = Convert.ToInt32(HeightMinTextbox.Text);
                MainGroup_MouseClick(null, null);
            }
            catch
            {
                HeightMinTextbox.Text = "0";
            }
        }

        private void HeightMaxTextbox_TextChanged(object sender, EventArgs e)
        {
            int V = 0;
            try
            {
                V = Convert.ToInt32(HeightMaxTextbox.Text);
                MainGroup_MouseClick(null, null);
            }
            catch
            {
                HeightMaxTextbox.Text = "0";
            }
        }

        private void TexturePathButton_Click(object sender, EventArgs e)
        {
            if (OpenTextureDialog.ShowDialog() != DialogResult.Cancel)
            {
                TexturePath = OpenTextureDialog.FileName;
            }
        }
    }
}
