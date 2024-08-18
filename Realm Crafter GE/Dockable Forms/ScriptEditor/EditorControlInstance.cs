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
using System.Xml;
using System.IO;
using Scripting;
using RenderingServices;
using System.Drawing;
using System.Text.RegularExpressions;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public class EditorControlInstance : EditorControlInstanceBase
    {
        public EditorControl EditorControl;
        public List<EditorPropertyInstance> Properties = new List<EditorPropertyInstance>();
        public List<EditorEventInstance> Events = new List<EditorEventInstance>();
        public EditorControlInstance Parent;
        public EditorControlInstance Master;
        public FormEditor Editor;
        public List<EditorControlInstance> Children = new List<EditorControlInstance>();
        public int AllocID = 0;

        public EditorControlInstance(EditorControl editorControl)
        {
            this.EditorControl = editorControl;

            foreach (EditorProperty P in editorControl.Properties)
            {
                Properties.Add(new EditorPropertyInstance(P));
            }

            foreach (EditorEvent E in editorControl.Events)
            {
                Events.Add(new EditorEventInstance(E));
            }
        }

        public void CreatePacket(PacketWriter Pa, ref int allocID)
        {
            ++allocID;
            AllocID = allocID;

            Pa.Write(EditorControl.SDKName, false);
            Pa.Write("ALID", false);
            Pa.Write(AllocID);

            foreach (EditorPropertyInstance P in Properties)
            {
                
                    if (this == Master && P.EditorProperty.Name.Equals("location", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Pa.Write(P.EditorProperty.SDKName, false);
                        EditorProperty.WritePacket(Pa, P.EditorProperty.Type, new Vector2(0, 0));

                    }
                    else if (this == Master && P.EditorProperty.Name.Equals("positiontype", StringComparison.CurrentCultureIgnoreCase))
                    {
                        Pa.Write(P.EditorProperty.SDKName, false);
                        EditorProperty.WritePacket(Pa, P.EditorProperty.Type, PositionType.Absolute);
                    }
                    else
                    {
                        if (P.EditorProperty.AlwaysSet || !P.Value.Equals(P.EditorProperty.DefaultValue))
                        {
                            Pa.Write(P.EditorProperty.SDKName, false);
                            EditorProperty.WritePacket(Pa, P.EditorProperty.Type, P.Value);
                        }
                    }
            }

            if (Children.Count > 0)
            {
                Pa.Write("CHLD", false);
                Pa.Write((ushort)Children.Count);

                foreach (EditorControlInstance C in Children)
                {
                    PacketWriter cPa = new PacketWriter();
                    C.CreatePacket(cPa, ref allocID);

                    Pa.Write(cPa.Length);
                    Pa.Write(cPa.ToArray(), 0);
                }
            }


            Pa.Write("ENDL", false);
        }

        public override object GetPropertyValue(int index)
        {
            if (index < 0 || index >= Properties.Count)
                return null;
            return Properties[index].Value;
        }

        public override void SetPropertyValue(int index, object value)
        {
            if (index < 0 || index >= Properties.Count)
                return;

            Properties[index].Value = value;

            if (Master != null && Master.Editor != null)
            {
                Master.Editor.SetSaved(false);

                if (Properties[index].EditorProperty.Name.Equals("name", StringComparison.CurrentCultureIgnoreCase))
                {
                    int ID = 0;
                    foreach (EditorControlInstance C in Master.Editor.AllControls)
                    {
                        if (C == this)
                            Program.GE.m_ScriptView.m_Properties.ControlsList.Items[ID] = value.ToString();

                        ++ID;
                    }
                }

                if (AllocID > 0)
                {
                    if (Master.Editor == Program.GE.m_ScriptView.CurrentForm)
                    {
                        // Write property update
                        PacketWriter Pa = new PacketWriter();
                        Pa.Write(AllocID);
                        Pa.Write(Properties[index].EditorProperty.SDKName, false);
                        EditorProperty.WritePacket(Pa, Properties[index].EditorProperty.Type, Properties[index].Value);

                        // Update
                        SDKNet.ControlHost.ProcessUpdateProperties(Pa.ToArray());

                        // Reset selection
                        Master.Editor.SelectControl(this);
                    }
                }
            }
        }

        public override string GetEventValue(int index)
        {
            if (index < 0 || index >= Events.Count)
                return "";

            return Events[index].FuncName;
        }

        ScriptForm IsScriptOpen()
        {
            if (Master == null || Master.Editor == null)
                return null;

            string file = Master.Editor.LoadedFileName.Length > 11 ? Master.Editor.LoadedFileName.Substring(0, Master.Editor.LoadedFileName.Length - 11) : Master.Editor.LoadedFileName;

            foreach (ScriptEditorForm documentForm in Program.GE.m_ScriptView.ScriptDock.Documents)
            {
                if (file == documentForm.TabText || file + " *" == documentForm.TabText)
                {
                    return documentForm as ScriptForm;
                    break;
                }
            }

            return null;
        }

        int MethodExistsInScript(string method, string[] data)
        {
            Regex R = new Regex("void[\\s]*" + method + "[\\s]*\\(");
            int ID = 1;
            foreach (string Ln in data)
            {
                if (R.IsMatch(Ln))
                {
                    return ID;
                }

                ++ID;
            }

            return 0;
        }

        public void GoToMethod(string method)
        {
            if (Master == null || Master.Editor == null)
                return;

            // Check and update script
            string file = Master.Editor.LoadedFileName.Length > 11 ? Master.Editor.LoadedFileName.Substring(0, Master.Editor.LoadedFileName.Length - 11) : Master.Editor.LoadedFileName;
            ScriptForm Sc = IsScriptOpen();
            string[] ScriptData = new string[] { };

            if (Sc != null)
            {
                ScriptData = new string[Sc.scriptText.Lines.Count];
                int ID = 0;
                foreach (ScintillaNet.Line SLine in Sc.scriptText.Lines)
                {
                    ScriptData[ID++] = SLine.Text;
                }
            }
            else
            {
                ScriptData = File.ReadAllLines(@"Data\Server Data\Scripts\" + file);
                for (int i = 0; i < ScriptData.Length; ++i)
                    ScriptData[i] += Environment.NewLine;

            }

            int LineNumber = MethodExistsInScript(method, ScriptData);

            // Not found!
            if (LineNumber > 0)
            {
                if (Sc == null)
                {
                    Program.GE.m_ScriptView.OpenSpecifiedScript(file);
                    Sc = IsScriptOpen();
                }

                if (Sc != null)
                {
                    string Texts = "";
                    int SelStart = 0;

                    for (int i = 0; i < ScriptData.Length; ++i)
                    {
                        Texts += ScriptData[i];

                        if (i == LineNumber + 1)
                            SelStart = Texts.Length;
                    }

                    Sc.scriptText.Selection.Start = SelStart - Environment.NewLine.Length;
                    Sc.scriptText.Selection.Length = 0;

                    Sc.Activate();
                }
            }
        }

        public override void SetEventValue(int index, string value)
        {
            if (index < 0 || index >= Events.Count)
                return;

            if (Events[index].FuncName != value)
            {
                Events[index].FuncName = value;

                if (Master == null || Master.Editor == null)
                    return;

                // Check and update script
                string file = Master.Editor.LoadedFileName.Length > 11 ? Master.Editor.LoadedFileName.Substring(0, Master.Editor.LoadedFileName.Length - 11) : Master.Editor.LoadedFileName;
                ScriptForm Sc = IsScriptOpen();
                string[] ScriptData = new string[] {};

                if (Sc != null)
                {
                    ScriptData = new string[Sc.scriptText.Lines.Count];
                    int ID = 0;
                    foreach (ScintillaNet.Line SLine in Sc.scriptText.Lines)
                    {
                        ScriptData[ID++] = SLine.Text;
                    }
                }
                else
                {
                    ScriptData = File.ReadAllLines(@"Data\Server Data\Scripts\" + file);
                    for (int i = 0; i < ScriptData.Length; ++i)
                        ScriptData[i] += Environment.NewLine;

                }

                int LineNumber = MethodExistsInScript(value, ScriptData);

                // Not found!
                if (LineNumber == 0)
                {
                    // Find insertion point
                    int InsertLine = 0;
                    int Cnt = 0;
                    Regex R = new Regex("[\\s]*}");
                    for (int i = ScriptData.Length - 1; i >= 0; --i)
                    {
                        if(R.IsMatch(ScriptData[i]))
                        {
                            ++Cnt;
                            if(Cnt == 2)
                            {
                                InsertLine = i;
                                break;
                            }
                        }
                    }

                    if (InsertLine > 0)
                    {
                        string WhiteSpace = ScriptData[InsertLine].Substring(0, ScriptData[InsertLine].IndexOf("}"));

                        if (Sc == null)
                        {
                            Program.GE.m_ScriptView.OpenSpecifiedScript(file);
                            Sc = IsScriptOpen();
                        }

                        string[] InsertData = new string[] {
                            WhiteSpace + "    private void " + value + "(" + Events[index].EditorEvent.Args + ")" + Environment.NewLine,
                            WhiteSpace + "    {" + Environment.NewLine,
                            WhiteSpace + "        " + Environment.NewLine,
                            WhiteSpace + "    }" + Environment.NewLine,
                            WhiteSpace + "    " + Environment.NewLine };

                        string InsertDataLn = "";
                        for (int i = 0; i < InsertData.Length; ++i)
                            InsertDataLn += InsertData[i];

                        int InsertPos = 0;
                        for (int i = 0; i < InsertLine; ++i)
                        {
                            InsertPos += ScriptData[i].Length;
                        }

                         List<string> NewData = new List<string>(ScriptData);
                         NewData.InsertRange(InsertLine, InsertData);

                        

                        if (Sc != null)
                        {
                            string Texts = "";
                            int SelStart = 0;

                            for(int i = 0; i < NewData.Count; ++i)
                            {
                                Texts += NewData[i];

                                if(i == InsertLine + 2)
                                    SelStart = Texts.Length;
                            }

                            //SelStart += InsertData[0].Length + InsertData[1].Length + InsertData[2].Length;

                            //Sc.scriptText.Text = Texts;
                            Sc.scriptText.InsertText(InsertPos, InsertDataLn);
                            Sc.scriptText.Selection.Start = (SelStart - Environment.NewLine.Length);
                            Sc.scriptText.Selection.Length = 0;

                            Sc.Activate();
                        }
                    }
                }
                

            }
        }

        public Vector2 GetAbsolutePosition()
        {
            if (this == Master)
                return new Vector2();

            Vector2 Out = new Vector2();

            PacketWriter Pa = new PacketWriter();
            Pa.Write(AllocID);
            Pa.Write(0.0f);
            Pa.Write(0.0f);

            PacketReader Reader = new PacketReader(SDKNet.ControlHost.ReadoutAbsoluteLocation(Pa.ToArray()));

            Reader.ReadInt32();
            Out.X = Reader.ReadSingle();
            Out.Y = Reader.ReadSingle();

            Out.X *= (float)Program.GE.RenderingPanel.Width;
            Out.Y *= (float)Program.GE.RenderingPanel.Height;

//             EditorPropertyInstance LocationProperty = FindPropertyInstance("location");
//             EditorPropertyInstance PositionTypeProperty = FindPropertyInstance("positiontype");
// 
//             if (LocationProperty != null && PositionTypeProperty != null)
//             {
//                 PositionType PType = (PositionType)PositionTypeProperty.Value;
//                 Vector2 PLoc = (Vector2)LocationProperty.Value;
// 
//                 if (PType == PositionType.Absolute)
//                 {
//                     Out = (Vector2)PLoc.Clone();
//                 }
//                 else if (PType == PositionType.Relative)
//                 {
//                     Out = (Vector2)PLoc.Clone();
//                     Out.X *= (float)Program.GE.RenderingPanel.Width;
//                     Out.Y *= (float)Program.GE.RenderingPanel.Height;
//                 }
//             }
// 
//             if (Parent != null)
//             {
//                 Vector2 ParentPos = Parent.GetAbsolutePosition();
//                 Out.X += ParentPos.X;
//                 Out.Y += ParentPos.Y;
//             }

            return Out;
        }

        public Vector2 GetAbsoluteSize()
        {
            Vector2 Out = new Vector2();

            EditorPropertyInstance SizeProperty = FindPropertyInstance("size");
            EditorPropertyInstance SizeTypeProperty = FindPropertyInstance("sizetype");

            if (SizeProperty != null && SizeTypeProperty != null)
            {
                SizeType SType = (SizeType)SizeTypeProperty.Value;
                Vector2 Size = (Vector2)SizeProperty.Value;

                if (SType == SizeType.Absolute)
                {
                    Out = (Vector2)Size.Clone();
                }
                else if (SType == SizeType.Relative)
                {
                    Out = (Vector2)Size.Clone();
                    Out.X *= (float)Program.GE.RenderingPanel.Width;
                    Out.Y *= (float)Program.GE.RenderingPanel.Height;
                }
            }

            return Out;
        }

        public EditorControlInstance FindPlacementParent(Point mousePos)
        {
            if (EditorControl.AllowChildren == false)
                return null;

            Vector2 Position = GetAbsolutePosition();
            Vector2 Size = GetAbsoluteSize();

            if ((float)mousePos.X > Position.X && (float)mousePos.Y > Position.Y)
            {
                if ((float)mousePos.X < Position.X + Size.X && (float)mousePos.Y < Position.Y + Size.Y)
                {
                    for (int i = Children.Count - 1; i >= 0; --i)
                    {
                        EditorControlInstance Out = Children[i].FindPlacementParent(mousePos);
                        if (Out != null)
                            return Out;
                    }

                    return this;
                }
            }

            return null;
        }

        public EditorControlInstance FindControl(Point mousePos)
        {
            Vector2 Position = GetAbsolutePosition();
            Vector2 Size = GetAbsoluteSize();

            if ((float)mousePos.X > Position.X && (float)mousePos.Y > Position.Y)
            {
                if ((float)mousePos.X < Position.X + Size.X && (float)mousePos.Y < Position.Y + Size.Y)
                {
                    for (int i = Children.Count - 1; i >= 0; --i)
                    {
                        EditorControlInstance Out = Children[i].FindControl(mousePos);
                        if (Out != null)
                            return Out;
                    }

                    return this;
                }
            }

            return null;
        }


        public EditorPropertyInstance FindPropertyInstance(string name)
        {
            foreach (EditorPropertyInstance P in Properties)
            {
                if (P.EditorProperty.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return P;
            }

            return null;
        }

        public EditorEventInstance FindOrCreateEvent(string name)
        {
            // Find existing event
            foreach (EditorEventInstance E in Events)
            {
                if (E.EditorEvent.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return E;
            }

            // Find an uncreated instance
            foreach (EditorEvent E in this.EditorControl.Events)
            {
                if (E.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                {
                    EditorEventInstance V = new EditorEventInstance(E);
                    Events.Add(V);
                    return V;
                }
            }

            // The name doesn't exist at all
            return null;
        }

        public void WriteControl(XmlTextWriter x)
        {
            x.WriteStartElement("control");
            x.WriteAttributeString("type", EditorControl.Name);

            foreach (EditorPropertyInstance P in Properties)
            {
                x.WriteStartElement("property");
                x.WriteAttributeString("name", P.EditorProperty.Name);
                x.WriteAttributeString("value", EditorProperty.ToString(P.EditorProperty.Type, P.Value));
                x.WriteEndElement();
            }

            foreach (EditorEventInstance E in Events)
            {
                if (E.FuncName.Length > 0)
                {
                    x.WriteStartElement("event");
                    x.WriteAttributeString("name", E.EditorEvent.Name);
                    x.WriteAttributeString("value", E.FuncName);
                    x.WriteEndElement();
                }
            }

            foreach (EditorControlInstance C in Children)
            {
                C.WriteControl(x);
            }

            x.WriteEndElement();
        }

        public void WriteInstantiators(StreamWriter w)
        {
            foreach (EditorControlInstance C in Children)
            {
                EditorPropertyInstance P = C.FindPropertyInstance("name");
                if (P != null)
                {
                    w.WriteLine("            " + P.Value.ToString() + " = new " + C.EditorControl.FullName + "();");
                }

                C.WriteInstantiators(w);
            }
        }

        public void WriteDefinitions(StreamWriter w)
        {
            foreach (EditorControlInstance C in Children)
            {
                EditorPropertyInstance P = C.FindPropertyInstance("name");
                if (P != null)
                {
                    w.WriteLine("        public " + C.EditorControl.FullName + " " + P.Value.ToString() + ";");
                }

                C.WriteDefinitions(w);
            }
        }

        public void WriteProperties(StreamWriter w)
        {
            foreach (EditorControlInstance C in Children)
            {
                EditorPropertyInstance P = C.FindPropertyInstance("name");
                if (P != null)
                {
                    w.WriteLine("            ///");
                    w.WriteLine("            ///" + P.Value.ToString());
                    w.WriteLine("            ///");

                    foreach (EditorPropertyInstance Ep in C.Properties)
                    {
                        if (Ep.EditorProperty.AlwaysSet || !Ep.Value.Equals(Ep.EditorProperty.DefaultValue))
                        {
                            w.WriteLine("            " + P.Value.ToString() + "." + Ep.EditorProperty.Name + EditorProperty.DesignerString(Ep.EditorProperty.Type, Ep.Value));
                        }
                    }

                    foreach (EditorEventInstance Ev in C.Events)
                    {
                        if(Ev.FuncName.Length > 0)
                            w.WriteLine("            " + P.Value.ToString() + "." + Ev.EditorEvent.Name + " += new " + Ev.EditorEvent.Handler + "(" + Ev.FuncName + ");");
                    }

                    if (Parent != null)
                    {
                        if (Parent != Master)
                        {
                            EditorPropertyInstance ParentP = Parent.FindPropertyInstance("name");
                            w.WriteLine("            " + P.Value.ToString() + ".Parent = " + ParentP.Value.ToString() + ";");
                        }
                        else
                        {
                            w.WriteLine("            " + P.Value.ToString() + ".Parent = this;");
                        }
                    }

                }

                C.WriteProperties(w);
            }

            if (Children.Count > 0)
            {
                string ThisName = "this";

                if (this != Master)
                {
                    EditorPropertyInstance ThisP = this.FindPropertyInstance("name");
                    if (ThisP != null)
                        ThisName = ThisP.Value.ToString();
                    else ThisName = "Error";
                }

                w.WriteLine("            ///");
                w.WriteLine("            ///" + ThisName.ToString());
                w.WriteLine("            ///");

                foreach (EditorControlInstance C in Children)
                {
                    EditorPropertyInstance P = C.FindPropertyInstance("name");
                    if (P != null)
                    {
                        w.WriteLine("            " + ThisName.ToString() + ".Controls.Add(" + P.Value.ToString() + ");");
                    }
                }
            }
                
        }


        public void Save(string xmlPath, string designerPath, string sourcePath)
        {
            try
            {
                // Write XML (most important)
                XmlTextWriter X = new XmlTextWriter(xmlPath, Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                X.WriteStartDocument();
                X.WriteStartElement("userform");

                Master.WriteControl(X);

                X.WriteEndElement();
                X.WriteEndDocument();
                X.Close();

                // Find class info
                StreamReader R = new StreamReader(sourcePath);
                string ClassName = "";
                string NamespaceName = "";

                while (!R.EndOfStream)
                {
                    string Ln = R.ReadLine();

                    if (ClassName.Length == 0)
                    {
                        int Off = Ln.IndexOf("partial class ");
                        if (Off > -1)
                        {
                            int Spc = Ln.IndexOf(" ", Off + 14);
                            if (Spc > -1)
                                ClassName = Ln.Substring(Off + 14, Spc - (Off + 14));
                            else
                                ClassName = Ln.Substring(Off + 14);
                        }
                    }

                    if (NamespaceName.Length == 0)
                    {
                        int Off = Ln.IndexOf("namespace ");
                        if (Off > -1)
                        {
                            int Spc = Ln.IndexOf(" ", Off + 10);
                            if (Spc > -1)
                                NamespaceName = Ln.Substring(Off + 10, Spc - (Off + 10));
                            else
                                NamespaceName = Ln.Substring(Off + 10);
                        }
                    }
                }

                R.Close();

                if (ClassName.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot write designer file, ClassName not found!\n" + designerPath);
                    return;
                }
                if (NamespaceName.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Cannot write designer file, NamespaceName not found!\n" + designerPath);
                    return;
                }

                // Write text
                StreamWriter W = new StreamWriter(designerPath);

                W.WriteLine("using System;");
                W.WriteLine("using System.Collections.Generic;");
                W.WriteLine("using System.Text;");
                W.WriteLine("using Scripting;");
                W.WriteLine("");
                W.WriteLine("namespace " + NamespaceName);
                W.WriteLine("{");
                W.WriteLine("    public partial class " + ClassName + " : " + Master.EditorControl.FullName);
                W.WriteLine("    {");
                W.WriteLine("        private void InitializeComponent()");
                W.WriteLine("        {");

                Master.WriteInstantiators(W);
                Master.WriteProperties(W);

                foreach (EditorPropertyInstance Ep in Master.Properties)
                {
                    if (Ep.EditorProperty.AlwaysSet || !Ep.Value.Equals(Ep.EditorProperty.DefaultValue))
                    {
                        W.WriteLine("            this." + Ep.EditorProperty.Name + EditorProperty.DesignerString(Ep.EditorProperty.Type, Ep.Value));
                    }
                }

                W.WriteLine("        }");
                W.WriteLine("        ");

                Master.WriteDefinitions(W);

                W.WriteLine("    }");
                W.WriteLine("}");
                W.WriteLine("");
                W.Close();



            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "EditorControlInstance.Save");
            }
        }

        public static EditorControlInstance Load(string path, List<EditorControlInstance> allInstances)
        {
            XmlTextReader X = new XmlTextReader(path);

            EditorControlInstance MasterControl = null;
            EditorControlInstance ParentControl = null;

            while (X.Read())
            {
                if (X.NodeType == XmlNodeType.Element && X.Name.Equals("control", StringComparison.CurrentCultureIgnoreCase))
                {
                    string TypeName = X.GetAttribute("type");

                    if (string.IsNullOrEmpty(TypeName)) { TypeName = ""; }

                    EditorControl TypeControl = EditorControl.FindType(TypeName);
                    if (TypeControl == null)
                        throw new Exception("Error: Couldn't find control type '" + TypeName + "'!");

                    EditorControlInstance Instance = new EditorControlInstance(TypeControl);
                    allInstances.Add(Instance);
                    if (MasterControl == null)
                        MasterControl = Instance;
                    Instance.Master = MasterControl;

                    Instance.Parent = ParentControl;
                    if (Instance.Parent != null)
                        Instance.Parent.Children.Add(Instance);
                    ParentControl = Instance;
                }
                else if (X.NodeType == XmlNodeType.EndElement && X.Name.Equals("control", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (ParentControl != null)
                        ParentControl = ParentControl.Parent;
                }
                else if (X.NodeType == XmlNodeType.Element && X.Name.Equals("property", StringComparison.CurrentCultureIgnoreCase))
                {
                    string Name = X.GetAttribute("name");
                    string ValueString = X.GetAttribute("value");

                    if (string.IsNullOrEmpty(Name)) { Name = ""; }
                    if (string.IsNullOrEmpty(ValueString)) { ValueString = ""; }

                    ValueString = ValueString.Replace("&lt;", "<");
                    ValueString = ValueString.Replace("&gt;", ">");

                    if (ParentControl != null)
                    {
                        EditorPropertyInstance P = ParentControl.FindPropertyInstance(Name);
                        if (P != null)
                        {
                            P.Value = EditorProperty.Parse(P.EditorProperty.Type, ValueString);
                        }
                    }
                }
                else if (X.NodeType == XmlNodeType.Element && X.Name.Equals("event", StringComparison.CurrentCultureIgnoreCase))
                {
                    string Name = X.GetAttribute("name");
                    string ValueString = X.GetAttribute("value");

                    if (string.IsNullOrEmpty(Name)) { Name = ""; }
                    if (string.IsNullOrEmpty(ValueString)) { ValueString = ""; }

                    if (ParentControl != null)
                    {
                        EditorEventInstance E = ParentControl.FindOrCreateEvent(Name);
                        if (E != null)
                        {
                            E.FuncName = ValueString;
                        }
                    }
                }

            }

            X.Close();

            return MasterControl;
        }

    }
}
