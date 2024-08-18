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
// Functions to mimic Blitz commands as a compatiblity shim
// Written by Rob W (rottbott@hotmail.com), November 2006

using System;
using System.IO;

public static class Blitz
{
    // File functions
    public static BinaryReader ReadFile(string Filename)
    {
        try
        {
            FileStream FS = new FileStream(Filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            BinaryReader F = new BinaryReader(FS);
            return F;
        }
        catch (IOException)
        {
            return null;
        }
    }

    public static BinaryWriter WriteFile(string Filename)
    {
        try
        {
            FileStream FS = new FileStream(Filename, FileMode.Create, FileAccess.Write, FileShare.None);
            BinaryWriter F = new BinaryWriter(FS);
            return F;
        }
        catch (IOException)
        {
            return null;
        }
    }

    // String read/write functions
    public static string ReadString(BinaryReader F)
    {
        int Length = F.ReadInt32();
        char[] StringData = F.ReadChars(Length);
        string S = new string(StringData);
        return S;
    }

    public static void WriteString(string S, BinaryWriter F)
    {
        char[] StringData = S.ToCharArray();
        F.Write(S.Length);
        F.Write(StringData, 0, S.Length);
    }

    public static string ReadLine(BinaryReader F)
    {
        char C;
        string S = "";
        while (F.BaseStream.Position < F.BaseStream.Length)
        {
            C = F.ReadChar();
            if (C == 13)
            {
                F.BaseStream.Position++;
                break;
            }
            else
            {
                S += C;
            }
        }
        return S;
    }

    public static void WriteLine(string S, BinaryWriter F)
    {
        char[] StringData = S.ToCharArray();
        F.Write(StringData, 0, S.Length);
        F.Write((byte) 13);
        F.Write((byte) 10);
    }

    // Mimics RottNet string building functions
    public static string StrFromInt(int n, int Bytes)
    {
        System.Text.Encoding Encoder = System.Text.Encoding.GetEncoding(850);
        byte[] Data = new byte[Bytes];
        for (int i = 0; i < Bytes; ++i)
        {
            Data[i] = (byte) (n >> (i * 8));
        }
        return Encoder.GetString(Data);
    }

    public static string StrFromInt(int n)
    {
        return StrFromInt(n, 4);
    }

    public static string StrFromFloat(float n)
    {
        System.Text.Encoding Encoder = System.Text.Encoding.GetEncoding(850);
        return Encoder.GetString(BitConverter.GetBytes(n));
    }

    public static int IntFromStr(string s)
    {
        int Result = 0;
        System.Text.Encoding E = System.Text.Encoding.GetEncoding(850);
        byte[] Chars = E.GetBytes(s);
        for (int i = 0; i < s.Length; ++i)
        {
            Result = Result | ((int) Chars[i] << (i * 8));
        }
        return Result;
    }

    public static float FloatFromStr(string s)
    {
        System.Text.Encoding E = System.Text.Encoding.GetEncoding(850);
        return BitConverter.ToSingle(E.GetBytes(s), 0);
    }

    // Trigonometry functions
    public static float Sin(float Number)
    {
        return (float) Math.Sin((Number * Math.PI) / 180);
    }

    public static float Cos(float Number)
    {
        return (float) Math.Cos((Number * Math.PI) / 180);
    }

    public static float Tan(float Number)
    {
        return (float) Math.Tan((Number * Math.PI) / 180);
    }

    public static float ASin(float Number)
    {
        return (float) ((Math.Asin(Number) * 180) / Math.PI);
    }

    public static float ACos(float Number)
    {
        return (float) ((Math.Acos(Number) * 180) / Math.PI);
    }

    public static float ATan(float Number)
    {
        return (float) ((Math.Atan(Number) * 180) / Math.PI);
    }
}