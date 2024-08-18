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
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing.Design;

namespace RenderingServices
{
    public partial class ColorEditorForm : Form
    {
        public object _SetColor;
        public Vector4 ActualColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

        private Color ChosenScale = Color.Red;
        private double Hue = 1;
        private double Saturation = 0.0;
        private double Value = 100.0;
        private double Red = 255.0, Green = 255.0, Blue = 255.0;
        private double Alpha = 255.0;
        private bool HueSelecting = false;
        private bool SatSelecting = false;
        private bool ValSelecting = false;
        private bool AlpSelecting = false;

        public ColorEditorForm()
        {
            InitializeComponent();
            TopLevel = false;
        }

        public object SelectedColor
        {
            get
            {
                if (_SetColor is Vector3)
                {
                    return new Vector3((float) Red / 255.0f, (float) Green / 255.0f, (float) Blue / 255.0f);
                }
                if (_SetColor is Vector4)
                {
                    return new Vector4((float) Red / 255.0f, (float) Green / 255.0f, (float) Blue / 255.0f,
                                       (float) Alpha / 255.0f);
                }
                if (_SetColor is Color)
                {
                    return System.Drawing.Color.FromArgb(Convert.ToInt32(Alpha * 255.0f), Convert.ToInt32(Red),
                                                         Convert.ToInt32(Green), Convert.ToInt32(Blue));
                }
                return null;
            }
            set
            {
                _SetColor = value;

                Vector4 SetColor = new Vector4();

                if (value is Vector3)
                {
                    SetColor = new Vector4(((Vector3) value).X, ((Vector3) value).Y, ((Vector3) value).Z, 1.0f);
                }
                if (value is Vector4)
                {
                    SetColor = new Vector4(((Vector4) value).X, ((Vector4) value).Y, ((Vector4) value).Z,
                                           ((Vector4) value).W);
                }
                if (value is Color)
                {
                    SetColor = new Vector4(Convert.ToSingle(((Color) value).R) / 255.0f,
                                           Convert.ToSingle(((Color) value).G) / 255.0f,
                                           Convert.ToSingle(((Color) value).B) / 255.0f,
                                           Convert.ToSingle(((Color) value).A) / 255.0f);
                }

                if (SetColor.X > 1.0f)
                {
                    SetColor.X = 1.0f;
                }
                if (SetColor.Y > 1.0f)
                {
                    SetColor.Y = 1.0f;
                }
                if (SetColor.Z > 1.0f)
                {
                    SetColor.Z = 1.0f;
                }
                if (SetColor.W > 1.0f)
                {
                    SetColor.W = 1.0f;
                }

                if (SetColor.X < 0.0f)
                {
                    SetColor.X = 0.0f;
                }
                if (SetColor.Y < 0.0f)
                {
                    SetColor.Y = 0.0f;
                }
                if (SetColor.Z < 0.0f)
                {
                    SetColor.Z = 0.0f;
                }
                if (SetColor.W < 0.0f)
                {
                    SetColor.W = 0.0f;
                }

                Red = (float) SetColor.X * 255.0;
                Green = (float) SetColor.Y * 255.0;
                Blue = (float) SetColor.Z * 255.0;
                Alpha = (float) SetColor.W * 255.0f;

                RBox.Text = ((int) Red).ToString();
                GBox.Text = ((int) Green).ToString();
                BBox.Text = ((int) Blue).ToString();

                ReCalcHSV();
            }
        }

