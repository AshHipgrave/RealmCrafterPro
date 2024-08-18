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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RenderingServices
{
    public partial class SliderEditorForm : Form
    {
        private bool XDown = false;
        private bool YDown = false;
        private bool ZDown = false;
        private bool WDown = false;

        private Vector4 _Min = new Vector4(0, 0, 0, 0);
        private Vector4 _Max = new Vector4(100, 100, 100, 100);
        private Vector4 _Diff = new Vector4(100, 100, 100, 100);
        private Vector4 _Step = new Vector4(1, 1, 1, 1);
        private Vector4 _Value = new Vector4(0, 0, 0, 0);

        private object _Object;

        public object SelectedObject
        {
            get
            {
                if (_Object is Vector4)
                {
                    Vector4 V = _Object as Vector4;
                    V.X = _Value.X;
                    V.Y = _Value.Y;
                    V.Z = _Value.Z;
                    V.W = _Value.W;

                    return V;
                }

                if (_Object is Vector3)
                {
                    Vector3 V = _Object as Vector3;
                    V.X = _Value.X;
                    V.Y = _Value.Y;
                    V.Z = _Value.Z;

                    return V;
                }

                if (_Object is Vector2)
                {
                    Vector2 V = _Object as Vector2;
                    V.X = _Value.X;
                    V.Y = _Value.Y;

                    return V;
                }

                if (_Object is Vector1)
                {
                    Vector1 V = _Object as Vector1;
                    V.X = _Value.X;

                    return V;
                }

                return null;
            }
            set
            {
                _Object = value;

                XSlide.Visible = true;
                YSlide.Visible = true;
                ZSlide.Visible = true;
                WSlide.Visible = true;

                XLabel.Visible = true;
                YLabel.Visible = true;
                ZLabel.Visible = true;
                WLabel.Visible = true;

                this.Height = 89;

                if (_Object is Vector4)
                {
                    _Value.X = (_Object as Vector4).X;
                    _Value.Y = (_Object as Vector4).Y;
                    _Value.Z = (_Object as Vector4).Z;
                    _Value.W = (_Object as Vector4).W;

                    Vector4 V = _Object as Vector4;
                    _Min = new Vector4(V._MinX, V._MinY, V._MinZ, V._MinW);
                    _Max = new Vector4(V._MaxX, V._MaxY, V._MaxZ, V._MaxW);
                }

                if (_Object is Vector3)
                {
                    _Value.X = (_Object as Vector3).X;
                    _Value.Y = (_Object as Vector3).Y;
                    _Value.Z = (_Object as Vector3).Z;

                    Vector3 V = _Object as Vector3;
                    _Min = new Vector4(V._MinX, V._MinY, V._MinZ, 0.0f);
                    _Max = new Vector4(V._MaxX, V._MaxY, V._MaxZ, 0.0f);

                    WSlide.Visible = false;

                    WLabel.Visible = false;

                    Height = 69;
                }

                if (_Object is Vector2)
                {
                    _Value.X = (_Object as Vector2).X;
                    _Value.Y = (_Object as Vector2).Y;

                    Vector2 V = _Object as Vector2;
                    _Min = new Vector4(V._MinX, V._MinY, 0.0f, 0.0f);
                    _Max = new Vector4(V._MaxX, V._MaxY, 0.0f, 0.0f);

                    ZSlide.Visible = false;
                    WSlide.Visible = false;

                    ZLabel.Visible = false;
                    WLabel.Visible = false;

                    Height = 49;
                }

                if (_Object is Vector1)
                {
                    _Value.X = (_Object as Vector1).X;

                    Vector1 V = _Object as Vector1;
                    _Min = new Vector4(V._MinX, 0.0f, 0.0f, 0.0f);
                    _Max = new Vector4(V._MaxX, 0.0f, 0.0f, 0.0f);

                    YSlide.Visible = false;
                    ZSlide.Visible = false;
                    WSlide.Visible = false;

                    YLabel.Visible = false;
                    ZLabel.Visible = false;
                    WLabel.Visible = false;

                    Height = 29;
                }

                _Diff.X = _Max.X - _Min.X;
                _Diff.Y = _Max.Y - _Min.Y;
                _Diff.Z = _Max.Z - _Min.Z;
                _Diff.W = _Max.W - _Min.W;

                XSlide.Invalidate();
                YSlide.Invalidate();
                ZSlide.Invalidate();
                WSlide.Invalidate();

                XLabel.Text = _Value.X.ToString();
                YLabel.Text = _Value.Y.ToString();
                ZLabel.Text = _Value.Z.ToString();
                WLabel.Text = _Value.W.ToString();
            }
        }

        public SliderEditorForm()
        {
            InitializeComponent();
            TopLevel = false;
        }

        private void XSlide_MouseDown(object sender, MouseEventArgs e)
        {
            XDown = true;
        }

        private void XSlide_MouseUp(object sender, MouseEventArgs e)
        {
            XDown = false;
        }

        private void YSlide_MouseDown(object sender, MouseEventArgs e)
        {
            YDown = true;
        }

        private void YSlide_MouseUp(object sender, MouseEventArgs e)
        {
            YDown = false;
        }

        private void ZSlide_MouseDown(object sender, MouseEventArgs e)
        {
            ZDown = true;
        }

        private void ZSlide_MouseUp(object sender, MouseEventArgs e)
        {
            ZDown = false;
        }

        private void WSlide_MouseDown(object sender, MouseEventArgs e)
        {
            WDown = true;
        }

        private void WSlide_MouseUp(object sender, MouseEventArgs e)
        {
            WDown = false;
        }

        private void XSlide_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new System.Drawing.Drawing2D.LinearGradientBrush(XSlide.ClientRectangle, Color.Black,
                                                                          Color.White, 10);
            Brush Black = Brushes.Black;
            Brush White = Brushes.White;
            e.Graphics.FillRectangle(Grad, 0, 0, XSlide.Width, 20);

            double Width = XSlide.Width;
            int Left = (int) (Width * ((_Value.X - _Min.X) / _Diff.X));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > XSlide.Width - 3)
            {
                Left = XSlide.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void YSlide_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new System.Drawing.Drawing2D.LinearGradientBrush(YSlide.ClientRectangle, Color.Black,
                                                                          Color.White, 10);
            Brush Black = Brushes.Black;
            Brush White = Brushes.White;
            e.Graphics.FillRectangle(Grad, 0, 0, YSlide.Width, 20);

            double Width = YSlide.Width;
            int Left = (int) (Width * ((_Value.Y - _Min.Y) / _Diff.Y));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > YSlide.Width - 3)
            {
                Left = YSlide.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void ZSlide_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new System.Drawing.Drawing2D.LinearGradientBrush(ZSlide.ClientRectangle, Color.Black,
                                                                          Color.White, 10);
            Brush Black = Brushes.Black;
            Brush White = Brushes.White;
            e.Graphics.FillRectangle(Grad, 0, 0, ZSlide.Width, 20);

            double Width = ZSlide.Width;
            int Left = (int) (Width * ((_Value.Z - _Min.Z) / _Diff.Z));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > ZSlide.Width - 3)
            {
                Left = ZSlide.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void WSlide_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new System.Drawing.Drawing2D.LinearGradientBrush(WSlide.ClientRectangle, Color.Black,
                                                                          Color.White, 10);
            Brush Black = Brushes.Black;
            Brush White = Brushes.White;
            e.Graphics.FillRectangle(Grad, 0, 0, WSlide.Width, 20);

            double Width = WSlide.Width;
            int Left = (int) (Width * ((_Value.W - _Min.W) / _Diff.W));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > WSlide.Width - 3)
            {
                Left = WSlide.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void XSlide_MouseMove(object sender, MouseEventArgs e)
        {
            if (XDown)
            {
                float Width = XSlide.Width;
                float V = (float) e.X;
                if (V > Width)
                {
                    V = Width;
                }
                if (V < 0.0f)
                {
                    V = 0.0f;
                }

                XSlide.Invalidate();

                float Perc = V / Width;
                _Value.X = (Perc * _Diff.X) + _Min.X;
                XLabel.Text = _Value.X.ToString();
            }
        }

        private void YSlide_MouseMove(object sender, MouseEventArgs e)
        {
            if (YDown)
            {
                float Width = YSlide.Width;
                float V = (float) e.X;
                if (V > Width)
                {
                    V = Width;
                }
                if (V < 0.0f)
                {
                    V = 0.0f;
                }

                YSlide.Invalidate();

                float Perc = V / Width;
                _Value.Y = (Perc * _Diff.Y) + _Min.Y;
                YLabel.Text = _Value.Y.ToString();
            }
        }

        private void ZSlide_MouseMove(object sender, MouseEventArgs e)
        {
            if (ZDown)
            {
                float Width = ZSlide.Width;
                float V = (float) e.X;
                if (V > Width)
                {
                    V = Width;
                }
                if (V < 0.0f)
                {
                    V = 0.0f;
                }

                ZSlide.Invalidate();

                float Perc = V / Width;
                _Value.Z = (Perc * _Diff.Z) + _Min.Z;
                ZLabel.Text = _Value.Z.ToString();
            }
        }

        private void WSlide_MouseMove(object sender, MouseEventArgs e)
        {
            if (WDown)
            {
                float Width = WSlide.Width;
                float V = (float) e.X;
                if (V > Width)
                {
                    V = Width;
                }
                if (V < 0.0f)
                {
                    V = 0.0f;
                }

                WSlide.Invalidate();

                float Perc = V / Width;
                _Value.W = (Perc * _Diff.W) + _Min.W;
                WLabel.Text = _Value.W.ToString();
            }
        }
    }
}