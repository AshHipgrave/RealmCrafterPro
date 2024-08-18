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
// Realm Crafter GE entry point/loader for RC 2.x, by Rob W (rottbott@hotmail.com)
// Original version February 2005, C# version October 2006

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;
using System.Windows.Forms;
using NGUINet;
using RealmCrafter;
using RealmCrafter_GE.RC_Crash_Handler;
using RealmCrafter_GE.Utilities.Fullscreen;
using RenderingServices;
using Environment=System.Environment;
using Timer=MLib.Timer;
using Scripting;

namespace RealmCrafter_GE
{

    internal static class Program
    {
        // Demo build toggle
        public const bool DemoVersion = false;
        public const bool TestingVersion = true;
        public static GE GE;
        public static ShaderManager ShaderManager = null;
        public static string ProjectName;
        public static string ScriptExtension = "cs";
        // Writes a line to the log file
        private static string LogPath;
        public static string Version;
        // For property grid
        public static List<string> MeshList = new List<string>();
        public static List<string> TextureList = new List<string>();
        public static List<string> MusicList = new List<string>();
        public static List<string> SoundsList = new List<string>();

        public static string ServerHost;
        public static string UpdatesHost;
        public static int ServerPort;

        public static KeyBinding.KeyBindings KeyBindings;

        #region LT Variables
        public static TreeEditorTree ActiveTree = null;
        public static uint TrunkEditorShader = 0;
        public static uint LeafEditorShader = 0;
        public static uint TrunkEditorShaderLOD = 0;
        public static uint LeafEditorShaderLOD = 0;
        public static uint PhysicsEditorShader = 0;
        public static uint TransformEditorShader = 0;
        public static uint TreeSwayCenterTexture = 0;
        public static Entity TreeCube, TreeSphere, TreeCapsule;

        public static LTNet.TreeManager Manager;
        public static LTNet.TreeZone CurrentTreeZone = null;

        public static TransformTool Transformer;

        public static GETreeManager GETreeManager;
        #endregion

        public static void WriteToLog(string Line)
        {
            BinaryWriter F = new BinaryWriter(
                new FileStream(LogPath, FileMode.Append, FileAccess.Write));
            Blitz.WriteLine(Line, F);
            F.Close();
        }

