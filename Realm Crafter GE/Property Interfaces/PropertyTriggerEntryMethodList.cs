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
// Displays list of script methods in a drop down box on a propertygrid
// Author: Shane Smith, Aug 2008
using System.ComponentModel;
using RealmCrafter.ClientZone;

namespace RealmCrafter_GE.Property_Interfaces
{
    public class PropertyTriggerEntryMethodList : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection
            GetStandardValues(ITypeDescriptorContext context)
        {
            // Generate list of methods
            string[] Result;
            if (Program.GE.ZoneSelected.Count > 0)
            {
                ZoneObject ZO = (ZoneObject) Program.GE.ZoneSelected[0];
                if (ZO is Trigger)
                {
                    Trigger _Object = (Trigger) Program.GE.ZoneSelected[0];
                    string Script = Program.GE.CurrentServerZone.TriggerScript[_Object.ServerID];
                    System.IO.BinaryReader F = Blitz.ReadFile(@"Data\Server Data\Scripts\" + Script + @".rsl");
                    if (F == null)
                    {
                        return null;
                    }

                    long Length = F.BaseStream.Length;
                    string NewLine, FunctionNames = "";
                    int Pos, Count = 0;
                    while (F.BaseStream.Position < Length)
                    {
                        NewLine = Blitz.ReadLine(F);

                        // Is this line a valid function declaration?
                        Pos = NewLine.ToUpper().IndexOf("FUNCTION");
                        if (Pos >= 0)
                        {
                            Pos = NewLine.IndexOf("(", Pos);
                            if (Pos > 0)
                            {
                                FunctionNames += NewLine.Substring(0, Pos + 1).Substring(9).Trim() + "|";
                                ++Count;
                            }
                        }
                    }

                    F.Close();
                    Result = new string[Count];

                    // Found at least one function
                    if (Count > 0)
                    {
                        for (int i = 0; i < Count; ++i)
                        {
                            Pos = FunctionNames.IndexOf("|");
                            NewLine = FunctionNames.Substring(0, Pos - 1);
                            FunctionNames = FunctionNames.Substring(Pos + 1);
                            Result[i] = NewLine;
                        }
                    }
                    else
                    {
                        Result = new string[1];
                        Result[0] = "No methods found";
                    }
                }
                else
                {
                    Result = new string[1];
                    Result[0] = "No methods found";
                }
            }
            else
            {
                Result = new string[1];
                Result[0] = "No methods found";
            }

            return new StandardValuesCollection(Result);
        }
    }
}