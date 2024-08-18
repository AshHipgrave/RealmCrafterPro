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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace RCTPlugin
{
    public partial class VariableHeight : UserControl
    {
        public event EventHandler ValueChanged;

        protected double _Low, _High, _Min, _Max;
        protected bool _MouseDown = false, _HighDown = false, _LowDown = false;

        public VariableHeight()
        {
            InitializeComponent();

            _Min = -100.0;
            _Max = 1000.0;
            _Low = -100.0;
            _High = 1000.0;
        }

        public int Min
        {
            get { return Convert.ToInt32(_Min); }
            set
            {
                bool Refit = false;
                _Min = Convert.ToDouble(value);

                if (_Low < _Min)
                {
                    Refit = true;
                    _Low = _Min;
                }
                if (_High < _Min)
                {
                    Refit = true;
                    _High = _Min;
                }

                if (Refit)
                    ValueChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        public int Max
        {
            get { return Convert.ToInt32(_Max); }
            set
            {
                bool Refit = false;
                _Max = Convert.ToDouble(value);
                if (_High > _Max)
                {
                    Refit = true;
                    _High = _Max;
                }
                if (_Low > _Max)
                {
                    Refit = true;
                    _Low = _Max;
                }

                if (Refit)
                    ValueChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        public int High
        {
            get { return Convert.ToInt32(_High); }
            set
            {
                bool Refit = false;
                _High = Convert.ToDouble(value);

                if (_High > _Max)
                {
                    Refit = true;
                    _High = _Max;
                }
                if (_High < _Min)
                {
                    Refit = true;
                    _High = _Min;
                }
                if (_High < _Low)
                {
                    Refit = true;
                    _Low = _High;
                }

                if (Refit)
                    ValueChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        public int Low
        {
            get { return Convert.ToInt32(_Low); }
            set
            {
                bool Refit = false;
                _Low = Convert.ToDouble(value);

                if (_Low > _Max)
                {
                    Refit = true;
                    _Low = _Max;
                }
                if (_Low < _Min)
                {
                    Refit = true;
                    _Low = _Min;
                }
                if (_High < _Low)
                {
                    Refit = true;
                    _Low = _High;
                }

                if (Refit)
                    ValueChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        protected void GetRectangles(ref Rectangle HighSelection, ref Rectangle LowSelection)
        {
            double DrawHeight = Convert.ToDouble(ClientRectangle.Height - 20);
            double One = DrawHeight / (_Max - _Min);

            Rectangle TopClip = new Rectangle(30, 10, 24, 0);
            Rectangle BottomClip = new Rectangle(30, ClientRectangle.Height - 20, 24, 0);
            Rectangle Middle = new Rectangle(30, 10, 24, ClientRectangle.Height - 20);

            //if (_Max > _High)
            TopClip = new Rectangle(30, 10, 24, Convert.ToInt32(One * (_Max - _High)));

            //if (_Min < _Low)
            BottomClip = new Rectangle(30, (ClientRectangle.Height - 10) - Convert.ToInt32(One * (_Low - _Min)), 24, Convert.ToInt32(One * (_Low - _Min)));

            //if (_Low != _High)
            Middle = new Rectangle(30, TopClip.Height + 10, 24, (BottomClip.Y - 10) - TopClip.Height);

            HighSelection = new Rectangle(10, Middle.Y - 5, 10, 10);
            LowSelection = new Rectangle(10, (Middle.Y + Middle.Height) - 5, 10, 10);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Rectangle HighSelection = Rectangle.Empty;
            Rectangle LowSelection = Rectangle.Empty;
            GetRectangles(ref HighSelection, ref LowSelection);

            if (e.X > HighSelection.X && e.X < HighSelection.X + HighSelection.Width && e.Y > HighSelection.Y && e.Y < HighSelection.Y + HighSelection.Height)
                _HighDown = true;
            else if (e.X > LowSelection.X && e.X < LowSelection.X + LowSelection.Width && e.Y > LowSelection.Y && e.Y < LowSelection.Y + LowSelection.Height)
                _LowDown = true;

            if (_HighDown || _LowDown)
                Invalidate();
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_HighDown || _LowDown)
                ValueChanged(this, EventArgs.Empty);

            _HighDown = false;
            _LowDown = false;
            Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            double DrawHeight = Convert.ToDouble(ClientRectangle.Height - 20);
            double One = DrawHeight / (_Max - _Min);

            Rectangle TopClip = new Rectangle(30, 10, 24, 0);
            Rectangle BottomClip = new Rectangle(30, ClientRectangle.Height - 20, 24, 0);
            Rectangle Middle = new Rectangle(30, 10, 24, ClientRectangle.Height - 20);

            //if (_Max > _High)
            //    TopClip = new Rectangle(30, 10, 24, Convert.ToInt32(One * _Max - _High));

            //if (_Min < _Low)
            //    BottomClip = new Rectangle(30, (ClientRectangle.Height - 10) - Convert.ToInt32(One * _Low - _Min), 24, Convert.ToInt32(One * _Low - _Min));

            //if (_Low != _High)
            //    Middle = new Rectangle(30, TopClip.Height + 10, 24, BottomClip.Y - TopClip.Height);

            double Y = Convert.ToDouble(e.Y);
            Y -= 10.0;
            Y /= One;
            Y = (_Max - Y);// +_Min;

            if (_HighDown)
            {
                if (Y >= _Min && Y <= _Max)
                {
                    if (Y <= _Low)
                    {
                        _Low = Y;
                        if (_Low < _Min)
                            _Low = _Min;
                    }
                    _High = Y;
                }
            }

            if (_LowDown)
            {
                if (Y >= _Min && Y <= _Max)
                {
                    if (Y >= _High)
                    {
                        _High = Y;
                        if (_High > _Max)
                            _High = _Max;
                    }
                    _Low = Y;
                }
            }

            //if(_HighDown || _LowDown)
            //    ValueChanged(this, EventArgs.Empty);

            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Brushes.White, ClientRectangle);

            double DrawHeight = Convert.ToDouble(ClientRectangle.Height - 20);
            double One = DrawHeight / (_Max - _Min);

            Brush ValidArea = new SolidBrush(Color.FromArgb(71, 255, 71));
            Brush InvalidArea = new SolidBrush(Color.FromArgb(255, 71, 71));


            Rectangle TopClip = new Rectangle(30, 10, 24, 0);
            Rectangle BottomClip = new Rectangle(30, ClientRectangle.Height - 20, 24, 0);
            Rectangle Middle = new Rectangle(30, 10, 24, ClientRectangle.Height - 20);

            //if (_Max > _High)
            {
                TopClip = new Rectangle(30, 10, 24, Convert.ToInt32(One * (_Max - _High)));
                e.Graphics.FillRectangle(InvalidArea, TopClip);
            }

            //if (_Min < _Low)
            {
                BottomClip = new Rectangle(30, (ClientRectangle.Height - 10) - Convert.ToInt32(One * (_Low - _Min)), 24, Convert.ToInt32(One * (_Low - _Min)));
                e.Graphics.FillRectangle(InvalidArea, BottomClip);
            }

            //if (_Low != _High)
            {
                Middle = new Rectangle(30, TopClip.Height + 10, 24, (BottomClip.Y - 10) - TopClip.Height);
                e.Graphics.FillRectangle(ValidArea, Middle);
            }

            e.Graphics.DrawLine(Pens.Black, new Point(15, Middle.Y), new Point(29, Middle.Y));
            e.Graphics.DrawLine(Pens.Black, new Point(15, Middle.Y + Middle.Height), new Point(29, Middle.Y + Middle.Height));

            Rectangle HighSelection = new Rectangle(10, Middle.Y - 5, 10, 10);
            Rectangle LowSelection = new Rectangle(10, (Middle.Y + Middle.Height) - 5, 10, 10);

            Brush HighUp = new System.Drawing.Drawing2D.LinearGradientBrush(HighSelection, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 45.0f);
            Brush LowUp = new System.Drawing.Drawing2D.LinearGradientBrush(LowSelection, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 45.0f);
            Brush HighDown = new System.Drawing.Drawing2D.LinearGradientBrush(HighSelection, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 225.0f);
            Brush LowDown = new System.Drawing.Drawing2D.LinearGradientBrush(LowSelection, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 225.0f);

            e.Graphics.FillEllipse(_HighDown ? HighDown : HighUp, HighSelection);
            e.Graphics.FillEllipse(_LowDown ? LowDown : LowUp, LowSelection);

            Rectangle ClientRectangleMinus = ClientRectangle;
            ClientRectangleMinus.Width = ClientRectangleMinus.Width - 1;
            ClientRectangleMinus.Height = ClientRectangleMinus.Height - 1;
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(51, 153, 255)), ClientRectangleMinus);

        }
    }
}