        private void HPick_Paint(object sender, PaintEventArgs e)
        {
            float y = 0;

            for (int c = 0; c < 255; c += 6)
            {
                e.Graphics.FillRectangle(
                    new System.Drawing.Drawing2D.LinearGradientBrush(HPick.ClientRectangle, Color.FromArgb(255, c, 0),
                                                                     Color.FromArgb(255, c, 0), 10), y, 0, 1, 48);
                y += 0.5f;
            }

            for (int c = 255; c > 0; c -= 6)
            {
                e.Graphics.FillRectangle(
                    new System.Drawing.Drawing2D.LinearGradientBrush(HPick.ClientRectangle, Color.FromArgb(c, 255, 0),
                                                                     Color.FromArgb(c, 255, 0), 10), y, 0, 1, 48);
                y += 0.5f;
            }

            for (int c = 0; c < 255; c += 6)
            {
                e.Graphics.FillRectangle(
                    new System.Drawing.Drawing2D.LinearGradientBrush(HPick.ClientRectangle, Color.FromArgb(0, 255, c),
                                                                     Color.FromArgb(0, 255, c), 10), y, 0, 1, 48);
                y += 0.5f;
            }

            for (int c = 255; c > 0; c -= 6)
            {
                e.Graphics.FillRectangle(
                    new System.Drawing.Drawing2D.LinearGradientBrush(HPick.ClientRectangle, Color.FromArgb(0, c, 255),
                                                                     Color.FromArgb(0, c, 255), 10), y, 0, 1, 48);
                y += 0.5f;
            }

            for (int c = 0; c < 255; c += 6)
            {
                e.Graphics.FillRectangle(
                    new System.Drawing.Drawing2D.LinearGradientBrush(HPick.ClientRectangle, Color.FromArgb(c, 0, 255),
                                                                     Color.FromArgb(c, 0, 255), 10), y, 0, 1, 48);
                y += 0.5f;
            }

            for (int c = 255; c > 0; c -= 6)
            {
                e.Graphics.FillRectangle(
                    new System.Drawing.Drawing2D.LinearGradientBrush(HPick.ClientRectangle, Color.FromArgb(255, 0, c),
                                                                     Color.FromArgb(255, 0, c), 10), y, 0, 1, 48);
                y += 0.5f;
            }

            Brush Black = Brushes.Black;
            Brush White = Brushes.White;

            double Width = HPick.Width;
            int Left = (int) (Width * (Hue / 360.0f));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > HPick.Width - 3)
            {
                Left = HPick.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void HPick_MouseDown(object sender, MouseEventArgs e)
        {
            HueSelecting = true;
        }

        private void HPick_MouseMove(object sender, MouseEventArgs e)
        {
            if (HueSelecting)
            {
                double Width = (double) HPick.Width;
                Hue = (double) e.X;
                if (Hue > Width)
                {
                    Hue = Width;
                }
                if (Hue < 1)
                {
                    Hue = 1;
                }

                HPick.Invalidate();
                SPick.Invalidate();
                BPick.Invalidate();

                double Perc = (double) Hue / Width;
                Hue = Perc * 360.0;
                if (Hue >= 360.0f)
                {
                    Hue = 359.0;
                }

                double r = 0, g = 0, b = 0;
                HSVtoRGB(Hue, 1.0f, 255.0f, ref r, ref g, ref b);

                if (r > 255)
                {
                    r = 255.0f;
                }
                if (g > 255)
                {
                    g = 255.0f;
                }
                if (b > 255)
                {
                    b = 255.0f;
                }

                ChosenScale = Color.FromArgb((int) r, (int) g, (int) b);
                ReCalcRGB();
            }
        }

        private void HPick_MouseUp(object sender, MouseEventArgs e)
        {
            HueSelecting = false;
        }

        private void SPick_MouseDown(object sender, MouseEventArgs e)
        {
            SatSelecting = true;
        }

        private void SPick_MouseMove(object sender, MouseEventArgs e)
        {
            if (SatSelecting)
            {
                double Width = SPick.Width;
                Saturation = (double) e.X;
                if (Saturation > Width)
                {
                    Saturation = Width;
                }
                if (Saturation < 1)
                {
                    Saturation = 1.0;
                }

                SPick.Invalidate();
                double Perc = (double) Saturation / Width;
                Saturation = (Perc * 100.0f);
                ReCalcRGB();
            }
        }

        private void SPick_MouseUp(object sender, MouseEventArgs e)
        {
            SatSelecting = false;
        }

        private void SPick_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new System.Drawing.Drawing2D.LinearGradientBrush(SPick.ClientRectangle, Color.White,
                                                                          ChosenScale, 10);
            Brush Black = Brushes.Black;
            Brush White = Brushes.White;
            e.Graphics.FillRectangle(Grad, 0, 0, SPick.Width, 20);

            double Width = SPick.Width;
            int Left = (int) (Width * (Saturation / 100.0f));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > SPick.Width - 3)
            {
                Left = SPick.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void BPick_MouseDown(object sender, MouseEventArgs e)
        {
            ValSelecting = true;
        }

        private void BPick_MouseMove(object sender, MouseEventArgs e)
        {
            if (ValSelecting)
            {
                double Width = BPick.Width;
                Value = (double) e.X;
                if (Value > Width)
                {
                    Value = Width;
                }
                if (Value < 1)
                {
                    Value = 1.0;
                }

                BPick.Invalidate();
                double Perc = (double) Value / Width;
                Value = (Perc * 100.0f);
                ReCalcRGB();
            }
        }

        private void BPick_MouseUp(object sender, MouseEventArgs e)
        {
            ValSelecting = false;
        }

        private void BPick_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new System.Drawing.Drawing2D.LinearGradientBrush(BPick.ClientRectangle, Color.Black,
                                                                          ChosenScale, 10);
            Brush Black = Brushes.Black;
            Brush White = Brushes.White;
            e.Graphics.FillRectangle(Grad, 0, 0, BPick.Width, 20);

            if (Value > 100.0)
            {
                Value = 100.0;
            }
            else if (Value < 0.0)
            {
                Value = 0.0;
            }

            double Width = BPick.Width;
            int Left = (int) (Width * (Value / 100.0f));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > BPick.Width - 3)
            {
                Left = BPick.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void RGBtoHSV2(double Red, double Green, double Blue, ref double Hue, ref double Saturation,
                               ref double Value)
        {
            Red /= 255.0;
            Green /= 255.0;
            Blue /= 255.0;

            double Min = Math.Min(Math.Min(Red, Green), Blue);
            double Max = Math.Max(Math.Max(Red, Green), Blue);
            double Delta = Max - Min;

            if (Delta == 0)
            {
                Hue = 0;
                Saturation = 0;
            }
            else
            {
                Saturation = Delta / Max;

                double dR = (((Max - Red) / 6.0) + (Delta / 2.0)) / Delta;
                double dG = (((Max - Green) / 6.0) + (Delta / 2.0)) / Delta;
                double dB = (((Max - Blue) / 6.0) + (Delta / 2.0)) / Delta;

                if (Red == Max)
                {
                    Hue = dB - dG;
                }
                else if (Green == Max)
                {
                    Hue = (1.0 / 3.0) + dR - dB;
                }
                else if (Blue == Max)
                {
                    Hue = (2.0 / 3.0) + dG - dR;
                }

                if (Hue < 0.0)
                {
                    Hue += 1.0;
                }

                if (Hue > 1.0)
                {
                    Hue -= 1.0;
                }
            }
            Hue *= 360;
            Saturation *= 100;
            Value *= 2.56;
        }

        private void RGBtoHSV(double Red, double Green, double Blue, ref double Hue, ref double Saturation,
                              ref double Value)
        {
            double Min, Max, Delta;

            Min = Math.Min(Red, Math.Min(Green, Blue));
            Max = Math.Max(Red, Math.Max(Green, Blue));
            Value = Max; // v

            Delta = Max - Min;

            if (Delta > 0.0f)
            {
                Saturation = Delta / Max; // s
            }
            else
            {
                Saturation = 0.0f;
                Hue = -1.0f;
                return;
            }

            if (Red == Max)
            {
                Hue = (Green - Blue) / Delta; // between yellow & magenta
            }
            else if (Green == Max)
            {
                Hue = 2.0f + (Blue - Red) / Delta; // between cyan & yellow
            }
            else
            {
                Hue = 4.0f + (Red - Green) / Delta; // between magenta & cyan
            }

            Hue *= 60.0f; // degrees
            if (Hue < 0.0f)
            {
                Hue += 360;
            }
        }

        private void HSVtoRGB(double Hue, double Saturation, double Value, ref double Red, ref double Green,
                              ref double Blue)
        {
            int i;
            double f, p, q, t;

            if (Saturation == 0)
            {
                // achromatic (grey)
                Red = Green = Blue = Value;
                return;
            }

            Hue /= 60.0f; // sector 0 to 5
            i = (int) Math.Floor(Hue);
            f = Hue - i; // factorial part of h
            p = Value * (1.0f - Saturation);
            q = Value * (1.0f - Saturation * f);
            t = Value * (1.0f - Saturation * (1.0f - f));

            switch (i)
            {
                case 0:
                    Red = Value;
                    Green = t;
                    Blue = p;
                    break;
                case 1:
                    Red = q;
                    Green = Value;
                    Blue = p;
                    break;
                case 2:
                    Red = p;
                    Green = Value;
                    Blue = t;
                    break;
                case 3:
                    Red = p;
                    Green = q;
                    Blue = Value;
                    break;
                case 4:
                    Red = t;
                    Green = p;
                    Blue = Value;
                    break;
                default: // case 5:
                    Red = Value;
                    Green = p;
                    Blue = q;
                    break;
            }
        }

        private void ReCalcRGB()
        {
            double r = 0, g = 0, b = 0;

            HSVtoRGB(Hue, Saturation / 100.0, (Value * 2.56), ref r, ref g, ref b);

            if (r > 255)
            {
                r = 255;
            }
            if (g > 255)
            {
                g = 255;
            }
            if (b > 255)
            {
                b = 255;
            }

            if (r < 0)
            {
                r = 0;
            }
            if (g < 0)
            {
                g = 0;
            }
            if (b < 0)
            {
                b = 0;
            }

            Red = r;
            Green = g;
            Blue = b;

            RBox.Text = ((int) Red).ToString();
            GBox.Text = ((int) Green).ToString();
            BBox.Text = ((int) Blue).ToString();

            NewColor.BackColor = Color.FromArgb((int) r, (int) g, (int) b);
            OldColor.BackColor = Color.FromArgb((int) r, (int) g, (int) b);
        }

        private void ReCalcHSV()
        {
            RGBtoHSV2(Red, Green, Blue, ref Hue, ref Saturation, ref Value);
            double Val = Value / 2.56f;
            if (Hue < 0.0f)
            {
                Hue = 0.0f;
            }

            double r = 0, g = 0, b = 0;
            HSVtoRGB(Hue, 1.0f, 255.0f, ref r, ref g, ref b);

            if (r > 255)
            {
                r = 255.0f;
            }
            if (g > 255)
            {
                g = 255.0f;
            }
            if (b > 255)
            {
                b = 255.0f;
            }

            ChosenScale = Color.FromArgb((int) r, (int) g, (int) b);

            HPick.Invalidate();
            SPick.Invalidate();
            BPick.Invalidate();
            APick.Invalidate();
        }

        private void OldColor_Paint(object sender, PaintEventArgs e)
        {
            Color C = this.BackColor;

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int) Red, (int) Green, (int) Blue)),
                                     OldColor.ClientRectangle);

            //e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(255 - (int)Alpha, C.R, C.G, C.G)), OldColor.ClientRectangle);
        }

        private void APick_MouseDown(object sender, MouseEventArgs e)
        {
            AlpSelecting = true;
        }

        private void APick_MouseMove(object sender, MouseEventArgs e)
        {
            if (AlpSelecting)
            {
                double Width = APick.Width;
                Alpha = (double) e.X;
                if (Alpha > Width)
                {
                    Alpha = Width;
                }
                if (Alpha < 1)
                {
                    Alpha = 1.0;
                }

                APick.Invalidate();
                OldColor.Invalidate();

                double Perc = (double) Alpha / Width;
                Alpha = (Perc * 255.0f);
                ReCalcRGB();
            }
        }

        private void APick_MouseUp(object sender, MouseEventArgs e)
        {
            AlpSelecting = false;
        }

        private void APick_Paint(object sender, PaintEventArgs e)
        {
            Brush Grad = new System.Drawing.Drawing2D.LinearGradientBrush(BPick.ClientRectangle, Color.Black,
                                                                          Color.White, 10);
            Brush Black = Brushes.Black;
            Brush White = Brushes.White;
            e.Graphics.FillRectangle(Grad, 0, 0, 256, 20);

            double Width = APick.Width;
            int Left = (int) (Width * (Alpha / 255.0f));

            if (Left == 0)
            {
                Left = 1;
            }
            if (Left > APick.Width - 3)
            {
                Left = APick.Width - 3;
            }

            e.Graphics.FillRectangle(Black, Left - 1, 0, 4, 12);
            e.Graphics.FillRectangle(White, Left, 1, 2, 10);
        }

        private void NewColor_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb((int) Red, (int) Green, (int) Blue)),
                                     OldColor.ClientRectangle);
        }

        private void RBox_KeyUp(object sender, KeyEventArgs e)
        {
            Regex R = new Regex("[^0-9]");

            string Text = RBox.Text;

            if (R.Match(Text).Success)
            {
                RBox.Text = "";
                return;
            }

            if (Text.Length == 0)
            {
                return;
            }

            double r = Convert.ToDouble(Text);
            if (r < 0)
            {
                r = 0;
            }
            if (r > 255.0)
            {
                r = 255.0;
            }

            RBox.Text = ((int) r).ToString();
            Red = r;
            ReCalcHSV();
        }

        private void GBox_KeyUp(object sender, KeyEventArgs e)
        {
            Regex R = new Regex("[^0-9]");

            string Text = GBox.Text;

            if (R.Match(Text).Success)
            {
                GBox.Text = "";
                return;
            }

            if (Text.Length == 0)
            {
                return;
            }

            double g = Convert.ToDouble(Text);
            if (g < 0)
            {
                g = 0;
            }
            if (g > 255.0)
            {
                g = 255.0;
            }

            GBox.Text = ((int) g).ToString();
            Green = g;
            ReCalcHSV();
        }

        private void BBox_KeyUp(object sender, KeyEventArgs e)
        {
            Regex R = new Regex("[^0-9]");

            string Text = BBox.Text;

            if (R.Match(Text).Success)
            {
                BBox.Text = "";
                return;
            }

            if (Text.Length == 0)
            {
                return;
            }

            double b = Convert.ToDouble(Text);
            if (b < 0)
            {
                b = 0;
            }
            if (b > 255.0)
            {
                b = 255.0;
            }

            BBox.Text = ((int) b).ToString();
            Blue = b;
            ReCalcHSV();
        }
    }
}