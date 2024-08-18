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
 * class control button
 * Author: Yeisnier Dominguez Silva, March 2009
 */

using System;
using System.ComponentModel;
using System.Xml;
using NGUINet;
using RealmCrafter_GE.Property_Interfaces;

namespace RealmCrafter_GE.Property_Interfaces
{
    public partial class cButton : cControl
    {
        TextAlign _Align;
        TextVAlign _VAlign;
        cBrowseProperty _UpImage, _DownImage, _HoverImage;

        #region Align
        [CategoryAttribute("Align")]
        public TextAlign Align
        {
            get { return _Align; }
            set
            {
                _Align = value;
                ((NButton)control).Align = (NTextAlign)_Align;
            }
        }

        [CategoryAttribute("Align")]
        public TextVAlign VAlign
        {
            get { return _VAlign; }
            set
            {
                _VAlign = value;
                ((NButton)control).VAlign = (NTextAlign)_VAlign;
            }
        }
        #endregion

        #region Border
        [CategoryAttribute("Border")]
        public bool Border
        {
            get { return ((NButton)control).UseBorder; }
            set { ((NButton)control).UseBorder = value; }
        }
        #endregion

        #region Image
        [CategoryAttribute("Image")]
        [Editor(typeof(cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public string UpImage
        {
            get { return _UpImage.selectedFile; }
            set 
            {
                if (OverrideImage.Length == 0)
                {
                    _UpImage.selectedFile = value;
                    ((NButton)control).SetUpImage(@"Data\Textures\" + value);
                }
            }
        }
        [CategoryAttribute("Image")]
        [Editor(typeof(cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public string DownImage
        {
            get { return _DownImage.selectedFile; }
            set
            {
                if (OverrideImage.Length == 0)
                {
                    _DownImage.selectedFile = value;
                    ((NButton)control).SetDownImage(@"Data\Textures\" + value);
                }
            }
        }
        [CategoryAttribute("Image")]
        [Editor(typeof(cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public string HoverImage
        {
            get { return _HoverImage.selectedFile; }
            set
            {
                if (OverrideImage.Length == 0)
                {
                    _HoverImage.selectedFile = value;
                    ((NButton)control).SetHoverImage(@"Data\Textures\" + value);
                }
            }
        }
        #endregion

        public cButton(string Name, string toSaveName, string toSaveParent)
        {
            control = GE.GUIManager.CreateButton(Name, new NVector2(0, 0), new NVector2(0, 0));
            control.Tag = this;
            ToSaveName = toSaveName;
            ToSaveParent = toSaveParent;

            _UpImage = new cBrowseProperty();
            _DownImage = new cBrowseProperty();
            _HoverImage = new cBrowseProperty();

            _Align = TextAlign.Center;
            _VAlign = TextVAlign.Middle;
        }

        public override void LoadControl(XmlTextReader X)
        {
            NButton button = (NButton)control;
            base.LoadControl(X);

            string attrib = X.GetAttribute("UseBorder");
            if (attrib != null)
            {
                attrib = attrib.ToLower();
                if (attrib == "true" || attrib == "1") button.UseBorder = true;
                else
                if (attrib == "false" || attrib == "0") button.UseBorder = false;
            }

            attrib = X.GetAttribute("UpImage");
            if (attrib != null)
            {
                if (attrib.Substring(0, 14).Equals(@"data\textures\", StringComparison.CurrentCultureIgnoreCase))
                    attrib = attrib.Substring(14);

                if (OverrideImage.Length == 0)
                {
                    _UpImage.selectedFile = attrib;
                    button.SetUpImage(@"Data\Textures\" + attrib);
                }
            }

            attrib = X.GetAttribute("DownImage");
            if (attrib != null)
            {
                if (attrib.Substring(0, 14).Equals(@"data\textures\", StringComparison.CurrentCultureIgnoreCase))
                    attrib = attrib.Substring(14);

                if (OverrideImage.Length == 0)
                {
                    _DownImage.selectedFile = attrib;
                    button.SetDownImage(@"Data\Textures\" + attrib);
                }
            }
            attrib = X.GetAttribute("HoverImage");
            if (attrib != null)
            {
                if (attrib.Substring(0, 14).Equals(@"data\textures\", StringComparison.CurrentCultureIgnoreCase))
                    attrib = attrib.Substring(14);

                if (OverrideImage.Length == 0)
                {
                    _HoverImage.selectedFile = attrib;
                    button.SetHoverImage(@"Data\Textures\" + attrib);
                }
            }

            if (OverrideImage.Length > 0)
            {
                button.SetUpImage(@"Data\Textures\" + OverrideImage);
                button.SetDownImage(@"Data\Textures\" + OverrideImage);
                button.SetHoverImage(@"Data\Textures\" + OverrideImage);
            }

            attrib = X.GetAttribute("Align");
            if (attrib != null)
            {
                attrib = attrib.ToLower();
                if (attrib == "right") _Align = TextAlign.Right;
                else
                    if (attrib == "center") _Align = TextAlign.Center;
                    else
                        _Align = TextAlign.Left;

                button.Align = (NTextAlign)_Align;
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

                button.VAlign = (NTextAlign)_VAlign;
            }
        }
        public override void SaveControl(XmlTextWriter X)
        {
            X.WriteStartElement("component");
            base.SaveControl(X);

            // TYPE
            X.WriteStartAttribute("type");
            X.WriteString("button");
            X.WriteEndAttribute();

            // BORDER
            if (!Border)
            {
                X.WriteStartAttribute("UseBorder");
                X.WriteString("false");
                X.WriteEndAttribute();
            }

            // IMAGE
            if (_UpImage.selectedFile.Length > 0)
            {
                X.WriteStartAttribute("UpImage");
                X.WriteString(@"Data\Textures\" + _UpImage.selectedFile);
                X.WriteEndAttribute();
            }
            if (_DownImage.selectedFile.Length > 0)
            {
                X.WriteStartAttribute("DownImage");
                X.WriteString(@"Data\Textures\" + _DownImage.selectedFile);
                X.WriteEndAttribute();
            }
            if (_HoverImage.selectedFile.Length > 0)
            {
                X.WriteStartAttribute("HoverImage");
                X.WriteString(@"Data\Textures\" + _HoverImage.selectedFile);
                X.WriteEndAttribute();
            }
            
            // TEXT ALIGN
            if (_Align != TextAlign.Center)
            {
                X.WriteStartAttribute("Align");
                X.WriteString(_Align.ToString());
                X.WriteEndAttribute();
            }
            if (_VAlign != TextVAlign.Middle)
            {
                X.WriteStartAttribute("VAlign");
                X.WriteString(_VAlign.ToString());
                X.WriteEndAttribute();
            }

            X.WriteEndElement();
        }
    }
}