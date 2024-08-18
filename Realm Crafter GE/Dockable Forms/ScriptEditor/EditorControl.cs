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
using System.IO;
using System.Xml;
using System.Drawing;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class EditorControl
    {
        public string Name = "";
        public string SDKName = "";
        public string DefaultName = "";
        public string FullName = "";
        public string DefaultEvent = "";
        public bool AllowChildren = false;
        public List<EditorProperty> Properties = new List<EditorProperty>();
        public List<EditorEvent> Events = new List<EditorEvent>();
        public Image Icon;
        public Type PropertyEditorType;

        public static List<EditorControl> Controls = new List<EditorControl>();

        public static EditorControl FindType(string typeName)
        {
            foreach (EditorControl C in Controls)
            {
                if (C.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase))
                    return C;
            }

            return null;
        }

        public static void Load(string path)
        {
            Program.GE.m_ScriptView.m_ToolBox.CreateSet("CTRL");
            Program.GE.m_ScriptView.m_ToolBox.AddCategory("CTRL", "All Controls");

            string[] Files = Directory.GetFiles(path, "*.xml");

            foreach (string Filename in Files)
            {
                XmlTextReader X = new XmlTextReader(Filename);

                EditorControl LoadingControl = new EditorControl();

                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("control", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Name = X.GetAttribute("name");
                        string SDKName = X.GetAttribute("sdktype");
                        string DefaultName = X.GetAttribute("defaultname");
                        string FullName = X.GetAttribute("fullname");
                        string AllowChildren = X.GetAttribute("allowchildren");
                        string DefaultEvent = X.GetAttribute("defaultevent");

                        if (string.IsNullOrEmpty(Name)) { Name = ""; }
                        if (string.IsNullOrEmpty(SDKName)) { SDKName = ""; }
                        if (string.IsNullOrEmpty(DefaultName)) { DefaultName = ""; }
                        if (string.IsNullOrEmpty(FullName)) { FullName = ""; }
                        if (string.IsNullOrEmpty(AllowChildren)) { AllowChildren = "false"; }
                        if (string.IsNullOrEmpty(DefaultEvent)) { DefaultEvent = ""; }

                        LoadingControl.Name = Name;
                        LoadingControl.SDKName = SDKName;
                        LoadingControl.DefaultName = DefaultName;
                        LoadingControl.FullName = FullName;
                        LoadingControl.AllowChildren = Convert.ToBoolean(AllowChildren);
                        LoadingControl.DefaultEvent = DefaultEvent;

                        try
                        {
                            LoadingControl.Icon = Image.FromFile(Path.Combine(path, Path.GetFileNameWithoutExtension(Filename)) + ".png");
                        }
                        catch (System.Exception ex)
                        {
                            LoadingControl.Icon = null;
                        }
                        
                    }
                    else if (X.NodeType == XmlNodeType.Element && X.Name.Equals("property", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Name = X.GetAttribute("name");
                        string SDKName = X.GetAttribute("sdkname");
                        string TypeName = X.GetAttribute("type");
                        string DefaultString = X.GetAttribute("default");
                        string AlwaysSetString = X.GetAttribute("alwaysset");

                        if (string.IsNullOrEmpty(Name)) { Name = ""; }
                        if (string.IsNullOrEmpty(SDKName)) { SDKName = ""; }
                        if (string.IsNullOrEmpty(TypeName)) { TypeName = ""; }
                        if (string.IsNullOrEmpty(DefaultString)) { DefaultString = ""; }
                        if (string.IsNullOrEmpty(AlwaysSetString)) { AlwaysSetString = "false"; }

                        PropertyType Type = EditorProperty.TypeFromString(TypeName);
                        object DefaultValue = EditorProperty.Parse(Type, DefaultString);
                        bool AlwaysSet = false;
                        try { AlwaysSet = Convert.ToBoolean(AlwaysSetString); }
                        catch (Exception) { }

                        EditorProperty Property = new EditorProperty();
                        Property.Name = Name;
                        Property.SDKName = SDKName;
                        Property.Type = Type;
                        Property.DefaultValue = DefaultValue;
                        Property.AlwaysSet = AlwaysSet;

                        LoadingControl.Properties.Add(Property);
                    }
                    else if (X.NodeType == XmlNodeType.Element && X.Name.Equals("event", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Name = X.GetAttribute("name");
                        string Handler = X.GetAttribute("handler");
                        string Args = X.GetAttribute("args");

                        if (string.IsNullOrEmpty(Name)) { Name = ""; }
                        if (string.IsNullOrEmpty(Handler)) { Handler = ""; }
                        if (string.IsNullOrEmpty(Args)) { Args = ""; }

                        EditorEvent Event = new EditorEvent();
                        Event.Name = Name;
                        Event.Handler = Handler;
                        Event.Args = Args;

                        LoadingControl.Events.Add(Event);
                    }

                }

                Controls.Add(LoadingControl);

                // Little hardcoded hack to hide the 'Form' control
                // Its not anything that people can place, its only ever a base.
                if(LoadingControl.Name != "Form")
                    Program.GE.m_ScriptView.m_ToolBox.Add("CTRL", "All Controls", LoadingControl);

                // Generate a property sheet.
                LoadingControl.CreatePropertySheet();

                X.Close();

            } // foreach(Files)

            Program.GE.m_ScriptView.m_ToolBox.ShowPage("CTRL");

        } // Load
    }
}
