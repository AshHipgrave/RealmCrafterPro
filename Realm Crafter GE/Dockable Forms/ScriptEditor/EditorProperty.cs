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
using RenderingServices;
using System.Drawing;
using Scripting;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public class EditorProperty
    {
        public string Name = "";
        public string SDKName = "";
        public PropertyType Type;
        public object DefaultValue;
        public bool AlwaysSet;

        public static string[] TypeNames = new string[] {
            "Bool",
            "Byte",
            "Short",
            "Int",
            "Float",
            "Vector2",
            "Vector3",
            "Color",
            "String",
            "StringArray",
            "PositionType",
            "SizeType",
            "TextAlign" };

        public static PropertyType TypeFromString(string value)
        {
            for (int i = 0; i < TypeNames.Length; ++i)
            {
                if (value.Equals(TypeNames[i], StringComparison.CurrentCultureIgnoreCase))
                {
                    return (PropertyType)i;
                }
            }

            // Nothing found? Display an error, since its specifically dev related
            MessageBox.Show("Error: PropertyType '" + value + "' was no understood!", "SDK GUI Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            return PropertyType.String;
        }

        public static string StringArrayToString(IEnumerable<string> array)
        {
            int TotalLength = 0;
            foreach (string S in array)
            {
                TotalLength += 4 + S.Length;
            }

            if (TotalLength == 0)
                return "";

            byte[] Buffer = new byte[TotalLength];
            int Offset = 0;
            foreach (string S in array)
            {
                BitConverter.GetBytes(S.Length).CopyTo(Buffer, Offset);
                Encoding.ASCII.GetBytes(S).CopyTo(Buffer, Offset + 4);

                Offset += 4 + S.Length;
            }

            return Convert.ToBase64String(Buffer);
        }

        public static List<string> StringToStringList(string value)
        {
            List<string> Out = new List<string>();
            if (value.Length == 0)
                return Out;

            byte[] Buffer = Convert.FromBase64String(value);

            int Offset = 0;
            while (Offset < Buffer.Length)
            {
                int ReadLength = BitConverter.ToInt32(Buffer, Offset);
                string ReadValue = Encoding.ASCII.GetString(Buffer, Offset + 4, ReadLength);

                Offset += 4 + ReadLength;
                Out.Add(ReadValue);
            }

            return Out;
        }

        public static object Parse(PropertyType type, string value)
        {
            switch (type)
            {
                case PropertyType.Bool:
                    {
                        try { return Convert.ToBoolean(value); }
                        catch (Exception) { return false; }
                        break;
                    }
                case PropertyType.Byte:
                    {
                        try { return Convert.ToByte(value); }
                        catch (Exception) { return 0; }
                        break;
                    }
                case PropertyType.Int16:
                    {
                        try { return Convert.ToInt16(value); }
                        catch (Exception) { return 0; }
                        break;
                    }
                case PropertyType.Int32:
                    {
                        try { return Convert.ToInt32(value); }
                        catch (Exception) { return 0; }
                        break;
                    }
                case PropertyType.Single:
                    {
                        try { return Convert.ToSingle(value); }
                        catch (Exception) { return 0.0f; }
                        break;
                    }
                case PropertyType.Vector2:
                    {
                        return (new Vector2OptionsConverter()).ConvertFrom(null, null, value);
                        break;
                    }
                case PropertyType.Vector3:
                    {
                        return (new Vector3OptionsConverter()).ConvertFrom(null, null, value);
                        break;
                    }
                case PropertyType.Color:
                    {
                        Vector4 C4 = (new Vector4OptionsConverter()).ConvertFrom(null, null, value) as Vector4;

                        Color Out = Color.FromArgb(
                            (byte)C4.X,
                            (byte)C4.Y,
                            (byte)C4.Z,
                            (byte)C4.W);

                        return Out;

                        break;
                    }
                case PropertyType.String:
                    {
                        return value;
                        break;
                    }
                case PropertyType.StringArray:
                    {
                        return StringToStringList(value);
                        break;
                    }
                case PropertyType.PositionType:
                    {
                        if (value.Equals("Absolute", StringComparison.CurrentCultureIgnoreCase) || value.Equals("0"))
                            return PositionType.Absolute;
                        else if (value.Equals("Relative", StringComparison.CurrentCultureIgnoreCase) || value.Equals("1"))
                            return PositionType.Relative;
                        else if (value.Equals("Centered", StringComparison.CurrentCultureIgnoreCase) || value.Equals("2"))
                            return PositionType.Centered;
                        else
                            return PositionType.Absolute;

                        break;
                    }
                case PropertyType.SizeType:
                    {
                        if (value.Equals("Absolute", StringComparison.CurrentCultureIgnoreCase) || value.Equals("0"))
                            return SizeType.Absolute;
                        else if (value.Equals("Relative", StringComparison.CurrentCultureIgnoreCase) || value.Equals("1"))
                            return SizeType.Relative;
                        else
                            return SizeType.Absolute;

                        break;
                    }
                case PropertyType.TextAlign:
                    {
                        if (value.Equals("-1") || value.Equals("Default", StringComparison.CurrentCultureIgnoreCase))
                            return TextAlign.Default;
                        else if (value.Equals("Left", StringComparison.CurrentCultureIgnoreCase))
                            return TextAlign.Left;
                        else if (value.Equals("Center", StringComparison.CurrentCultureIgnoreCase))
                            return TextAlign.Center;
                        else if (value.Equals("Right", StringComparison.CurrentCultureIgnoreCase))
                            return TextAlign.Right;
                        else if (value.Equals("Top", StringComparison.CurrentCultureIgnoreCase))
                            return TextAlign.Top;
                        else if (value.Equals("Middle", StringComparison.CurrentCultureIgnoreCase))
                            return TextAlign.Middle;
                        else if (value.Equals("Bottom", StringComparison.CurrentCultureIgnoreCase))
                            return TextAlign.Bottom;
                        else
                            return TextAlign.Default;
                        break;
                    }
            }

            return "";
        }

        public static void WritePacket(PacketWriter Pa, PropertyType type, object value)
        {
            switch (type)
            {
                case PropertyType.Bool:
                    {
                        Pa.Write((byte)(((bool)value) ? 1 : 0));
                        break;
                    }
                case PropertyType.Byte:
                    {
                        Pa.Write((byte)value);
                        break;
                    }
                case PropertyType.Int16:
                    {
                        Pa.Write((short)value);
                        break;
                    }
                case PropertyType.Int32:
                    {
                        Pa.Write((int)value);
                        break;
                    }
                case PropertyType.Single:
                    {
                        Pa.Write((float)value);
                        break;
                    }
                case PropertyType.String:
                    {
                        Pa.Write((string)value, true);
                        break;
                    }
                case PropertyType.PositionType:
                    {
                        Pa.Write((byte)(PropertyType)value);
                        break;
                    }
                case PropertyType.SizeType:
                    {
                        Pa.Write((byte)(SizeType)value);
                        break;
                    }
                case PropertyType.TextAlign:
                    {
                        Pa.Write((byte)(TextAlign)value);
                        break;
                    }
                case PropertyType.Vector2:
                    {
                        Pa.Write(((Vector2)value).X);
                        Pa.Write(((Vector2)value).Y);
                        break;
                    }
                case PropertyType.Vector3:
                    {
                        Pa.Write(((Vector3)value).X);
                        Pa.Write(((Vector3)value).Y);
                        Pa.Write(((Vector3)value).Z);
                        break;
                    }
                case PropertyType.Color:
                    {
                        Pa.Write(((Color)value).ToArgb());
                        break;
                    }
                case PropertyType.StringArray:
                    {
                        List<string> Val = value as List<string>;


                        Pa.Write(Val.Count);
                        foreach (string S in Val)
                            Pa.Write(S, true);
                        break;
                    }

            }

            return;
        }

        public static string DesignerString(PropertyType type, object value)
        {
            switch (type)
            {
                case PropertyType.Bool:
                    {
                        return " = " + value.ToString().ToLower() + ";";
                        break;
                    }
                case PropertyType.Byte:
                case PropertyType.Int16:
                case PropertyType.Int32:
                case PropertyType.Single:
                    {
                        return " = " + value.ToString() + ";";
                        break;
                    }
                case PropertyType.String:
                    {
                        return " = \"" + value.ToString().Replace(@"\", @"\\") + "\";";
                        break;
                    }
                case PropertyType.PositionType:
                    {
                        return " = Scripting.Forms.PositionType." + value.ToString() + ";";
                        break;
                    }
                case PropertyType.SizeType:
                    {
                        return " = Scripting.Forms.SizeType." + value.ToString() + ";";
                        break;
                    }
                case PropertyType.TextAlign:
                    {
                        return " = Scripting.Forms.TextAlign." + value.ToString() + ";";
                        break;
                    }
                case PropertyType.Vector2:
                    {
                        return " = new Scripting.Math.Vector2(" + value.ToString() + ");";
                        break;
                    }
                case PropertyType.Vector3:
                    {
                        return " = new Scripting.Math.Vector3(" + value.ToString() + ");";
                        break;
                    }
                case PropertyType.Color:
                    {
                        Color C = (Color)value;

                        return " = System.Drawing.Color.FromArgb("
                            + C.A.ToString() + ", "
                            + C.R.ToString() + ", "
                            + C.G.ToString() + ", "
                            + C.B.ToString() + ");";
                        break;
                    }
                case PropertyType.StringArray:
                    {
                        List<string> Val = value as List<string>;
                        string Out = ".AddRange(new string[] { ";

                        for (int i = 0; i < Val.Count; ++i)
                        {
                            Out += "\"" + Val[i].Replace(@"\", @"\\") + "\"";

                            if (i < Val.Count - 1)
                                Out += ", ";
                        }

                        Out += "});";

                        return Out;
                        break;
                    }

            }

            return "";
        }

        public static string ToString(PropertyType type, object value)
        {
            switch (type)
            {
                case PropertyType.Bool:
                case PropertyType.Byte:
                case PropertyType.Int16:
                case PropertyType.Int32:
                case PropertyType.Single:
                case PropertyType.Vector2:
                case PropertyType.Vector3:
                case PropertyType.String:
                case PropertyType.PositionType:
                case PropertyType.SizeType:
                case PropertyType.TextAlign:
                    {
                        return value.ToString();
                        break;
                    }
                case PropertyType.Color:
                    {
                        Color C = (Color)value;
                        return C.A.ToString() + ", "
                            + C.R.ToString() + ", "
                            + C.G.ToString() + ", "
                            + C.B.ToString();

                        break;
                    }
                case PropertyType.StringArray:
                    {
                        return StringArrayToString(value as IEnumerable<string>);
                        break;
                    }

            }

            return "";
        }


    } // Class
}
