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
// Realm Crafter ServerZone module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port November 2006

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using RealmCrafter_GE;
using RenderingServices;
using System.Text;

namespace RealmCrafter.ServerZone
{
    // Everything the server needs to know about a zone (except water)
    public class Zone
    {
        // Index of all zones
        public static ArrayList ZonesList = new ArrayList();
        // Zone name
        public string Name;
        // Environment
        public bool Outdoors;
        public ushort Gravity; // Gravity strength (0 - 1000)

        // Scripts
        public string Script;

        // Sizing and assignments
        public ushort SectorsX = 0;
        public ushort SectorsZ = 0;
        public byte[,] SectorAssignments;

        public List<RealmCrafter_GE.Waypoint> Waypoints = new List<RealmCrafter_GE.Waypoint>();
        public List<RealmCrafter_GE.Portal> Portals = new List<RealmCrafter_GE.Portal>();
        public List<RealmCrafter_GE.Trigger> Triggers = new List<RealmCrafter_GE.Trigger>();



        // Other settings
        public bool PvP;
        // Instances list
        public ZoneInstance[] Instances = new ZoneInstance[100];

        // Constructor
        public Zone(string ZoneName)
        {
            Name = ZoneName;
            Outdoors = false;
            Gravity = 300;
            new ZoneInstance(this, 0);

            Script = "";
            PvP = false;
        }

        // Unload this zone and anything depending on it
        public void Delete()
        {
            // Remove all water areas belonging to this zone
            WaterArea W;
            for (int i = 0; i < WaterArea.WaterList.Count; ++i)
            {
                W = (WaterArea) WaterArea.WaterList[i];
                if (W.Zone == this)
                {
                    WaterArea.WaterList.Remove(W);
                }
            }

            // Remove this zone from the index
            ZonesList.Remove(this);
        }

        public int WaypointIDFromList(Waypoint wp)
        {
            for (int i = 0; i < Waypoints.Count; ++i)
            {
                if (Waypoints[i] == wp)
                    return i;
            }
            return -1;
        }

        // Make a copy of this zone
//         public Zone Copsy(string ZoneName)
//         {
//             Zone Z = new Zone(ZoneName);
// 
//             Z.Outdoors = Outdoors;
//             Z.Gravity = Gravity;
//             Z.PvP = PvP;
//             Z.EntryScript = EntryScript;
//             Z.ActivateScript = ActivateScript;
// 
//             for (int i = 0; i < Triggers.Count; ++i)
//             {
//                 Trigger Tf = Triggers[i];
//                 Trigger Tt = new Trigger(Z, Program.GE, i);
// 
//                 Tt.X = Tf.X;
//                 Tt.Y = Tf.Y;
//                 Tt.Z = Tf.Z;
//                 Tt.Size = Tf.Size;
//                 Tt.Script = Tf.Script;
//             }
// 
//             for (int i = 0; i < Waypoints.Count; ++i)
//             {
//                 Waypoint Wf = Waypoints[i];
//                 Waypoint Wt = new Waypoint(Z, Program.GE, i);
// 
//                 Wt.X = Wf.X;
//                 Wt.Y = Wf.Y;
//                 Wt.Z = Wf.Z;
//                 Wt.TNextA = WaypointIDFromList(Wf.NextA);
//                 Wt.TNextB = WaypointIDFromList(Wf.NextB);
//                 Wt.TPrev = WaypointIDFromList(Wf.Prev);
//                 Wt.PauseTime = Wf.PauseTime;
//                 Wt.ActorID = Wf.ActorID;
//                 Wt.Size = Wf.Size;
//                 Wt.Range = Wf.Range;
//                 Wt.Script = Wf.Script;
//                 Wt.Frequency = Wf.Frequency;
//                 Wt.Max = Wf.Max;
//             }
//             for (int i = 0; i < Z.Waypoints.Count; ++i)
//             {
//                 Waypoint W = Z.Waypoints[i];
// 
//                 if (W.TNextA < Z.Waypoints.Count)
//                     W.NextA = Z.Waypoints[W.TNextA];
//                 if (W.TNextB < Z.Waypoints.Count)
//                     W.NextB = Z.Waypoints[W.TNextB];
//                 if (W.TPrev < Z.Waypoints.Count)
//                     W.Prev = Z.Waypoints[W.TPrev];
//             }
// 
// 
//             for (int i = 0; i < Portals.Count; ++i)
//             {
//                 Portal Pf = Portals[i];
//                 Portal Pt = new Portal(Z, Program.GE, i);
// 
//                 Pt.Name = Pf.Name;
//                 Pt.LinkArea = Pf.LinkArea;
//                 Pt.LinkName = Pf.LinkName;
//                 Pt.X = Pf.X;
//                 Pt.Y = Pf.Y;
//                 Pt.Z = Pf.Z;
//                 Pt.Size = Pf.Size;
//                 Pt.Yaw = Pf.Yaw;
//             }
// 
//             return Z;
//         }

