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
 * This class common control interface
 * Author: Yeisnier Dominguez Silva, March 2009
 */

using System;
using System.ComponentModel;
using System.Xml;
using NGUINet;

namespace RealmCrafter_GE.Property_Interfaces
{
    public enum eLocationType
    {
        ABSOLUTE_ABSOLUTE,
        ABSOLUTE_RELATIVE,
        ABSOLUTE_CENTER,

        RELATIVE_ABSOLUTE,
        RELATIVE_RELATIVE,
        RELATIVE_CENTER,

        CENTER_ABSOLUTE,
        CENTER_RELATIVE,
        CENTER_CENTER
   };
    public enum eSizeType
    {
        ABSOLUTE_ABSOLUTE,
        ABSOLUTE_RELATIVE,

        RELATIVE_ABSOLUTE,
        RELATIVE_RELATIVE
    };

    public partial class cControl
    {
        protected string ToSaveName, ToSaveParent;
        protected string OverrideImage = "";
        public NControl control;

        protected eLocationType Position_Type;
        protected eSizeType Size_Type;
        protected NVector2 Location, Size;

        #region Position
        [CategoryAttribute("Position")]
        public eLocationType posType
         {
             get { return Position_Type; }
             set 
             { 
                 Position_Type = value;
                 Location = LocationFromNVector2(control.Location, Position_Type);
             }
         }

         [CategoryAttribute("Position")]
         public float PosX
         {
             get { return Location.X; }
             set
             {
                 Location = new NVector2(value, Location.Y);
                 control.Location = NVector2FromLocation(Location, Position_Type);
             }
         }

         [CategoryAttribute("Position")]
         public float PosY
         {
             get { return Location.Y; }
             set
             {
                 Location = new NVector2(Location.X, value);
                 control.Location = NVector2FromLocation(Location, Position_Type);
             }
         }
        #endregion

        #region Size
        [CategoryAttribute("Size")]
        public eSizeType sizeType
        {
            get { return Size_Type; }
            set
            {
                Size_Type = value;
                Size = SizeFromNVector2(control.Size, Size_Type);
            }
        }
        [CategoryAttribute("Size")]
        public float Width
        {
            get { return Size.X; }
            set
            {
                control.Size = NVector2FromSize(new NVector2(value, Size.Y), Size_Type);
                Size = SizeFromNVector2(control.Size, Size_Type);
            }
        }
        [CategoryAttribute("Size")]
        public float Height
        {
            get { return Size.Y; }
            set
            {
                control.Size = NVector2FromSize(new NVector2(Size.X, value), Size_Type);
                Size = SizeFromNVector2(control.Size, Size_Type);
            }

        }
        #endregion

        #region Text
        [CategoryAttribute("Text")]
        public string Text
        {
            get { return control.Text; }
            set { control.Text = value; }
        }
        #endregion

