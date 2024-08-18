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
// Realm Crafter Encryption module by Rob W (rottbott@hotmail.com)
// Written November 2006

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RealmCrafter
{
    public static class Encryption
    {
        [DllImport("rc64.dll")]
        private static extern void BF_Start(int MagicNumber);

        [DllImport("rc64.dll")]
        private static extern void BF_KPush(string One, string Two);

        [DllImport("rc64.dll")]
        private static extern void BF_encrypt(byte[] Memory, int Size);

        [DllImport("rc64.dll")]
        private static extern void BF_decrypt(byte[] Memory, int Size);

        // Initialises the encryption DLL
        public static void Initialise(int MagicNumber)
        {
            BF_Start(MagicNumber);
        }

        // Encrypts a file
        public static void EncryptFile(string SourceFile, string DestinationFile)
        {
            // Load file into memory
            byte[] Data = File.ReadAllBytes(SourceFile);

            // Encrypt
            BF_encrypt(Data, Data.Length);

            // Resave file to disk
            File.WriteAllBytes(DestinationFile, Data);

            // Delete old file
            File.Delete(SourceFile);
        }

        // Decrypts/encrypts data in memory
        public static void DecryptMemory(byte[] Data)
        {
            BF_decrypt(Data, Data.Length);
        }

        public static void EncryptMemory(byte[] Data)
        {
            BF_encrypt(Data, Data.Length);
        }
    }
}