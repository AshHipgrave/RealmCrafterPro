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
using NGUINet;
using RealmCrafter_GE.Property_Interfaces;
using System.ComponentModel;
using System.Xml;

namespace RealmCrafter_GE.Property_Interfaces
{
    public partial class cPictureBox : cControl
    {
        cBrowseProperty _Image;

        #region Image
        [CategoryAttribute("Image")]
        [Editor(typeof(cBrowseProperty), typeof(System.Drawing.Design.UITypeEditor))]
        public string Image
        {
            get { return _Image.selectedFile; }
            set
            {
                if (OverrideImage.Length > 0)
                    return;

                _Image.selectedFile = value;

                if (value.Length == 0)
                    ((NPictureBox)control).SetImage(Program.TestingVersion ? @".\Data\DefaultTex.png" : @"..\..\Data\GUE\DefaultTex.PNG");
                else
                   ((NPictureBox)control).SetImage(@"Data\Textures\" + value);
            }
        }
        #endregion

        public cPictureBox(string Name, string toSaveName, string toSaveParent)
        {
            control = GE.GUIManager.CreatePictureBox(Name, new NVector2(0, 0), new NVector2(0, 0));
            control.Tag = this;
            ToSaveName = toSaveName;
            ToSaveParent = toSaveParent;

            _Image = new cBrowseProperty();
        }

        public override void LoadControl(XmlTextReader X)
        {
            NPictureBox picturebox = (NPictureBox)control;
            base.LoadControl(X);

            string attrib = X.GetAttribute("Image");
            if (attrib != null)
            {
                if (attrib.Substring(0, 14).Equals(@"data\textures\", StringComparison.CurrentCultureIgnoreCase))
                    attrib = attrib.Substring(14);

                _Image.selectedFile = attrib;
                picturebox.SetImage(@"Data\Textures\" + attrib);
            }

            if(attrib == null || attrib.Length == 0)
            {
                if (OverrideImage.Length > 0)
                    picturebox.SetImage(@"Data\Textures\" + OverrideImage, System.Drawing.Color.Black);
                else
                    picturebox.SetImage(Program.TestingVersion ? @".\Data\DefaultTex.png" : @"..\..\Data\GUE\DefaultTex.PNG");
            }

        }
        public override void SaveControl(XmlTextWriter X)
        {
            X.WriteStartElement("component");
            base.SaveControl(X);

            // TYPE
            X.WriteStartAttribute("type");
            X.WriteString("picturebox");
            X.WriteEndAttribute();

            // IMAGE
            if (_Image.selectedFile.Length > 0)
            {
                X.WriteStartAttribute("Image");
                X.WriteString(@"Data\Textures\" + _Image.selectedFile);
                X.WriteEndAttribute();
            }


            X.WriteEndElement();
        }
    }
}
