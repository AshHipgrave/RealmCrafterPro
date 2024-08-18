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
    public class Vector4ColorOptionsConverter : ExpandableObjectConverter
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
                Vector4 C = (Vector4) value;
                int R = (int) (C.X * 255.0f);
                int G = (int) (C.Y * 255.0f);
                int B = (int) (C.Z * 255.0f);
                int A = (int) (C.W * 255.0f);

                if (R > 255)
                {
                    R = 255;
                }
                if (G > 255)
                {
                    G = 255;
                }
                if (B > 255)
                {
                    B = 255;
                }
                if (A > 255)
                {
                    A = 255;
                }

                if (R < 0)
                {
                    R = 0;
                }
                if (G < 0)
                {
                    G = 0;
                }
                if (B < 0)
                {
                    B = 0;
                }
                if (A < 0)
                {
                    A = 0;
                }

                string sR = R.ToString("X");
                string sG = G.ToString("X");
                string sB = B.ToString("X");
                string sA = A.ToString("X");

                if (sR.Length == 1)
                {
                    sR = "0" + sR;
                }
                if (sG.Length == 1)
                {
                    sG = "0" + sG;
                }
                if (sB.Length == 1)
                {
                    sB = "0" + sB;
                }
                if (sA.Length == 1)
                {
                    sA = "0" + sA;
                }

                return sA + sR + sG + sB;
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

                    if (s.Length != 8)
                    {
                        return base.ConvertFrom(context, culture, value);
                    }

                    string sA = s.Substring(0, 2);
                    string sR = s.Substring(2, 2);
                    string sG = s.Substring(4, 2);
                    string sB = s.Substring(6, 2);

                    int R = 0, G = 0, B = 0, A = 0;
                    ;

                    try
                    {
                        R = int.Parse(sR, System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                        R = 0;
                    }

                    try
                    {
                        G = int.Parse(sG, System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                        G = 0;
                    }

                    try
                    {
                        B = int.Parse(sB, System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                        B = 0;
                    }

                    try
                    {
                        A = int.Parse(sA, System.Globalization.NumberStyles.HexNumber);
                    }
                    catch
                    {
                        A = 0;
                    }

                    float fR = (float) R;
                    float fG = (float) G;
                    float fB = (float) B;
                    float fA = (float) A;

                    fR /= 255.0f;
                    fG /= 255.0f;
                    fB /= 255.0f;
                    fA /= 255.0f;

                    return new Vector4(fR, fG, fB, fA);
                }
                catch
                {
                    //throw new ArgumentException(
                    //    "Can not convert '" + (string)value +
                    //                       "' to type Vector3");
                }
            }
            return base.ConvertFrom(context, culture, value);
        }
    }
}