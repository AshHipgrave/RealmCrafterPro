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
// Realm Crafter Interface module by Rob W (rottbott@hotmail.com)
// Original version August 2004, C# port October 2006

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Win32;
using RealmCrafter_GE;

namespace RealmCrafter.Interface
{
    // Standard interface component
    public class Component
    {
        // Members
        private static List<Component> _visited;
        public static Component FirstComponent;
        public float Alpha; // Position/size on screen and transparency level
        public byte B; // Colour
        public string displayed_name; //RT ad

/*
        // Standard game interface components
        public static Component StartGameButton, GraphicsOptionsButton, ControlOptionsButton, MiscOptionsButton, QuitGameButton;
        public static Component Chat, ChatEntry, ActorEffectsArea, Radar,
            Compass, InventoryWindow, InventoryDrop, InventoryEat, InventoryGold;
        public static Component[] AttributeDisplays = new Component[Attributes.TotalAttributes];
        public static Component[] InventoryButtons = new Component[46];
		public static Component ActionBar, XPBar, ChatButton, MapButton, InventoryButton, AbilitiesButton, CharStatsButton,
			QuestLogButton, PartyButton, HelpButton, NextBarButton, PrevBarButton;
		public static Component[] ActionBarButtons = new Component[12];
*/

        // Linked list
        private Component firstChild;
        public byte G; // Colour
        public object Handle; // Handle of an entity, Gooey gadget, etc.
        public float Height; // Position/size on screen and transparency level
        public string name; //RT ad
        public Component NextComponent;
        private Component nextSibling;
        private Component parentNode;
        public byte R; // Colour
        public ushort Texture; // Media ID for a texture
        public float Width; // Position/size on screen and transparency level
        public float X, Y; // Position/size on screen and transparency level

        public Component(BinaryReader reader, string name, string displayName, Component parent)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }
            if (displayName == null)
            {
                throw new ArgumentNullException("displayName");
            }

            this.name = name;
            displayed_name = displayName;

            // Read data from stream
            X = reader.ReadSingle();
            Y = reader.ReadSingle();
            Width = reader.ReadSingle();
            Height = reader.ReadSingle();
            Alpha = reader.ReadSingle();
            R = reader.ReadByte();
            G = reader.ReadByte();
            B = reader.ReadByte();

            // Add to linked list
            NextComponent = FirstComponent;
            FirstComponent = this;

