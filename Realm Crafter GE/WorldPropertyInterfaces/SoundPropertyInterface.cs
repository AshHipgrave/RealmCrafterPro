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
/* This class interfaces the property grid with the selected SoundZone object
 * retrieving information from the SoundZone object class
 * Author: Shane Smith, Aug 2008
 */

using System;
using System.ComponentModel;
using IrrlichtSound;
using RealmCrafter.ClientZone;
using RealmCrafter;

namespace RealmCrafter_GE.Property_Interfaces
{
    public class SoundPropertyInterface
    {
        private SoundZone _Object;

        public SoundPropertyInterface(SoundZone Object)
        {
            _Object = Object;
        }

        #region Entity

        #region Location
        [Category("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }
            set
            {
                _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z());
                //// Program.GE.PositionSelection(value, _Object.EN.Y(), _Object.EN.Z());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }

        [Category("Location")]
        public float LocY
        {
            get { return _Object.EN.Y(); }
            set
            {
                _Object.EN.Position(_Object.EN.X(), value, _Object.EN.Z());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
            }
        }

        [Category("Location")]
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
        [Category("Scale")]
        public float Scale
        {
            get { return _Object.EN.ScaleX(); }

            set { _Object.EN.Scale(value, value, value); }
        }

        #endregion

        #endregion

        #region Sound Properties
        [Category("Sound properties"),
         Description("Percentage")]
        public byte Volume
        {
            get { return _Object.Volume; }
            set
            {
                if (value > 100)
                {
                    value = 100;
                }

                if (value < 1)
                {
                    value = 1;
                }

                _Object.Volume = value;
            }
        }

        [Category("Sound properties"),
         Description("Time in seconds to wait before repeating the sound, -1 to play once only")]
        public int SecondsUntilRepeat
        {
            get { return _Object.RepeatTime; }
            set
            {
                if (value > 1000)
                {
                    value = 1000;
                }

                if (value < -1)
                {
                    value = -1;
                }

                _Object.RepeatTime = value;
            }
        }

        [Category("Sound properties"),
         TypeConverter(typeof(PropertySoundList))]
        public String SoundID
        {
            get { return GE.NiceSoundName(_Object.SoundID); }
            set
            {
                string s = value;
                int ColonIndex = s.LastIndexOf(":");
                string q = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
                ushort SelectedSoundID = Convert.ToUInt16(q);
                Sound F = Media.GetSound(SelectedSoundID);
                if (F != null)
                {
                    if (_Object.MusicID != 65535)
                    {
                        _Object.MusicID = 65535;
                    }

                    Program.GE.m_propertyWindow.RefreshObjectWindow();
                    _Object.SoundID = SelectedSoundID;
                }
            }
        }

        [Category("Sound properties"),
         TypeConverter(typeof(PropertyMusicList))]
        public string MusicID
        {
            get { return GE.NiceMusicName(_Object.MusicID); }
            set
            {
                string s = value;
                int ColonIndex = s.LastIndexOf(":");
                string q = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
                ushort SelectedMusicID = Convert.ToUInt16(q);
                string F = Media.GetMusicName(SelectedMusicID);
                if (F != "")
                {
                    if (_Object.SoundID != 65535)
                    {
                        _Object.SoundID = 65535;
                    }

                    Program.GE.m_propertyWindow.RefreshObjectWindow();
                    _Object.MusicID = SelectedMusicID;
                }
            }
        }
        #endregion
    }
}