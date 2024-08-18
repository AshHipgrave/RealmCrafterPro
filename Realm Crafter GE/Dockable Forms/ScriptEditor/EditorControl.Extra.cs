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
using Microsoft.CSharp;
using System.Windows.Forms;
using System.Reflection;
using System.CodeDom.Compiler;
using System.IO;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class EditorControl
    {
        public void CreatePropertySheet()
        {
            string SheetData = Header(Name);

            int ID = 0;
            foreach (EditorProperty P in Properties)
            {
                switch (P.Type)
                {
                    case PropertyType.Bool:
                        {
                            SheetData += PropertyBool(P.Name, ID);
                            break;
                        }
                    case PropertyType.Byte:
                        {
                            SheetData += PropertyByte(P.Name, ID);
                            break;
                        }
                    case PropertyType.Int16:
                        {
                            SheetData += PropertyShort(P.Name, ID);
                            break;
                        }
                    case PropertyType.Int32:
                        {
                            SheetData += PropertyInt(P.Name, ID);
                            break;
                        }
                    case PropertyType.Single:
                        {
                            SheetData += PropertyFloat(P.Name, ID);
                            break;
                        }
                    case PropertyType.TextAlign:
                        {
                            SheetData += PropertyTextAlign(P.Name, ID);
                            break;
                        }
                    case PropertyType.SizeType:
                        {
                            SheetData += PropertySizeType(P.Name, ID);
                            break;
                        }
                    case PropertyType.PositionType:
                        {
                            SheetData += PropertyPositionType(P.Name, ID);
                            break;
                        }
                    case PropertyType.String:
                        {
                            SheetData += PropertyString(P.Name, ID);
                            break;
                        }
                    case PropertyType.Vector2:
                        {
                            SheetData += PropertyVector2(P.Name, ID);
                            break;
                        }
                    case PropertyType.Vector3:
                        {
                            SheetData += PropertyVector3(P.Name, ID);
                            break;
                        }
                    case PropertyType.StringArray:
                        {
                            SheetData += PropertyStringArray(P.Name, ID);
                            break;
                        }
                    case PropertyType.Color:
                        {
                            SheetData += PropertyColor(P.Name, ID);
                            break;
                        }
                }

                ++ID;
            } // Foreach

            ID = 0;
            foreach (EditorEvent Ev in Events)
            {
                SheetData += EventString(Ev.Name, ID);
                ++ID;
            }

            SheetData += Footer();

            // Compile!
            CSharpCodeProvider Provider = new CSharpCodeProvider();
            if (Provider == null)
            {
                throw new Exception("Could not create code provider!");
            }

            string rootDir = @"../../";
            if (Program.TestingVersion)
                rootDir = @"./";

            CompilerParameters Parameters = new CompilerParameters();
            Parameters.GenerateInMemory = false;
            Parameters.GenerateExecutable = false;
            Parameters.ReferencedAssemblies.Add(Path.Combine(rootDir, "SDKCompiler.dll"));
            Parameters.ReferencedAssemblies.Add("System.dll");
            Parameters.ReferencedAssemblies.Add("System.Data.dll");
            Parameters.ReferencedAssemblies.Add("System.Drawing.dll");
            
            Parameters.IncludeDebugInformation = false;
            Parameters.TempFiles.KeepFiles = true;
            //Parameters.TempFiles.AddExtension("pdb", true);

            int Count = 0;
            bool Errors = false;

            CompilerResults Results = Provider.CompileAssemblyFromSource(Parameters, new string[] { SheetData });

            bool Failed = false;
            if (Results.Errors.Count > 0)
            {
                foreach (CompilerError E in Results.Errors)
                {
                    if (!E.IsWarning)
                    {
                        Failed = true;
                        Errors = true;
                    }

                }
            }

            if (Failed)
            {
                // DV: Dump results to file
                StringBuilder errorStr = new StringBuilder();

                errorStr.Append("Errors:\n");
                foreach (CompilerError E in Results.Errors)
                {
                    errorStr.AppendFormat("  {5}({0},{1}): {2} {3}: {4}\n", new object[] { E.Line, E.Column, E.IsWarning ? "Warning" : "Error", E.ErrorNumber, E.ErrorText, Path.GetFileName(E.FileName) });
                }
                errorStr.Append("\n\n--------------------------------------------------------\n\n");
                errorStr.Append(SheetData);

                File.WriteAllText("EditorControl error log.txt", errorStr.ToString());

                MessageBox.Show("Failed to compile assembly for: '" + Name + "'");
                throw new Exception("Failed to compile assembly for: '" + Name + "'");
            }

            Assembly LoadedAssembly = Results.CompiledAssembly;
            PropertyEditorType = LoadedAssembly.GetType("RealmCrafter_GE.Dockable_Forms.ScriptEditor." + Name + "ControlProperties");
            if (PropertyEditorType == null)
            {
                MessageBox.Show("Couldn't locate Type for: '" + Name + "'");
                throw new Exception("Couldn't locate Type for: '" + Name + "'");
            }
        }

        string EventString(string name, int id)
        {
            return "[CategoryAttribute(\"Events\")]" +
                "public string " + name +
                "{" +
                "get { return Parent.GetEventValue(" + id + "); }" +
                "set { Parent.SetEventValue(" + id + ", value); }" +
                "}";
        }

        string Header(string name)
        {
            return "using System;" +
                "using System.Collections.Generic;" +
                "using System.Text;" +
                "using RenderingServices;" +
                "using System.ComponentModel;" +
                "using System.Drawing;" +
                "" +
                "namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor" +
                "{" +
                "public class " + name + "ControlProperties : ControlProperties" +
                "{";
        }

        string Footer()
        {
            return "}}";
        }

        string PropertyBool(string name, int id)
        {
            return DefaultProperty("bool", name, id, "");
        }

        string PropertyByte(string name, int id)
        {
            return DefaultProperty("byte", name, id, "");
        }

        string PropertyShort(string name, int id)
        {
            return DefaultProperty("short", name, id, "");
        }

        string PropertyInt(string name, int id)
        {
            return DefaultProperty("int", name, id, "");
        }

        string PropertyFloat(string name, int id)
        {
            return DefaultProperty("float", name, id, "");
        }

        string PropertyString(string name, int id)
        {
            return DefaultProperty("string", name, id, "");
        }

        string PropertyTextAlign(string name, int id)
        {
            return DefaultProperty("TextAlign", name, id, "");
        }

        string PropertyPositionType(string name, int id)
        {
            return DefaultProperty("PositionType", name, id, "");
        }

        string PropertySizeType(string name, int id)
        {
            return DefaultProperty("SizeType", name, id, "");
        }

        string PropertyVector2(string name, int id)
        {
            return DefaultProperty("Vector2", name, id, ", TypeConverterAttribute(typeof(Vector2OptionsConverter))");
        }

        string PropertyVector3(string name, int id)
        {
            return DefaultProperty("Vector3", name, id, ", TypeConverterAttribute(typeof(Vector3OptionsConverter))");
        }

        string PropertyStringArray(string name, int id)
        {
            return DefaultProperty("List<string>", name, id, ", TypeConverterAttribute(typeof(CollectionOptionsConverter)), Editor(typeof(CollectionTypeEditor), typeof(System.Drawing.Design.UITypeEditor))");
        }
        
        string DefaultProperty(string type, string name, int id, string extraProperties)
        {
            return "[CategoryAttribute(\"Properties\") " + extraProperties + "]" +
                "public " + type + " " + name +
                "{"+
                "get { return (" + type + ")Parent.GetPropertyValue(" + id + "); }"+
                "set { Parent.SetPropertyValue(" + id + ", value); }"+
                "}";
        }

        string PropertyColor(string name, int id)
        {
            return "[CategoryAttribute(\"Properties\"), TypeConverterAttribute(typeof(Vector4ColorOptionsConverter)), Editor(typeof(Vector4ColorEditor), typeof(System.Drawing.Design.UITypeEditor))]" +
                "public Vector4 " + name +
                "{" +
                "get" +
                "{" +
                "Color C = (Color)Parent.GetPropertyValue(" + id + ");" +
                "Vector4 Out = new Vector4(" +
                "((float)C.R) / 255.0f," +
                "((float)C.G) / 255.0f," +
                "((float)C.B) / 255.0f," +
                "((float)C.A) / 255.0f);" +
                "return Out;" +
                "}" +
                "set" +
                "{" +
                "value.X *= 255.0f;" +
                "value.Y *= 255.0f;" +
                "value.Z *= 255.0f;" +
                "value.W *= 255.0f;" +
                "Parent.SetPropertyValue(" + id + ", Color.FromArgb(" +
                "(byte)value.W," +
                "(byte)value.X," +
                "(byte)value.Y," +
                "(byte)value.Z));" +
                "}" +
                "}";
        }

    }
}