            // Add to hierarchy.
            ParentNode = parent;
        }

        private Component(string name, Component parent)
            : this()
        {
            this.name = name;
            ParentNode = parent;
        }

        private Component(string name)
            : this()
        {
            this.name = name;
        }

        private Component()
        {
            // Default data
            X = 0f;
            Y = 0f;
            Width = 0.1f;
            Height = 0.1f;
            Alpha = 1.0f;
            R = 255;
            G = 255;
            B = 255;

            // Maintain linked list
            NextComponent = FirstComponent;
            FirstComponent = this;
        }

        public Component ParentNode
        {
            get { return parentNode; }

            private set
            {
                if (parentNode != null)
                {
                    throw new ApplicationException("This component already has a parent.");
                }
                if (value == null)
                {
                    throw new ArgumentNullException("value");
                }

                parentNode = value;

                // Place the this node at the end of the sibling list. Although this is
                // less efficient than placing it at the beginning, it has the benefit of
                // storing the siblings in the order in which they were added. This is
                // beneficial for the tree control on the Interface tab, because it 
                // displays the components in the order in which they are found in this 
                // list.
                if (parentNode.firstChild == null)
                {
                    parentNode.firstChild = this;
                }
                else
                {
                    Component sibling = parentNode.firstChild;
                    while (sibling.nextSibling != null)
                    {
                        sibling = sibling.nextSibling;
                    }
                    sibling.nextSibling = this;
                }
                nextSibling = null;
            }
        }

        public Component FirstChild
        {
            get { return firstChild; }
            private set { firstChild = value; }
        }

        public Component NextSibling
        {
            get { return nextSibling; }
            private set { nextSibling = value; }
        }

        // Writes an interface component to a stream
        public void WriteComponent(BinaryWriter F)
        {
            F.Write(X);
            F.Write(Y);
            F.Write(Width);
            F.Write(Height);
            F.Write(Alpha);
            F.Write(R);
            F.Write(G);
            F.Write(B);
        }

        public static Component FindComponent(string component_name)
        {
            Component temp = FirstComponent;
            while (temp != null)
            {
                if (temp.name == component_name)
                {
                    return temp;
                }
                else
                {
                    temp = temp.NextComponent;
                }
            }
            MessageBox.Show("Can not find ''" + component_name + "'' component!");
            return null;
        }

        public static bool HaveParent(Component child, string parentNodeName)
        {
            Component temp = child;
            while (temp.ParentNode != null)
            {
                temp = temp.ParentNode;
                if (temp.name == parentNodeName)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool GenerateAttributesTree()
        {
            //CharacterCreation
            Component C = FirstComponent;
            while (C != null)
            {
                if (C.name == "CharacterAttributes")
                {
                    break;
                }
                else
                {
                    C = C.NextComponent;
                }
            }
            if (C == null)
            {
                return false;
            }

            //delete previous content of the tree node
            while (C.FirstChild != null)
            {
                Component temp = C.FirstChild;
                C.FirstChild = C.FirstChild.FirstChild;
                Component temp2 = FirstComponent;
                while (temp2.NextComponent != temp)
                {
                    temp2 = temp2.NextComponent;
                }
                temp2.NextComponent = temp.NextComponent;
                GC.Collect();
            }

            int k = 0;
            //for (int contor = 0; contor < Attributes.TotalAttributes; contor++)
            for (int contor = Attributes.TotalAttributes - 1; contor >= 0; contor--)
            {
                if (Attributes.Hidden[contor] == false && Attributes.Names[contor] != "")
                {
                    k++;
                    Component temp = new Component();
                    temp.name = Attributes.Names[contor];
                    temp.displayed_name = Attributes.Names[contor];
                    temp.X = 0.6738281F;
                    temp.Y = 0.1002604F + k * 0.047F;
                    temp.Width = 0.3115234F;
                    temp.Height = 0.04166667F;
                    temp.ParentNode = C;
                    temp.NextSibling = C.FirstChild;
                    C.FirstChild = temp;
                }
            }

            //GameScreen
            C = FirstComponent;
            while (C != null)
            {
                if (C.name == "AttributeBars")
                {
                    break;
                }
                else
                {
                    C = C.NextComponent;
                }
            }
            if (C == null)
            {
                return false;
            }

            //delete previous content of the tree node
            while (C.FirstChild != null)
            {
                Component temp = C.FirstChild;
                C.FirstChild = C.FirstChild.FirstChild;
                Component temp2 = FirstComponent;
                while (temp2.NextComponent != temp)
                {
                    temp2 = temp2.NextComponent;
                }
                temp2.NextComponent = temp.NextComponent;
                GC.Collect();
            }

            k = 0;
            //for (int contor = 0; contor < Attributes.TotalAttributes; contor++)
            for (int contor = Attributes.TotalAttributes - 1; contor >= 0; contor--)
            {
                if (Attributes.Hidden[contor] == false && Attributes.Names[contor] != "")
                {
                    k++;
                    Component temp = new Component();
                    temp.name = Attributes.Names[contor] + "Bar";
                    temp.displayed_name = Attributes.Names[contor] + " bar";
                    temp.X = 0.7705078F;
                    temp.Y = 0.140625F + k * 0.047F;
                    temp.Width = 0.2197266F;
                    temp.Height = 0.03776042F;
                    temp.ParentNode = C;
                    temp.NextSibling = C.FirstChild;
                    C.FirstChild = temp;
                }
            }
            return true;
        }

        public static bool LoadAll(string Filename)
        {
            // Note: string literals will be removed when data-access code is factored
            // out to a separate component.

            if (Filename == null)
            {
                throw new ArgumentNullException("Filename");
            }

            using (BinaryReader F = Blitz.ReadFile(Filename))
            {
                if (F == null)
                {
                    return false;
                }

                // Note: The Romanian team created a component hierarchy by allowing
                // components to have parents and siblings. They use this hierarchy to
                // drive the tree control in the Interface tab. The parent components
                // are actually dummy components whose only purpose is to contain the
                // real components. The old (.dat) file format didn't have this concept.
                // In order for us to read from the old format without having to change
                // the treeview-related code, we have to create the dummy parent 
                // components here.

                // Create dummy components to act as containers.
                Component interfaceComponents = new Component("InterfaceComponents");
                Component mainMenu = new Component("MainMenu", interfaceComponents);
                Component inventory = new Component("Inventory", interfaceComponents);
                Component inventorySlots = new Component("InventorySlots", inventory);
                Component gameScreen = new Component("GameScreen", interfaceComponents);
                Component chatEntryComponents = new Component("ChatEntryComponents", gameScreen);
                Component actionBarButtons = new Component("ActionBarButtons", gameScreen);

                // Main game screen
                Component chat = new Component(F, "Chat", "Chat text area", gameScreen);
                chat.Texture = F.ReadUInt16();
                Component chatEntry = new Component(F, "ChatEntry", "Chat entry box", gameScreen);
                for (int i = 0; i < Attributes.TotalAttributes; ++i)
                {
                    string name = "GameScreen_ChatEntry_" + i.ToString(CultureInfo.InvariantCulture);
                    string displayName = "Chat Entry " + (i + 1).ToString(CultureInfo.InvariantCulture);
                    Component component = new Component(F, name, displayName, chatEntryComponents);
                }
                Component actorEffectsArea = new Component(F, "ActorEffectsArea", "Actor effect icons area", gameScreen);
                Component radar = new Component(F, "Radar", "Radar map", gameScreen);
                Component compass = new Component(F, "Compass", "Compass", gameScreen);

                // Inventory
                Component inventoryWindow = new Component(F, "InventoryWindow", "Inventory Window", inventory);
                Component inventoryDrop = new Component(F, "DropItemButton", "Drop item button", inventory);
                Component inventoryEat = new Component(F, "UseItemButton", "Use item button", inventory);
                Component inventoryGold = new Component(F, "MoneyDisplay", "Money display", inventory);
                for (int i = 0; i < 46; ++i)
                {
                    string name = "InventorySlot_" + i.ToString(CultureInfo.InvariantCulture);
                    string displayName = "Inventory Slot " + (i + 1).ToString(CultureInfo.InvariantCulture);
                    Component component = new Component(F, name, displayName, inventorySlots);
                }

                // Main menu buttons
                Component startGameButton = new Component(F, "StartGame", "Start Game", mainMenu);
                Component graphicsOptionsButton = new Component(F, "GraphicsOptions", "Graphics Options", mainMenu);
                Component controlOptionsButton = new Component(F, "ControlOptions", "Control Options", mainMenu);
                Component miscOptionsButton = new Component(F, "OtherOptions", "Other Options", mainMenu);
                Component quitGameButton = new Component(F, "QuitGame", "Quit Game", mainMenu);

                // Action bar
                Component actionBar = new Component(F, "ActionBar", "Action bar", gameScreen);
                Component xpBar = new Component(F, "XPBar", "XP points bar", gameScreen);
                Component chatButton = new Component(F, "ChatButton", "Chat button", gameScreen);
                Component mapButton = new Component(F, "MapButton", "Map button", gameScreen);
                Component inventoryButton = new Component(F, "InventoryButton", "Inventory button", gameScreen);
                Component abilitiesButton = new Component(F, "AbilitiesButton", "Abilities button", gameScreen);
                Component charStatsButton = new Component(F, "CharStatsButton", "Character button", gameScreen);
                Component questLogButton = new Component(F, "QuestLogButton", "Quest log button", gameScreen);
                Component partyButton = new Component(F, "PartyButton", "Party button", gameScreen);
                Component helpButton = new Component(F, "HelpButton", "Help button", gameScreen);
                Component nextBarButton = new Component(F, "NextBarButton", "Next bar button", gameScreen);
                Component prevBarButton = new Component(F, "PrevBarButton", "Previous bar button", gameScreen);
                for (int i = 0; i < 12; ++i)
                {
                    string name = "ActionBar_Button_" + i.ToString(CultureInfo.InvariantCulture);
                    string displayName = "Action bar button " + (i + 1).ToString(CultureInfo.InvariantCulture);
                    Component component = new Component(F, name, displayName, actionBarButtons);
                }
            }

            //_DumpComponentTree();

            return true;
        }

        // Only used for debugging.
        private static void _DumpComponentTree()
        {
            List<Component> roots = new List<Component>();
            Component c = FirstComponent;
            while (c != null)
            {
                Component root = c._FindRoot();
                if (!roots.Contains(root))
                {
                    roots.Add(root);
                }
                c = c.NextComponent;
            }
            int i = roots.Count;

            _visited = new List<Component>();
            foreach (Component root in roots)
            {
                _Dump(root);
            }

            int j = _visited.Count;
        }

        // Only used for debugging.
        private Component _FindRoot()
        {
            Component root = this;
            while (root.ParentNode != null)
            {
                root = root.ParentNode;
            }
            return root;
        }

        // Only used for debugging.

        // Only used for debugging.
        private static void _Dump(Component component)
        {
            _Dump(component, "");
        }

        // Only used for debugging.
        private static void _Dump(Component component, string indent)
        {
            bool wasAlreadyVisited = _visited.Contains(component);

            Console.WriteLine("{0}{1} (\"{11}\" X={2} Y={3} Width={4} Height={5} Alpha={6} R={7} G={8} B={9}){10}",
                              indent, component.name,
                              component.X,
                              component.Y,
                              component.Width,
                              component.Height,
                              component.Alpha,
                              component.R, component.G, component.B,
                              wasAlreadyVisited ? " (already visited)" : "",
                              component.displayed_name);
            Component child = component.FirstChild;
            while (child != null)
            {
                _Dump(child, indent + "    ");
                child = child.NextSibling;
            }

            _visited.Add(component);
        }

        // Saves the interface layout to a file
        public static bool SaveAll(string Filename)
        {
            // Notes: Mark has indicated that delivering the first release is number-one 
            // priority, and that design corrections will wait until later. Thus, I 
            // haven't taken any measures to extract string constants to a suitable place.
            // (Doing so would require some overall direction for strings and other
            // constants.) I also haven't made changes that would improve the efficiency
            // of this method. For example, calling FindComponent repeatedly seems hacky. 
            // My understanding is that after initial delivery Mark would like data 
            // access code to be factored into a separate component to be used by all RC 
            // applications. Design problems can be fixed then for data-access code.

            if (!Program.DemoVersion)
            {
                using (BinaryWriter writer = Blitz.WriteFile(Filename))
                {
                    if (writer == null)
                    {
                        return false;
                    }

                    // Main game screen
                    Component Chat = FindComponent("Chat");
                    Chat.WriteComponent(writer);
                    writer.Write(Chat.Texture);
                    FindComponent("ChatEntry").WriteComponent(writer);
                    for (int i = 0; i < Attributes.TotalAttributes; ++i)
                    {
                        string name = "GameScreen_ChatEntry_" + i.ToString(CultureInfo.InvariantCulture);
                        FindComponent(name).WriteComponent(writer);
                    }
                    FindComponent("ActorEffectsArea").WriteComponent(writer);
                    FindComponent("Radar").WriteComponent(writer);
                    FindComponent("Compass").WriteComponent(writer);

                    // Inventory
                    FindComponent("InventoryWindow").WriteComponent(writer);
                    FindComponent("DropItemButton").WriteComponent(writer);
                    // known in RCClient as InventoryDropComponent
                    FindComponent("UseItemButton").WriteComponent(writer); // known in RCClient as InventoryEat
                    FindComponent("MoneyDisplay").WriteComponent(writer); // known in RCClient as InventoryGold
                    for (int i = 0; i < 46; ++i)
                    {
                        string name = "InventorySlot_" + i.ToString(CultureInfo.InvariantCulture);
                        FindComponent(name).WriteComponent(writer);
                    }

                    // Main menu buttons
                    FindComponent("StartGame").WriteComponent(writer);
                    FindComponent("GraphicsOptions").WriteComponent(writer);
                    FindComponent("ControlOptions").WriteComponent(writer);
                    FindComponent("OtherOptions").WriteComponent(writer);
                    FindComponent("QuitGame").WriteComponent(writer);

                    // Action bar
                    FindComponent("ActionBar").WriteComponent(writer);
                    FindComponent("XPBar").WriteComponent(writer);
                    FindComponent("ChatButton").WriteComponent(writer);
                    FindComponent("MapButton").WriteComponent(writer);
                    FindComponent("InventoryButton").WriteComponent(writer);
                    FindComponent("AbilitiesButton").WriteComponent(writer);
                    FindComponent("CharStatsButton").WriteComponent(writer);
                    FindComponent("QuestLogButton").WriteComponent(writer);
                    FindComponent("PartyButton").WriteComponent(writer);
                    FindComponent("HelpButton").WriteComponent(writer);
                    FindComponent("NextBarButton").WriteComponent(writer);
                    FindComponent("PrevBarButton").WriteComponent(writer);
                    for (int i = 0; i < 12; ++i)
                    {
                        string name = "ActionBar_Button_" + i.ToString(CultureInfo.InvariantCulture);
                        ;
                        FindComponent(name).WriteComponent(writer);
                    }

                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        public static void GenerateDefaultInterface()
        {
            string path = @"Data\Game Data\DefaultInterface.dat";

            if (File.Exists(path))
            {
                LoadAll(path);
            }
            else
            {
                string message = string.Format(
                    "File not found: {0}.\nUnable to generate a default interface.", path);
                MessageBox.Show(message, "Realm Crafter Game Editor", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Assigns textures to all components' entities.
        /// </summary>
        /// <param name="filename">The path of the texture index XML file.</param>
        public static void LoadTextures(string filename)
        {
            if (filename == null)
            {
                throw new ArgumentNullException("filename");
            }

            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(filename);

            Component component = FirstComponent;
            while (component != null)
            {
                // If it's a real component rather than a container component.
                if (component.displayed_name != null)
                {
                    // Find the <texture> element whose name attribute matches the 
                    // component's name.
                    string xpath = string.Format(CultureInfo.InstalledUICulture,
                                                 "/Textures/texture[@name=\"{0}\"]", component.name);
                    XmlNode node = xmlDocument.SelectSingleNode(xpath);

                    if (node == null)
                    {
                        string message = string.Format(CultureInfo.InvariantCulture,
                                                       "Component name \"{0}\" not found in texture index file \"{1}\".",
                                                       component.name, filename);
                        throw new ApplicationException(message);
                    }

                    // Read the path and textureflag values from the <texture> element. 
                    // path is required, but textureflag isn't.
                    string path = node.Attributes["path"].Value;
                    int textureFlags = 0;
                    if (node.Attributes["textureflag"] != null)
                    {
                        textureFlags = Convert.ToInt32(node.Attributes["textureflag"].Value);
                    }

                    // Load the texture and attach it to the component's entity.
                    GE.PerformTexture(component.Handle, path, textureFlags);
                }
                component = component.NextComponent;
            }
        }
    }

    // Text rendering
    public class Text3D
    {
        // The client's character set
        public const string Letters =
            @" ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789,./\'#?<>[]();:!£$%^&*+-@~=_|àáâãäçèéêëïñòóôõöùúûüýÿßÆæØøÅåñÑ";

        // Generates a new font bitmap
        public static Bitmap GenerateFont(string Name, Font Font,
                                          int R, int G, int B,
                                          int SR, int SG, int SB, bool Shadow)
        {
            // Create rendering objects
            float ShadowOffset = (Font.Height / 12);
            Bitmap Image = new Bitmap(512, 512);
            Brush ShadowBrush = new SolidBrush(Color.FromArgb(SR, SG, SB));
            Brush TextBrush = new SolidBrush(Color.FromArgb(R, G, B));
            Graphics GraphicsImage = Graphics.FromImage(Image);
            GraphicsImage.TextRenderingHint = TextRenderingHint.AntiAlias;
            GraphicsImage.SmoothingMode = SmoothingMode.HighQuality;
            GraphicsImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
            GraphicsImage.CompositingQuality = CompositingQuality.HighQuality;

            // Arrays to store character data
            int[] CharWidth = new int[256];
            int[] CharX = new int[256];
            int[] CharY = new int[256];

            // Draw each letter in turn
            float X = 0, Y = 0, Width;
            for (int i = 0; i < Letters.Length; ++i)
            {
                // Get letter width
                Width = MeasureDisplaystringWidth(GraphicsImage, Letters.Substring(i, 1), Font) - 4f;
                // Make space character wider
                if (i == 0)
                {
                    Width = 45f;
                }
                // Wrap to next line if required
                if (X + Width >= 508f)
                {
                    X = 0f;
                    Y += Font.Height + 2f + ShadowOffset;
                }
                // Store character position/size
                CharWidth[i] = (int) Math.Floor(Width);
                CharX[i] = (int) (Math.Ceiling(X) + 3f);
                CharY[i] = (int) Y;
                if (Shadow)
                {
                    GraphicsImage.DrawString(Letters.Substring(i, 1), Font, ShadowBrush,
                                             new PointF(X + ShadowOffset, (Y - 2f) + ShadowOffset));
                }
                GraphicsImage.DrawString(Letters.Substring(i, 1), Font, TextBrush, new PointF(X, Y - 2f));
                X += (float) Math.Ceiling(Width) + 3f;
            }

            // Free rendering objects
            ShadowBrush.Dispose();
            TextBrush.Dispose();
            GraphicsImage.Dispose();

            // Save image and data
            if (!string.IsNullOrEmpty(Name))
            {
                File.Delete(Name + ".bmp");
                Image.Save(Name + ".bmp", ImageFormat.Bmp);

                BinaryWriter F = Blitz.WriteFile(Name + ".dat");
                F.Write((ushort) (Font.Height + 2));
                for (int i = 0; i < 256; ++i)
                {
                    F.Write(CharWidth[i]);
                    F.Write(CharX[i]);
                    F.Write(CharY[i]);
                }
                F.Close();
            }

            // Done
            return Image;
        }

        // Measures string widths more accurately
        public static float MeasureDisplaystringWidth(Graphics Graphics, string Text,
                                                      Font Font)
        {
            StringFormat format = new StringFormat();
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = {new CharacterRange(0, Text.Length)};
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);

            regions = Graphics.MeasureCharacterRanges(Text, Font, rect, format);
            rect = regions[0].GetBounds(Graphics);

            return rect.Right;
        }
    }

    // In-game controls
    public static class Controls
    {
        // All control bindings

        #region Key enum
        public enum Key
        {
            Forward,
            Back,
            TurnRight,
            TurnLeft,
            Run,
            FlyUp,
            FlyDown,
            Jump,
            AlwaysRun,
            CameraRight,
            CameraLeft,
            CameraIn,
            CameraOut,
            ChangeViewMode,
            Attack,
            CycleTarget,
            MoveTo,
            TalkTo,
            Select
        } ;
        #endregion

        public static bool AlwaysRun;

        public static byte InvertAxis1, InvertAxis3;
        public static int[] Keys = new int[19];

        // Other global variables
        public static byte ViewMode;

        // Control bindings load/save functions
        public static void LoadControlBindings(string Filename)
        {
            BinaryReader F = Blitz.ReadFile(Filename);

            F.Close();
        }

        public static void SaveControlBindings(string Filename)
        {
            BinaryWriter F = Blitz.WriteFile(Filename);

            F.Close();
        }

        // Control usage functions
        public static bool ControlHit(int Ctrl)
        {
            return false;
        }

        public static bool ControlDown(int Ctrl)
        {
            return false;
        }

        public static string ControlName(int Ctrl)
        {
            return "(Unknown)";
        }
    }

    public static class utils
    {
        public static string GetDefaultTextEditor()
        {
            RegistryKey r = Registry.ClassesRoot;
            r = r.OpenSubKey(@"SystemFileAssociations\text\shell\open\command");
            string[] registryNames = r.GetValueNames();
            string registryValue = r.GetValue(registryNames[0]).ToString();
            registryValue = registryValue.Replace(" %", "\0");
            return registryValue;
        }

        public static string GetDefaultPlayer()
        {
            RegistryKey r = Registry.ClassesRoot;
            r = r.OpenSubKey(@"SystemFileAssociations\video\DefaultIcon");
            string[] registryNames = r.GetValueNames();
            string registryValue = r.GetValue(registryNames[0]).ToString();
            registryValue = registryValue.Replace(",", "\0");
            return registryValue;
        }
    }
}