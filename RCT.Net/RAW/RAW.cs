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
using System.IO;
using RCTPlugin;

namespace RAW
{
    [RCTPluginAttribute]
    public class HeightMap : IRCTPlugin
    {
        public string Name { get { return "RAW Import"; } }
        public string Description { get { return "Imports RAW terrain files or various formats"; } }
        public string Author { get { return "Jared Belkus"; } }
        public string Version { get { return "1.0"; } }

        public RCTImportData Import(IntPtr device)
        {
            RAWImportDialog D = new RAWImportDialog();
            if (D.ShowDialog() == DialogResult.Cancel)
                return null;
                    
            //public string RAWPath { get { return HMPathTextbox.Text; } }
            //public string Texture0Path { get { return Tex0PathTextbox.Text; } }
            //public string Texture1Path { get { return Tex1PathTextbox.Text; } }
            //public string Texture2Path { get { return Tex2PathTextbox.Text; } }
            //public string Texture3Path { get { return Tex3PathTextbox.Text; } }
            //public string Texture4Path { get { return Tex4PathTextbox.Text; } }
            //public int RAWFormat { get { return FormatCombobox.SelectedIndex; } }
            //public float YScale { get { return Convert.ToSingle(YScaleTextbox.Text); } }
            //public int HeaderLength { get { return Convert.ToInt32(HeaderSizeTextbox.Text); } }
            //public int TerrainWidth { get { return Convert.ToInt32(TerrainWidthTextbox.Text); } }
            //public bool AutoDetectWidth { get { return AutoDetectCheckbox.Checked; } }

            BinaryReader Br = new BinaryReader(File.Open(D.RAWPath, FileMode.Open));

            if (Br.BaseStream.Length < D.HeaderLength + 1)
            {
                MessageBox.Show("Error: Header was too large!");
                return null;
            }

            // Move in
            Br.BaseStream.Seek(D.HeaderLength, SeekOrigin.Begin);

            int PointSize = 0;
            switch(D.RAWFormat)
            {
                case 0:
                    {
                        PointSize = 1;
                        break;
                    }
                case 1:
                    {
                        PointSize = 2;
                        break;
                    }
                case 2:
                    {
                        PointSize = 2;
                        break;
                    }
                case 3:
                    {
                        PointSize = 4;
                        break;
                    }
            }

            long FullLength = (Br.BaseStream.Length - D.HeaderLength) / PointSize;
            int Width = (int)Math.Pow((double)FullLength, 0.5);

            if (!D.AutoDetectWidth)
            {
                if (D.TerrainWidth > Width)
                {
                    MessageBox.Show("Error: User specified width was too high!");
                    return null;
                }
                Width = D.TerrainWidth;
            }

            if (Width < 32 || Width > 2048 || Width % 32 != 0)
            {
                MessageBox.Show("Error: Terrain width is invalid!");
                return null;
            }

            RCTImportData Out = new RCTImportData();
            Out.Height = Out.Width = Convert.ToUInt32(Width);
            Out.HeightData = new float[(Out.Width + 1) * (Out.Width + 1)];
            Out.SplatData = new RCTSplatData[(Out.Width + 1) * (Out.Width + 1)];
            Out.GrassData = new byte[(Out.Width + 1) * (Out.Width + 1)];

            Out.TexturePaths[0] = D.Texture0Path;
            Out.TexturePaths[1] = D.Texture1Path;
            Out.TexturePaths[2] = D.Texture2Path;
            Out.TexturePaths[3] = D.Texture3Path;
            Out.TexturePaths[4] = D.Texture4Path;

            // Allocate basic data
            for (int i = 0; i < Out.SplatData.Length; ++i)
            {
                Out.SplatData[i] = new RCTSplatData();
                Out.HeightData[i] = 0.0f;
                Out.GrassData[i] = 0;
            }

            switch (D.RAWFormat)
            {
                case 0:
                    {
                        ReadU8(ref Out, ref Br);
                        break;
                    }
                case 1:
                    {
                        ReadS16(ref Out, ref Br);
                        break;
                    }
                case 2:
                    {
                        ReadU16(ref Out, ref Br);
                        break;
                    }
                case 3:
                    {
                        ReadFP32(ref Out, ref Br);
                        break;
                    }
            }

            for (int x = 0; x < Out.Width; ++x)
                Out.HeightData[(Out.Width) * (Out.Width + 1) + x] = Out.HeightData[(Out.Width - 1) * (Out.Width + 1) + x];

            for(int z = 0; z <= Out.Width; ++z)
                Out.HeightData[z * (Out.Width + 1) + Out.Width] = Out.HeightData[z * (Out.Width + 1) + (Out.Width - 1)];

            float YScale = D.YScale;
            for (int i = 0; i < Out.HeightData.Length; ++i)
                Out.HeightData[i] *= YScale;

            

            return Out;
        }

        private void ReadU8(ref RCTImportData Out, ref BinaryReader Br)
        {
            for (int z = 0; z < Out.Width; ++z)
            {
                for (int x = 0; x < Out.Width; ++x)
                {
                    Out.HeightData[z * (Out.Width + 1) + x] = Convert.ToSingle(Br.ReadByte());
                }
            }
        }


        private void ReadS16(ref RCTImportData Out, ref BinaryReader Br)
        {
            for (int z = 0; z < Out.Width; ++z)
            {
                for (int x = 0; x < Out.Width; ++x)
                {
                    Out.HeightData[z * (Out.Width + 1) + x] = Convert.ToSingle(Br.ReadInt16());
                }
            }
        }

        private void ReadU16(ref RCTImportData Out, ref BinaryReader Br)
        {
            for (int z = 0; z < Out.Width; ++z)
            {
                for (int x = 0; x < Out.Width; ++x)
                {
                    Out.HeightData[z * (Out.Width + 1) + x] = Convert.ToSingle(Br.ReadUInt16());
                }
            }
        }

        private void ReadFP32(ref RCTImportData Out, ref BinaryReader Br)
        {
            for (int z = 0; z < Out.Width; ++z)
            {
                for (int x = 0; x < Out.Width; ++x)
                {
                    Out.HeightData[z * (Out.Width + 1) + x] = Br.ReadSingle();
                }
            }
        }
    }
}
