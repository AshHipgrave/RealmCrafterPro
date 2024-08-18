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
using RenderingServices;
using System.ComponentModel;
using System.Drawing;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public class ControlProperties
    {
        public EditorControlInstanceBase Parent;

//         [CategoryAttribute("Properties"), TypeConverterAttribute(typeof(Vector2OptionsConverter))]
//         public Vector2 Location
//         {
//             get { return (Vector2)Parent.Properties[0].Value; }
//             set { Parent.Properties[0].Value = value; Parent.PropertyUpdate(0); }
//         }
// 
//         [CategoryAttribute("Properties")]
//         public PositionType PositionType
//         {
//             get { return (PositionType)Parent.Properties[1].Value; }
//             set { Parent.Properties[1].Value = value; Parent.PropertyUpdate(1); }
//         }
// 
//         [CategoryAttribute("Properties"), TypeConverterAttribute(typeof(Vector2OptionsConverter))]
//         public Vector2 Size
//         {
//             get { return Parent.Properties[2].Value as Vector2; }
//             set { Parent.Properties[2].Value = value; Parent.PropertyUpdate(2); }
//         }
// 
//         [CategoryAttribute("Properties")]
//         public SizeType SizeType
//         {
//             get { return (SizeType)Parent.Properties[3].Value; }
//             set { Parent.Properties[3].Value = value; Parent.PropertyUpdate(3); }
//         }
// 
//         [CategoryAttribute("Properties")]
//         public bool Enabled
//         {
//             get { return (bool)Parent.Properties[4].Value; }
//             set { Parent.Properties[4].Value = value; Parent.PropertyUpdate(4); }
//         }
// 
//         [CategoryAttribute("Properties")]
//         public bool Visible
//         {
//             get { return (bool)Parent.Properties[5].Value; }
//             set { Parent.Properties[5].Value = value; Parent.PropertyUpdate(5); }
//         }
// 
//         [CategoryAttribute("Properties")]
//         public string Name
//         {
//             get { return (string)Parent.Properties[6].Value; }
//             set { Parent.Properties[6].Value = value; Parent.PropertyUpdate(6); }
//         }
// 
//         [CategoryAttribute("Properties")]
//         public string Text
//         {
//             get { return (string)Parent.Properties[7].Value; }
//             set { Parent.Properties[7].Value = value; Parent.PropertyUpdate(7); }
//         }
// 
//         [CategoryAttribute("Properties"), TypeConverterAttribute(typeof(Vector4ColorOptionsConverter)), Editor(typeof(Vector4ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
//         public Vector4 BackColor
//         {
//             get
//             {
//                 Color C = (Color)Parent.Properties[8].Value;
//                 Vector4 Out = new Vector4(
//                     ((float)C.R) / 255.0f,
//                     ((float)C.G) / 255.0f,
//                     ((float)C.B) / 255.0f,
//                     ((float)C.A) / 255.0f);
// 
//                 return Out;
//             }
//             set
//             {
//                 value.X *= 255.0f;
//                 value.Y *= 255.0f;
//                 value.Z *= 255.0f;
//                 value.W *= 255.0f;
// 
//                 Parent.Properties[8].Value = Color.FromArgb(
//                     (byte)value.W,
//                     (byte)value.X,
//                     (byte)value.Y,
//                     (byte)value.Z);
//                 Parent.PropertyUpdate(8);
//             }
//         }
// 
//         [CategoryAttribute("Properties"), TypeConverterAttribute(typeof(Vector4ColorOptionsConverter)), Editor(typeof(Vector4ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
//         public Vector4 ForeColor
//         {
//             get
//             {
//                 Color C = (Color)Parent.Properties[9].Value;
//                 Vector4 Out = new Vector4(
//                     ((float)C.R) / 255.0f,
//                     ((float)C.G) / 255.0f,
//                     ((float)C.B) / 255.0f,
//                     ((float)C.A) / 255.0f);
// 
//                 return Out;
//             }
//             set
//             {
//                 value.X *= 255.0f;
//                 value.Y *= 255.0f;
//                 value.Z *= 255.0f;
//                 value.W *= 255.0f;
// 
//                 Parent.Properties[9].Value = Color.FromArgb(
//                     (byte)value.W,
//                     (byte)value.X,
//                     (byte)value.Y,
//                     (byte)value.Z);
//                 Parent.PropertyUpdate(9);
//             }
//         }
// 
//         [CategoryAttribute("Properties")]
//         public int Skin
//         {
//             get { return (int)Parent.Properties[10].Value; }
//             set { Parent.Properties[10].Value = value; Parent.PropertyUpdate(10); }
//         }
// 
//         /*
//         [CategoryAttribute("Properties")]
//         public TextAlign Align
//         {
//             get { return (TextAlign)Parent.Properties[11].Value; }
//             set { Parent.Properties[11].Value = value; Parent.PropertyUpdate(11); }
//         }
//         */
// 
//         [CategoryAttribute("Properties"), TypeConverterAttribute(typeof(CollectionOptionsConverter)), Editor(typeof(CollectionTypeEditor), typeof(System.Drawing.Design.UITypeEditor))]
//         public List<string> Items
//         {
//             get { return (List<string>)Parent.Properties[12].Value; }
//             set { Parent.Properties[12].Value = value; Parent.PropertyUpdate(12); }
//         }
    }
}
