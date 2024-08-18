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
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using RCTPlugin;

namespace HeightMap
{
    [RCTPluginAttribute]
    public class HeightMap : IRCTPlugin
    {
        public string Name { get { return "HeightMap Import"; } }
        public string Description { get { return "Imports greyscale heightmaps."; } }
        public string Author { get { return "Jared Belkus"; } }
        public string Version { get { return "1.0"; } }

        public RCTImportData Import(IntPtr device)
        {
            HeightMapImportDialog D = new HeightMapImportDialog();
            if (D.ShowDialog() == DialogResult.Cancel)
                return null;

            Bitmap HM = null;

            try
            {
                Image HMImage = Image.FromFile(D.HeightMapPath);
                HM = new Bitmap(HMImage);
            }catch(System.IO.FileNotFoundException e)
            {
                return null;
            }catch(Exception e)
            {
                MessageBox.Show("Error: Could not load heightmap image!\n\n" + e.Message);
                return null;
            }

            if (HM.Width != HM.Height)
            {
                MessageBox.Show("Error: Heightmap is not square!");
                return null;
            }

            if (HM.Width < 32 || HM.Width > 2048 || HM.Width % 32 != 0)
            {
                MessageBox.Show("Error: Heightmap dimensions are incorrect!");
                return null;
            }

            RCTImportData Out = new RCTImportData();
            Out.Height = Out.Width = Convert.ToUInt32(HM.Width);
            Out.HeightData = new float[(HM.Width + 1) * (HM.Width + 1)];
            Out.SplatData = new RCTSplatData[(HM.Width + 1) * (HM.Width + 1)];
            Out.GrassData = new byte[(HM.Width + 1) * (HM.Width + 1)];

            Out.TexturePaths[0] = D.Texture0Path;
            Out.TexturePaths[1] = D.Texture1Path;
            Out.TexturePaths[2] = D.Texture2Path;
            Out.TexturePaths[3] = D.Texture3Path;
            Out.TexturePaths[4] = D.Texture4Path;

            float Max = D.MaximumHeight;
            float Min = D.MinimumHeight;

            float Scale = (Max - Min) / 256.0f;
            float Bias = Min;

            for (int i = 0; i < Out.SplatData.Length; ++i)
            {
                Out.SplatData[i] = new RCTSplatData();
                Out.GrassData[i] = 0;
            }

            for (int z = 0; z < HM.Width; ++z)
            {
                for (int x = 0; x < HM.Width; ++x)
                {
                    Color Pixel = HM.GetPixel(x, z);
                    Out.HeightData[(z * (HM.Width + 1)) + x] = (Convert.ToSingle(Convert.ToInt32(Pixel.R)) * Scale) + Bias;
                }
            }

            for (int x = 0; x < HM.Width; ++x)
                Out.HeightData[(HM.Width * (HM.Width + 1)) + x] = Out.HeightData[(HM.Width * (HM.Width)) + x];
            for (int z = 0; z < HM.Width + 1; ++z)
                Out.HeightData[(z * (HM.Width + 1)) + HM.Width] = Out.HeightData[(z * (HM.Width + 1)) + (HM.Width - 1)];
            

            return Out;
        }
    }
}
