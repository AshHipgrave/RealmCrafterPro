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

namespace RealmCrafter_GE.Dockable_Forms.PostProcess.ParamTypes
{
    class cParam_Float4
    {
        OnParamChangeCallback ParamChangeCallback;
        string Value;

        System.Windows.Forms.GroupBox gb;
        cParam_Float Red, Green, Blue, Alpha;


        public cParam_Float4(System.Windows.Forms.Control Parent,
                string Name, string value, OnParamChangeCallback paramChangeCallback) 
        {
            ParamChangeCallback = paramChangeCallback;
            Value = value;

            gb = new System.Windows.Forms.GroupBox();
            Parent.Controls.Add(this.gb);

            // 
            // gb
            // 
            this.gb.Dock = System.Windows.Forms.DockStyle.Top;
            this.gb.Name = "gb";
            this.gb.Size = new System.Drawing.Size(326, 220);
            this.gb.TabIndex = 1;
            this.gb.TabStop = false;
            this.gb.Text = Name;

            string separator = " ";
            string[] aValues = Value.Split(separator.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

            Alpha = new cParam_Float(this.gb, "Alpha", aValues[3], new OnParamChangeCallback(OnParamChange));
            Blue = new cParam_Float(this.gb, "Blue", aValues[2], new OnParamChangeCallback(OnParamChange));
            Green = new cParam_Float(this.gb, "Green", aValues[1], new OnParamChangeCallback(OnParamChange));
            Red = new cParam_Float(this.gb, "Red", aValues[0], new OnParamChangeCallback(OnParamChange));
        }

        void OnParamChange(string value, string paramName)
        {
            string separator = " ";
            string[] aValues = Value.Split(separator.ToCharArray(), System.StringSplitOptions.RemoveEmptyEntries);

            if (paramName == "Red") aValues[0] = value;
            else
            if (paramName == "Green") aValues[1] = value;
            else
            if (paramName == "Blue") aValues[2] = value;
            else
            if (paramName == "Alpha") aValues[3] = value;

            Value = aValues[0] + " " + aValues[1] + " " + aValues[2] + " " + aValues[3];
            ParamChangeCallback(Value, gb.Text);
        }
    }
}
