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
 * This class interfaces the property grid with the selected Emitter object
 * retrieving information from the Emitter object class
 * Author: Shane Smith, Aug 2008
 */

using System;
using System.ComponentModel;
using System.Windows.Forms;
using RealmCrafter.ClientZone;
using RealmCrafter;

namespace RealmCrafter_GE.Property_Interfaces
{
    public class EmitterPropertyInterface
    {
        private readonly Emitter _Object;

        public EmitterPropertyInterface(Emitter Object)
        {
            _Object = Object;
        }

        #region Location
        [Category("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }
            set { _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z()); }
        }

        [Category("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }
            set { _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z()); }
        }

        [Category("Location")]
        public float LocZ
        {
            get { return _Object.EN.Z(); }
            set { _Object.EN.Position(_Object.EN.X(), _Object.EN.Y(), value); }
        }
        #endregion

        #region Rotation
        [Category("Rotation")]
        public float Pitch
        {
            get { return _Object.EN.Pitch(); }

            set { _Object.EN.Rotate(value, _Object.EN.Yaw(), _Object.EN.Roll()); }
        }

        [Category("Rotation")]
        public float Roll
        {
            get { return _Object.EN.Roll(); }

            set { _Object.EN.Rotate(_Object.EN.Pitch(), _Object.EN.Yaw(), value); }
        }

        [Category("Rotation")]
        public float Yaw
        {
            get { return _Object.EN.Yaw(); }

            set { _Object.EN.Rotate(_Object.EN.Pitch(), value, _Object.EN.Roll()); }
        }
        #endregion

        // Really ugly,
        // Todo: Change this to open up a media selection form.
        [Category("Emitter Configuration"), TypeConverter(typeof(PropertyTextureList))]
        public string Texture
        {
            get { return GE.NiceTextureName(_Object.TextureID); }

            set
            {
                ushort SelectedTextureID;
                if (value == "No Texture")
                {
                    if (!(_Object.TextureID == _Object.Config.DefaultTextureID))
                    {
                        SelectedTextureID = _Object.Config.DefaultTextureID;
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    string s = value;
                    int ColonIndex = s.LastIndexOf(":");
                    string q = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
                    SelectedTextureID = Convert.ToUInt16(q);
                }

                uint Tex = Media.GetTexture(SelectedTextureID, false);
                if (Tex != 0)
                {
                    Media.UnloadTexture(_Object.TextureID);
                    _Object.Config.ChangeTexture(Tex);
                    _Object.TextureID = SelectedTextureID;
                }
                else
                {
                    MessageBox.Show("Invalid Texture");
                }
            }
        }

        // Really ugly
        // Todo: Automatically reload the config - not requiring a zone refresh
        [Category("Emitter Configuration"),
         Description("Must reload zone to see changes"),
         TypeConverter(typeof(PropertyEmitterList))]
        public string ConfigName
        {
            get { return _Object.ConfigName; }

            set
            {
                _Object.ConfigName = value;
                _Object.Config.Name = value;
            }
        }
    }
}