        public static List<string> LoadPortalNames(string name)
        {
            List<string> Names = new List<string>();

            BinaryReader F = Blitz.ReadFile(@"Data\Server Data\Areas\" + name + ".dat");
            if (F == null)
                return Names;

            while (F.BaseStream.Position < F.BaseStream.Length)
            {
                FileChunk Chunk = FileChunk.ReadChunk(F);

                if (Chunk.Name == "PORT")
                {
                    ushort Cnt = F.ReadUInt16();

                    for (int i = 0; i < Cnt; ++i)
                    {
                        Names.Add(Blitz.ReadString(F));

                        Blitz.ReadString(F);
                        Blitz.ReadString(F);

                        
                        F.ReadUInt16();
                        F.ReadUInt16();

                        F.ReadSingle();
                        F.ReadSingle();
                        F.ReadSingle();



                        bool square = F.ReadByte() > 0;

                        if (!square)
                        {
                            F.ReadSingle();
                            F.ReadSingle();
                            F.ReadSingle();
                        }
                        else
                        {
                           F.ReadSingle();
                            F.ReadSingle();
                            F.ReadSingle();
                        }

                    }

                    F.Close();
                    return Names;
                }
                else
                {
                    Chunk.Skip();
                }
            }

            F.Close();
            return Names;
        }

        // Load zone from file
        public static Zone Load(string ZoneName, ClientZone.Zone clientZone)
        {
            BinaryReader F = Blitz.ReadFile(@"Data\Server Data\Areas\" + ZoneName + ".dat");
            if (F == null)
            {
                return null;
            }

            Zone Z = new Zone(ZoneName);

            while (F.BaseStream.Position < F.BaseStream.Length)
            {
                FileChunk Chunk = FileChunk.ReadChunk(F);

                if (Chunk.Name == "PROP" && Chunk.Version == 1)
                {
                    Z.Script = Blitz.ReadString(F);
                    if (!Z.Script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Z.Script += ".cs";
                    }
                    Z.PvP = F.ReadBoolean();
                    Z.Gravity = F.ReadUInt16();
                    Z.Outdoors = F.ReadBoolean();

                    Z.SectorsX = F.ReadUInt16();
                    Z.SectorsZ = F.ReadUInt16();
                    Z.SectorAssignments = new byte[Z.SectorsX, Z.SectorsZ];
                }
                else if (Chunk.Name == "ZONE" && Chunk.Version == 1)
                {
                    if (Z.SectorsX == 0 || Z.SectorsZ == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Zone contained 0 width sectors, skipping sector assignments!");
                        Chunk.Skip();
                        continue;
                    }

                    if (Chunk.Length != Z.SectorsX * Z.SectorsZ)
                    {
                        System.Windows.Forms.MessageBox.Show("Sector assignment lengths were incorrect. Either the chunk is in the wrong order or corrupt. Skipping.");
                        Chunk.Skip();
                        continue;
                    }

                    for (int z = 0; z < Z.SectorsZ; ++z)
                    {
                        for (int x = 0; x < Z.SectorsX; ++x)
                        {
                            Z.SectorAssignments[x, z] = F.ReadByte();
                        }
                    }
                }
                else if (Chunk.Name == "PORT" && Chunk.Version == 1)
                {
                    ushort Cnt = F.ReadUInt16();

                    for (int i = 0; i < Cnt; ++i)
                    {
                        Portal P = new Portal(Z, Program.GE, i);

                        P.Name = Blitz.ReadString(F);
                        P.LinkArea = Blitz.ReadString(F);
                        P.LinkName = Blitz.ReadString(F);

                        ushort SectorX = F.ReadUInt16();
                        ushort SectorZ = F.ReadUInt16();

                        P.X = F.ReadSingle();
                        P.Y = F.ReadSingle();
                        P.Z = F.ReadSingle();

                        P.X += ((float)SectorX * 768.0f);
                        P.Z += ((float)SectorZ * 768.0f);

                        P.IsSquare = F.ReadByte() > 0;

                        if (!P.IsSquare)
                        {
                            P.Size = F.ReadSingle();
                            P.Yaw = F.ReadSingle();
                            F.ReadSingle();
                        }
                        else
                        {
                            P.Width = F.ReadSingle();
                            P.Height = F.ReadSingle();
                            P.Depth = F.ReadSingle();
                        }

//                         P.EN.Position(P.X, P.Y, P.Z);
//                         P.EN.Rotate(0f, P.Yaw, 0f);
//                         P.EN.Scale(P.Size, P.Size, P.Size);
                        P.ReBuild();
                    }
                }
                else if (Chunk.Name == "TRIG" && Chunk.Version == 1)
                {
                    ushort Cnt = F.ReadUInt16();

                    for (int i = 0; i < Cnt; ++i)
                    {
                        ushort SectorX = F.ReadUInt16();
                        ushort SectorZ = F.ReadUInt16();
                        float X = F.ReadSingle();
                        float Y = F.ReadSingle();
                        float pZ = F.ReadSingle();
                        bool IsSquare = F.ReadByte() > 0;
                        float DimX = F.ReadSingle();
                        float DimY = F.ReadSingle();
                        float DimZ = F.ReadSingle();
                        string Script = Blitz.ReadString(F);

                        Trigger T = new Trigger(Z, Program.GE, i);
                        T.X = X + ((float)SectorX * 768.0f);
                        T.Y = Y;
                        T.Z = pZ + ((float)SectorZ * 768.0f);
                        T.IsSquare = IsSquare;

                        if (!T.IsSquare)
                        {
                            T.Size = DimX;
                        }
                        else
                        {
                            T.Width = DimX;
                            T.Height = DimY;
                            T.Depth = DimZ;
                        }

                        T.Script = Script;


//                         T.EN.Position(T.X, T.Y, T.Z);
//                         T.EN.Scale(T.Size, T.Size, T.Size);
                        T.ReBuild();
                    }
                }
                else if (Chunk.Name == "WAYP" && Chunk.Version == 1)
                {
                    ushort Cnt = F.ReadUInt16();

                    for (int i = 0; i < Cnt; ++i)
                    {
                        ushort SectorX = F.ReadUInt16();
                        ushort SectorZ = F.ReadUInt16();
                        float X = F.ReadSingle();
                        float Y = F.ReadSingle();
                        float pZ = F.ReadSingle();
                        ushort TNextA = F.ReadUInt16();
                        ushort TNextB = F.ReadUInt16();
                        ushort TPrev = F.ReadUInt16();
                        int PauseTime = F.ReadInt32();

                        Waypoint W = new Waypoint(Z, Program.GE, i);
                        W.X = X + ((float)SectorX * 768.0f);
                        W.Y = Y;
                        W.Z = pZ + ((float)SectorZ * 768.0f);
                        W.TNextA = TNextA;
                        W.TNextB = TNextB;
                        W.TPrev = TPrev;
                        W.PauseTime = PauseTime;
                    }

                    for (int i = 0; i < Z.Waypoints.Count; ++i)
                    {
                        Waypoint W = Z.Waypoints[i];

                        if (W.TNextA < Z.Waypoints.Count)
                            W.NextA = Z.Waypoints[W.TNextA];
                        if (W.TNextB < Z.Waypoints.Count)
                            W.NextB = Z.Waypoints[W.TNextB];
                        if (W.TPrev < Z.Waypoints.Count)
                            W.Prev = Z.Waypoints[W.TPrev];
                    }
                }
                else if (Chunk.Name == "SPAW" && Chunk.Version == 1)
                {
                    ushort Cnt = F.ReadUInt16();

                    for (int i = 0; i < Cnt; ++i)
                    {
                        ushort ActorID = F.ReadUInt16();
                        ushort WPID = F.ReadUInt16();
                        float Size = F.ReadSingle();
                        string Script = Blitz.ReadString(F);
                        if (Script.Length > 0)
                            Script += ".cs";
                        ushort Max = F.ReadUInt16();
                        ushort Freq = F.ReadUInt16();
                        float Range = F.ReadSingle();

                        if (WPID < Z.Waypoints.Count)
                        {
                            Waypoint W = Z.Waypoints[WPID];
                            W.ActorID = ActorID;
                            W.Size = Size;
                            W.Script = Script;
                            W.Max = Max;
                            W.Frequency = Freq;
                            W.Range = Range;

                            if (W.Max > 0)
                            {
                                Actor A = Actor.Index[ActorID];

                                if (A != null)
                                    W.Label.Text = A.Race + " [" + A.Class + "]";
                                else
                                    W.Label.Text = "UNKNOWN ACTORID";
                            }
                        }

                    }
                }
                else if (Chunk.Name == "WATR" && Chunk.Version == 1)
                {
                    ushort Cnt = F.ReadUInt16();

                    for (int i = 0; i < Cnt; ++i)
                    {
                        ushort SectorX = F.ReadUInt16();
                        ushort SectorZ = F.ReadUInt16();
                        float X = F.ReadSingle();
                        float Y = F.ReadSingle();
                        float pZ = F.ReadSingle();

                        WaterArea W = new WaterArea(Z);
                        W.X = X + ((float)SectorX * 768.0f);
                        W.Y = Y;
                        W.Z = pZ + ((float)SectorZ * 768.0f);
                        W.Width = F.ReadSingle();
                        W.Depth = F.ReadSingle();
                        W.Damage = F.ReadUInt16();
                        W.DamageType = F.ReadUInt16();
                    }
                }
                else if (Chunk.Name == "ISCN" && Chunk.Version == 1)
                {
                    if (clientZone != null)
                    {
                        ushort Cnt = F.ReadUInt16();
                        for (int i = 0; i < Cnt; ++i)
                        {
                            // Read stored data
                            ushort SectorX = F.ReadUInt16();
                            ushort SectorZ = F.ReadUInt16();
                            float pX = F.ReadSingle();
                            float pY = F.ReadSingle();
                            float pZ = F.ReadSingle();
                            ushort MeshID = F.ReadUInt16();
                            string ScriptName = Blitz.ReadString(F);
                            if (!ScriptName.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                                ScriptName += ".cs";

                            pX = (768.0f * (float)SectorX) + pX;
                            pZ = (768.0f * (float)SectorZ) + pZ;

                            // Find associated mesh and populate it
                            foreach (ClientZone.Scenery S in clientZone.Sceneries)
                            {
                                if (S.MeshID == MeshID && Math.Abs(S.EN.X() - pX) < 0.01f && Math.Abs(S.EN.Y() - pY) < 0.01f && Math.Abs(S.EN.Z() - pZ) < 0.01f)
                                {
                                    if (S.Interactive)
                                    {
                                        S.NameScript = ScriptName;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Chunk.Skip();
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("Unable to read zone chunk: " + Chunk.Name + ":" + Chunk.Version.ToString() + ". This data will be skipped, it is recommended that you do not save if the retention of this data is important!");
                    Chunk.Skip();
                }
            }

            for (int i = 0; i < Z.Waypoints.Count; ++i)
            {
                Waypoint WP = Z.Waypoints[i];

                WP.EN.Position(WP.X, WP.Y, WP.Z);
                WP.EN.Scale(3f, 3f, 3f);

                if (WP.Max > 0)
                {
                    WP.EN.Scale(WP.Size, WP.Size, WP.Size);
                    if (WP.Range > 2f)
                    {
                        Entity ChildEN = Entity.CreateSphere();
                        ChildEN.Name = "Waypoint Auto-movement Sphere";
                        ChildEN.Texture(Program.GE.BlueTex);
                        ChildEN.Shader = Shaders.FullbrightAlpha;
                        ChildEN.AlphaState = true;
                        ChildEN.AlphaNoSolid(0.5f);
                        ChildEN.Parent(WP.EN, true);
                        ChildEN.Position(0f, 0f, 0f);
                        ChildEN.Scale(WP.Range, WP.Range, WP.Range, true);
                    }
                }
            }

            F.Close();

            return Z;
        }

        static void WriteSectorVector(BinaryWriter o, float x, float y, float z)
        {
            ushort SectorX = 0;
            ushort SectorZ = 0;

            while (x > 768.0f)
            {
                x -= 768.0f;
                ++SectorX;
            }

            while (z > 768.0f)
            {
                z -= 768.0f;
                ++SectorZ;
            }

            o.Write(SectorX);
            o.Write(SectorZ);
            o.Write(x);
            o.Write(y);
            o.Write(z);
        }

        static Stack<long> Chunks = new Stack<long>();

        public static void WriteStartChunk(BinaryWriter o, string name, byte version)
        {
            byte[] nameBytes = Encoding.ASCII.GetBytes(name);

            o.Write(nameBytes, 0, 4);
            o.Write(version);

            // We need to write back the length
            Chunks.Push(o.BaseStream.Position);
            o.Write((int)0);
        }

        public static void WriteEndChunk(BinaryWriter o)
        {
            if (Chunks.Count == 0)
                throw new Exception("Cannot close chunk, none left to close");

            long Position = Chunks.Pop();
            long PosNow = o.BaseStream.Position;
            long Length = PosNow - (Position + 4);

            o.BaseStream.Position = Position;
            o.Write((int)Length);
            o.BaseStream.Position = PosNow;
        }

        // Save this zone to file
        public bool Save(ClientZone.Zone clientZone)
        {
            if (!RealmCrafter_GE.Program.DemoVersion)
            {
                Chunks.Clear();
                BinaryWriter O = Blitz.WriteFile(@"Data\Server Data\Areas\" + Name + ".dat");
                if (O == null)
                    throw new Exception("Could not write to: " + @"Data\Server Data\Areas\" + Name + ".dat");

//                 float MaxX = 0.0f;
//                 float MaxZ = 0.0f;
// 
//                 if (clientZone != null)
//                 {
//                     foreach (ClientZone.Scenery Scn in clientZone.Sceneries)
//                     {
//                         if (Scn.EN != null)
//                         {
//                             float Mx = Scn.EN.MeshWidth + Scn.EN.X();
//                             float Mz = Scn.EN.MeshDepth + Scn.EN.Z();
// 
//                             MaxX = Math.Max(MaxX, Mx);
//                             MaxZ = Math.Max(MaxZ, Mz);
//                         }
//                     }
//                 }

                // TODO: Change this when MT2 lands (well, just if() it).
                //SectorsX

                // Write major properties first
                WriteStartChunk(O, "PROP", 1);

                if(Script.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                    Blitz.WriteString(Script.Substring(0, Script.Length - 3), O);
                else
                    Blitz.WriteString(Script, O);
                O.Write(PvP);
                O.Write(Gravity);
                O.Write(Outdoors);

                // Dimensions (these default to 10,10 for all converted zones, GE will update)
                O.Write((ushort)10);
                O.Write((ushort)10);

                WriteEndChunk(O);

                // Zone sector assignments (0 byte default, MT allows this to change)
                WriteStartChunk(O, "ZONE", 1);

                for (int z = 0; z < 10; ++z)
                    for (int x = 0; x < 10; ++x)
                        O.Write((byte)0);

                WriteEndChunk(O);

                // Portals
                WriteStartChunk(O, "PORT", 1);

                O.Write((ushort)Portals.Count);

                foreach (Portal P in Portals)
                {
                    Blitz.WriteString(P.Name, O);
                    Blitz.WriteString(P.LinkArea, O);
                    Blitz.WriteString(P.LinkName, O);

                    WriteSectorVector(O, P.X, P.Y, P.Z);
                    O.Write((byte)(P.IsSquare ? 1 : 0)); // IsSquare
                    if (P.IsSquare)
                    {
                        O.Write(P.Width);
                        O.Write(P.Height);
                        O.Write(P.Depth);
                    }
                    else
                    {
                        O.Write(P.Size); // Radius or Width
                        O.Write(P.Yaw); // Yaw or Height
                        O.Write(0.0f); // Nothing or Depth
                    }
                }

                WriteEndChunk(O);

                // Triggers
                WriteStartChunk(O, "TRIG", 1);

                O.Write((ushort)Triggers.Count);
                foreach (Trigger T in Triggers)
                {
                    WriteSectorVector(O, T.X, T.Y, T.Z);
                    O.Write((byte)(T.IsSquare ? 1 : 0)); // IsSquare
                    if (T.IsSquare)
                    {
                        O.Write(T.Width);
                        O.Write(T.Height);
                        O.Write(T.Depth);
                    }
                    else
                    {
                        O.Write(T.Size); // Radius or Width
                        O.Write(0.0f); // Nothing or Height
                        O.Write(0.0f); // Nothing or Depth
                    }
                    Blitz.WriteString(T.Script, O);
                }

                WriteEndChunk(O);

                // Waypoints
                WriteStartChunk(O, "WAYP", 1);

                O.Write((ushort)Waypoints.Count);
                foreach (Waypoint WP in Waypoints)
                {
                    WriteSectorVector(O, WP.X, WP.Y, WP.Z);
                    O.Write((ushort)WaypointIDFromList(WP.NextA));
                    O.Write((ushort)WaypointIDFromList(WP.NextB));
                    O.Write((ushort)WaypointIDFromList(WP.Prev));
                    O.Write(WP.PauseTime);
                }

                WriteEndChunk(O);

                // Spawns
                WriteStartChunk(O, "SPAW", 1);

                ushort SpawnsCount = 0;
                foreach (Waypoint WP in Waypoints)
                {
                    if(WP.Max > 0)
                        ++SpawnsCount;
                }

                O.Write((ushort)SpawnsCount);

                foreach (Waypoint S in Waypoints)
                {
                    if (S.Max > 0)
                    {
                        O.Write((ushort)S.ActorID);
                        O.Write((ushort)WaypointIDFromList(S));
                        O.Write(S.Size);
                        string ScriptOut = S.Script;
                        if (ScriptOut.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                            ScriptOut = ScriptOut.Substring(0, ScriptOut.Length - 3);
                        Blitz.WriteString(ScriptOut, O);
                        O.Write((ushort)S.Max);
                        O.Write((ushort)S.Frequency);
                        O.Write(S.Range);
                    }
                }

                WriteEndChunk(O);

                // Waters
                WriteStartChunk(O, "WATR", 1);

                WaterArea W;
                ushort WaterCount = 0;
                for (int i = 0; i < WaterArea.WaterList.Count; ++i)
                {
                    W = (WaterArea) WaterArea.WaterList[i];
                    if (W.Zone == this)
                    {
                        WaterCount++;
                    }
                }

                O.Write(WaterCount);
                for (int i = 0; i < WaterArea.WaterList.Count; ++i)
                {
                    W = (WaterArea) WaterArea.WaterList[i];
                    if (W.Zone == this)
                    {
                        WriteSectorVector(O, W.X, W.Y, W.Z);
                    O.Write(W.Width);
                    O.Write(W.Depth);
                    O.Write((ushort)W.Damage);
                    O.Write((ushort)W.DamageType);
                    }
                }

                WriteEndChunk(O);

                if (clientZone != null)
                {
                    // Interactive sceneries
                    WriteStartChunk(O, "ISCN", 1);

                    // Save interactive sceneries
                    ushort ScnCount = 0;
                    foreach (ClientZone.Scenery S in clientZone.Sceneries)
                    {
                        if (S.Interactive && !string.IsNullOrEmpty(S.NameScript))
                        {
                            ++ScnCount;
                        }
                    }

                    O.Write((ushort)ScnCount);

                    foreach (ClientZone.Scenery S in clientZone.Sceneries)
                    {
                        if (S.Interactive && !string.IsNullOrEmpty(S.NameScript))
                        {
                            float X = S.EN.X();
                            float Y = S.EN.Y();
                            float Z = S.EN.Z();
                            ushort SectorX = 0, SectorZ = 0;

                            while (X > 768.0f)
                            {
                                X -= 768.0f;
                                SectorX++;
                            }

                            while (Z > 768.0f)
                            {
                                Z -= 768.0f;
                                SectorZ++;
                            }

                            
                            O.Write(SectorX);
                            O.Write(SectorZ);
                            O.Write(X);
                            O.Write(Y);
                            O.Write(Z);
                            O.Write(S.MeshID);

                            string ScriptName = S.NameScript;
                            if (ScriptName.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                                ScriptName = ScriptName.Substring(0, ScriptName.Length - 3);

                            Blitz.WriteString(ScriptName, O);
                        }
                    }
                }

                WriteEndChunk(O);


                // Close
                O.Close();


                //BinaryWriter F = Blitz.WriteFile(@"Data\Server Data\Areas\" + Name + ".dat");
                //if (F == null)
                //{
                //    return false;
                //}

                //// General settings
                //for (int i = 0; i < 5; ++i)
                //{
                //    F.Write(WeatherChance[i]);
                //}
                //Blitz.WriteString(Script, F);
                //Blitz.WriteString("", F);
                //F.Write(PvP);
                //F.Write(Gravity);
                //F.Write(Outdoors);
                //Blitz.WriteString(WeatherLink, F);


                //// Triggers
                //for (int i = 0; i < Triggers.Count; ++i)
                //{
                //    Trigger T = Triggers[i];

                //    F.Write(T.X);
                //    F.Write(T.Y);
                //    F.Write(T.Z);
                //    F.Write(T.Size);
                //    Blitz.WriteString(T.Script, F);
                //    Blitz.WriteString("", F);
                //}
                //for (int i = Triggers.Count; i < 150; ++i)
                //{
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    Blitz.WriteString("", F);
                //    Blitz.WriteString("", F);
                //}

                //// Waypoints
                //for (int i = 0; i < Waypoints.Count; ++i)
                //{
                //    Waypoint WP = Waypoints[i];

                //    F.Write(WP.X);
                //    F.Write(WP.Y);
                //    F.Write(WP.Z);

                //    if (WP.NextA != null)
                //        F.Write((ushort)WaypointIDFromList(WP.NextA));
                //    else
                //        F.Write((ushort)65535);

                //    if (WP.NextB != null)
                //        F.Write((ushort)WaypointIDFromList(WP.NextB));
                //    else
                //        F.Write((ushort)65535);

                //    if (WP.Prev != null)
                //        F.Write((ushort)WaypointIDFromList(WP.Prev));
                //    else
                //        F.Write((ushort)65535);

                //    F.Write(WP.PauseTime);
                //}
                //for (int i = Waypoints.Count; i < 2000; ++i)
                //{
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    F.Write((ushort)2005);
                //    F.Write((ushort)2005);
                //    F.Write((ushort)2005);
                //    F.Write(0);
                //}

                //// Portals
                //for (int i = 0; i < Portals.Count; ++i)
                //{
                //    Portal P = Portals[i];

                //    Blitz.WriteString(P.Name, F);
                //    Blitz.WriteString(P.LinkArea, F);
                //    Blitz.WriteString(P.LinkName, F);
                //    F.Write(P.X);
                //    F.Write(P.Y);
                //    F.Write(P.Z);
                //    F.Write(P.Size);
                //    F.Write(P.Yaw);
                //}
                //for (int i = Portals.Count; i < 100; ++i)
                //{
                //    Blitz.WriteString("", F);
                //    Blitz.WriteString("", F);
                //    Blitz.WriteString("", F);
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    F.Write((float)0);
                //    F.Write((float)0);
                //}

                //// Spawn points
                //int SpawnCount = 0;
                //for (int i = 0; i < Waypoints.Count; ++i)
                //{
                //    if (Waypoints[i].Max > 0)
                //        ++SpawnCount;
                //}

                //for (int i = 0; i < SpawnCount; ++i)
                //{
                //    foreach (Waypoint WP in Waypoints)
                //    {
                //        if (WP.Max > 0)
                //        {
                //            F.Write(WP.ActorID);
                //            F.Write((ushort)WaypointIDFromList(WP));
                //            F.Write(WP.Size);

                //            string WriteName = WP.Script;
                //            if (WriteName.Length > 3)
                //            {
                //                if (WriteName.Substring(WriteName.Length - 3).Equals(".cs", StringComparison.CurrentCultureIgnoreCase))
                //                {
                //                    WriteName = WriteName.Substring(0, WriteName.Length - 3);
                //                }
                //            }

                //            Blitz.WriteString(WriteName, F);
                //            Blitz.WriteString(/*SpawnActorScript[i]*/"", F);
                //            Blitz.WriteString(/*SpawnDeathScript[i]*/"", F);
                //            F.Write(WP.Max);
                //            F.Write(WP.Frequency);
                //            F.Write(WP.Range);
                //        }
                //    }
                //}
                //for (int i = SpawnCount; i < 1000; ++i)
                //{
                //    F.Write((ushort)65535);
                //    F.Write((ushort)2005);
                //    F.Write((float)0);
                //    Blitz.WriteString("", F);
                //    Blitz.WriteString("", F);
                //    Blitz.WriteString("", F);
                //    F.Write((ushort)0);
                //    F.Write((ushort)0);
                //    F.Write((float)0);
                //}


                //// Water areas
                //WaterArea W;
                //ushort Count = 0;
                //for (int i = 0; i < WaterArea.WaterList.Count; ++i)
                //{
                //    W = (WaterArea) WaterArea.WaterList[i];
                //    if (W.Zone == this)
                //    {
                //        Count++;
                //    }
                //}
                //F.Write(Count);
                //for (int i = 0; i < WaterArea.WaterList.Count; ++i)
                //{
                //    W = (WaterArea) WaterArea.WaterList[i];
                //    if (W.Zone == this)
                //    {
                //        F.Write(W.X);
                //        F.Write(W.Y);
                //        F.Write(W.Z);
                //        F.Write(W.Width);
                //        F.Write(W.Depth);
                //        F.Write(W.Damage);
                //        F.Write(W.DamageType);
                //    }
                //}

                //if (clientZone == null)
                //{
                //    F.Write((ushort)0);
                //}
                //else
                //{
                //    // Save interactive sceneries
                //    Count = 0;
                //    foreach (ClientZone.Scenery S in clientZone.Sceneries)
                //    {
                //        if (S.Interactive && !string.IsNullOrEmpty(S.NameScript))
                //        {
                //            ++Count;
                //        }
                //    }

                //    F.Write((ushort)Count);

                //    foreach (ClientZone.Scenery S in clientZone.Sceneries)
                //    {
                //        if (S.Interactive && !string.IsNullOrEmpty(S.NameScript))
                //        {
                //            float X = S.EN.X();
                //            float Y = S.EN.Y();
                //            float Z = S.EN.Z();
                //            ushort SectorX = 0, SectorZ = 0;

                //            while (X > 768.0f)
                //            {
                //                X -= 768.0f;
                //                SectorX++;
                //            }

                //            while (Z > 768.0f)
                //            {
                //                Z -= 768.0f;
                //                SectorZ++;
                //            }

                //            F.Write(X);
                //            F.Write(Y);
                //            F.Write(Z);
                //            F.Write(SectorX);
                //            F.Write(SectorZ);
                //            F.Write(S.MeshID);

                //            string ScriptName = S.NameScript;
                //            if (ScriptName.EndsWith(".cs", StringComparison.CurrentCultureIgnoreCase))
                //                ScriptName = ScriptName.Substring(0, ScriptName.Length - 3);

                //            F.Write(ScriptName);
                //        }
                //    }
                //}

                //F.Close();

                return true;
            }
            else
            {
                return false;
            }
        }

        // Returns the ID of a spawn point associated with a given waypoint, if any
//         public int GetSpawnPoint(int Waypoint)
//         {
//             for (int i = 0; i < 1000; ++i)
//             {
//                 if (SpawnWaypoint[i] == Waypoint && SpawnMax[i] > 0)
//                 {
//                     return i;
//                 }
//             }
//             return -1;
//         }

        // Find a particular zone by name
        public static Zone Find(string Name)
        {
            Name = Name.ToUpper();
            Zone Z;
            for (int i = 0; i < ZonesList.Count; ++i)
            {
                Z = (Zone) ZonesList[i];
                if (Z.Name.ToUpper() == Name)
                {
                    return Z;
                }
            }
            return null;
        }
    }

    // Water areas for damaging actor instances
    public class WaterArea
    {
        // Index of all water
        public static ArrayList WaterList = new ArrayList();

        // Water settings
        public Zone Zone;
        public float X, Y, Z, Width, Depth;
        public ushort Damage, DamageType;

        // Constructor
        public WaterArea(Zone Z)
        {
            Zone = Z;
            WaterList.Add(this);
        }
    }

    // An instance of a zone
    public class ZoneInstance
    {
        public Zone Zone;
        public int ID;
        public LinkedList<ActorInstance> ActorInstances; // List of all actor instances in this instance
        public int CurrentWeatherTime;
        public int[] SpawnLast = new int[1000], Spawned = new int[1000];
        public OwnedScenery[] OwnedScenery = new OwnedScenery[500];

        // Constructor
        public ZoneInstance(Zone Z, int ID)
        {
            // Register instance
            Z.Instances[ID] = this;
            Zone = Z;
            this.ID = ID;

            // Create actor instance list
            ActorInstances = new LinkedList<ActorInstance>();

            // Initial spawn point times
            for (int i = 0; i < 1000; ++i)
            {
                SpawnLast[i] = MLib.Timer.Ticks;
            }

            // Copy ownable scenery data from default instance
            if (ID > 0)
            {
                for (int i = 0; i < 500; ++i)
                {
                    if (Z.Instances[0].OwnedScenery[i] != null)
                    {
                        OwnedScenery[i] = new OwnedScenery();
                        OwnedScenery[i].InventorySize = Z.Instances[0].OwnedScenery[i].InventorySize;
                        if (OwnedScenery[i].InventorySize > 0)
                        {
                            OwnedScenery[i].Inventory = new Inventory();
                        }
                    }
                }
            }
        }

        // Update the weather status for this zone instance
//         public void UpdateWeather()
//         {
//             CurrentWeatherTime--;
// 
//             // Time to change weather
//             if (CurrentWeatherTime <= 0)
//             {
//                 // Get weather from linked zone
//                 if (Zone.WeatherLinkZone != null)
//                 {
//                     CurrentWeatherTime = Zone.WeatherLinkZone.Instances[0].CurrentWeatherTime;
//                     CurrentWeather = Zone.WeatherLinkZone.Instances[0].CurrentWeather;
//                 }
//                     // Or choose own weather from probabilities
//                 else
//                 {
//                     Random R = new Random(MLib.Timer.Ticks);
//                     CurrentWeatherTime = R.Next(2500, 10000);
//                     CurrentWeather = Environment.Weather.Default;
//                     int NewWeather = R.Next(1, 100);
//                     int Min = 0, Max;
//                     for (int i = 0; i < 5; ++i)
//                     {
//                         if (Zone.WeatherChance[i] > 0)
//                         {
//                             Max = Min + Zone.WeatherChance[i];
//                             if (NewWeather >= Min && NewWeather < Max)
//                             {
//                                 CurrentWeather = (Environment.Weather) (i + 1);
//                                 break;
//                             }
//                             Min = Max;
//                         }
//                     }
//                 }
// 
//                 // Inform all players in this zone instance
//                 LinkedListNode<ActorInstance> Node = ActorInstances.First;
//                 while (Node != null)
//                 {
//                     if (Node.Value.RNID > 0)
//                     {
//                         //RN_Send(Host, Node.Value.RNID, P_WeatherChange, RN_StrFromInt$(Handle(A), 4) + RN_StrFromInt$(A\CurrentWeather, 1), True);
//                         Node = Node.Next;
//                     }
//                 }
//             }
//         }

        // Returns true if this instance has any ownership information which should be saved
        internal bool HasOwnershipData()
        {
            for (int j = 0; j < 500; ++j)
            {
                if (OwnedScenery[j] != null)
                {
                    if (!string.IsNullOrEmpty(OwnedScenery[j].AccountName))
                    {
                        return true;
                    }
                    else
                    {
                        for (int k = 0; k < OwnedScenery[j].InventorySize; ++k)
                        {
                            if (OwnedScenery[j].Inventory.Items[k] != null)
                            {
                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }

    // Each area instance may have up to 500 player owned items of scenery (e.g. chests, doors, etc.)
    public class OwnedScenery
    {
        public byte InventorySize;
        public Inventory Inventory;
        public string AccountName = "";
        public byte CharacterNumber;
    }
}