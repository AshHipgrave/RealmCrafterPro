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
/* This class interfaces the property grid with the selected Trigger object
 * retrieving information from the Trigger object class
 * Author: Shane Smith, Aug 2008
 */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using RenderingServices;

namespace RealmCrafter_GE.Property_Interfaces
{
    public class WaypointPropertyInterface
    {
        private Waypoint _Object;
        private ushort ActorValue;

        public WaypointPropertyInterface(Waypoint Object)
        {
            _Object = Object;
        }

        #region Location
        [CategoryAttribute("Location")]
        public float LocX
        {
            get { return _Object.EN.X(); }
            set
            {
                _Object.EN.Position(value, _Object.EN.Y(), _Object.EN.Z());
                _Object.UpdateServerVersion(Program.GE.CurrentServerZone);
                Program.GE.UpdateWaypointLinks();
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
                Program.GE.UpdateWaypointLinks();
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
                Program.GE.UpdateWaypointLinks();
            }
        }
        #endregion

        #region Actor Spawns
        [CategoryAttribute("Actor Spawning")]
        public ushort SpawnQuantity
        {
            get
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    return Program.GE.CurrentServerZone.SpawnMax[SP];
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    Program.GE.CurrentServerZone.SpawnMax[SP] = value;
                }
            }
        }

        [CategoryAttribute("Actor Spawning")]
        public ushort SpawnDelay
        {
            get
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    return Program.GE.CurrentServerZone.SpawnFrequency[SP];
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    Program.GE.CurrentServerZone.SpawnFrequency[SP] = value;
                }
            }
        }

        [CategoryAttribute("Actor Spawning")]
        public float AutoMoveRange
        {
            get
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    return Program.GE.CurrentServerZone.SpawnRange[SP];
                }
                else
                {
                    return 0;
                }
            }

            set
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    Program.GE.CurrentServerZone.SpawnRange[SP] = value;
                    if (Program.GE.CurrentServerZone.SpawnRange[SP] < 2f)
                    {
                        Entity ChildEN = _Object.EN.FindChild("Waypoint Auto-movement Sphere");
                        if (ChildEN != null)
                        {
                            ChildEN.Free();
                        }
                    }
                    else
                    {
                        Entity ChildEN = _Object.EN.FindChild("Waypoint Auto-movement Sphere");
                        if (ChildEN == null)
                        {
                            ChildEN = Entity.CreateSphere();
                            ChildEN.Name = "Waypoint Auto-movement Sphere";
                            ChildEN.Texture(Program.GE.BlueTex);
                            ChildEN.Shader = Shaders.FullbrightAlpha;
                            ChildEN.AlphaState = true;
                            ChildEN.AlphaNoSolid(0.5f);
                            ChildEN.Parent(_Object.EN, true);
                            ChildEN.Position(0f, 0f, 0f);
                        }

                        ChildEN.Scale(Program.GE.CurrentServerZone.SpawnRange[SP],
                                      Program.GE.CurrentServerZone.SpawnRange[SP],
                                      Program.GE.CurrentServerZone.SpawnRange[SP], true);
                    }
                }
            }
        }

        [CategoryAttribute("Actor Spawning"),
         TypeConverter(typeof(PropertyActorsList))]
        public string SpawnActor
        {
            get
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);

                if (SP >= 0)
                {
                    List<string> ActorNames = GE.ActorList();

                    ActorValue = Program.GE.CurrentServerZone.SpawnActor[_Object.ServerID];
                    string Actor = ActorNames.Find(IsActorID);
                    return Actor;
                }
                else
                {
                    return "";
                }
            }

            set
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);

                if (SP >= 0 && value == "")
                {
                    Program.GE.CurrentServerZone.SpawnMax[SP] = 0;
                    _Object.EN.Scale(3f, 3f, 3f);
                    return;
                }
                else if (SP < 0 && value != "")
                {
                    string s = value;
                    int ColonIndex = s.LastIndexOf(":");
                    string q = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
                    ushort SelectedActorID;
                    SelectedActorID = Convert.ToUInt16(q);

                    // Find free spawn point
                    bool Found = false;
                    for (int i = 0; i < 1000; ++i)
                    {
                        if (Program.GE.CurrentServerZone.SpawnMax[i] == 0)
                        {
                            Program.GE.CurrentServerZone.SpawnActor[i] = SelectedActorID;
                            Program.GE.CurrentServerZone.SpawnWaypoint[i] = (ushort) _Object.ServerID;
                            Program.GE.CurrentServerZone.SpawnMax[i] = 1;
                            Program.GE.CurrentServerZone.SpawnScript[i] = "";
                            Program.GE.CurrentServerZone.SpawnActorScript[i] = "";
                            Program.GE.CurrentServerZone.SpawnDeathScript[i] = "";
                            Program.GE.CurrentServerZone.SpawnRange[i] = 0f;
                            Program.GE.CurrentServerZone.SpawnFrequency[i] = 10;
                            Program.GE.CurrentServerZone.SpawnSize[i] = 5f;
                            _Object.EN.Scale(5f, 5f, 5f);
                            Program.GE.SetWorldSavedStatus(false);
                            Found = true;
                            break;
                        }
                    }

                    if (!Found)
                    {
                        MessageBox.Show("Limit of 1000 spawn points per zone already reached!", "Error");
                    }
                }
                else if (SP >= 0 && value != "")
                {
                    string s = value;
                    int ColonIndex = s.LastIndexOf(":");
                    string ActorID = s.Substring(ColonIndex + 2, s.Length - ColonIndex - 2);
                    ushort SelectedActorID = Convert.ToUInt16(ActorID);

                    if (Program.GE.CurrentServerZone.SpawnActor[SP] != SelectedActorID)
                    {
                        Program.GE.CurrentServerZone.SpawnActor[SP] = SelectedActorID;
                    }
                }
            }
        }

        private bool IsActorID(string ActorString)
        {
            if (ActorString.Length > 0)
            {
                int ColonIndex = ActorString.LastIndexOf(":");
                if (ColonIndex == 0)
                {
                    return false;
                }

                string ActorID = ActorString.Substring(ColonIndex + 2, ActorString.Length - ColonIndex - 2);
                if (ActorValue.ToString() == ActorID)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /*
        public ushort SpawnActorRaw
        {
            get
            {
                ushort ActorValue;
                ActorValue = Program.GE.CurrentServerZone.SpawnActor[_Object.ServerID];

                return ActorValue;
            }
        }
        */
        #endregion

        #region Actor Pausing
        [CategoryAttribute("Actor Delay")]
        public int WaypointPause
        {
            get { return Program.GE.CurrentServerZone.WaypointPause[_Object.ServerID]; }

            set { Program.GE.CurrentServerZone.WaypointPause[_Object.ServerID] = value; }
        }
        #endregion

        #region Scripts
        [TypeConverter(typeof(PropertyScriptList)),
         CategoryAttribute("Scripts")]
        public string SpawnScript
        {
            get
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    return Program.GE.CurrentServerZone.SpawnScript[SP];
                }
                else
                {
                    return "";
                }
            }

            set
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    if (value == "(None)")
                    {
                        value = "";
                    }

                    Program.GE.CurrentServerZone.SpawnScript[SP] = value;
                }
            }
        }

        [TypeConverter(typeof(PropertyScriptList)),
         CategoryAttribute("Scripts")]
        public string DeathScript
        {
            get
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    return Program.GE.CurrentServerZone.SpawnDeathScript[SP];
                }
                else
                {
                    return "";
                }
            }

            set
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    if (value == "(None)")
                    {
                        value = "";
                    }

                    Program.GE.CurrentServerZone.SpawnDeathScript[SP] = value;
                }
            }
        }

        [TypeConverter(typeof(PropertyScriptList)),
         CategoryAttribute("Scripts")]
        public string InteractScript
        {
            get
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    return Program.GE.CurrentServerZone.SpawnActorScript[SP];
                }
                else
                {
                    return "";
                }
            }

            set
            {
                int SP = Program.GE.CurrentServerZone.GetSpawnPoint(_Object.ServerID);
                if (SP >= 0)
                {
                    if (value == "(None)")
                    {
                        value = "";
                    }

                    Program.GE.CurrentServerZone.SpawnActorScript[SP] = value;
                }
            }
        }
        #endregion
    }
}