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
using RenderingServices;
using System.Drawing;

namespace RealmCrafter_GE
{
    public class LinePrim
    {
        protected List<RenderingServices.Line3D> Lines;
        protected Color _Color;
        protected Entity _Parent;

        public LinePrim()
        {
            Lines = new List<Line3D>();
            _Color = Color.White;
        }

        protected virtual void Rebuild(bool UseOld)
        {
        }

        public Color Color
        {
            get { return _Color; }
            set
            {
                _Color = value;
                Rebuild(true);
            }
        }
    }

    public class LineCircle : LinePrim
    {
        protected float _Detail;
        protected float _Size;
        protected int _Direction;

        public LineCircle()
        {
            _Detail = 10.0f;
            _Size = 20.0f;
            Rebuild(false);
        }

        public LineCircle(float Detail, float Size, int Direction)
        {
            _Detail = Detail;
            _Size = Size;
            _Direction = Direction;
            Rebuild(false);
        }

        public LineCircle(float Detail, float Size, int Direction, Entity Parent)
        {
            _Detail = Detail;
            _Size = Size;
            _Parent = Parent;
            _Direction = Direction;
            Rebuild(false);
        }

        public float Detail
        {
            get { return _Detail; }
            set
            {
                _Detail = value;
                Rebuild(false);
            }
        }

        public float Size
        {
            get { return _Size; }
            set
            {
                _Size = value;
                Rebuild(true);
            }
        }

        public int Direction
        {
            get { return _Direction; }
            set
            {
                _Direction = value;
                Rebuild(true);
            }
        }

        protected override void Rebuild(bool UseOld)
        {
            if (!UseOld)
            {
                // Clear out current lines
                foreach (RenderingServices.Line3D L in Lines)
                {
                    L.Free();
                }
                Lines.Clear();
            }
            int Cnt = 0;

            double Turn = (Math.PI * 2.0f) / (float) Detail;

            // Loop locals
            double i = 0.0f;
            int f = 0;
            while (i < (Math.PI * 2.0f))
            {
                double X = 0, Y = 0, Z = 0, X2 = 0, Y2 = 0, Z2 = 0;

                if (_Direction == 0)
                {
                    X = Math.Sin(i);
                    Y = Math.Cos(i);
                    Z = 0.0f;

                    X2 = Math.Sin(i + Turn);
                    Y2 = Math.Cos(i + Turn);
                    Z2 = 0.0f;
                }
                else if (_Direction == 1)
                {
                    X = Math.Sin(i);
                    Y = 0.0f;
                    Z = Math.Cos(i);

                    X2 = Math.Sin(i + Turn);
                    Y2 = 0.0f;
                    Z2 = Math.Cos(i + Turn);
                }
                else if (_Direction == 2)
                {
                    X = 0.0f;
                    Y = Math.Cos(i);
                    Z = Math.Sin(i);

                    X2 = 0.0f;
                    Y2 = Math.Cos(i + Turn);
                    Z2 = Math.Sin(i + Turn);
                }

                Line3D L = null;

                if (UseOld)
                {
                    L = Lines[Cnt];
                    L.SetPositions((float) (X * Size), (float) (Y * Size), (float) (Z * Size), (float) (X2 * Size),
                                   (float) (Y2 * Size), (float) (Z2 * Size));
                }
                else
                {
                    L = new Line3D((float) (X * Size), (float) (Y * Size), (float) (Z * Size), (float) (X2 * Size),
                                   (float)(Y2 * Size), (float)(Z2 * Size), _Parent, true, false);
                }
                ++Cnt;

                L.SetColor(_Color.R, _Color.G, _Color.B);
                Lines.Add(L);

                ++f;
                i += Turn;
            }

            Line3D Lx = (!UseOld) ? new Line3D(Size, 0, 0, -Size, 0, 0, _Parent, true, false) : Lines[Cnt];
            ++Cnt;
            Line3D Ly = (!UseOld) ? new Line3D(0, Size, 0, 0, -Size, 0, _Parent, true, false) : Lines[Cnt];
            ++Cnt;
            Line3D Lz = (!UseOld) ? new Line3D(0, 0, Size, 0, 0, -Size, _Parent, true, false) : Lines[Cnt];
            ++Cnt;

            Lx.SetPositions(Size, 0, 0, -Size, 0, 0);
            Ly.SetPositions(0, Size, 0, 0, -Size, 0);
            Lz.SetPositions(0, 0, Size, 0, 0, -Size);

            Lx.SetColor(_Color.R, _Color.G, _Color.B);
            Ly.SetColor(_Color.R, _Color.G, _Color.B);
            Lz.SetColor(_Color.R, _Color.G, _Color.B);

            Lines.Add(Lx);
            Lines.Add(Ly);
            Lines.Add(Lz);
        }
    }
}