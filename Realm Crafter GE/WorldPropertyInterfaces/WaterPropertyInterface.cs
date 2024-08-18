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
        private static Water _Object;

        public void SetWaterObject( Water W ) { _Object = W; }

        #region Advanced
            [
            CategoryAttribute("Advanced"),
            DescriptionAttribute("Water Shaders from 'Data\\Game Data\\Shaders\\Default\\Water\\'"), TypeConverter(typeof(PropertyWaterShaders))]
            public string ShaderFX
            {
                get
                {
                    return ShaderManager.GetShaderName( _Object.ShaderID );
                }
                set
                {

                    _Object.ShaderID = ShaderManager.GetShader(value);
                    _Object.EN.Shader = _Object.ShaderID;
                    RenderingServices.RenderWrapper.ResetNodeParameters(_Object.EN.Handle);
                    Program.GE.m_propertyWindow.UpdatePropertyGridWith_Water(_Object);
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

//         [CategoryAttribute("Colour"),
//          DescriptionAttribute("Blue amount")]
//         public byte Blue
//         {
//             get { return _Object.Blue; }
// 
//             set
//             {
//                 _Object.EN.Color(_Object.Red, _Object.Green, value);
//                 _Object.Blue = value;
//             }
//         }
// 
//         [CategoryAttribute("Colour"),
//          DescriptionAttribute("Green amount")]
//         public byte Green
//         {
//             get { return _Object.Green; }
// 
//             set
//             {
//                 _Object.EN.Color(_Object.Red, value, _Object.Blue);
//                 _Object.Green = value;
//             }
//         }
// 
//         [CategoryAttribute("Colour"),
//          DescriptionAttribute("Alpha value")]
//         public byte Alpha
//         {
//             get { return _Object.Alpha; }
// 
//             set
//             {
//                 if (value > 100)
//                 {
//                     value = 100;
//                 }
// 
//                 if (value < 0)
//                 {
//                     value = 0;
//                 }
// 
//                 _Object.EN.Alpha((float) value / 100);
//                 _Object.Alpha = value;
//             }
//         }
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

        #region Shader Parameters

            public static object GetShaderParameter(string ParamName)
            {
                return RenderingServices.RenderWrapper.GetEntityParameterValue(_Object.EN.Handle, ParamName);
                //return RenderingServices.RenderWrapper.GetParameterValue(_Object.ShaderID, iParam);
            }

            public static void SetShaderParameter(string Name, object Value)
            {
                if (Value is Vector1)
                    RenderingServices.RenderWrapper.EntityConstantFloat(_Object.EN.Handle, Name, (Value as Vector1).X);
                if (Value is Vector2)
                    RenderingServices.RenderWrapper.EntityConstantFloat2(_Object.EN.Handle, Name, (Value as Vector2).X, (Value as Vector2).Y);
                if (Value is Vector3)
                    RenderingServices.RenderWrapper.EntityConstantFloat3(_Object.EN.Handle, Name, (Value as Vector3).X, (Value as Vector3).Y, (Value as Vector3).Z);
                if (Value is Vector4)
                    RenderingServices.RenderWrapper.EntityConstantFloat4(_Object.EN.Handle, Name, (Value as Vector4).X, (Value as Vector4).Y, (Value as Vector4).Z, (Value as Vector4).W);
            }

            public static string GetShaderTexture( int iTex )
            {
                return GE.NiceTextureName((int)_Object.TextureID[iTex]);
            }

            public static void SetShaderTexture(int iTex, string value)
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
                    if (Tex != 0) _Object.EN.Texture(Tex, iTex);
                             else SelectedTextureID = (ushort)_Object.TextureID[iTex];
                    
                    _Object.TextureID[iTex] = (int)SelectedTextureID;
                    _Object.TexHandle[iTex] = Tex;
                }
                else
                {
                    SelectedTextureID = 65535;
                    uint Tex = Media.GetTexture(SelectedTextureID, false);
                    if (Tex != 0) _Object.EN.Texture(Tex, iTex);
                             else SelectedTextureID = (ushort)_Object.TextureID[iTex];

                    _Object.TextureID[iTex] = (int)SelectedTextureID;
                    _Object.TexHandle[iTex] = Tex;
                }
            }

        #endregion
    }
}