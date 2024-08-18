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

namespace RCTPlugin
{
    public class RCTSplatData
    {
        public byte S0, S1, S2, S3;

        public RCTSplatData()
        {
            S0 = S1 = S2 = S3 = 0;
        }

        public void Set(byte s0, byte s1, byte s2, byte s3)
        {
            S0 = s0;
            S1 = s1;
            S2 = s2;
            S3 = s3;
        }
    }

    public struct RCTTerrainPosition
    {
        private int x;
        private int y;

        public RCTTerrainPosition(int setX, int setY)
        {
            x = setX;
            y = setY;
        }

        public int X
        {
            get { return x; }
            set { x = value; }
        }

        public int Y
        {
            get { return y; }
            set { y = value; }
        }

    }

    public class RCTImportData
    {
        public float[] HeightData = null;
        public RCTSplatData[] SplatData = null;
        public byte[] GrassData = null;
        public uint Width = 0;
        public uint Height = 0;
        public string[] TexturePaths = new string[] { "", "", "", "", "" };
        public float[] TextureScales = new float[] { 1.0f, 1.0f, 1.0f, 1.0f, 1.0f };
        public string[] GrassTypes = new string[] { "", "", "", "", "", "", "", "" };
        public List<RCTTerrainPosition> ExclusionZones = new List<RCTTerrainPosition>();

        public RCTImportData()
        {
        }
    }

    public interface IRCTPlugin
    {
        string Name { get;}
        string Description { get;}
        string Author { get;}
        string Version { get;}

        RCTImportData Import(IntPtr device);
    }
}
