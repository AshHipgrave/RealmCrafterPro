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
using System.IO;
using RCTPlugin;

namespace RCTEStandard
{
    [RCTPluginAttribute]
    public class RCTEStandard : IRCTPlugin
    {
        public string Name { get { return "RCS RCTE Import"; } }
        public string Description { get { return "Imports RCTE terrains from Realm Crafter Standard"; } }
        public string Author { get { return "Jared Belkus"; } }
        public string Version { get { return "1.0"; } }

        public RCTImportData Import(IntPtr device)
        {
            RCTImportData OutputData = new RCTImportData();

            //OutputData.HeightData = new float[33 * 33];
            //OutputData.SplatData = new RCTSplatData[33 * 33];

            //for (int i = 0; i < 33 * 33; ++i)
            //{
            //    OutputData.HeightData[i] = -5.0f;
            //    OutputData.SplatData[i] = new RCTSplatData();
            //    OutputData.SplatData[i].Set(255, 0, 0, 0);
            //}

            //OutputData.Width = 32;
            //OutputData.Height = 32;

            //OutputData.TexturePaths[0] = "RCT\\Texs\\0.png";
            //OutputData.TexturePaths[1] = "RCT\\Texs\\1.png";
            //OutputData.TexturePaths[2] = "RCT\\Texs\\2.png";
            //OutputData.TexturePaths[3] = "RCT\\Texs\\4.png";
            //OutputData.TexturePaths[4] = "RCT\\Texs\\3.png";

            BinaryReader F = new BinaryReader(File.Open("Main Island.rct", FileMode.Open));

            int LayerCount = F.ReadInt32();
            int LengthSegments = F.ReadInt32();

            OutputData.Width = OutputData.Height = Convert.ToUInt32(LengthSegments);
            OutputData.HeightData = new float[(LayerCount + 1) * (LayerCount + 1)];
            OutputData.SplatData = new RCTSplatData[(LayerCount + 1) * (LayerCount + 1)];

            for (int i = 0; i < LayerCount; ++i)
            {
                int Length = F.ReadInt32();
                string S = BitConverter.ToString(F.ReadBytes(Length));
                float TexScale = F.ReadSingle();

                if(i < 5)
                    OutputData.TexturePaths[i] = S;
            }

            for (int i = 0; i < LayerCount; ++i)
            {
                for (int v = 0; v < LayerCount * LayerCount; ++v)
                {

                }
            }

            
            /*

			
			For i=1 To numlayers
				s=GetSurface(layer(i),1)
				vc=CountVertices(s)-1
				
				For v=0 To vc
					cx#=ReadFloat(lfile)
					cy#=ReadFloat(lfile)
					cz#=ReadFloat(lfile)
				
					cr=ReadInt(lfile)
					cg=ReadInt(lfile)
					cb=ReadInt(lfile)
					
					ca#=ReadFloat(lfile)
					
					VertexCoords s,v,cx,cy,cz
					VertexColor s,v,cr,cg,cb,ca#
					VertexCoords s,v,VertexX(s,v),VertexY(s,v),VertexZ(S,v)
  				
				Next

				UpdateNormals layer(i)
			Next
             * */


            return OutputData;
        }
    }
}
