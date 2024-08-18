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
/* This class interfaces the property grid with the selected Trigger object
 * retrieving information from the Trigger object class
 * Author: Shane Smith, Aug 2008
 */
using System.ComponentModel;
using RealmCrafter.ClientZone;
using System;

namespace RealmCrafter_GE.Property_Interfaces
{
    public class TriggerPropertyInterface
    {
        private Trigger _Object;

        public TriggerPropertyInterface(Trigger Object)
        {
            _Object = Object;
        }

        #region Location
        [CategoryAttribute("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }

            set
            {
                _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }

        [CategoryAttribute("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }
            set
            {
                _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }

        [CategoryAttribute("Location")]
        public float LocZ
        {
            get { return _Object.EN.Z(); }
            set
            {
                _Object.EN.Position(_Object.EN.X(), _Object.EN.Y(), value);
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }
        #endregion

        [CategoryAttribute("Trigger")]
        public bool IsSquare
        {
            get { return _Object.IsSquare; }
            set
            {
                if (_Object.IsSquare == value)
                    return;

                // Its becoming a square, copy over Size
                if (value)
                {
                    _Object.Width = _Object.Height = _Object.Depth = _Object.Size;
                }
                else
                {
                    _Object.Size = Math.Max(_Object.Width, Math.Max(_Object.Height, _Object.Depth));
                }

                _Object.IsSquare = value;
                _Object.ReBuild();
            }

        }

        [CategoryAttribute("Scale")]
        public float Width
        {
            get { return (_Object.IsSquare ? _Object.Width * 20 : _Object.Size * 20); }
            set
            {
                if (_Object.IsSquare)
                {
                    _Object.Width = value / 20.0f;
                    _Object.EN.Scale(_Object.Width * 0.5f, _Object.Height * 0.5f, _Object.Depth * 0.5f);
                }
                else
                {
                    _Object.Size = value / 20.0f;
                    _Object.EN.Scale(value / 20, value / 20, value / 20);
                }
            }
        }

        [CategoryAttribute("Scale")]
        public float Height
        {
            get { return (_Object.IsSquare ? _Object.Height * 20 : _Object.Size * 20); }
            set
            {
                if (_Object.IsSquare)
                {
                    _Object.Height = value / 20.0f;
                    _Object.EN.Scale(_Object.Width * 0.5f, _Object.Height * 0.5f, _Object.Depth * 0.5f);
                }
                else
                {
                    _Object.Size = value / 20.0f;
                    _Object.EN.Scale(value / 20, value / 20, value / 20);
                }
            }
        }

        [CategoryAttribute("Scale")]
        public float Depth
        {
            get { return (_Object.IsSquare ? _Object.Depth * 20 : _Object.Size * 20); }
            set
            {
                if (_Object.IsSquare)
                {
                    _Object.Depth = value / 20.0f;
                    _Object.EN.Scale(_Object.Width * 0.5f, _Object.Height * 0.5f, _Object.Depth * 0.5f);
                }
                else
                {
                    _Object.Size = value / 20.0f;
                    _Object.EN.Scale(value / 20, value / 20, value / 20);
                }
            }
        }

        [TypeConverter(typeof(PropertyScriptList)),
         CategoryAttribute("Scripts"),
         DescriptionAttribute("Script fired by entering the trigger")]
        public string Script
        {
            get { return _Object.Script; }
            set
            {
                _Object.Script = value;
                _Object.Label.Text = "Trigger: " + value;
            }
        }

//         [TypeConverter(typeof(PropertyTriggerEntryMethodList)),
//          CategoryAttribute("Scripts"),
//          DescriptionAttribute("Entry method used by the script")]
//         public string ScriptEntryMethod
//         {
//             get { return Program.GE.CurrentServerZone.TriggerMethod[_Object.ServerID]; }
//             set { Program.GE.CurrentServerZone.TriggerMethod[_Object.ServerID] = value; }
//         }
// 
//         public string[] GetMethods(string ScriptName)
//         {
//             string[] Result;
//             if (Program.GE.ZoneSelected.Count > 0)
//             {
//                 ZoneObject ZO = (ZoneObject) Program.GE.ZoneSelected[0];
//                 if (ZO is Trigger)
//                 {
//                     Trigger _Object = (Trigger) Program.GE.ZoneSelected[0];
//                     System.IO.BinaryReader F = Blitz.ReadFile(@"Data\Server Data\Scripts\" + ScriptName);
//                     if (F == null)
//                     {
//                         Result = new string[1];
//                         Result[0] = "";
//                         return Result;
//                     }
// 
//                     long Length = F.BaseStream.Length;
//                     string NewLine, FunctionNames = "";
//                     int Pos, Count = 0;
//                     while (F.BaseStream.Position < Length)
//                     {
//                         NewLine = Blitz.ReadLine(F);
// 
//                         // Is this line a valid function declaration?
//                         Pos = NewLine.ToUpper().IndexOf("FUNCTION");
//                         if (Pos >= 0)
//                         {
//                             Pos = NewLine.IndexOf("(", Pos);
//                             if (Pos > 0)
//                             {
//                                 FunctionNames += NewLine.Substring(0, Pos + 1).Substring(9).Trim() + "|";
//                                 ++Count;
//                             }
//                         }
//                     }
// 
//                     F.Close();
//                     Result = new string[Count];
// 
//                     // Found at least one function
//                     if (Count > 0)
//                     {
//                         for (int i = 0; i < Count; ++i)
//                         {
//                             Pos = FunctionNames.IndexOf("|");
//                             NewLine = FunctionNames.Substring(0, Pos - 1);
//                             FunctionNames = FunctionNames.Substring(Pos + 1);
//                             Result[i] = NewLine;
//                         }
//                     }
//                     else
//                     {
//                         Result = new string[1];
//                         Result[0] = "";
//                     }
//                 }
//                 else
//                 {
//                     Result = new string[1];
//                     Result[0] = "";
//                 }
//             }
//             else
//             {
//                 Result = new string[1];
//                 Result[0] = "";
//             }
// 
//             return Result;
//         }
    }
}