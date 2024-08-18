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
    [StructLayout(LayoutKind.Sequential, Size = 12)]
    public class Vector3 : ICloneable
    {
        public float _X = 0;
        public float _Y = 0;
        public float _Z = 0;

        public float _MinX = 0, _MinY = 0, _MinZ = 0;
        public float _MaxX = 0, _MaxY = 0, _MaxZ = 0;

        public Vector3()
        {
        }

        public Vector3(float x, float y, float z)
        {
            _X = x;
            _Y = y;
            _Z = z;
        }

        public float X
        {
            get { return _X; }
            set { _X = value; }
        }

        public float Y
        {
            get { return _Y; }
            set { _Y = value; }
        }

        public float Z
        {
            get { return _Z; }
            set { _Z = value; }
        }

        public object Clone()
        {
            Vector3 O = new Vector3();

            O.X = X;
            O.Y = Y;
            O.Z = Z;

            return O;
        }

        public void Normalize()
        {
            float Magnitude = Convert.ToSingle(Math.Sqrt(Convert.ToDouble(_X * _X + _Y * _Y + _Z * _Z)));

            _X /= Magnitude;
            _Y /= Magnitude;
            _Z /= Magnitude;
        }

        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString();
        }
    }
}