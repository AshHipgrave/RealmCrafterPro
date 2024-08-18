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
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace RenderingServices
{
    public class Vector4OptionsConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(Vector4))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture,
                                         object value, Type destinationType)
        {
            if (destinationType == typeof(System.String) && value is Vector4)
            {
                Vector4 v = (Vector4) value;

                return v.ToString();
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string s = (string) value;
                    int comma1 = s.IndexOf(',');
                    int comma2 = s.IndexOf(',', comma1 + 1);
                    int comma3 = s.IndexOf(',', comma2 + 2);

                    if (comma1 != -1 && comma2 != -1 && comma3 != -1)
                    {
                        string X = s.Substring(0, comma1).Trim();
                        string Y = s.Substring(comma1 + 1, comma2 - comma1 - 1).Trim();
                        string Z = s.Substring(comma2 + 1, comma3 - comma2 - 1).Trim();
                        string W = s.Substring(comma3 + 1).Trim();

                        Vector4 V = new Vector4();
                        V.X = Convert.ToSingle(X);
                        V.Y = Convert.ToSingle(Y);
                        V.Z = Convert.ToSingle(Z);
                        V.W = Convert.ToSingle(W);

                        return V;
                    }
                }
                catch
                {
                    throw new ArgumentException(
                        "Can not convert '" + (string) value +
                        "' to type SpellingOptions");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}