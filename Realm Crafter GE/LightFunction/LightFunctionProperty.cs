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
using System.Drawing.Design;

namespace RealmCrafter
{
    public class LightFunctionProperty
    {
        DataGridViewRow Row = null;

        public LightFunctionProperty(DataGridViewRow row)
        {
            Row = row;
        }

        [CategoryAttribute("Time"), Description("Trigger time or duration of this event."),
            //TypeConverter(typeof(RenderingServices.Vector1OptionsConverter))
            Editor(typeof(LightFunctionTimeEditor), typeof(UITypeEditor))]
        public LightFunctionTime Time
        {
            get { return Row.Cells[0].Tag as LightFunctionTime; }
            set
            {
                Row.Cells[0].Tag = value;
                Row.Cells[0].Value = value.ToString();
            }
            //set { type.Name = value; changedEvent.Invoke(type, EventArgs.Empty); }
        }

        [CategoryAttribute("Time"), Description("Chooses whether this event will fade from the previous")]
        //[Editor(typeof(RealmCrafter_GE.Property_Interfaces.cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public bool Interpolate
        {
            get { return Convert.ToBoolean(Row.Cells[1].Tag.ToString()); }
            set
            {
                Row.Cells[1].Tag = value.ToString();
                Row.Cells[1].Value = value.ToString();
            }
        }

        [CategoryAttribute("Light"), Description("Color of the light during this event."), TypeConverter(typeof(RenderingServices.Vector3ColorOptionsConverter)), Editor(typeof(RenderingServices.Vector3ColorEditor), typeof(UITypeEditor))]
        public RenderingServices.Vector3 Color
        {
            get
            {
                return Row.Cells[2].Tag as RenderingServices.Vector3;
            }

            set
            {
                Row.Cells[2].Tag = value.Clone();
                Row.Cells[2].Style.BackColor = System.Drawing.Color.FromArgb(
                    Convert.ToInt32(Color.X * 255.0f),
                    Convert.ToInt32(Color.Y * 255.0f),
                    Convert.ToInt32(Color.Z * 255.0f));
            }
        }

        [CategoryAttribute("Light"), Description("Radius of the light during this event,")]//, TypeConverter(typeof(RenderingServices.Vector1OptionsConverter)), Editor(typeof(RenderingServices.Vector1SliderEditor), typeof(UITypeEditor))]
        public int Radius
        {
            get
            {
                return Convert.ToInt32(Row.Cells[3].Tag.ToString());
            }
            set
            {
                Row.Cells[3].Tag = value.ToString();
                Row.Cells[3].Value = value.ToString();
            }
        }

    }
}
