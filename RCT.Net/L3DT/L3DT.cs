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
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Drawing2D;
using RCTPlugin;

namespace L3DT
{
    [RCTPluginAttribute]
    public class HeightMap : IRCTPlugin
    {
        public string Name { get { return "L3DT Import"; } }
        public string Description { get { return "Imports L3DT LFZ and Splat files."; } }
        public string Author { get { return "Jared Belkus"; } }
        public string Version { get { return "1.0"; } }

        public RCTImportData Import(IntPtr device)
        {
            L3DTImportDialog D = new L3DTImportDialog();
            if (D.ShowDialog() == DialogResult.Cancel)
                return null;

            float[] Data = LoadEx(D.HFZPath);

            if (Data == null)
            {
                MessageBox.Show("Error: Could not load HFZ file!");
                return null;
            }

            uint Width = Convert.ToUInt32(Math.Pow((double)Data.Length, 0.5));
            if (Width < 32 || Width > 2048 || Width % 32 != 0)
            {
                MessageBox.Show("Error: HFZ File contained incorrect dimensions!");
                return null;
            }

            // Setup data structures
            RCTImportData Out = new RCTImportData();
            Out.Height = Out.Width = Convert.ToUInt32(Math.Pow((double)Data.Length, 0.5));
            Out.HeightData = new float[(Out.Width + 1) * (Out.Width + 1)];
            Out.SplatData = new RCTSplatData[(Out.Width + 1) * (Out.Width + 1)];
            Out.GrassData = new byte[(Out.Width + 1) * (Out.Width + 1)];

            // HFZ Copy
            for (int i = 0; i < Out.SplatData.Length; ++i)
                Out.SplatData[i] = new RCTSplatData();

            for (int z = 0; z < Out.Width; ++z)
            {
                for (int x = 0; x < Out.Width; ++x)
                {
                    Out.HeightData[z * Out.Width + x] = Data[z * (Out.Width - 1) + x] * 2.0f;
                    Out.GrassData[z * Out.Width + x] = 0;
                }
            }

            for (int z = 0; z < Out.Width + 1; ++z)
            {
                float THeight = Out.HeightData[z * (Out.Width + 1) + 3];
                Out.HeightData[z * (Out.Width + 1) + 0] = THeight;
                Out.HeightData[z * (Out.Width + 1) + 1] = THeight;
                Out.HeightData[z * (Out.Width + 1) + 2] = THeight;
            }

            for (int x = 0; x < Out.Width + 1; ++x)
            {
                Out.HeightData[(Out.Width) * (Out.Width + 1) + x] = Out.HeightData[(Out.Width - 1) * (Out.Width + 1) + x];
            }

            // Texture Copy
            Out.TexturePaths[0] = D.Texture0Path;
            Out.TexturePaths[1] = D.Texture1Path;
            Out.TexturePaths[2] = D.Texture2Path;
            Out.TexturePaths[3] = D.Texture3Path;
            Out.TexturePaths[4] = D.Texture4Path;

            // Splat Import
            try
            {
                ImportSplat(D.SplatRPath, 0, ref Out);
                ImportSplat(D.SplatGPath, 1, ref Out);
                ImportSplat(D.SplatBPath, 2, ref Out);
                ImportSplat(D.SplatAPath, 3, ref Out);
            }
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.Message);
                return null;
            }


            return Out;
        }

        private void ImportSplat(string path, int index, ref RCTImportData Out)
        {
            if (path.Length == 0)
                return;

            Bitmap HM = null;

            Image HMImage = Image.FromFile(path);
            HM = new Bitmap(HMImage);

            if (HM.Width != HM.Height)
                throw new Exception("Error: splat map is not square!");

            if (Convert.ToUInt32(HM.Width) != Out.Width)
                throw new Exception("Error: Splat texture does not contain the same dimensions as its HFZ file!");

            for (int z = 0; z < HM.Width; ++z)
            {
                for (int x = 0; x < HM.Width; ++x)
                {
                    Color C = HM.GetPixel(x, (HM.Width - 1) - z);

                    switch (index)
                    {
                        case 0:
                            {
                                Out.SplatData[z * (HM.Width + 1) + x].S0 = C.R;
                                break;
                            }
                        case 1:
                            {
                                Out.SplatData[z * (HM.Width + 1) + x].S1 = C.R;
                                break;
                            }
                        case 2:
                            {
                                Out.SplatData[z * (HM.Width + 1) + x].S2 = C.R;
                                break;
                            }
                        case 3:
                            {
                                Out.SplatData[z * (HM.Width + 1) + x].S3 = C.R;
                                break;
                            }
                    }
                }
            }
        }

        [DllImport("libhfz.dll")]
        public static extern IntPtr LoadHFZ(string Path);

        [DllImport("libhfz.dll")]
        public static extern void FreePointer(IntPtr Ptr);

        public static float[] LoadEx(string path)
        {
            IntPtr inData = LoadHFZ(path);
            if (inData == null)
                return null;
            int Length = Marshal.ReadInt32(inData, 0);
            float[] Data = new float[Length];

            for (int i = 0; i < Length; ++i)
                Data[i] = BitConverter.ToSingle(BitConverter.GetBytes(Marshal.ReadInt32(inData, (i + 1) * 4)), 0);
            
            //FreePointer(inData);

            return Data;
        }
    }
}