        [STAThread]
        private static void Main()
        {
            //Application.ThreadException += new ThreadExceptionEventHandler(new ThreadExceptionHandler().ApplicationThreadException);
            DateTime StartTime = DateTime.Now; //Debugging length of operations


            // Load key bindings
            KeyBindings = new KeyBinding.KeyBindings();

            ///*RenderWrapper.StartRemoteDebugging("localhost", 6543);

            #region Create log file
            LogPath = @"Data\GE Log.txt";
            if (!Directory.Exists(Environment.CurrentDirectory + @"\Data"))
            {
                MessageBox.Show(@"Folder not found: \Data\!", "Error");
                return;
            }
            if (File.Exists(LogPath))
            {
                File.Delete(LogPath);
            }
            try
            {
                WriteToLog("Starting GE");
            }
            catch (IOException)
            {
                MessageBox.Show("Could not write to log file!", "Error");
                return;
            }
            #endregion

            //System.Globalization.CultureInfo formatFractii = new System.Globalization.CultureInfo("ro");
            //System.Globalization.NumberFormatInfo.CurrentInfo.NumberDecimalSeparator.;
            System.Globalization.CultureInfo ForceCulture = System.Globalization.CultureInfo.GetCultureInfo("en-US");
            Application.CurrentCulture = ForceCulture;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Show splash screen
            WriteToLog("Creating splash screen");
            Splash_Screen Splash = new Splash_Screen();
            Splash.Show();

            #region Load all data
            BinaryReader F;
            #region Get version
            if (!File.Exists(@"Data\Version.dat"))
            {
                MessageBox.Show(@"File not found: Data\Version.dat!", "Error");
                return;
            }


            WriteToLog("Reading version number");

            F = new BinaryReader(new FileStream(@"Data\Version.dat", FileMode.Open));

            try
            {
                Version = Blitz.ReadLine(F);
            }
            catch
            {
                Version = "Unknown";
            }

            if (TestingVersion)
                Version = "Development Build";

            F.Close();
            #endregion

            // If we're testing, ignore projection selection

            ProjectName = "TestingProject";
            if (!TestingVersion)
            {
                #region Get selected project
                if (!File.Exists(@"Data\Selected.dat"))
                {
                    MessageBox.Show(@"File not found: Data\Selected.dat!", "Error");
                    return;
                }
                WriteToLog("Getting selected project");
                F = new BinaryReader(new FileStream(@"Data\Selected.dat", FileMode.Open));
                if (F.BaseStream.Length == 0)
                {
                    MessageBox.Show("No project selected!");
                    return;
                }

                ProjectName = Blitz.ReadString(F);
                F.Close();
                #endregion

                #region Change current folder to selected project
                if (!Directory.Exists(Environment.CurrentDirectory + @"\Projects\" + ProjectName))
                {
                    MessageBox.Show(@"Project not found: " + ProjectName + "!", "Error");
                    return;
                }

                WriteToLog("Switching to project folder");
                Environment.CurrentDirectory += @"\Projects\" + ProjectName;
                LogPath = @"..\..\" + LogPath;
                #endregion
            }
            else
            {
                //Environment.CurrentDirectory += "\..\Sandbox";
            }

            #region Load list of available scripts
            /*if (!System.IO.Directory.Exists(System.Environment.CurrentDirectory + @"\Data\Server Data\Scripts"))
            {
                MessageBox.Show(@"Folder not found: Data\Server Data\Scripts\!", "Error");
                return;
            }*/
            WriteToLog("Reading available scripts");
            string[] ScriptFiles = Directory.GetFiles(@"Data\Server Data\Scripts", "*.cs");

            string[] TempArray = new string[ScriptFiles.GetUpperBound(0) + 1];
            int Count = 0;
            foreach (string S in ScriptFiles)
            {
                if (Path.GetExtension(S) == "." + ScriptExtension)
                {
                    TempArray[Count] = Path.GetFileName(S);
                    ++Count;
                }
            }
            GE.ScriptsList = new string[Count];
            //GE.ScriptsList = (string[]ResizeArray
            Array.Copy(TempArray, GE.ScriptsList, Count);
            #endregion

            #region Load all misc settings
            // Hosts.dat (client)
            WriteToLog("Reading Hosts.dat");


            F = Blitz.ReadFile(@"Data\Game Data\Hosts.dat");
            if (F == null)
            {
                MessageBox.Show(@"File not found: Data\Game Data\Hosts.dat!", "Error");
                return;
            }

            ServerHost = Blitz.ReadLine(F);
            UpdatesHost = Blitz.ReadLine(F);
            F.Close();

            // Money.dat (client)
            WriteToLog("Reading Money.dat");
            string Money1;
            string Money2;
            ushort Money2x;
            string Money3;
            ushort Money3x;
            string Money4;
            ushort Money4x;

            F = Blitz.ReadFile(@"Data\Game Data\Money.dat");
            if (F == null)
            {
                MessageBox.Show(@"File not found: Data\Game Data\Money.dat!", "Error");
                return;
            }

            Money1 = Blitz.ReadString(F);
            Money2 = Blitz.ReadString(F);
            Money2x = F.ReadUInt16();
            Money3 = Blitz.ReadString(F);
            Money3x = F.ReadUInt16();
            Money4 = Blitz.ReadString(F);
            Money4x = F.ReadUInt16();
            F.Close();

            // Other.dat (client)
            WriteToLog("Reading Other.dat");
            byte HideNametags;
            bool DisableCollisions;
            byte ViewModes;            
            bool RequireMemorise;
            byte UseBubbles;
            byte BubblesRed = 0;
            byte BubblesGreen = 0;
            byte BubblesBlue = 0;

            bool bShowRadar;
            ushort iPlayer, iEnemy, iFriendly, iNPC, iOther;

            F = Blitz.ReadFile(@"Data\Game Data\Other.dat");
            if (F == null)
            {
                MessageBox.Show(@"File not found: Data\Game Data\Other.dat!", "Error");
                return;
            }

            HideNametags = F.ReadByte();
            DisableCollisions = F.ReadBoolean();
            ViewModes = F.ReadByte();
            ServerPort = F.ReadInt32();
            if (ServerPort < 1)
            {
                ServerPort = 25000;
            }
            RequireMemorise = F.ReadBoolean();
            UseBubbles = F.ReadByte();
            if (F.BaseStream.Length > 9)
            {
                BubblesRed = F.ReadByte();
                BubblesGreen = F.ReadByte();
                BubblesBlue = F.ReadByte();
            }

            // Radar
            bShowRadar = true;
            iPlayer = 65535;
            iEnemy = 65535;
            iFriendly = 65535;
            iNPC = 65535;
            iOther = 65535;

            try
            {
                bShowRadar = Convert.ToBoolean(F.ReadByte());
                iPlayer = F.ReadUInt16();
                iEnemy = F.ReadUInt16();
                iFriendly = F.ReadUInt16();
                iNPC = F.ReadUInt16();
                iOther = F.ReadUInt16();
            }
            catch (System.IO.EndOfStreamException)
            {
            }

            F.Close();

            // Combat.dat (client)
            WriteToLog("Reading Combat.dat");
            ushort CombatInfoStyle;

            F = Blitz.ReadFile(@"Data\Game Data\Combat.dat");
            if (F == null)
            {
                MessageBox.Show(@"File not found: Data\Game Data\Combat.dat!", "Error");
                return;
            }
            F.BaseStream.Seek(2, SeekOrigin.Begin);
            CombatInfoStyle = F.ReadByte();
            F.Close();

            // Misc.dat (server)
            WriteToLog("Reading Misc.dat");
            int StartGold;
            int StartReputation;
            bool ForcePortals;
            ushort CombatDelay;
            byte CombatFormula;
            byte CombatRatingAdjust;
            bool AllowAccountCreation;
            byte MaxAccountChars;

            F = Blitz.ReadFile(@"Data\Server Data\Misc.dat");
            if (F == null)
            {
                MessageBox.Show(@"File not found: Data\Server Data\Misc.dat!", "Error");
                return;
            }

            StartGold = F.ReadInt32();
            StartReputation = F.ReadInt32();
            ForcePortals = F.ReadBoolean();
            CombatDelay = F.ReadUInt16();
            CombatFormula = F.ReadByte();
            Item.WeaponDamageEnabled = F.ReadBoolean();
            Item.ArmourDamageEnabled = F.ReadBoolean();
            CombatRatingAdjust = F.ReadByte();
            AllowAccountCreation = F.ReadBoolean();
            MaxAccountChars = F.ReadByte();
            F.Close();
            #endregion

            #region Load interface
            #endregion

            #region Load shader library
            WriteToLog("Loading shader database");
            //Media.LoadShaderNames();
            #endregion

            #region Load actors, items etc.
            WriteToLog("Loading projectiles");
            if (Projectile.Load(@"Data\Server Data\Projectiles.dat") < 0)
            {
                MessageBox.Show(@"Could not load projectiles from \Data\Server Data\Projectiles.dat!", "Error");
                return;
            }

            WriteToLog("Loading gubbin names");
            if (!Actors3D.LoadGubbinNames())
            {
                MessageBox.Show(@"Could not load gubbin names from \Data\Game Data\Gubbins.dat!", "Error");
                return;
            }

            WriteToLog("Loading attributes");
            if (!Attributes.Load(@"Data\Server Data\Attributes.dat"))
            {
                MessageBox.Show(@"Could not load attributes from \Data\Server Data\Attributes.dat!", "Error");
                return;
            }

            //modify the current game interface
            //Component.GenerateAttributesTree();
            //RealmCrafter.Interface.Component.GenerateAttributesTree("AttributeBars", " bar");

            WriteToLog("Loading damage types");
            if (!Item.LoadDamageTypes(@"Data\Server Data\Damage.dat"))
            {
                MessageBox.Show(@"Could not load damage types from \Data\Server Data\Damage.dat!", "Error");
                return;
            }

            WriteToLog("Loading animation sets");
            if (AnimSet.Load(@"Data\Game Data\Animations.dat") < 0)
            {
                MessageBox.Show(@"Could not load animation sets from \Data\Game Data\Animations.dat!", "Error");
                return;
            }

            WriteToLog("Loading abilities");
            if (Ability.Load(Ability.SpellDatabase) < 0)
            {
                MessageBox.Show(@"Could not load abilities from \Data\Server Data\Spells.s3db!", "Error");
                return;
            }

            // Marian Voicu
            WriteToLog("Loading character");
            // todo: Load "Character Set" when that feature is returned to the program.
            // end (MV)
            WriteToLog("Loading factions");
            if (Actor.LoadFactions(Actor.FactionDatabase) < 0)
            {
                MessageBox.Show(@"Could not load factions from \Data\Game Data\Factions.s3db!", "Error");
                return;
            }

            WriteToLog("Loading items");
            int TotalItems = Item.Load(GE.ItemsFile);
            if (TotalItems < 0)
            {
                MessageBox.Show(@"Could not load items from " + GE.ItemsFile, "Error");
                return;
            }

            WriteToLog("Loading actors");
            int TotalActors = Actor.Load(Actor.ActorDatabase);
            if (TotalActors < 0)
            {
                MessageBox.Show(@"Could not load actors from \Data\Server Data\Actors.dat!", "Error");
                return;
            }

            WriteToLog("Loading environment");
            if (!RealmCrafter.Environment.LoadEnvironment())
            {
                MessageBox.Show(@"Could not load environment from \Data\Server Data\Environment.dat!", "Error");
                return;
            }

            


            #endregion

            #endregion

            // Create GE window
            WriteToLog("Creating main window");

            using (GE G = new GE())
            {
                GE = G;

                WriteToLog("Loading Gubbins");
                GE.m_GubbinEditor.LoadTemplates();

                foreach (Actor A in Actor.Index.Values)
                {
                    if (A == null)
                        continue;

                    foreach (ushort GubbinID in A.TempGubbins)
                    {
                        GubbinTemplate T = null;

                        foreach (GubbinTemplate tT in GE.m_GubbinEditor.Templates)
                        {
                            if (tT.ID == GubbinID)
                            {
                                T = tT;
                                break;
                            }
                        }

                        if (T != null)
                        {
                            A.DefaultGubbins.Add(T);
                        }
                    }

                    for (int bid = 0; bid < A.TempBeards.Count; ++bid)
                    {
                        A.Beards.Add(new List<GubbinTemplate>());

                        for (int i = 0; i < A.TempBeards[bid].Length; ++i)
                        {
                            GubbinTemplate T = null;

                            foreach (GubbinTemplate tT in GE.m_GubbinEditor.Templates)
                            {
                                if (tT.ID == A.TempBeards[bid][i])
                                {
                                    T = tT;
                                    break;
                                }
                            }

                            if (T != null)
                            {
                                A.Beards[bid].Add(T);
                            }
                        }

                        A.TempBeards[bid] = null;
                    }

                    for (int bid = 0; bid < A.TempMaleHairs.Count; ++bid)
                    {
                        A.MaleHairs.Add(new List<GubbinTemplate>());

                        for (int i = 0; i < A.TempMaleHairs[bid].Length; ++i)
                        {
                            GubbinTemplate T = null;

                            foreach (GubbinTemplate tT in GE.m_GubbinEditor.Templates)
                            {
                                if (tT.ID == A.TempMaleHairs[bid][i])
                                {
                                    T = tT;
                                    break;
                                }
                            }

                            if (T != null)
                            {
                                A.MaleHairs[bid].Add(T);
                            }
                        }

                        A.TempMaleHairs[bid] = null;
                    }

                    for (int bid = 0; bid < A.TempFemaleHairs.Count; ++bid)
                    {
                        A.FemaleHairs.Add(new List<GubbinTemplate>());

                        for (int i = 0; i < A.TempFemaleHairs[bid].Length; ++i)
                        {
                            GubbinTemplate T = null;

                            foreach (GubbinTemplate tT in GE.m_GubbinEditor.Templates)
                            {
                                if (tT.ID == A.TempFemaleHairs[bid][i])
                                {
                                    T = tT;
                                    break;
                                }
                            }

                            if (T != null)
                            {
                                A.FemaleHairs[bid].Add(T);
                            }
                        }

                        A.TempFemaleHairs[bid] = null;
                    }

                    A.TempBeards.Clear();
                    A.TempMaleHairs.Clear();
                    A.TempFemaleHairs.Clear();
                    A.TempBeards = null;
                    A.TempMaleHairs = null;
                    A.TempFemaleHairs = null;
                    A.TempGubbins = null;
                }

                foreach (Item I in Item.Index.Values)
                {
                    if (I == null || I.TempGubbins.Length == 0)
                        continue;

                    foreach (ushort GubbinID in I.TempGubbins)
                    {
                        GubbinTemplate T = null;

                        foreach (GubbinTemplate tT in GE.m_GubbinEditor.Templates)
                        {
                            if (tT.ID == GubbinID)
                            {
                                T = tT;
                                break;
                            }
                        }

                        if (T != null)
                        {
                            I.GubbinTemplates.Add(T);
                        }
                    }
                    I.TempGubbins = null;
                }


                #region 3D rendering
                WriteToLog("Initializing 3D rendering");
                Render.Init(G.RenderingPanel.Handle, 0);
                WriteToLog("BBDX Init complete, calling Graphics3D");
                Render.Graphics3D(1024, 768, 32, 2, 0, 0,
                                  TestingVersion ? @".\Data\DefaultTex.png" : @"..\..\Data\GUE\DefaultTex.PNG");
                WriteToLog("BBDX Graphics3D complete, creating camera");
                G.Camera = Entity.CreateCamera();
                Actors3D.Camera = G.Camera;
                Timer.MaxFPS = 100; // Framerate limiter
                #endregion 3D rendering

                #region load shaders
                // Default shaders
                WriteToLog("Loading default shaders");
                Shaders.LitObject1 = Shaders.Load(@"Data\Game Data\Shaders\Default\LitObject_Medium.fx");
                if (Shaders.LitObject1 == 0)
                {
                    MessageBox.Show(@"Could not load LitObject1 shader!", "Error");
                    return;
                }
                /*Shaders.LitObject2 = Shaders.Load(@"Data\Game Data\Shaders\Default\LitObject2.fx");
                if (Shaders.LitObject2 == 0)
                {
                    MessageBox.Show(@"Could not load LitObject2 shader!", "Error");
                    return;
                }
                */
                Shaders.Terrain = Shaders.Load(@"Data\Game Data\Shaders\Default\Terrain.fx");
                if (Shaders.Terrain == 0)
                {
                    MessageBox.Show(@"Could not load Terrain shader!", "Error");
                    return;
                }


                Shaders.SAQuad = Shaders.Load(@"Data\Game Data\Shaders\Default\InterfaceQuad.fx");
                if (Shaders.SAQuad == 0)
                {
                    MessageBox.Show(@"Could not load InterfaceQuad shader!", "Error");
                    return;
                }

                Shaders.FullbrightAlpha = Shaders.Load(@"Data\Game Data\Shaders\Default\FullbrightAlpha.fx");
                if (Shaders.FullbrightAlpha == 0)
                {
                    MessageBox.Show(@"Could not load FullbrightAlpha shader!", "Error");
                    return;
                }

                Shaders.FullbrightMultiply = Shaders.Load(@"Data\Game Data\Shaders\Default\FullbrightMultiply.fx");
                if (Shaders.FullbrightMultiply == 0)
                {
                    MessageBox.Show(@"Could not load FullbrightMultiply shader!", "Error");
                    return;
                }

                Shaders.FullbrightAdd = Shaders.Load(@"Data\Game Data\Shaders\Default\FullbrightAdd.fx");
                if (Shaders.FullbrightAdd == 0)
                {
                    MessageBox.Show(@"Could not load FullbrightAdd shader!", "Error");
                    return;
                }
                
                Shaders.Sky = Shaders.Load(@"Data\Game Data\Shaders\Default\nps_Sky.fx");
                if (Shaders.Sky == 0)
                {
                    MessageBox.Show(@"Could not load sky shader!", "Error");
                    return;
                }

                Shaders.Clouds = Shaders.Load(@"Data\Game Data\Shaders\Default\nps_cloud.fx");
                if (Shaders.Clouds == 0)
                {
                    MessageBox.Show(@"Could not load cloud shader!", "Error");
                    return;
                }

                Shaders.Line = Shaders.Load(@"Data\Game Data\Shaders\Default\3DLine.fx");
                if (Shaders.Line == 0)
                {
                    MessageBox.Show(@"Could not load 3DLine shader!", "Error");
                    return;
                }
                Shaders.Animated = Shaders.Load(@"Data\Game Data\Shaders\Default\AnimatedMesh_Medium.fx");
                if (Shaders.Animated == 0)
                {
                    MessageBox.Show(@"Could not load AnimatedMesh shader!", "Error");
                    return;
                }
                Shaders.WireFrame = Shaders.Load(TestingVersion ? @".\GUE\WireFrame.fx" : @"..\..\Data\GUE\WireFrame.fx");
                if (Shaders.WireFrame == 0)
                {
                    MessageBox.Show(@"Could not load WireFrame shader!", "Error");
                    return;
                }

                Shaders.DefaultDepthShader = Shaders.Load(@"Data\Game Data\Shaders\Default\DepthMap.fx");
                if (Shaders.DefaultDepthShader == 0)
                {
                    MessageBox.Show(@"Could not load ShadowDepth shader!", "Error");
                    return;
                }


                Shaders.DefaultAnimatedDepthShader = Shaders.Load(@"Data\Game Data\Shaders\Default\AnimatedDepthMap.fx");
                if (Shaders.DefaultAnimatedDepthShader == 0)
                {
                    MessageBox.Show(@"Could not load Animated ShadowDepth shader!", "Error");
                    return;
                }

                Shaders.ShadowBlurH = Shaders.Load(@"Data\Game Data\Shaders\Default\ShadowBlurH.fx");
                if (Shaders.ShadowBlurH == 0)
                {
                    MessageBox.Show(@"Could not load ShadowBlurH shader!", "Error");
                    return;
                }

                Shaders.ShadowBlurV = Shaders.Load(@"Data\Game Data\Shaders\Default\ShadowBlurV.fx");
                if (Shaders.ShadowBlurV == 0)
                {
                    MessageBox.Show(@"Could not load ShadowBlurV shader!", "Error");
                    return;
                }

                RealmCrafter_GE.ShaderManager.LoadShaders();
                Media.LoadEntityParameters();
                ShaderManager = new ShaderManager();
                //ShaderManager.Show();

                RenderWrapper.ShadowShader(Shaders.DefaultDepthShader);
                RenderWrapper.ShadowBlurShader(Shaders.ShadowBlurH, Shaders.ShadowBlurV);

                RenderWrapper.SetShadowMapSize(1024);
                RenderWrapper.ShadowLevel(1);

                RenderWrapper.ShadowDistance(200.0f);
                RenderWrapper.LightDistance(500.0f);

                // Shaders list
                WriteToLog("Filling shaders list");
                //G.UpdateShaderCombo();
                #endregion

                #region Important meshes
                // Required meshes
                WriteToLog("Loading template meshes");
                if (!Entity.LoadSphereMesh(TestingVersion ? @".\Data\Sphere.b3d" : @"..\..\Data\GUE\Sphere.b3d"))
                {
                    MessageBox.Show(@"Could not load Data\GUE\Sphere.b3d!", "Error");
                    return;
                }

                if (!Entity.LoadCubeMesh(TestingVersion ? @".\Data\Box.b3d" : @"..\..\Data\GUE\Box.b3d"))
                {
                    MessageBox.Show(@"Could not load Data\GUE\Box.b3d!", "Error");
                    return;
                }

                G.PortalTemplate = Entity.LoadMesh(TestingVersion ? @".\GUE\Portal.b3d" : @"..\..\Data\GUE\Portal.b3d");
                if (G.PortalTemplate == null)
                {
                    MessageBox.Show(@"Could not load Data\GUE\Portal.b3d!", "Error");
                    return;
                }
                G.WaypointTemplate =
                    Entity.LoadMesh(TestingVersion ? @".\GUE\Waypoint.b3d" : @"..\..\Data\GUE\Waypoint.b3d");
                if (G.WaypointTemplate == null)
                {
                    MessageBox.Show(@"Could not load Data\GUE\Waypoint.b3d!", "Error");
                    return;
                }
                G.WaypointLinkTemplate =
                    Entity.LoadMesh(TestingVersion ? @".\GUE\Waypoint Link.b3d" : @"..\..\Data\GUE\Waypoint Link.b3d");
                if (G.WaypointLinkTemplate == null)
                {
                    MessageBox.Show(@"Could not load Data\GUE\Waypoint Link.b3d!", "Error");
                    return;
                }
                G.WaypointLinkTemplate.FX = 1;
                G.PortalTemplate.FitMesh(-1f, -1f, -1f, 2f, 2f, 2f, true);
                G.WaypointTemplate.FitMesh(-1f, -1f, -1f, 2f, 2f, 2f, true);
                G.WaypointLinkTemplate.FitMesh(-0.1f, -0.1f, 0f, 0.2f, 0.2f, 1f, false);
                G.PortalTemplate.Visible = false;
                G.WaypointTemplate.Visible = false;
                G.WaypointLinkTemplate.Visible = false;
                // Coloured textures
                WriteToLog("Loading template textures");
                G.RedTex = Render.LoadTexture(TestingVersion ? @".\GUE\Red.png" : @"..\..\Data\GUE\Red.png");
                if (G.RedTex == 0)
                {
                    MessageBox.Show(@"Could not load Data\GUE\Red.png!", "Error");
                    return;
                }
                G.BlueTex = Render.LoadTexture(TestingVersion ? @".\GUE\Blue.png" : @"..\..\Data\GUE\Blue.png");
                if (G.BlueTex == 0)
                {
                    MessageBox.Show(@"Could not load Data\GUE\Blue.png!", "Error");
                    return;
                }
                G.OrangeTex = Render.LoadTexture(TestingVersion ? @".\GUE\Orange.png" : @"..\..\Data\GUE\Orange.png");
                if (G.OrangeTex == 0)
                {
                    MessageBox.Show(@"Could not load Data\GUE\Orange.png!", "Error");
                    return;
                }
                G.YellowTex = Render.LoadTexture(TestingVersion ? @".\GUE\Yellow.png" : @"..\..\Data\GUE\Yellow.png");
                if (G.YellowTex == 0)
                {
                    MessageBox.Show(@"Could not load Data\GUE\Yellow.png!", "Error");
                    return;
                }
                // Lighting
                G.DefaultLight = Light.CreateDirectionalLight();
                G.DefaultLight.Enable(0);

                // Initialize Collision System
                if (!DisableCollisions)
                {
                    Collision.Collisions(CollisionType.Player, CollisionType.Actor, 2, 3); // Actor->Scenery
                    Collision.Collisions(CollisionType.Actor, CollisionType.Player, 2, 3);
                    Collision.Collisions(CollisionType.Actor, CollisionType.Actor, 2, 3);
                    Collision.Collisions(CollisionType.Player, CollisionType.ActorTri2, 2, 3);
                    Collision.Collisions(CollisionType.Actor, CollisionType.ActorTri2, 2, 3);
                }
                Collision.Collisions(CollisionType.Player, CollisionType.Sphere, 2, 3);
                Collision.Collisions(CollisionType.Player, CollisionType.Box, 2, 3);
                Collision.Collisions(CollisionType.Player, CollisionType.Triangle, 2, 3);
                Collision.Collisions(CollisionType.Actor, CollisionType.Sphere, 2, 3);
                Collision.Collisions(CollisionType.Actor, CollisionType.Box, 2, 3);
                Collision.Collisions(CollisionType.Actor, CollisionType.Triangle, 2, 3);
                Collision.Collisions(CollisionType.ActorTri1, CollisionType.Sphere, 2, 3);
                Collision.Collisions(CollisionType.ActorTri1, CollisionType.Box, 2, 3);
                Collision.Collisions(CollisionType.ActorTri1, CollisionType.Triangle, 2, 3);
                Collision.Collisions(CollisionType.ActorTri1, CollisionType.PickableNone, 2, 3);
                #endregion

                #region Project tab
                WriteToLog("Initialising Project tab");
                G.GameName = ProjectName;
                //G.ProjectName.Text = "Project: " + ProjectName;
                G.MainBuildLabel.Text = "Version: " + Version;
                G.TotalActors = TotalActors;
                G.TotalItems = TotalItems;
                //G.ProjectActors.Text = "Actors: " + TotalActors;
                //G.ProjectItems.Text = "Items: " + TotalItems;
//                 G.ServerHostText.Text = ServerHost;
//                 G.ServerPortSpinner.Value = ServerPort;
//                 G.UpdatesHostText.Text = UpdatesHost;
                //G.AllowAccountCreation.Checked = AllowAccountCreation;
                //G.MaxCharsPerAccount.Value = MaxAccountChars;
                #endregion

                #region Media tab
                WriteToLog("Initializing Media tab");

                Encryption.Initialise(5378);
                WriteToLog("Creating media dialogs");
                    Media.ConvertMeshDatabase();
                //MediaDialogs.Init(G.MediaTree);
                //G.ProjectMeshes.Text = "Meshes: " + MediaDialogs.TotalMeshes;
                //G.ProjectTextures.Text = "Textures: " + MediaDialogs.TotalTextures;
                //G.ProjectSounds.Text = "Sounds: " + MediaDialogs.TotalSounds;
                //G.ProjectMusic.Text = "Music: " + MediaDialogs.TotalMusic;
                #endregion

                #region Attributes lists
                WriteToLog("Filling attributes lists");
                for (int i = 0; i < Attributes.TotalAttributes; ++i)
                {
                    if (Attributes.Names[i].Length > 0)
                    {
                        ListBoxItem LBI = new ListBoxItem(Attributes.Names[i], (uint)i);
                        G.ItemAttributesList.Items.Add(LBI);
                        G.ActorAttributesList.Items.Add(LBI);
                    }
                }
                #endregion

                #region Actors tab
                WriteToLog("Initialising Actors tab");
                // Settings
                G.SuppressActorSettingChange = true;
                G.ActorCombatDelaySpinner.Value = CombatDelay;
                G.ActorCombatFormula.SelectedIndex = CombatFormula - 1;
                G.ActorCombatDamage.SelectedIndex = CombatInfoStyle - 1;
                G.ActorCombatFactionHit.Value = CombatRatingAdjust;
                G.ActorVisualShowNametags.SelectedIndex = HideNametags;
                if (ViewModes == 1)
                {
                    G.ActorVisualViewModes.SelectedIndex = 0;
                }
                else if (ViewModes == 2)
                {
                    G.ActorVisualViewModes.SelectedIndex = 2;
                }
                else if (ViewModes == 3)
                {
                    G.ActorVisualViewModes.SelectedIndex = 1;
                }
                G.ActorVisualCollisionsCheck.Checked = DisableCollisions;
                G.ActorVisualBubbles.SelectedIndex = UseBubbles - 1;
                G.ActorVisualBubbleColour.BackColor = Color.FromArgb(BubblesRed, BubblesGreen,
                                                                     BubblesBlue);
                G.MoneyUnits1.Text = Money1;
                G.MoneyUnits2.Text = Money2;
                G.MoneyUnits3.Text = Money3;
                G.MoneyUnits4.Text = Money4;
                G.MoneyMultiplier2.Value = Money2x;
                G.MoneyMultiplier3.Value = Money3x;
                G.MoneyMultiplier4.Value = Money4x;
                G.ActorStartReputation.Value = StartReputation;
                G.ActorStartMoney.Value = StartGold;
                G.ActorRequireMemorisationCheck.Checked = RequireMemorise;
                G.SuppressActorSettingChange = false;

                G.ActorsSettingsShowActorsCheckBox.Checked = bShowRadar;
                G.ActorsSettingsPlayerButton.Tag = iPlayer;
                G.ActorsSettingsEnemyButton.Tag = iEnemy;
                G.ActorsSettingsFriendlyButton.Tag = iFriendly;
                G.ActorsSettingsNPCButton.Tag = iNPC;
                G.ActorsSettingsOtherButton.Tag = iOther; 

                // Marian Voicu
                //G.ActorSettingsSelecterImageButton.Text = GE.NiceTextureName((int)SelecterImageId);
                //G.ActorSettingsUseRadarCheckBox.Checked = UseRadar;
                //G.ActorSettingsSelecterImageID = SelecterImageId;
                //G.ActorsSettingsDiscoverMapInComboBox.Text = DiscoverMap;
                //G.ActorsSettingsShowActorsCheckBox.Checked = ShowActorOnRadar;
                //G.ActorsSettingsEnemyButton.BackColor = System.Drawing.Color.FromArgb(EnemyColor);
                //G.ActorsSettingsFriendlyButton.BackColor = System.Drawing.Color.FromArgb(FriendlyColor);
                //G.ActorsSettingsPlayerButton.BackColor = System.Drawing.Color.FromArgb(PlayerColor);
                //G.ActorsSettingsNPCButton.BackColor = System.Drawing.Color.FromArgb(NpcColor);
                //G.ActorsSettingsOtherButton.BackColor = System.Drawing.Color.FromArgb(OtherColor);
                G.SetActorsSavedStatus(true);
                // end (MV)
                // Resistances list
                G.ActorResistancesList.BeginUpdate();
                for (int i = 0; i < Item.TotalDamageTypes; ++i)
                {
                    if (Item.DamageTypes[i] != "")
                    {
                        G.ActorResistancesList.Items.Add(new ListBoxItem(Item.DamageTypes[i], (uint)i));
                    }
                }
                G.ActorResistancesList.EndUpdate();
                // Main list
                G.ActorsList.BeginUpdate();
                Actor Ac = Actor.FirstActor;
                while (Ac != null)
                {
                    G.ActorsList.Items.Add(new ListBoxItem(Ac.Race + " [" + Ac.Class + "]", (uint)Ac.ID));
                    //G.ActorAnimationListBox.Items.Add(new ListBoxItem(Ac.Race + " [" + Ac.Class + "]", Ac.ID));
                    Ac = Ac.NextActor;
                }
                G.ActorsList.EndUpdate();
                // Factions
                for (int i = 0; i < Actor.TotalFactions; ++i)
                {
                    if (Actor.FactionNames[i] != "")
                    {
                        G.ActorFactionCombo.Items.Add(new ListBoxItem(Actor.FactionNames[i], (uint)i));
                    }
                }
                // Animation sets
                G.AnimSetList.BeginUpdate();
                for (int i = 0; i < AnimSet.TotalAnimationSets; ++i)
                {
                    if (AnimSet.Index[i] != null)
                    {
                        G.AnimSetList.Items.Add(new ListBoxItem(AnimSet.Index[i].Name, (uint)i));
                        G.ActorMAnimSetCombo.Items.Add(new ListBoxItem(AnimSet.Index[i].Name, (uint)i));
                        G.ActorFAnimSetCombo.Items.Add(new ListBoxItem(AnimSet.Index[i].Name, (uint)i));
                    }
                }
                G.AnimSetList.EndUpdate();
                // Abilities
                G.AbilitiesList.BeginUpdate();
                Ability Ab = Ability.FirstAbility;
                while (Ab != null)
                {
                    G.AbilitiesList.Items.Add(new ListBoxItem(Ab.Name, Ab.ID));

                    Ab = Ab.NextAbility;
                }
                G.AbilitiesList.EndUpdate();
                // Scripts
                foreach (string S in GE.ScriptsList)
                {
                    G.AbilityScriptCombo.Items.Add(S);
                }
                Actors3D.HideNametags = 1;
                // Scripts from world object (Marian Voicu)
                /*  G.WorldObjectRunScriptComboBox.Items.Add("(None)");
                foreach (string S in GE.ScriptsList)
                {
                    G.WorldObjectRunScriptComboBox.Items.Add(S);
                }*/
                // end (MV)
                // Character set

                #endregion

                #region Items tab
                WriteToLog("Initialising Items tab");
                foreach (string S in GE.ScriptsList)
                {
                    G.ItemUseScriptCombo.Items.Add(S);
                }
                G.ItemsList.BeginUpdate();
               
                foreach (Item item in Item.Index.Values)
                {
                    G.ItemsList.Items.Add(new ListBoxItem(item.Name, item.ID));
                }
                G.ItemsList.EndUpdate();
                #endregion

                #region Interface tab
//                 WriteToLog("Initialising Interface tab");
//                 // Create all interface components
//                 WriteToLog("Creating interface component entities");
//                 Component C = Component.FirstComponent;
//                 while (C != null)
//                 {
//                     if (C.displayed_name != null)
//                     {
//                         C.Handle = Entity.CreateSAQuad();
//                     }
//                     C = C.NextComponent;
//                 }
// 
//                 // Texture them
//                 WriteToLog("Texturing interface component entities");
//                 Component.LoadTextures(@"Data\Game Data\TexturePaths.xml");
                #endregion

                #region Particles tab
                WriteToLog("Initialising Particles tab");
                // Load default texture
                WriteToLog("Loading default particle texture");
                G.DefaultParticleTexture = Render.LoadTexture(@"..\..\Data\GUE\DefaultParticle.bmp");
                // Fill emitters list
                WriteToLog("Filling emitters list");
                string[] Emitters = Directory.GetFiles(@"Data\Emitter Configs\");
                string Filename;
                foreach (string S in Emitters)
                {
                    Filename = Path.GetFileNameWithoutExtension(S);
                    G.EmitterEmitterCombo.Items.Add(Filename);
                    //   G.WorldPlaceEmitterCombo.Items.Add(Filename);
                    WriteToLog("Emitters: " + Filename);
                }
                #endregion

                #region World tab
                WriteToLog("Initialising World tab");
                // List of actors
                /*     Ac = Actor.FirstActor;
                while (Ac != null)
                {
                    G.WorldObjectWaypointActor.Items.Add(new ListBoxItem(Ac.Race + " [" + Ac.Class + "]", Ac.ID));
                    Ac = Ac.NextActor;
                }*/
                // Lists of scripts
                /*foreach (string S in GE.ScriptsList)
                {
                      G.WorldPlaceTriggerScriptCombo.Items.Add(S);
                    G.WorldZoneEntryScriptCombo.Items.Add(S);
                    G.WorldZoneExitScriptCombo.Items.Add(S);
                    G.WorldObjectWaypointSpawnScript.Items.Add(S);
                    G.WorldObjectWaypointInteractScript.Items.Add(S);
                    G.WorldObjectWaypointDeathScript.Items.Add(S);
                   
                }*/
                // Lists of zones
                WriteToLog("Filling zones list");
                int TotalZones = 0;
                string[] Zones = Directory.GetFiles(@"Data\Areas\", "*.dat");
                foreach (string S in Zones)
                {
                    TotalZones++;
                    string Name = Path.GetFileNameWithoutExtension(S);

                    G.ActorStartZoneCombo.Items.Add(Name);
                    /*
                    G.WorldPlacePortalLinkCombo.Items.Add(Name);
                    G.WorldObjectPortalLinkCombo.Items.Add(Name);
                    G.WorldZoneWeatherLinkCombo.Items.Add(Name);
                    G.WorldZonesTree.Nodes.Add(Name);
                    G.WorldZonesTree.Nodes[TotalZones - 1].Name = Name;
                     * */
                }
                /*
                for (int i = 0; i < G.WorldZonesTree.GetNodeCount(false); i++)
                {
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Scenery objects");
                    G.WorldZonesTree.Nodes[i].Nodes[0].Name = "Node0";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Terrains");
                    G.WorldZonesTree.Nodes[i].Nodes[1].Name = "Node1";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Emitters");
                    G.WorldZonesTree.Nodes[i].Nodes[2].Name = "Node2";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Water areas");
                    G.WorldZonesTree.Nodes[i].Nodes[3].Name = "Node3";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Collision boxes");
                    G.WorldZonesTree.Nodes[i].Nodes[4].Name = "Node4";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Sound zones");
                    G.WorldZonesTree.Nodes[i].Nodes[5].Name = "Node5";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Dynamic lights");
                    G.WorldZonesTree.Nodes[i].Nodes[6].Name = "Node6";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Triggers");
                    G.WorldZonesTree.Nodes[i].Nodes[7].Name = "Node7";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Waypoints");
                    G.WorldZonesTree.Nodes[i].Nodes[8].Name = "Node8";
                    G.WorldZonesTree.Nodes[i].Nodes.Add("Portals");
                    G.WorldZonesTree.Nodes[i].Nodes[9].Name = "Node9";
                }
                 */
                G.TotalZones = TotalZones;
                //G.ProjectZones.Text = "Zones: " + TotalZones;



                #endregion

                LightFunctionList.LoadFunctions();

                // Remove the splash screen
                WriteToLog("Destroying splash screen");

                Splash.Close();

                Splash.Dispose();

                // Run GE window
                if (DemoVersion)
                {
                    G.Text += " (DEMO)";
                }

                WriteToLog("Loading GUI Manager");
                // It requires the directx device, resolution and the path to its shader
                GE.GUIManager = new NGUIManager(RenderWrapper.bbdx2_GetIDirect3DDevice9(),
                    new NVector2(1024, 768), @".\Data\Game Data\Shaders\Default\GUI.fx");
                GE.GUIManager.CursorVisible = false;

                // Set the font directory
                // The font directory is used when a skin requires a font
                GE.GUIManager.FontDirectory = @".\Data\Fonts";

                // Load the default skin. At least one skin is required for any rendering
                GE.GUIManager.LoadAndAddSkin(@".\Data\Skins\Default\Theme.xml");

                // This is a little hacky on .NET.. but it has to be done with IntPtr and unmanaged
                // callbacks. The GUI callback is rendered before the scene is finalized
                RenderWrapper.SetRenderGUICallback(0, GE.GUIManager.GetRenderCallback());

                // NGUI must destroy all of its resources if the device is lost or reset
                RenderWrapper.SetDeviceLostCallback(0, GE.GUIManager.GetLostCallback());

                // NGUI has to recreate resources on reset. Note the BBDX callback is "DeviceResetXY"
                // and NOT "DeviceReset".
                RenderWrapper.SetDeviceResetXYCallback(0, GE.GUIManager.GetResetCallback());

                WriteToLog("Environment");

                Environment3D.InitializeEnvironment3D(GE.Camera);
                RealmCrafter.SDK.MediaDialogsWrapper.Init();

                // Setup RCT
                WriteToLog("Loading RCT");
                
                RCTTest.RCTImportPlugins.LoadPlugins();
                RealmCrafter.SDK.SDKPlugins.LoadPlugins();

                // DV: Fix for running in testing mode
                string rctGUIBasePath = @"..\..\Data\GUE\";
                if (TestingVersion)
                    rctGUIBasePath = @".\GUE\";

                GE.TerrainManager = new RealmCrafter.RCT.TerrainManager(RenderWrapper.bbdx2_GetIDirect3DDevice9(),
                    Path.Combine(rctGUIBasePath, "RCT.fx"),
                    Path.Combine(rctGUIBasePath, "RCTEditor.fx"),
                    @".\Data\Game Data\Shaders\Default\Grass.fx",
                    @".\Data\Game Data\Shaders\Default\RCTDepth.fx");
                Program.GE.m_TerrainEditor.TManager = GE.TerrainManager;
                Program.GE.m_TerrainEditor.GrassSelector.TManager = GE.TerrainManager;
                Program.GE.m_TerrainEditor.LoadToolOptions(Path.Combine(rctGUIBasePath, "RCTConfig.xml"));

                GE.TerrainManager.EditorMode = true;
                GE.TerrainManager.BrushRadius = 0.0f;
                GE.TerrainManager.LoadGrassTypes(@".\Data\Game Data\GrassTypes.xml");

                GE.GrassTypesEditor = new RCTTest.GrassTypesEditor();
                GE.GrassTypesEditor.Reset();

                GE.TerrainManager.TerrainRender += new EventHandler(TerrainManager_TerrainRender);
                GE.TerrainManager.TerrainRenderDepth += new EventHandler(TerrainManager_TerrainRenderDepth);
                GE.TerrainManager.CollisionChanged += new EventHandler(TerrainManager_CollisionChanged);

                RenderWrapper.SetRenderSolidCallbackRT(0, GE.TerrainManager.GetRenderCallback());
                RenderWrapper.SetRenderShadowDepthCallback(0, GE.TerrainManager.GetRenderDepthCallback());
                RenderWrapper.SetDeviceLostCallback(1, GE.TerrainManager.GetLostCallback());
                RenderWrapper.SetDeviceResetXYCallback(1, GE.TerrainManager.GetResetCallback());

                // Load Trees
                Program.Manager = LTNet.TreeManager.CreateTreeManager(RenderWrapper.bbdx2_GetIDirect3DDevice9(),
                    @"Data\Game Data\Shaders\Default\Trunk.fx",
                    @"Data\Game Data\Shaders\Default\Leaf.fx",
                    @"Data\Game Data\Shaders\Default\LOD.fx");
                if (Program.Manager == null)
                {
                    MessageBox.Show("Critical Error: Could not initialize LT.Net (or LT).\n\nCheck system compatibility; driver compatibility or effect files", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                RenderWrapper.SetRenderSolidCallbackRT(1, Program.Manager.GetRenderCallback());
                RenderWrapper.SetDeviceLostCallback(2, Program.Manager.GetLostCallback());
                RenderWrapper.SetDeviceResetXYCallback(2, Program.Manager.GetResetCallback());

                Program.Manager.TreeRender += new EventHandler(Manager_TreeRender);
                Program.Manager.CollisionChanged += new EventHandler(Manager_CollisionChanged);

                string PathPrefix = "";
                if (!TestingVersion)
                    PathPrefix = "..\\..\\";

                Program.TrunkEditorShader = Shaders.Load(PathPrefix + "Tree\\EditorTrunk.fx");
                Program.LeafEditorShader = Shaders.Load(PathPrefix + "Tree\\EditorLeaf.fx");
                Program.TrunkEditorShaderLOD = Shaders.Load(PathPrefix + "Tree\\EditorTrunkOrtho.fx");
                Program.LeafEditorShaderLOD = Shaders.Load(PathPrefix + "Tree\\EditorLeafOrtho.fx");
                Program.PhysicsEditorShader = Shaders.Load(PathPrefix + "Tree\\EditorPhysics.fx");
                Program.TransformEditorShader = Shaders.Load(PathPrefix + "Tree\\EditorTransform.fx");

                Program.TreeSwayCenterTexture = Render.LoadTexture(PathPrefix + "Tree\\SwayIcon.png");

                Program.TreeCube = Entity.LoadMesh(PathPrefix + "Tree\\PhysicsCube.b3d");
                Program.TreeSphere = Entity.LoadMesh(PathPrefix + "Tree\\PhysicsSphere.b3d");
                Program.TreeCapsule = Entity.LoadMesh(PathPrefix + "Tree\\PhysicsCapsule.b3d");

                Program.TreeCube.Visible = false;
                Program.TreeSphere.Visible = false;
                Program.TreeCapsule.Visible = false;


                Program.GETreeManager = new GETreeManager();
                Program.GETreeManager.LoadTrees();

                WriteToLog("Loading SDKControls");

                Dockable_Forms.ScriptEditor.EditorControl.Load("SDKData\\Controls\\");

                /*
                List<Dockable_Forms.ScriptEditor.EditorControlInstance> AllControls = new List<Dockable_Forms.ScriptEditor.EditorControlInstance>();
                Dockable_Forms.ScriptEditor.EditorControlInstance Master =
                    Dockable_Forms.ScriptEditor.EditorControlInstance.Load(@"Data\Server Data\Scripts\__MyTestForm.xml", AllControls);

                string DBL = "";

                foreach (Dockable_Forms.ScriptEditor.EditorControlInstance C in AllControls)
                {
                    string Name = "";
                    string Parent = "None";

                    Dockable_Forms.ScriptEditor.EditorPropertyInstance P = C.FindPropertyInstance("name");
                    if(P != null)
                        Name = P.Value.ToString();
                    if (C.Parent != null)
                        Parent = C.Parent.EditorControl.Name;

                    if (C != Master)
                        DBL += "    ";
                    DBL += C.EditorControl.Name + "(Name=" + Name + ", Parent=" + Parent + ")\n";
                }

                MessageBox.Show(DBL);

                Dockable_Forms.ScriptEditor.ControlProperties Props =
                    (Dockable_Forms.ScriptEditor.ControlProperties)Activator.CreateInstance(AllControls[4].EditorControl.PropertyEditorType);

                Props.Parent = AllControls[4];

                GE.m_ScriptView.m_Properties.PropertyGrid.SelectedObject = Props;

                PacketWriter Pa = new PacketWriter();
                Pa.Write("CWND", false);
                Pa.Write("ALID", false);
                Pa.Write(1);
                Pa.Write("ENDL", false);

                SDKNet.ControlHost.ProcessFormOpen(Pa.ToArray());

                AllControls[0].Save(@"Data\Server Data\Scripts\__MyTestForm.xml",
                    @"Data\Server Data\Scripts\__MyTestForm.Designer.cs",
                    @"Data\Server Data\Scripts\MyTestForm.cs");

                */

//                 	RealmCrafter::PacketWriter Pa;
// 	Pa.Write(NGin::Type("CSEL", "").Guid());
// 	Pa.Write(std::string("ALID"), false);
// 	Pa.Write(1);
// //	Pa.Write(std::string("Children"), true);
// //	Pa.Write((ushort)2);
// // 	
// // 	Pa.Write(ButtonWriter.GetLength());
// // 	Pa.Write(ButtonWriter);
// // 	Pa.Write(PBoxWriter.GetLength());
// // 	Pa.Write(PBoxWriter);
//  
//  	Pa.Write(std::string("ENDL"), false);


                

                WriteToLog("Registering main loop method");
                Application.Idle += G.MainLoop;

                Program.GE.RenderingPanel.MouseDown += new MouseEventHandler(Program.GE.m_ZoneRender.RenderingPanel_MouseDown);
                Program.GE.RenderingPanel.MouseUp += new MouseEventHandler(Program.GE.m_ZoneRender.RenderingPanel_MouseUp);

                WriteToLog("Running application");
                DateTime EndTime = DateTime.Now;
                TimeSpan Duration = EndTime - StartTime;
                WriteToLog("Complete time to load: " + Duration.TotalSeconds + " Seconds");

                // Small fix for anyone without \Music\
                if (!Directory.Exists(@"Data\Music\"))
                    Directory.CreateDirectory(@"Data\Music\");


                Application.Run(G);
                HandleTaskBar.showTaskBar();
                // Free all unmanaged resources

                Render.EndGraphics();
            }
        }

        public class TerrainTagItem
        {
            public Entity RenderMesh = null;
            public Vector3 Position = new Vector3(0, 0, 0);
        }

        static uint LastTreeID = 1;
        static LinkedList<RealmCrafter.ClientZone.Tree> ActiveTrees = new LinkedList<RealmCrafter.ClientZone.Tree>();

        static void Manager_CollisionChanged(object sender, EventArgs e)
        {
            LTNet.CollisionChangedEventArgs E = e as LTNet.CollisionChangedEventArgs;
            if (E == null)
                return;

            Entity MasterPivot = null;
            uint IntTag = E.TreeInstance.IntTag;
            RealmCrafter.ClientZone.Tree Tree = null;

            foreach (RealmCrafter.ClientZone.Tree T in ActiveTrees)
            {
                if (T != null && T.ActiveID == IntTag)
                {
                    Tree = T;
                    break;
                }
            }

            if (E.CollisionData == null)
            {
                if (Tree != null && Tree.EN != null)
                {
                    if (Program.Transformer != null)
                    {
                        if (Program.Transformer.HasParent(Tree.EN))
                        {
                            Program.Transformer.Free();
                            Program.Transformer = null;
                        }
                    }

                    Tree.EN.Free();
                    Tree.EN = null;
                }

                

                if(Tree != null)
                    GE.m_ZoneList.WorldZonesTree.Nodes[11].Nodes.Remove(Tree.Node);
                ActiveTrees.Remove(Tree);

                return;
            }

            if (E.CollisionData.InstanceCount == 0)
                return;

            // New Instance
            bool Recycled = true;
            
            if(Tree != null)
                MasterPivot = Tree.EN;

            if (MasterPivot != null)
            {
                //return;
                //MasterPivot.Free();
            }
            else
            {
                Recycled = false;
                MasterPivot = Entity.CreatePivot();
            }

            Collision.EntityPickMode(MasterPivot, PickMode.Polygon);
            if (Tree == null)
            {
                Tree = new RealmCrafter.ClientZone.Tree(E.TreeInstance, MasterPivot);
                MasterPivot.ExtraData = Tree;
                Tree.ActiveID = ++LastTreeID;
                E.TreeInstance.IntTag = Tree.ActiveID;
                GE.m_ZoneList.WorldZonesTree.Nodes[11].Nodes.Add(Tree.Node);
                ActiveTrees.AddLast(Tree);
            }

            uint Desc = RenderWrapper.bbdx2_CreatePhysicsDesc(1, MasterPivot.Handle);

            for (int i = 0; i < E.CollisionData.InstanceCount; ++i)
            {
                switch ((int)E.CollisionData.Instances[i].Type)
                {
                    case 0:
                        {
                            float Low = E.CollisionData.Instances[i].DimensionsX;
                            if (E.CollisionData.Instances[i].DimensionsZ < Low)
                                Low = E.CollisionData.Instances[i].DimensionsZ;
                            if (E.CollisionData.Instances[i].DimensionsY < Low)
                                Low = E.CollisionData.Instances[i].DimensionsY;

                            RenderWrapper.bbdx2_AddSphere(Desc,
                                E.CollisionData.Instances[i].OffsetX,
                                E.CollisionData.Instances[i].OffsetY,
                                E.CollisionData.Instances[i].OffsetZ,
                                Low, -1.0f);

                            break;
                        }
                    case 1:
                        {
                            float Low = E.CollisionData.Instances[i].DimensionsX;
                            if (E.CollisionData.Instances[i].DimensionsZ < Low)
                                Low = E.CollisionData.Instances[i].DimensionsZ;

                            RenderWrapper.bbdx2_AddCapsule(Desc,
                                E.CollisionData.Instances[i].OffsetX,
                                E.CollisionData.Instances[i].OffsetY,
                                E.CollisionData.Instances[i].OffsetZ,
                                Low, E.CollisionData.Instances[i].DimensionsY, -1.0f);

                            break;
                        }
                    case 2:
                        {

                            RenderWrapper.bbdx2_AddBox(Desc,
                                E.CollisionData.Instances[i].OffsetX,
                                E.CollisionData.Instances[i].OffsetY,
                                E.CollisionData.Instances[i].OffsetZ,
                                E.CollisionData.Instances[i].DimensionsX,
                                E.CollisionData.Instances[i].DimensionsY,
                                E.CollisionData.Instances[i].DimensionsZ,
                                -1.0f);

                            break;
                        }
                }
            }

            RenderWrapper.bbdx2_ClosePhysicsDesc(Desc);

            LTNet.TreeInstance Instance = E.TreeInstance;
            if (!Recycled)
            {
                MasterPivot.Position(Instance.X(), Instance.Y(), Instance.Z());

                float P = Instance.Pitch();
                float Y = Instance.Yaw();
                float R = Instance.Roll();

                while (R > 90.0f)
                    R -= 180.0f;
                while (R < -90.0f)
                    R += 180.0f;


                MasterPivot.Rotate(P, Y, R);
                MasterPivot.Scale(Instance.ScaleX(), Instance.ScaleY(), Instance.ScaleZ());

                Tree.Node.Text = String.Format("{0:0.00}, {1:0.00}, {2:0.00}", Instance.X(), Instance.Y(), Instance.Z());
            }

            //Instance.IntTag = MasterPivot.Handle;
        }

        static void Manager_TreeRender(object sender, EventArgs e)
        {
            if (Program.Manager == null)
                return;
            if (Program.GE.RenderingPanelCurrentIndex != -3)
                return;

            IntPtr Directions = Program.Manager.GetDirectionBuffer();
            IntPtr Colors = Program.Manager.GetColorBuffer();

            RenderWrapper.bbdx2_GetDirectionalLights(Directions, Colors);

            Program.Manager.SetDirectionBuffer(Directions);
            Program.Manager.SetColorBuffer(Colors);

            IntPtr Count = Marshal.AllocHGlobal(4);
            IntPtr Stride = Marshal.AllocHGlobal(4);

            IntPtr LightPtr = RenderWrapper.bbdx2_GetPointLights(Count, Stride);
            Program.Manager.SetPointLights(LightPtr, Count, Stride);

            Marshal.FreeHGlobal(Count);
            Marshal.FreeHGlobal(Stride);

            float FogNear = RenderWrapper.bbdx2_GetFogNear();
            float FogFar = RenderWrapper.bbdx2_GetFogFar();
            Program.Manager.SetFog(System.Drawing.Color.FromArgb(RenderWrapper.bbdx2_GetFogColor()), FogNear, FogFar);

            

            IntPtr View = RenderWrapper.bbdx2_GetViewMatrixPtr();
            IntPtr Projection = RenderWrapper.bbdx2_GetProjectionMatrixPtr();
            IntPtr LightView = RenderWrapper.GetLightMatrix();

            float[] V = new float[16];
            float[] P = new float[16];
            float[] LP = new float[16];

            Marshal.Copy(View, V, 0, 16);
            Marshal.Copy(Projection, P, 0, 16);
            Marshal.Copy(LightView, LP, 0, 16);

            Program.Manager.SetShadowMap(RenderWrapper.GetShadowMap());

            if(GE.Camera != null && !((RenderWrapper.bbdx2_GetRenderMask() & 4) > 0))
                Program.Manager.Render(V, P, LP, GE.Camera.X(), GE.Camera.Y(), GE.Camera.Z());

            Program.Manager.SetShadowMap(IntPtr.Zero);

            RenderWrapper.bbdx2_FreeMatrixPtr(View);
            RenderWrapper.bbdx2_FreeMatrixPtr(Projection);
        }

        static void TerrainManager_CollisionChanged(object sender, EventArgs args)
        {
            RealmCrafter.RCT.CollisionChangedEventArgs e = args as RealmCrafter.RCT.CollisionChangedEventArgs;
            if (e == null)
                return;

            RealmCrafter.RCT.RCTerrain Terrain = sender as RealmCrafter.RCT.RCTerrain;
            if (Terrain == null)
                return;

            List<TerrainTagItem> TerrainTag = Terrain.Tag as List<TerrainTagItem>;
            if (TerrainTag == null)
                return;

            bool Found = false;
            foreach (TerrainTagItem I in TerrainTag)
            {
                if (Convert.ToInt32(I.Position.X) == Convert.ToInt32(e.Position.X)
                    && Convert.ToInt32(I.Position.Z) == Convert.ToInt32(e.Position.Z))
                {
                    if (e.TriangeList.ToInt32() == 0)
                    {
                        RenderWrapper.bbdx2_ASyncCancelInject(I.RenderMesh.Handle);
                        RenderWrapper.bbdx2_FreeCollisionInstance(I.RenderMesh.Handle);
                    }
                    else
                    {
                        I.RenderMesh.Position(e.Position.X + e.TerrainPosition.X, e.Position.Y + e.TerrainPosition.Y, e.Position.Z + e.TerrainPosition.Z);
                        RenderWrapper.bbdx2_ASyncInjectCollisionMesh(I.RenderMesh.Handle, e.TriangeList, e.VertexCount, e.HighPriority ? 1 : 0);
                    }
                    Found = true;
                }
            }

            if (!Found)
            {
                Entity Mesh = Entity.CreateMesh();
                Mesh.Position(e.Position.X + e.TerrainPosition.X, e.Position.Y + e.TerrainPosition.Y, e.Position.Z + e.TerrainPosition.Z);
                //Collision.EntityType(Mesh, 2);

                Collision.EntityType(Mesh, (byte)CollisionType.Triangle);
                Collision.EntityPickMode(Mesh, PickMode.Polygon);

                if (e.TriangeList.ToInt32() == 0)
                    RenderWrapper.bbdx2_FreeCollisionInstance(Mesh.Handle);
                else
                    RenderWrapper.bbdx2_ASyncInjectCollisionMesh(Mesh.Handle, e.TriangeList, e.VertexCount, e.HighPriority ? 1 : 0);

                TerrainTagItem Tag = new TerrainTagItem();
                Tag.RenderMesh = Mesh;
                Tag.Position = new Vector3(e.Position.X, e.Position.Y, e.Position.Z);
                TerrainTag.Add(Tag);
            }
        }

        static void TerrainManager_TerrainRender(object sender, EventArgs e)
        {
            if (GE.TerrainManager != null)
            {
                IntPtr View = RenderWrapper.bbdx2_GetViewMatrixPtr();
                IntPtr Projection = RenderWrapper.bbdx2_GetProjectionMatrixPtr();
                IntPtr LightView = RenderWrapper.GetLightMatrix();
                RenderWrapper.D3DXMatrixMultiply(View, View, Projection);

                float[] VP = new float[16];
                float[] LP = new float[16];

                Marshal.Copy(View, VP, 0, 16);
                Marshal.Copy(LightView, LP, 0, 16);

                GE.TerrainManager.RenderTerrain = !((RenderWrapper.bbdx2_GetRenderMask() & 8) > 0);
                GE.TerrainManager.RenderGrass = !((RenderWrapper.bbdx2_GetRenderMask() & 2) > 0);
                GE.TerrainManager.RenderCollision = ((RenderWrapper.bbdx2_GetRenderMask() & 32) > 0);

                GE.TerrainManager.Render(VP, LP, RenderWrapper.GetShadowMap());

                RenderWrapper.bbdx2_FreeMatrixPtr(View);
                RenderWrapper.bbdx2_FreeMatrixPtr(Projection);
            }
        }

        static void TerrainManager_TerrainRenderDepth(object sender, EventArgs e)
        {
            
            if (GE.TerrainManager != null)
            {
                //MessageBox.Show((RenderWrapper.GetLightMatrix()).ToString());
                IntPtr View = RenderWrapper.GetLightMatrix();
                //IntPtr View = RenderWrapper.bbdx2_GetViewMatrixPtr();
                //IntPtr Projection = RenderWrapper.bbdx2_GetProjectionMatrixPtr();
                //RenderWrapper.D3DXMatrixMultiply(View, View, Projection);

                float[] LP = new float[16];

                Marshal.Copy(View, LP, 0, 16);

                //GE.TerrainManager.RenderDepth(LP);

                RenderWrapper.bbdx2_FreeMatrixPtr(View);
                //RenderWrapper.bbdx2_FreeMatrixPtr(Projection);
            }
        }

        public static void UpdateTerrainManager()
        {
            if (GE.TerrainManager == null)
                return;

            IntPtr Directions = GE.TerrainManager.GetDirectionBuffer();
            IntPtr Colors = GE.TerrainManager.GetColorBuffer();

            RenderWrapper.bbdx2_GetDirectionalLights(Directions, Colors);

            GE.TerrainManager.SetDirectionBuffer(Directions);
            GE.TerrainManager.SetColorBuffer(Colors);

            IntPtr Count = Marshal.AllocHGlobal(4);
            IntPtr Stride = Marshal.AllocHGlobal(4);

            IntPtr LightPtr = RenderWrapper.bbdx2_GetPointLights(Count, Stride);
            GE.TerrainManager.SetPointLights(LightPtr, Count, Stride);

            Marshal.FreeHGlobal(Count);
            Marshal.FreeHGlobal(Stride);

            float FogNear = RenderWrapper.bbdx2_GetFogNear();
            float FogFar = RenderWrapper.bbdx2_GetFogFar();
            GE.TerrainManager.SetFog(System.Drawing.Color.FromArgb(RenderWrapper.bbdx2_GetFogColor()), new NVector2(FogNear, FogFar));

            

            //byte[] Directions = new byte[3 * 3 * 4];
            //byte[] Colors = new byte[3 * 3 * 4];
            //for (int i = 0; i < Directions.Length; ++i)
            //{
            //    Colors[i] = 0;
            //    Directions[i] = 0;
            //}

            //RenderWrapper.bbdx2_GetDirectionalLights(Directions, Colors);

            //for (int i = 0; i < 3; ++i)
            //{
            //    NVector3 Normal = new NVector3(BitConverter.ToSingle(Directions, (i * 12) + 0), BitConverter.ToSingle(Directions, (i * 12) + 4), BitConverter.ToSingle(Directions, (i * 12) + 8));
            //    NVector3 VColor = new NVector3(BitConverter.ToSingle(Colors, (i * 12) + 0), BitConverter.ToSingle(Colors, (i * 12) + 4), BitConverter.ToSingle(Colors, (i * 12) + 8));
            //    System.Drawing.Color Color = Color.FromArgb(Convert.ToInt32(VColor.X * 255.0f), Convert.ToInt32(VColor.Y * 255.0f), Convert.ToInt32(VColor.Z * 255.0f));

            //    GE.TerrainManager.SetLightNormal(i, Normal);
            //    GE.TerrainManager.SetLightColor(i, Color);
            //}
        }

        public static bool IsRendered(Entity e)
        {
            if (e == null)
                return false;

            int Mask = RenderWrapper.bbdx2_GetRenderMask();

            for (int i = 0; i < 32; ++i)
            {
                int v = (int)Math.Pow(2.0, (double)i);
                if (((Mask & v) > 0) && ((e.RenderMask & v) > 0))
                    return false;
            }

            return true;
        }

        public static void WriteString(System.IO.BinaryWriter writer, string data)
        {
            writer.Write(Convert.ToInt32(data.Length));
            writer.Write(System.Text.Encoding.ASCII.GetBytes(data));
        }

        [DllImport("msvcrt.dll")]
        static extern int _mkdir(string path);

        public static void CreateDirectory(string path)
        {
            _mkdir(Path.GetFullPath(path));
        }

        #region PeekMessage code
        // Detects whether the application is still idle using PeekMessage()
        public static bool AppStillIdle
        {
            get
            {
                PeekMsg msg;
                return !PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }


        // Win API definition for PeekMessage()

        // Import PeekMessage() function
        [SuppressUnmanagedCodeSecurity, DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool PeekMessage(out PeekMsg msg, IntPtr hWnd, uint messageFilterMin,
                                               uint messageFilterMax, uint flags);

        [StructLayout(LayoutKind.Sequential)]
        private struct PeekMsg
        {
            public IntPtr hWnd;
            public Message msg;
            public IntPtr wParam;
            public IntPtr lParam;
            public uint time;
            public Point p;
        }
        #endregion

/*
        public static NVector2 SFromA(NVector2 Vec)
        {
            // Control positions and sizes are all relative, so need to be translated back into
            // 3D screenspace
            NVector2 Res = GE.GUIManager.GetResolution();
            return new NVector2(Vec.X / Res.X, Vec.Y / Res.Y);
        }

        public static NVector2 SFromA(float X, float Y)
        {
            return SFromA(new NVector2(X, Y));
        }*/

    }

}