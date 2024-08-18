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
/* 
 * class control CheckBox
 * Author: Yeisnier Dominguez Silva, March 2009
 */

using System;
using System.ComponentModel;
using System.Xml;
using NGUINet;
using RealmCrafter_GE.Property_Interfaces;

namespace RealmCrafter_GE.Property_Interfaces
{
    public partial class cCheckBox : cControl
    {
        public cCheckBox(string Name, string toSaveName, string toSaveParent)
        {
            control = GE.GUIManager.CreateCheckBox(Name, new NVector2(0, 0), new NVector2(0, 0));
            control.Tag = this;
            ToSaveName = toSaveName;
            ToSaveParent = toSaveParent;
        }

        public override void LoadControl(XmlTextReader X)
        {
            NCheckBox checkbox = (NCheckBox)control;
            base.LoadControl(X);
        }

        public override void SaveControl(XmlTextWriter X)
        {
            X.WriteStartElement("component");
            base.SaveControl(X);

            // TYPE
            X.WriteStartAttribute("type");
            X.WriteString("button");
            X.WriteEndAttribute();

            X.WriteEndElement();
        }
    }
}