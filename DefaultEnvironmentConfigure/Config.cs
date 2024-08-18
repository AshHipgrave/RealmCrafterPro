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

namespace DefaultEnvironmentConfigure
{
    [RealmCrafter.SDK.EnvironmentConfig]
    public class Config : RealmCrafter.SDK.ISDKEnvironmentConfigurator
    {
        protected string ReadString(BinaryReader reader)
        {
            int Length = reader.ReadInt32();

            byte[] Buffer = reader.ReadBytes(Length);
            return ASCIIEncoding.ASCII.GetString(Buffer);
        }

        protected void WriteString(BinaryWriter writer, string value)
        {
            byte[] Buffer = ASCIIEncoding.ASCII.GetBytes(value);

            writer.Write(Buffer.Length);
            writer.Write(Buffer);
        }

        public string GetName()
        {
            return "default";
        }

        public void Show(string zoneName)
        {
            ConfigOptions O = new ConfigOptions();

            O.StarsPath = "Unknown";
            O.CloudsPath = "Unknown";
            O.GradientPath = "Unknown";
            O.FlarePath = "Unknown";

            if (File.Exists(@"Data\Areas\" + zoneName + ".env"))
            {
                try
                {
                    BinaryReader Reader = new BinaryReader(File.OpenRead(@"Data\Areas\" + zoneName + ".env"));

                    O.CloudsPath = ReadString(Reader);
                    O.StarsPath = ReadString(Reader);
                    O.GradientPath = ReadString(Reader);
                    O.FlarePath = ReadString(Reader);

                    Reader.Close();
                }
                catch (System.Exception)
                {

                }
            }

            if (O.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    BinaryWriter Writer = new BinaryWriter(File.Open(@"Data\Areas\" + zoneName + ".env", FileMode.Create));

                    WriteString(Writer, O.CloudsPath);
                    WriteString(Writer, O.StarsPath);
                    WriteString(Writer, O.GradientPath);
                    WriteString(Writer, O.FlarePath);

                    Writer.Close();
                }
                catch (System.Exception)
                {

                }
            }

            // Done
        }
    }
}