        #region Color
        //[CategoryAttribute("Color")]
        //public System.Drawing.Color BackColor
        //{
        //    get { return (System.Drawing.Color)control.BackColor; }
        //    set { control.BackColor = value; }
        //}
        [CategoryAttribute("Color"), TypeConverter(typeof(RenderingServices.Vector4OptionsConverter)), Editor(typeof(RenderingServices.Vector4ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public RenderingServices.Vector4 BackColor
        {
            get
            {
                RenderingServices.Vector4 C = new RenderingServices.Vector4();
                System.Drawing.Color sC = (System.Drawing.Color)control.BackColor;
                C.X = Convert.ToSingle(sC.R) / 255.0f;
                C.Y = Convert.ToSingle(sC.G) / 255.0f;
                C.Z = Convert.ToSingle(sC.B) / 255.0f;
                C.W = Convert.ToSingle(sC.A) / 255.0f;
                return C;
                //return (System.Drawing.Color)control.ForeColor;
            }
            set
            {
                control.BackColor = System.Drawing.Color.FromArgb(
                    Convert.ToInt32(value.W * 255.0f),
                    Convert.ToInt32(value.X * 255.0f),
                    Convert.ToInt32(value.Y * 255.0f),
                    Convert.ToInt32(value.Z * 255.0f));
                //control.ForeColor = value; 
            }
        }

        [CategoryAttribute("Color"), TypeConverter(typeof(RenderingServices.Vector4OptionsConverter)), Editor(typeof(RenderingServices.Vector4ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public RenderingServices.Vector4 ForeColor
        {
            get
            {
                RenderingServices.Vector4 C = new RenderingServices.Vector4();
                System.Drawing.Color sC = (System.Drawing.Color)control.ForeColor;
                C.X = Convert.ToSingle(sC.R) / 255.0f;
                C.Y = Convert.ToSingle(sC.G) / 255.0f;
                C.Z = Convert.ToSingle(sC.B) / 255.0f;
                C.W = Convert.ToSingle(sC.A) / 255.0f;
                return C;
                //return (System.Drawing.Color)control.ForeColor;
            }
            set
            {
                control.ForeColor = System.Drawing.Color.FromArgb(
                    Convert.ToInt32(value.W * 255.0f),
                    Convert.ToInt32(value.X * 255.0f),
                    Convert.ToInt32(value.Y * 255.0f),
                    Convert.ToInt32(value.Z * 255.0f));
                //control.ForeColor = value; 
            }
        }
        #endregion

//         #region Skin
//         [CategoryAttribute("Skin")]
//         public int Skin
//         {
//             get { return control.Skin; }
//             set { control.Skin = value; }
//         }
//         #endregion

        public void SynchonizeLocation( NVector2 d )
        {
            NVector2 Res = GE.GUIManager.GetResolution();
            switch (Position_Type)
            {
                case eLocationType.ABSOLUTE_ABSOLUTE:
                    if (( (Location.X + d.X) * Location.X) < 0) Location.X = 1*((Location.X>=0)?1:-1);
                    else Location.X += d.X;
                    if (((Location.Y + d.Y) * Location.Y) < 0) Location.Y = 1 * ((Location.Y >= 0) ? 1 : -1);
                    else Location.Y += d.Y;
                    break;
                case eLocationType.ABSOLUTE_CENTER:
                case eLocationType.ABSOLUTE_RELATIVE:
                    if (((Location.X + d.X) * Location.X) < 0) Location.X = 1 * ((Location.X >= 0) ? 1 : -1);
                    else Location.X += d.X;
                    Location.Y += d.Y * 100 / Res.Y; 
                    break;

                case eLocationType.RELATIVE_ABSOLUTE:
                    if (((Location.Y + d.Y) * Location.Y) < 0) Location.Y = 1 * ((Location.Y >= 0) ? 1 : -1);
                    else Location.Y += d.Y;
                    Location.X += d.X * 100 / Res.X;
                    break;
                case eLocationType.RELATIVE_CENTER:
                case eLocationType.RELATIVE_RELATIVE:
                    Location.X += d.X * 100 / Res.X;
                    Location.Y += d.Y * 100 / Res.Y;
                    break;

                case eLocationType.CENTER_ABSOLUTE:
                    Location.X += d.X * 100 / Res.X;
                    if (((Location.Y + d.Y) * Location.Y) < 0) Location.Y = 1 * ((Location.Y >= 0) ? 1 : -1);
                    else Location.Y += d.Y;
                    break;
                case eLocationType.CENTER_CENTER:
                case eLocationType.CENTER_RELATIVE:
                    Location.X += d.X * 100 / Res.X;
                    Location.Y += d.Y * 100 / Res.Y;
                    break;
            }

            control.Location = NVector2FromLocation(Location, Position_Type);
        }

        private NVector2 LocationFromNVector2(NVector2 v, eLocationType PositionType)
        {
            float X = v.X, Y = v.Y;
            NVector2 Res = GE.GUIManager.GetResolution();

            switch (PositionType)
            {
                case eLocationType.ABSOLUTE_ABSOLUTE:
                    return new NVector2(X * Res.X, Y * Res.Y);
                    break;
                case eLocationType.ABSOLUTE_CENTER:
                    control.Location = new NVector2(control.Location.X, control.Location.Y - control.Size.Y / 2);
                    return new NVector2(X * Res.X, Y * 100);
                    break;
                case eLocationType.ABSOLUTE_RELATIVE:
                    return new NVector2(X * Res.X, Y * 100);
                    break;

                case eLocationType.RELATIVE_ABSOLUTE:
                    return new NVector2(X * 100, Y * Res.Y);
                    break;
                case eLocationType.RELATIVE_CENTER:
                    control.Location = new NVector2(control.Location.X, control.Location.Y - control.Size.Y / 2);
                    return new NVector2(X * 100, Y * 100);
                    break;
                case eLocationType.RELATIVE_RELATIVE:
                    return new NVector2(X * 100, Y * 100);
                    break;

                case eLocationType.CENTER_ABSOLUTE:
                    control.Location = new NVector2(control.Location.X - control.Size.X / 2, control.Location.Y);
                    return new NVector2(X * 100, Y * Res.Y);
                    break;
                case eLocationType.CENTER_CENTER:
                    control.Location = new NVector2(control.Location.X - control.Size.X / 2,
                                                    control.Location.Y - control.Size.Y / 2);
                    return new NVector2(X * 100, Y * 100);
                    break;
                case eLocationType.CENTER_RELATIVE:
                    control.Location = new NVector2(control.Location.X - control.Size.X / 2, control.Location.Y);
                    return new NVector2(X * 100, Y * 100);
                    break;
            }

            return new NVector2(X, Y);
        }
        virtual protected NVector2 NVector2FromLocation(NVector2 v, eLocationType PositionType)
        {
            float X = v.X, Y = v.Y;
            NVector2 Res = GE.GUIManager.GetResolution();

            switch (PositionType)
            {
                case eLocationType.ABSOLUTE_ABSOLUTE:
                    if (X < 0 && Y < 0) return new NVector2(1 + (X / Res.X/* - control.Size.X*/), 1 + (Y / Res.Y/* - control.Size.Y*/) );
                    else
                    if (X < 0) return new NVector2(1 + (X / Res.X /*- control.Size.X*/), Y / Res.Y);
                    else
                    if (Y < 0) return new NVector2(X / Res.X, 1 + (Y / Res.Y/* - control.Size.Y*/));
                    return new NVector2(X / Res.X, Y / Res.Y);
                    break;
                case eLocationType.ABSOLUTE_CENTER:
                    if (X < 0) return new NVector2(1 + (X / Res.X/* - control.Size.X*/), Y / 100 + control.Size.Y / 2);
                    return new NVector2(X / Res.X, Y / 100 - control.Size.Y / 2);
                    break;
                case eLocationType.ABSOLUTE_RELATIVE:
                    if (X < 0) return new NVector2(1 + (X / Res.X/* - control.Size.X*/), Y / 100);
                    return new NVector2(X / Res.X, Y / 100);
                    break;

                case eLocationType.RELATIVE_ABSOLUTE:
                    if (Y < 0) return new NVector2(X / 100, 1 + (Y / Res.Y/* - control.Size.Y*/));
                    return new NVector2(X / 100, Y / Res.Y);
                    break;
                case eLocationType.RELATIVE_CENTER:
                    return new NVector2(X / 100, Y / 100 - control.Size.Y / 2);
                    break;
                case eLocationType.RELATIVE_RELATIVE:
                    return new NVector2(X / 100, Y / 100);
                    break;

                case eLocationType.CENTER_ABSOLUTE:
                    if (Y < 0) return new NVector2(X / 100 + control.Size.X / 2, 1 + (Y / Res.Y/* - control.Size.Y*/));
                    return new NVector2(X / 100 - control.Size.X / 2, Y / Res.Y);
                    break;
                case eLocationType.CENTER_CENTER:
                    return new NVector2(X/100 - control.Size.X / 2, Y/100 - control.Size.Y / 2);
                    break;
                case eLocationType.CENTER_RELATIVE:
                    return new NVector2(X / 100 - control.Size.X / 2, Y / 100);
                    break;
            }

            return new NVector2(X, Y);
        }
        protected string StringFromLocation(NVector2 v, eLocationType PositionType)
        {
            string[] vX = { "", "", "", "%", "%", "%", "c", "c", "c" };
            string[] vY = { "", "%", "c", "", "%", "c", "", "%", "c" };

            return v.X.ToString() + vX[(int)PositionType] + ", " + v.Y.ToString() + vY[(int)PositionType];
        }
        protected NVector2 LocationFromString(string attrib, ref eLocationType PositionType)
        {
            string s = ",";
            string[] xy = attrib.Split(s.ToCharArray(),StringSplitOptions.RemoveEmptyEntries);

            if (xy.Length != 2) return new NVector2(0, 0);

            int x = 0, y = 0; // 0-Absolute, 1-Relative, 2-Center
            xy[0] = xy[0].Trim();
            xy[1] = xy[1].Trim();

            // X
            if (xy[0][xy[0].Length - 1] == '%')
            {
                x = 1; // relative
                xy[0] = xy[0].Substring(0, xy[0].Length - 1);
            }
            else
            if (xy[0][xy[0].Length - 1] == 'c')
            {
                x = 2; // center
                xy[0] = xy[0].Substring(0, xy[0].Length - 1);
            }

            // Y
            if (xy[1][xy[1].Length - 1] == '%')
            {
                y = 1; // relative
                xy[1] = xy[1].Substring(0, xy[1].Length - 1);
            }
            else
            if (xy[1][xy[1].Length - 1] == 'c')
            {
                y = 2; // center
                xy[1] = xy[1].Substring(0, xy[1].Length - 1);
            }

            PositionType = (eLocationType)(x * 3 + y);
            return new NVector2((float)System.Convert.ToDouble(xy[0]), (float)System.Convert.ToDouble(xy[1]));
        }
        NVector2 OnLoadLocation(NVector2 v, eLocationType PositionType)
        {
            float X = v.X, Y = v.Y;
            NVector2 Res = GE.GUIManager.GetResolution();
            switch (PositionType)
            {
                case eLocationType.ABSOLUTE_ABSOLUTE:
                    return new NVector2(X / Res.X, Y / Res.Y);
                    break;
                case eLocationType.ABSOLUTE_RELATIVE:
                    return new NVector2(X / Res.X, Y / 100);
                    break;
                case eLocationType.ABSOLUTE_CENTER:
                    return new NVector2(X / Res.X, (Y - control.Size.Y / 2) / 100);
                    break;

                case eLocationType.RELATIVE_ABSOLUTE:
                    return new NVector2(X / 100, Y / Res.Y);
                    break;
                case eLocationType.RELATIVE_RELATIVE:
                    return new NVector2(X / 100, Y / 100);
                    break;
                case eLocationType.RELATIVE_CENTER:
                    return new NVector2(X / 100, (Y - control.Size.Y / 2) / 100);
                    break;

                case eLocationType.CENTER_ABSOLUTE:
                    return new NVector2((X - control.Size.X / 2) / 100 , Y / Res.Y);
                    break;
                case eLocationType.CENTER_RELATIVE:
                    return new NVector2((X - control.Size.X / 2) / 100, Y / 100);
                    break;
                case eLocationType.CENTER_CENTER:
                    return new NVector2((X - control.Size.X / 2) / 100, (Y - control.Size.Y / 2) / 100);
                    break;
            }

            return new NVector2(X, Y);
        }

        virtual protected NVector2 SizeFromNVector2(NVector2 v, eSizeType SizeType)
        {
            float X = v.X, Y = v.Y;
            NVector2 Res = GE.GUIManager.GetResolution();

            switch (SizeType)
            {
                case eSizeType.ABSOLUTE_ABSOLUTE:
                    return new NVector2(X * Res.X, Y * Res.Y);
                    break;
                case eSizeType.ABSOLUTE_RELATIVE:
                    return new NVector2(X * Res.X, Y * 100);
                    break;

                case eSizeType.RELATIVE_ABSOLUTE:
                    return new NVector2(X * 100, Y * Res.Y);
                    break;
                case eSizeType.RELATIVE_RELATIVE:
                    return new NVector2(X * 100, Y * 100);
                    break;
            }

            return new NVector2(X, Y);
        }
        private NVector2 NVector2FromSize(NVector2 v, eSizeType SizeType)
        {
            float X = v.X, Y = v.Y;
            NVector2 Res = GE.GUIManager.GetResolution();

            switch (SizeType)
            {
                case eSizeType.ABSOLUTE_ABSOLUTE:
                    return new NVector2(X / Res.X, Y / Res.Y);
                    break;
                case eSizeType.ABSOLUTE_RELATIVE:
                    return new NVector2(X / Res.X, Y / 100);
                    break;

                case eSizeType.RELATIVE_ABSOLUTE:
                    return new NVector2(X / 100, Y / Res.Y);
                    break;
                case eSizeType.RELATIVE_RELATIVE:
                    return new NVector2(X / 100, Y / 100);
                    break;
            }

            return new NVector2(X, Y);
        }
        protected string StringFromSize(NVector2 v, eSizeType SizeType)
        {
            string[] vX = { "", "", "%", "%" };
            string[] vY = { "", "%", "", "%" };

            return v.X.ToString() + vX[(int)SizeType] + ", " + v.Y.ToString() + vY[(int)SizeType];
        }
        private NVector2 SizeFromString(string attrib, ref eSizeType SizeType)
        {
            string s = ",";
            string[] xy = attrib.Split(s.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            if (xy.Length != 2) return new NVector2(0, 0);

            int x = 0, y = 0; // 0-Absolute, 1-Relative
            xy[0] = xy[0].Trim();
            xy[1] = xy[1].Trim();

            // X
            if (xy[0][xy[0].Length - 1] == '%')
            {
                x = 1; // relative
                xy[0] = xy[0].Substring(0, xy[0].Length - 1);
            }

            // Y
            if (xy[1][xy[1].Length - 1] == '%')
            {
                y = 1; // relative
                xy[1] = xy[1].Substring(0, xy[1].Length - 1);
            }

            SizeType = (eSizeType)(x * 2 + y);
            return new NVector2((float)System.Convert.ToDouble(xy[0]), (float)System.Convert.ToDouble(xy[1]));
        }
        private NVector2 OnLoadSize(NVector2 v, eSizeType SizeType)
        {
            float X = v.X, Y = v.Y;
            NVector2 Res = GE.GUIManager.GetResolution();
            switch (SizeType)
            {
                case eSizeType.ABSOLUTE_ABSOLUTE:
                    return new NVector2(X / Res.X, Y / Res.Y);
                    break;
                case eSizeType.ABSOLUTE_RELATIVE:
                    return new NVector2(X / Res.X, Y / 100);
                    break;

                case eSizeType.RELATIVE_ABSOLUTE:
                    return new NVector2(X / 100, Y / Res.Y);
                    break;
                case eSizeType.RELATIVE_RELATIVE:
                    return new NVector2(X / 100, Y / 100);
                    break;
            }

            return new NVector2(X, Y);
        }

        int ByteFromHexVal(string val)
        {
            char[] CVal = val.ToLower().ToCharArray();
            if (CVal[0] > 47 && CVal[0] < 58)
                return (byte)(CVal[0] - 48);
            if (CVal[0] > 96 && CVal[0] < 103)
                return (byte)(CVal[0] - 87);
            return 0;
        }

        private ValueType ColorFromString(string Data)
        {
            if(Data.Length > 0 && Data.Substring(0, 1).Equals("#"))
                Data = Data.Substring(1);

            for (int i = Data.Length; i < 8; ++i)
            {
                Data = "0" + Data;
            }

            int A = (ByteFromHexVal(Data.Substring(0, 1)) * 16) + ByteFromHexVal(Data.Substring(1, 1));
            int R = (ByteFromHexVal(Data.Substring(2, 1)) * 16) + ByteFromHexVal(Data.Substring(3, 1));
            int G = (ByteFromHexVal(Data.Substring(4, 1)) * 16) + ByteFromHexVal(Data.Substring(5, 1));
            int B = (ByteFromHexVal(Data.Substring(6, 1)) * 16) + ByteFromHexVal(Data.Substring(7, 1));

            return System.Drawing.Color.FromArgb(A, R, G, B);
        }

        public virtual void LoadControl(XmlTextReader X)
        {
            BackColor = new RenderingServices.Vector4(1, 1, 1, 1);//System.Drawing.Color.White;
            ForeColor = new RenderingServices.Vector4(1, 1, 1, 1);//System.Drawing.Color.White;

            string attrib = X.GetAttribute("size");
            if (attrib != null) control.Size = OnLoadSize(SizeFromString(attrib, ref Size_Type), Size_Type);

            attrib = X.GetAttribute("location");
            if (attrib != null)
            {
                Location = LocationFromString(attrib, ref Position_Type);
                control.Location = NVector2FromLocation(Location, Position_Type);
            }

            attrib = X.GetAttribute("Text");
            if (attrib != null) control.Text = attrib;

            attrib = X.GetAttribute("Visible");
            if (attrib != null) control.Visible = System.Convert.ToBoolean(attrib);

            attrib = X.GetAttribute("Enabled");
            if (attrib != null) control.Enabled = System.Convert.ToBoolean(attrib);

            attrib = X.GetAttribute("BackColor");
            if (attrib != null) control.BackColor = ColorFromString(attrib);

            attrib = X.GetAttribute("ForeColor");
            if (attrib != null) control.ForeColor = ColorFromString(attrib);

            attrib = X.GetAttribute("Skin");
            if (attrib != null) control.Skin = System.Convert.ToInt16(X.GetAttribute(attrib));

            attrib = X.GetAttribute("overrideimage");
            if (attrib != null) OverrideImage = attrib;

//             Location = LocationFromNVector2(control.Location, Position_Type);
            Size = SizeFromNVector2(control.Size, Size_Type);

            //BackColor = (System.Drawing.Color)control.BackColor;
            //ForeColor = (System.Drawing.Color)control.ForeColor;
        }
        public virtual void SaveControl(XmlTextWriter X)
        {
            // NAME
            X.WriteStartAttribute("name");
            X.WriteString( ToSaveName );
            X.WriteEndAttribute();

            // Override image
            if (OverrideImage.Length > 0)
            {
                X.WriteStartAttribute("overrideimage");
                X.WriteString(OverrideImage);
                X.WriteEndAttribute();
            }

            // PARENT
            X.WriteStartAttribute("parent");
            X.WriteString(ToSaveParent);
            X.WriteEndAttribute();

            // LOCATION
            X.WriteStartAttribute("location");
            X.WriteString( StringFromLocation(Location, Position_Type) );
            X.WriteEndAttribute();

            // SIZE
            X.WriteStartAttribute("size");
            X.WriteString( StringFromSize(Size, Size_Type));
            X.WriteEndAttribute();

            // FORECOLOR
            if ( ((System.Drawing.Color)control.ForeColor).ToArgb() != System.Drawing.Color.White.ToArgb() || (control is NLabel))
            {
                X.WriteStartAttribute("ForeColor");
                X.WriteString( "#" + ((System.Drawing.Color)control.ForeColor).Name.ToString());
                X.WriteEndAttribute();
            }

            // BACKCOLOR
            // Use very specific colour for windows because client gives them to random ass texture instead if its not. - Love Ben
            if (((System.Drawing.Color)control.BackColor).ToArgb() != System.Drawing.Color.White.ToArgb() && !(control is NWindow))
            {
                X.WriteStartAttribute("BackColor");
                if(control is NWindow)
                    X.WriteString("#00ffffff");                    
                else
                    X.WriteString("#" + ((System.Drawing.Color)control.BackColor).Name.ToString());
                X.WriteEndAttribute();
            }

           /* Don't think this is actually used and it totally fucks up client visibility - Ben
           X.WriteAttributeString("Visible", control.Visible.ToString());
           */
            // TEXT
            if (control.Name != Text && Text.Length > 0)
            {
                X.WriteStartAttribute("Text");
                X.WriteString(Text);
                X.WriteEndAttribute();
            }
        }
    }
}