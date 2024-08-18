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
using System.ComponentModel;
using RealmCrafter;
using RealmCrafter.ClientZone;
using RenderingServices;

namespace RealmCrafter_GE.Property_Interfaces
{
    public class WaterPropertyInterface
    {
        private readonly Water _Object;

        public WaterPropertyInterface(Water Object)
        {
            _Object = Object;
        }

        #region Advanced
        [CategoryAttribute("Advanced"),
         DescriptionAttribute("Changes raw TextureID"), TypeConverter(typeof(PropertyTextureList))]
        public string TextureID
        {
            get { return GE.NiceTextureName(_Object.TextureID); }
            set
            {
                ushort SelectedTextureID;
                if (value == "No Texture")
                {
                    SelectedTextureID = 65535;
                }
                else
                {
                    string s = value;
                    int ColonIndex = s.LastIndexOf(":");
                    string q = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
                    SelectedTextureID = Convert.ToUInt16(q);
                }
                if (SelectedTextureID < 65535)
                {
                    uint Tex = Media.GetTexture(SelectedTextureID, false);
                    if (Tex != 0)
                    {
                        _Object.EN.Texture(Tex, 1);
                    }
                    else
                    {
                        SelectedTextureID = _Object.TextureID;
                    }

                    _Object.TextureID = SelectedTextureID;
                }
                else
                {
                    SelectedTextureID = 65535;
                    uint Tex = Media.GetTexture(SelectedTextureID, false);
                    if (Tex != 0)
                    {
                        _Object.EN.Texture(Tex, 1);
                    }
                    else
                    {
                        SelectedTextureID = _Object.TextureID;
                    }

                    _Object.TextureID = SelectedTextureID;
                }
            }
        }
        #endregion

        #region Colour values
        [CategoryAttribute("Colour"),
         DescriptionAttribute("Red amount")]
        public byte Red
        {
            get { return _Object.Red; }

            set
            {
                _Object.EN.Color(value, _Object.Green, _Object.Blue);
                _Object.Red = value;
            }
        }

        [CategoryAttribute("Colour"),
         DescriptionAttribute("Blue amount")]
        public byte Blue
        {
            get { return _Object.Blue; }

            set
            {
                _Object.EN.Color(_Object.Red, _Object.Green, value);
                _Object.Blue = value;
            }
        }

        [CategoryAttribute("Colour"),
         DescriptionAttribute("Green amount")]
        public byte Green
        {
            get { return _Object.Green; }

            set
            {
                _Object.EN.Color(_Object.Red, value, _Object.Blue);
                _Object.Green = value;
            }
        }

        [CategoryAttribute("Colour"),
         DescriptionAttribute("Alpha value")]
        public byte Alpha
        {
            get { return _Object.Alpha; }

            set
            {
                if (value > 100)
                {
                    value = 100;
                }

                if (value < 0)
                {
                    value = 0;
                }

                _Object.EN.Alpha((float) value / 100);
                _Object.Alpha = value;
            }
        }
        #endregion

        #region Texture
        /*
        [CategoryAttribute("Texture"),
         DescriptionAttribute("Texture Scale")]
        public float TextureScale
        {
            get { return _Object.TexScale; }

            set
            {
                Render.ScaleTexture(_Object.TexHandle, value, value);
                _Object.TexScale = value;
            }
        }
         */
        #endregion

        #region Entity

        #region Location
        [CategoryAttribute("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }

            set
            {
                _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }

        [CategoryAttribute("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }

            set
            {
                _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }

        [CategoryAttribute("Location")]
        public float LocZ
        {
            get { return _Object.EN.Z(); }

            set
            {
                _Object.EN.Position(_Object.EN.X(), _Object.EN.Y(), value);
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }
        #endregion

        #region Scale
        [CategoryAttribute("Scale")]
        public float ScaleX
        {
            get { return _Object.EN.ScaleX() * 20; }

            set
            {
                _Object.EN.Scale(value / 20, _Object.EN.ScaleY(), _Object.EN.ScaleZ());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }

        [CategoryAttribute("Scale")]
        public float ScaleZ
        {
            get { return _Object.EN.ScaleZ() * 20; }

            set
            {
                _Object.EN.Scale(_Object.EN.ScaleX(), _Object.EN.ScaleY(), value / 20);
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }
        #endregion

        #endregion

        #region Water Damage
        [CategoryAttribute("Water Damage")]
        public ushort DamageAmount
        {
            get { return _Object.ServerWater.Damage; }

            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (value > 65535)
                {
                    value = 65535;
                }

                _Object.ServerWater.Damage = value;
            }
        }

        [CategoryAttribute("Water Damage"), TypeConverter(typeof(PropertyDamageTypeList))]
        public string DamageType
        {
            get { return Item.DamageTypes[_Object.ServerWater.DamageType]; }
            set
            {
                if (value != "")
                {
                    _Object.ServerWater.DamageType = (ushort) Item.FindDamageType(value);
                }
            }
        }
        #endregion
    }
}