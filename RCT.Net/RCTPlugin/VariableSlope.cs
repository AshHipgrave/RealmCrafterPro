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
    public partial class VariableSlope : UserControl
    {
        public event EventHandler ValueChanged;

        protected double _Min, _Max;
        protected bool _MouseDown = false, _DownMinS = false, _DownMaxS = false, _DownMinL = false, _DownMaxL = false;

        public VariableSlope()
        {
            InitializeComponent();

            ValueChanged = new EventHandler(VariableSlope_ValueChanged);

            _Min = 0.3;
            _Max = 0.7;


        }

        private void VariableSlope_ValueChanged(object sender, EventArgs e)
        {
        }

        public double Min
        {
            get { return 1.0 - _Min; }
            set
            {
                bool Refit = false;

                _Min = 1.0 - value;
                if (_Min > 1.0)
                {
                    Refit = true;
                    _Min = 1.0;
                }
                if (_Min < 0.0)
                {
                    Refit = true;
                    _Min = 0.0;
                }
                if (_Min > _Max)
                {
                    Refit = true;
                    _Max = _Min;
                }

                if(Refit)
                    ValueChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        public double Max
        {
            get { return 1.0 - _Max; }
            set
            {
                bool Refit = false;
                _Max = 1.0 - value;

                if (_Max > 1.0)
                {
                    Refit = true;
                    _Max = 1.0;
                }
                if (_Max < 0.0)
                {
                    Refit = true;
                    _Max = 0.0;
                }
                if (_Max < _Min)
                {
                    Refit = true;
                    _Min = _Max;
                }

                if (Refit)
                    ValueChanged(this, EventArgs.Empty);

                Invalidate();
            }
        }

        protected void BuildRectangles(ref Rectangle MinMS, ref Rectangle MaxMS, ref Rectangle MinML, ref Rectangle MaxML)
        {
            double InnerWidth = Convert.ToSingle(ClientRectangle.Width - 40);
            double InnerHeight = Convert.ToSingle(ClientRectangle.Height - 30);
            InnerWidth = 180.0 / InnerWidth;

            double MinACos = (Math.PI * 0.5) * _Min;
            double MinDegAngle = MinACos * (180.0 / Math.PI);

            double MaxACos = (Math.PI * 0.5) * _Max;
            double MaxDegAngle = MaxACos * (180.0 / Math.PI);

            double MinRadAngle = MinDegAngle * (Math.PI / 180.0);
            double MinCosS = Math.Cos(MinRadAngle);
            double MinSY = (MinCosS * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MinP = new Point(10, Convert.ToInt32(MinSY));
            Point MinPm = new Point(ClientRectangle.Width - 20, MinP.Y);

            double MaxRadAngle = MaxDegAngle * (Math.PI / 180.0);
            double MaxCosS = Math.Cos(MaxRadAngle);
            double MaxSY = (MaxCosS * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MaxP = new Point(10, Convert.ToInt32(MaxSY));
            Point MaxPm = new Point(ClientRectangle.Width - 20, MaxP.Y);

            double MinRadAngleL = ((90.0 - MinDegAngle) + 90.0) * (Math.PI / 180.0);
            double MinCosL = Math.Cos(MinRadAngleL);
            double MinLY = (MinCosL * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MinL = new Point(10, Convert.ToInt32(MinLY));
            Point MinLm = new Point(ClientRectangle.Width - 20, MinL.Y);

            double MaxRadAngleL = ((90.0 - MaxDegAngle) + 90.0) * (Math.PI / 180.0);
            double MaxCosL = Math.Cos(MaxRadAngleL);
            double MaxLY = (MaxCosL * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MaxL = new Point(10, Convert.ToInt32(MaxLY));
            Point MaxLm = new Point(ClientRectangle.Width - 20, MaxL.Y);

            MinMS = new Rectangle(10, MinP.Y - 5, 10, 10);
            MaxMS = new Rectangle(10, MaxP.Y - 5, 10, 10);
            MinML = new Rectangle(10, MinL.Y - 5, 10, 10);
            MaxML = new Rectangle(10, MaxL.Y - 5, 10, 10);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            Rectangle MinMS = Rectangle.Empty, MaxMS = Rectangle.Empty, MinML = Rectangle.Empty, MaxML = Rectangle.Empty;
            BuildRectangles(ref MinMS, ref MaxMS, ref MinML, ref MaxML);

            if (e.X > MinMS.X && e.X < MinMS.X + MinMS.Width && e.Y > MinMS.Y && e.Y < MinMS.Y + MinMS.Height)
                _DownMinS = true;
            else if (e.X > MaxMS.X && e.X < MaxMS.X + MaxMS.Width && e.Y > MaxMS.Y && e.Y < MaxMS.Y + MaxMS.Height)
                _DownMaxS = true;
            else if (e.X > MinML.X && e.X < MinML.X + MinML.Width && e.Y > MinML.Y && e.Y < MinML.Y + MinML.Height)
                _DownMinL = true;
            else if (e.X > MaxML.X && e.X < MaxML.X + MaxML.Width && e.Y > MaxML.Y && e.Y < MaxML.Y + MaxML.Height)
                _DownMaxL = true;

            if (_DownMinS || _DownMaxS || _DownMinL || _DownMaxL)
                this.Invalidate();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            double InnerHeight = Convert.ToSingle(ClientRectangle.Height - 30);

            if (_DownMinS)
            {
                double iMinCosS = (Convert.ToDouble(e.Y) - (InnerHeight * 0.5 + 15.0)) / (InnerHeight * 0.5);
                double iAngle = Math.Acos(iMinCosS);
                iAngle = iAngle / (Math.PI * 0.5);

                if (iMinCosS > 1.0)
                    iAngle = 0.0;

                if (iAngle < 0.0 || iAngle > 1.0 || double.IsNaN(iAngle))
                    return;
                if (iAngle >= _Max)
                {
                    _Max = iAngle;
                    iAngle -= 0.01;
                }

                _Min = iAngle;
                //System.Diagnostics.Trace.WriteLine("Slider: (" + _Min.ToString() + ", " + _Max.ToString() + ")");
                //ValueChanged(this, EventArgs.Empty);
                Invalidate();
            }

            if (_DownMaxS)
            {
                double iMaxCosS = (Convert.ToDouble(e.Y) - (InnerHeight * 0.5 + 15.0)) / (InnerHeight * 0.5);
                double iAngle = Math.Acos(iMaxCosS);

                iAngle = iAngle / (Math.PI * 0.5);

                if (iAngle < 0.0 || double.IsNaN(iAngle))
                    return;
                if (iAngle <= _Min)
                {
                    _Min = iAngle;
                    iAngle += 0.01;
                }
                if (iAngle > 1.0f)
                    iAngle = 1.0f;

                _Max = iAngle;
                //System.Diagnostics.Trace.WriteLine("Slider: (" + _Min.ToString() + ", " + _Max.ToString() + ")");
                //ValueChanged(this, EventArgs.Empty);
                Invalidate();
            }

            if (_DownMinL)
            {
                double iMinCosL = (Convert.ToDouble(e.Y) - (InnerHeight * 0.5 + 15.0)) / (InnerHeight * 0.5);
                double iAngle = Math.Acos(iMinCosL);
                
                iAngle -= (Math.PI / 2.0);
                iAngle = ((Math.PI / 2.0) - iAngle);
                iAngle = iAngle / (Math.PI * 0.5);

                //System.Diagnostics.Trace.WriteLine(iMinCosL.ToString() + ", " + iAngle.ToString());

                if (iMinCosL < -1.0)
                    iAngle = 0.0;

                if (iAngle < 0.0 || iAngle > 1.0 || double.IsNaN(iAngle))
                    return;
                if (iAngle >= _Max)
                {
                    _Max = iAngle;
                    iAngle -= 0.01;
                }

                _Min = iAngle;
                //System.Diagnostics.Trace.WriteLine("Slider: (" + _Min.ToString() + ", " + _Max.ToString() + ")");
                //ValueChanged(this, EventArgs.Empty);
                Invalidate();
            }

            if (_DownMaxL)
            {
                double iMaxCosL = (Convert.ToDouble(e.Y) - (InnerHeight * 0.5 + 15.0)) / (InnerHeight * 0.5);
                double iAngle = Math.Acos(iMaxCosL);
                iAngle -= (Math.PI / 2.0);
                iAngle = ((Math.PI / 2.0) - iAngle);
                iAngle = iAngle / (Math.PI * 0.5);

                if (iAngle < 0.0 || double.IsNaN(iAngle))
                    return;
                if (iAngle <= _Min)
                {
                    _Min = iAngle;
                    iAngle += 0.01;
                }

                if (iAngle > 1.0f)
                    iAngle = 1.0f;

                _Max = iAngle;
                //System.Diagnostics.Trace.WriteLine("Slider: (" + _Min.ToString() + ", " + _Max.ToString() + ")");
                //ValueChanged(this, EventArgs.Empty);
                Invalidate();
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);

            if (_DownMinS || _DownMaxS || _DownMinL || _DownMaxL)
                ValueChanged(this, EventArgs.Empty);

            _DownMinS = _DownMaxS = _DownMinL = _DownMaxL = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Brushes.White, ClientRectangle);

            double InnerWidth = Convert.ToSingle(ClientRectangle.Width - 40);
            double InnerHeight = Convert.ToSingle(ClientRectangle.Height - 30);
            InnerWidth = 180.0 / InnerWidth;
            double InnerWidthCounter = 0.0f;

            double MinACos = (Math.PI * 0.5) * _Min;
            double MinDegAngle = MinACos * (180.0 / Math.PI);

            double MaxACos = (Math.PI * 0.5) * _Max;
            double MaxDegAngle = MaxACos * (180.0 / Math.PI);

            double MinRadAngle = MinDegAngle * (Math.PI / 180.0);
            double MinCosS = Math.Cos(MinRadAngle);
            double MinSY = (MinCosS * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MinP = new Point(12, Convert.ToInt32(MinSY));
            Point MinPm = new Point(ClientRectangle.Width - 20, MinP.Y);

            double MaxRadAngle = MaxDegAngle * (Math.PI / 180.0);
            double MaxCosS = Math.Cos(MaxRadAngle);
            double MaxSY = (MaxCosS * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MaxP = new Point(12, Convert.ToInt32(MaxSY));
            Point MaxPm = new Point(ClientRectangle.Width - 20, MaxP.Y);

            double MinRadAngleL = ((90.0 - MinDegAngle) + 90.0) * (Math.PI / 180.0);
            double MinCosL = Math.Cos(MinRadAngleL);
            double MinLY = (MinCosL * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MinL = new Point(12, Convert.ToInt32(MinLY));
            Point MinLm = new Point(ClientRectangle.Width - 20, MinL.Y);

            double MaxRadAngleL = ((90.0 - MaxDegAngle) + 90.0) * (Math.PI / 180.0);
            double MaxCosL = Math.Cos(MaxRadAngleL);
            double MaxLY = (MaxCosL * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0);
            Point MaxL = new Point(12, Convert.ToInt32(MaxLY));
            Point MaxLm = new Point(ClientRectangle.Width - 20, MaxL.Y);

            e.Graphics.DrawLine(Pens.Black, MinP, MinPm);
            e.Graphics.DrawLine(Pens.Black, MinL, MinLm);
            e.Graphics.DrawLine(Pens.Black, MaxP, MaxPm);
            e.Graphics.DrawLine(Pens.Black, MaxL, MaxLm);

            Pen ValidArea = new Pen(Color.FromArgb(71, 255, 71));
            Pen InvalidArea = new Pen(Color.FromArgb(255, 71, 71));

            for (double x = 20.0; x < Convert.ToDouble(ClientRectangle.Width - 20); x += 0.2)
            {
                double Angle = InnerWidthCounter * (Math.PI / 180.0);
                double Cos = Math.Cos(Angle);

                Pen DrawLine = InvalidArea;

                double RealAngle = (InnerWidthCounter > 90.0 ? 90.0 - (InnerWidthCounter - 90.0) : InnerWidthCounter);
                if (RealAngle >= MinDegAngle && RealAngle <= MaxDegAngle)
                    DrawLine = ValidArea;


                Point Ps = new Point(Convert.ToInt32(x), Convert.ToInt32(
                        (Cos * (InnerHeight * 0.5)) + (InnerHeight * 0.5 + 15.0)
                        ));

                e.Graphics.DrawLine(DrawLine,
                    Ps,
                    new Point(ClientRectangle.Width - 15, Ps.Y));

                InnerWidthCounter += (InnerWidth * 0.2);
            }

            Rectangle MinMS = new Rectangle(10, MinP.Y - 5, 10, 10);
            Rectangle MaxMS = new Rectangle(10, MaxP.Y - 5, 10, 10);
            Rectangle MinML = new Rectangle(10, MinL.Y - 5, 10, 10);
            Rectangle MaxML = new Rectangle(10, MaxL.Y - 5, 10, 10);

            Brush MinBS = new System.Drawing.Drawing2D.LinearGradientBrush(MinMS, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 45.0f);
            Brush MaxBS = new System.Drawing.Drawing2D.LinearGradientBrush(MaxMS, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 45.0f);
            Brush MinBL = new System.Drawing.Drawing2D.LinearGradientBrush(MinML, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 45.0f);
            Brush MaxBL = new System.Drawing.Drawing2D.LinearGradientBrush(MaxML, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 45.0f);

            Brush MinDS = new System.Drawing.Drawing2D.LinearGradientBrush(MinMS, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 225.0f);
            Brush MaxDS = new System.Drawing.Drawing2D.LinearGradientBrush(MaxMS, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 225.0f);
            Brush MinDL = new System.Drawing.Drawing2D.LinearGradientBrush(MinML, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 225.0f);
            Brush MaxDL = new System.Drawing.Drawing2D.LinearGradientBrush(MaxML, Color.FromArgb(255, 255, 255), Color.FromArgb(17, 47, 145), 225.0f);


            e.Graphics.FillEllipse(_DownMinS ? MinDS : MinBS, MinMS);
            e.Graphics.FillEllipse(_DownMaxS ? MaxDS : MaxBS, MaxMS);
            e.Graphics.FillEllipse(_DownMinL ? MinDL : MinBL, MinML);
            e.Graphics.FillEllipse(_DownMaxL ? MaxDL : MaxBL, MaxML);

            Pen BlueLine = new Pen(Brushes.RoyalBlue);
            BlueLine.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
            BlueLine.DashPattern = new float[] { 4.0f, 4.0f };

            e.Graphics.DrawLine(BlueLine,
                new Point(10, Convert.ToInt32((InnerHeight * 0.5 + 15.0))),
                new Point(ClientRectangle.Width / 2, Convert.ToInt32((InnerHeight * 0.5 + 15.0)))
            );

            Rectangle ClientRectangleMinus = ClientRectangle;
            ClientRectangleMinus.Width = ClientRectangleMinus.Width - 1;
            ClientRectangleMinus.Height = ClientRectangleMinus.Height - 1;
            e.Graphics.DrawRectangle(new Pen(Color.FromArgb(51, 153, 255)), ClientRectangleMinus);
        }
    }
}
