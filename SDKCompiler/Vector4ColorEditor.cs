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
    public class Vector4ColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            IWindowsFormsEditorService Wfes = provider.GetService(
                                                  typeof(IWindowsFormsEditorService)) as IWindowsFormsEditorService;

            if (Wfes != null)
            {
                ColorEditorForm CForm = new ColorEditorForm();
                CForm.SelectedColor = value;

                Wfes.DropDownControl(CForm);
                value = CForm.SelectedColor;
            }
            return value;
        }

        public override void PaintValue(PaintValueEventArgs e)
        {
            base.PaintValue(e);

            if (e.Value is Vector4)
            {
                Vector4 C = e.Value as Vector4;
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

                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(R, G, B)), e.Bounds);
            }
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}