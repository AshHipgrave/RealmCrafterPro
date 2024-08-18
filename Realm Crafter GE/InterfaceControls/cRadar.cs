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
 * class control Radar
 * Author: Yeisnier Dominguez Silva, October 2009
 */

using System;
using System.ComponentModel;
using System.Xml;
using NGUINet;
using RealmCrafter_GE.Property_Interfaces;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;
using RealmCrafter;

namespace RealmCrafter_GE.Property_Interfaces
{
    public partial class cRadar : cControl
    {
        cBrowseProperty _RadarImage, _RadarBorder;
        System.Drawing.Point viewRadius, imageTop, imageSize;

        #region Radar
        [CategoryAttribute("Radar")]
        [Editor(typeof(cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public string Image
        {
            get { return _RadarImage.selectedFile; }
            set
            {
                _RadarImage.selectedFile = value;
                ((NRadar)control).SetImage(@"Data\Textures\" + value, @"");
            }
        }
        [CategoryAttribute("Radar")]
        [Editor(typeof(cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public string Border
        {
            get { return _RadarBorder.selectedFile; }
            set
            {
                _RadarBorder.selectedFile = value;
                ((NRadar)control).SetImage(@"", @"Data\Textures\" + value);
            }
        }

        [CategoryAttribute("Radar")]
        public System.Drawing.Point ViewRadius
        {
            get { return viewRadius; }
            set
            {
                viewRadius = value;
                ((NRadar)control).ViewRadius = new NVector2(viewRadius.X, viewRadius.Y);
            }
        }

        [CategoryAttribute("Radar")]
        public System.Drawing.Point ImageTop
        {
            get { return imageTop; }
            set
            {
                imageTop = value;
                ((NRadar)control).ImageTop = new NVector2(imageTop.X, imageTop.Y);
            }
        }

        [CategoryAttribute("Radar")]
        public System.Drawing.Point ImageSize
        {
            get { return imageSize; }
            set
            {
                imageSize = value;
                ((NRadar)control).ImageSize = new NVector2(imageSize.X, imageSize.Y);
            }
        }
        #endregion

        public cRadar(string Name, string toSaveName, string toSaveParent)
        {
            control = GE.GUIManager.CreateRadar(Name, new NVector2(0, 0), new NVector2(0, 0));
            control.Tag = this;
            ToSaveName = toSaveName;
            ToSaveParent = toSaveParent;

            _RadarImage = new cBrowseProperty();
            _RadarBorder = new cBrowseProperty();
        }

        public override void LoadControl(XmlTextReader X)
        {
            NRadar radar = (NRadar)control;
            base.LoadControl(X);

            string attrib = X.GetAttribute("image");
            if (attrib != null)
            {
                _RadarImage.selectedFile = attrib;
                radar.SetImage(@"Data\Textures\" + attrib, @"");
            }

            attrib = X.GetAttribute("radarBorder");
            if (attrib != null)
            {
                _RadarBorder.selectedFile = attrib;
                radar.SetImage(@"", @"Data\Textures\" + attrib);
            }

            eLocationType temp = eLocationType.ABSOLUTE_ABSOLUTE;
            attrib = X.GetAttribute("viewRadius");
            if (attrib != null)
            {
                radar.ViewRadius = LocationFromString(attrib, ref temp);
                viewRadius.X = (int)radar.ViewRadius.X;
                viewRadius.Y = (int)radar.ViewRadius.Y;
            }

            attrib = X.GetAttribute("imageTop");
            if (attrib != null)
            {
                radar.ImageTop = LocationFromString(attrib, ref temp);
                imageTop.X = (int)radar.ImageTop.X;
                imageTop.Y = (int)radar.ImageTop.Y;
            }

            attrib = X.GetAttribute("imageSize");
            if (attrib != null)
            {
                radar.ImageSize = LocationFromString(attrib, ref temp);
                imageSize.X = (int)radar.ImageSize.X;
                imageSize.Y = (int)radar.ImageSize.Y;
            }
        }
        public override void SaveControl(XmlTextWriter X)
        {
            NRadar radar = (NRadar)control;

            X.WriteStartElement("component");
            base.SaveControl(X);

            // IMAGE
            if (_RadarImage.selectedFile.Length > 0)
            {
                X.WriteStartAttribute("image");
                X.WriteString(_RadarImage.selectedFile);
                X.WriteEndAttribute();
            }

            // BORDER
            if (_RadarBorder.selectedFile.Length > 0)
            {
                X.WriteStartAttribute("radarBorder");
                X.WriteString(_RadarBorder.selectedFile);
                X.WriteEndAttribute();
            }

            // ViewRadius
            X.WriteStartAttribute("viewRadius");
            X.WriteString(StringFromLocation(radar.ViewRadius, eLocationType.ABSOLUTE_ABSOLUTE));
            X.WriteEndAttribute();

            // ImageTop
            X.WriteStartAttribute("imageTop");
            X.WriteString(StringFromLocation(radar.ImageTop, eLocationType.ABSOLUTE_ABSOLUTE));
            X.WriteEndAttribute();

            // ImageSize
            X.WriteStartAttribute("imageSize");
            X.WriteString(StringFromLocation(radar.ImageSize, eLocationType.ABSOLUTE_ABSOLUTE));
            X.WriteEndAttribute();

            // TYPE
            X.WriteStartAttribute("type");
            X.WriteString("radar");
            X.WriteEndAttribute();

            X.WriteEndElement();
        }
    }
}