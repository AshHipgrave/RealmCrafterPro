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
 * class control Label
 * Author: Yeisnier Dominguez Silva, March 2009
 */

using System;
using System.ComponentModel;
using System.Xml;
using NGUINet;
using RealmCrafter_GE.Property_Interfaces;

namespace RealmCrafter_GE.Property_Interfaces
{
    public enum TextAlign
    {
        Left,
        Center,
        Right
    };
    public enum TextVAlign
    {
        Top,
        Middle,
        Bottom
    };

    public partial class cLabel : cControl
    {
        TextAlign _Align;
        TextVAlign _VAlign;

        #region Align
        [CategoryAttribute("Align")]
        public TextAlign Align
        {
            get { return _Align; }
            set 
            { 
                _Align = value;
                ((NLabel)control).Align = (NTextAlign)_Align;
            }
        }

        [CategoryAttribute("Align")]
        public TextVAlign VAlign
        {
            get { return _VAlign; }
            set 
            { 
                _VAlign = value;
                ((NLabel)control).VAlign = (NTextAlign)_VAlign;
            }
        }
        #endregion

        public cLabel(string Name, string toSaveName, string toSaveParent)
        {
            control = GE.GUIManager.CreateLabel(Name, new NVector2(0, 0), new NVector2(0, 0));
            control.Tag = this;
            ToSaveName = toSaveName;
            ToSaveParent = toSaveParent;

            _Align = TextAlign.Left;
            _VAlign = TextVAlign.Top;
        }

        public override void LoadControl(XmlTextReader X)
        {
            NLabel label = (NLabel)control;
            base.LoadControl(X);

            string attrib = X.GetAttribute("Align");
            if (attrib != null)
            {
                attrib = attrib.ToLower();
                if ( attrib == "right" ) _Align = TextAlign.Right; 
                else
                if ( attrib == "center" ) _Align = TextAlign.Center;
                else
                    _Align = TextAlign.Left;

                label.Align = (NTextAlign)_Align;
            }
            attrib = X.GetAttribute("valign");
            if (attrib != null)
            {
                attrib = attrib.ToLower();
                if (attrib == "bottom") _VAlign = TextVAlign.Bottom;
                else
                if (attrib == "middle") _VAlign = TextVAlign.Middle;
                else
                    _VAlign = TextVAlign.Top;
        
                label.VAlign = (NTextAlign)_VAlign;
            }
        }
        public override void SaveControl(XmlTextWriter X)
        {
            X.WriteStartElement("component");
            base.SaveControl(X);
            
            // TYPE
            X.WriteStartAttribute("type");
            X.WriteString("label");
            X.WriteEndAttribute(); 
            
            // TEXT ALIGN
            if (_Align != TextAlign.Left)
            {
                X.WriteStartAttribute("Align");
                X.WriteString(_Align.ToString());
                X.WriteEndAttribute();
            }
            if (_VAlign != TextVAlign.Top)
            {
                X.WriteStartAttribute("VAlign");
                X.WriteString(_VAlign.ToString());
                X.WriteEndAttribute();
            }

            X.WriteEndElement();
        }
    }
}