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
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Windows.Forms;
using System.IO;

namespace RealmCrafter_GE
{
    // Text rendering
    public class Text3D
    {
        // The client's character set
        public const string Letters =
            @" ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789,./\'#?<>[]();:!£$%^&*+-@~=_|àáâãäçèéêëïñòóôõöùúûüýÿßÆæØøÅåñÑ";

        // Generates a new font bitmap
        public static Bitmap GenerateFont(string Name, Font Font,
                                          int R, int G, int B,
                                          int SR, int SG, int SB, bool Shadow)
        {
            // Create rendering objects
            float ShadowOffset = (Font.Height / 12);
            Bitmap Image = new Bitmap(512, 512);
            Brush ShadowBrush = new SolidBrush(Color.FromArgb(SR, SG, SB));
            Brush TextBrush = new SolidBrush(Color.FromArgb(R, G, B));
            Graphics GraphicsImage = Graphics.FromImage(Image);
            GraphicsImage.TextRenderingHint = TextRenderingHint.AntiAlias;
            GraphicsImage.SmoothingMode = SmoothingMode.HighQuality;
            GraphicsImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicsImage.CompositingQuality = CompositingQuality.HighQuality;

            // Arrays to store character data
            int[] CharWidth = new int[256];
            int[] CharX = new int[256];
            int[] CharY = new int[256];

            // Draw each letter in turn
            float X = 0, Y = 0, Width;
            for (int i = 0; i < Letters.Length; ++i)
            {
                // Get letter width
                Width = MeasureDisplaystringWidth(GraphicsImage, Letters.Substring(i, 1), Font) - 4f;
                // Make space character wider
                if (i == 0)
                {
                    Width = 45f;
                }
                // Wrap to next line if required
                if (X + Width >= 508f)
                {
                    X = 0f;
                    Y += Font.Height + 2f + ShadowOffset;
                }
                // Store character position/size
                CharWidth[i] = (int)Math.Floor(Width);
                CharX[i] = (int)(Math.Ceiling(X) + 3f);
                CharY[i] = (int)Y;
                if (Shadow)
                {
                    GraphicsImage.DrawString(Letters.Substring(i, 1), Font, ShadowBrush,
                                             new PointF(X + ShadowOffset, (Y - 2f) + ShadowOffset));
                }
                GraphicsImage.DrawString(Letters.Substring(i, 1), Font, TextBrush, new PointF(X, Y - 2f));
                X += (float)Math.Ceiling(Width) + 3f;
            }

            // Free rendering objects
            ShadowBrush.Dispose();
            TextBrush.Dispose();
            GraphicsImage.Dispose();

            // Save image and data
            if (!string.IsNullOrEmpty(Name))
            {
                File.Delete(Name + ".bmp");
                Image.Save(Name + ".bmp", ImageFormat.Bmp);

                BinaryWriter F = Blitz.WriteFile(Name + ".dat");
                F.Write((ushort)(Font.Height + 2));
                for (int i = 0; i < 256; ++i)
                {
                    F.Write(CharWidth[i]);
                    F.Write(CharX[i]);
                    F.Write(CharY[i]);
                }
                F.Close();
            }

            // Done
            return Image;
        }

        // Measures string widths more accurately
        public static float MeasureDisplaystringWidth(Graphics Graphics, string Text,
                                                      Font Font)
        {
            StringFormat format = new StringFormat();
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = { new CharacterRange(0, Text.Length) };
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = Graphics.MeasureCharacterRanges(Text, Font, rect, format);
            rect = regions[0].GetBounds(Graphics);

            return rect.Right;
        }
    }

    public partial class FontEditor : Form
    {
        private int ForeR = 255, ForeG = 255, ForeB = 255, ShadowR = 2, ShadowG = 2, ShadowB = 2;

        // Constructor
        public FontEditor()
        {
            InitializeComponent();

            if (System.IO.File.Exists(@"Data\UI\Fonts\Title.bmp"))
            {
                Bitmap bmp = new Bitmap(@"Data\UI\Fonts\Title.bmp");
                bmp.MakeTransparent(bmp.GetPixel(1, 1));

                FontPreview.Image = bmp;
            }
            else
            {
                FontPreview.Image = Text3D.GenerateFont("", FontEditFontDialog.Font, ForeR, ForeG, ForeB,
                                                        ShadowR, ShadowG, ShadowB, UseShadowCheck.Checked);
            }
        }

        // Close the window
        private void DoneButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        // Change font settings
        private void ChooseFontButton_Click(object sender, EventArgs e)
        {
            DialogResult Result = FontEditFontDialog.ShowDialog();
            if (Result != DialogResult.Cancel)
            {
                FontPreview.Image = Text3D.GenerateFont("", FontEditFontDialog.Font, ForeR, ForeG, ForeB,
                                                        ShadowR, ShadowG, ShadowB, UseShadowCheck.Checked);
            }
        }

        private void UseShadowCheck_CheckedChanged(object sender, EventArgs e)
        {
            FontPreview.Image = Text3D.GenerateFont("", FontEditFontDialog.Font, ForeR, ForeG, ForeB,
                                                    ShadowR, ShadowG, ShadowB, UseShadowCheck.Checked);
        }

        private void ChooseColourButton_Click(object sender, EventArgs e)
        {
            FontEditColourDialog.Color = Color.FromArgb(ForeR, ForeG, ForeB);
            DialogResult Result = FontEditColourDialog.ShowDialog();
            if (Result != DialogResult.Cancel)
            {
                ForeR = FontEditColourDialog.Color.R;
                ForeG = FontEditColourDialog.Color.G;
                ForeB = FontEditColourDialog.Color.B;
                FontPreview.Image = Text3D.GenerateFont("", FontEditFontDialog.Font, ForeR, ForeG, ForeB,
                                                        ShadowR, ShadowG, ShadowB, UseShadowCheck.Checked);
            }
        }

        private void ChooseShadowColourButton_Click(object sender, EventArgs e)
        {
            FontEditColourDialog.Color = Color.FromArgb(ShadowR, ShadowG, ShadowB);
            DialogResult Result = FontEditColourDialog.ShowDialog();
            if (Result != DialogResult.Cancel)
            {
                ShadowR = FontEditColourDialog.Color.R;
                ShadowG = FontEditColourDialog.Color.G;
                ShadowB = FontEditColourDialog.Color.B;
                FontPreview.Image = Text3D.GenerateFont("", FontEditFontDialog.Font, ForeR, ForeG, ForeB,
                                                        ShadowR, ShadowG, ShadowB, UseShadowCheck.Checked);
            }
        }

        // Save font to file
        private void SaveFontButton_Click(object sender, EventArgs e)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                Text3D.GenerateFont(@"Data\UI\Fonts\Title", FontEditFontDialog.Font, ForeR, ForeG, ForeB,
                                    ShadowR, ShadowG, ShadowB, UseShadowCheck.Checked);
            }
        }
    }
}