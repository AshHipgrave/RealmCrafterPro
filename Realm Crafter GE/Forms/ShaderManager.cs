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
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Xml;

namespace RealmCrafter_GE
{
    public partial class ShaderManager : Form
    {
        public static LoadedEffect[] LoadedShaders = new LoadedEffect[2048];
        public static ShaderProfile[] LoadedProfiles = new ShaderProfile[2048];
        public static ShaderProfile DefaultStatic = null, DefaultAnimated = null;

        public static uint GetShader(int ID)
        {
            if (ID < 0 || ID > LoadedShaders.Length)
            {
                return 0;
            }

            if (LoadedShaders[ID] == null)
            {
                return 0;
            }

            return LoadedShaders[ID].Effect;
        }

        public static uint GetShader(string Name)
        {
            foreach (LoadedEffect Le in LoadedShaders)
            {
                if (Le != null && Le.Name == Name)
                {
                    return Le.Effect;
                }
            }
            return 65535;
        }
        


        public static int GetShaderID(uint Handle)
        {
            foreach (LoadedEffect Le in LoadedShaders)
            {
                if (Le != null && Le.Effect == Handle)
                {
                    return Le.ID;
                }
            }
            return 0;
        }

        public static string GetShaderName(uint Handle)
        {
            foreach (LoadedEffect Le in LoadedShaders)
            {
                if (Le != null && Le.Effect == Handle)
                {
                    return Le.Name;
                }
            }
            return "No Shader";
        }


        public static ShaderProfile GetProfile(int ID)
        {
            foreach (ShaderProfile Pr in LoadedProfiles)
            {
                if (Pr != null && Pr.ID == ID)
                {
                    return Pr;
                }
            }
            return null;
        }

        /// <summary>
        /// Load shaders into memory ready for editing and applying.
        /// </summary>
        public static void LoadShaders()
        {
            // Reader
            XmlTextReader X = null;

            try
            {
                // Open file
                X = new XmlTextReader(@"Data\Game Data\Shaders\Shaders.xml");
                DefaultStatic = null;
                DefaultAnimated = null;

                // Read next element
                while (X.Read())
                {
                    // If its a <shader> element
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "shader")
                    {
                        // Setup effect
                        LoadedEffect Le = new LoadedEffect();
                        Le.Name = X.GetAttribute("name");
                        Le.ID = Convert.ToInt32(X.GetAttribute("id"));
                        Le.Path = X.GetAttribute("path");
                        //Le.bIsWaterShader = X.GetAttribute("water")
                        string sWater = X.GetAttribute("water");
                        if (sWater == null) Le.bIsWaterShader = false;
                                      else  Le.bIsWaterShader = sWater.Equals("true", StringComparison.CurrentCultureIgnoreCase);
                        Le.Effect = RenderingServices.Shaders.Load("Data\\Game Data\\Shaders\\" + Le.Path);

                        // Assign list index
                        LoadedShaders[Le.ID] = Le;
                    }
                }
            }
            catch (System.Xml.XmlException e)
            {
                // Eek an error
                MessageBox.Show("Error: " + e.Message, "LoadShaders", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                // Cleanup
                X.Close();
            }

            try
            {
                // Open file
                X = new XmlTextReader(@"Data\Game Data\Shaders\Profiles.xml");

                // Read next element
                while (X.Read())
                {
                    // If its a <shader> element
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "profile")
                    {
                        int ID = Convert.ToInt32(X.GetAttribute("id"));
                        float Range = Convert.ToSingle(X.GetAttribute("range"));
                        String Name = X.GetAttribute("name");

                        int hnid = Convert.ToInt32(X.GetAttribute("hn"));
                        int hfid = Convert.ToInt32(X.GetAttribute("hf"));
                        int mnid = Convert.ToInt32(X.GetAttribute("mn"));
                        int mfid = Convert.ToInt32(X.GetAttribute("mf"));
                        int lnid = Convert.ToInt32(X.GetAttribute("ln"));
                        int lfid = Convert.ToInt32(X.GetAttribute("lf"));

                        ShaderProfile Pr = new ShaderProfile(Name, ID);
                        Pr.HighNear = GetShader(hnid);
                        Pr.HighFar = GetShader(hfid);
                        Pr.MediumNear = GetShader(mnid);
                        Pr.MediumFar = GetShader(mfid);
                        Pr.LowNear = GetShader(lnid);
                        Pr.LowFar = GetShader(lfid);
                        Pr.Range = Range;

                        String Default = X.GetAttribute("default");
                        if (Default != null && Default.ToLower() == "static")
                        {
                            DefaultStatic = Pr;
                        }
                        if (Default != null && Default.ToLower() == "animated")
                        {
                            DefaultAnimated = Pr;
                        }

                        if (DefaultStatic == null && DefaultAnimated != Pr)
                        {
                            DefaultStatic = Pr;
                        }
                        if (DefaultAnimated == null && DefaultStatic != Pr)
                        {
                            DefaultAnimated = Pr;
                        }

                        LoadedProfiles[ID] = Pr;
                    }
                }
            }
            catch (System.Xml.XmlException e)
            {
                // Eek an error
                MessageBox.Show("Error: " + e.Message, "LoadProfiles", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                // Cleanup
                X.Close();
            }
        }

        /// <summary>
        /// Save Shaders to their XML files
        /// </summary>
        public static void SaveShaders()
        {
            try
            {
                // Write XML file
                XmlTextWriter X = new XmlTextWriter(@"Data\Game Data\Shaders\Shaders.xml", Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                // Base parts
                X.WriteStartDocument();
                X.WriteStartElement("effects");

                // Write effects
                foreach (LoadedEffect Le in LoadedShaders)
                {
                    if (Le != null)
                    {
                        X.WriteStartElement("shader");

                        X.WriteStartAttribute("id");
                        X.WriteString(Le.ID.ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("name");
                        X.WriteString(Le.Name);
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("path");
                        X.WriteString(Le.Path);
                        X.WriteEndAttribute();

                        if (Le.bIsWaterShader) 
                        {
                            X.WriteStartAttribute("water");
                            X.WriteString(Le.bIsWaterShader.ToString());
                            X.WriteEndAttribute();
                        }

                        X.WriteEndElement();
                    }
                }

                // Close and clean
                X.WriteEndElement();
                X.WriteEndDocument();
                X.Flush();
                X.Close();
            }
            catch (Exception e)
            {
                // Eek an error
                MessageBox.Show("Error: " + e.Message, "SaveShaders", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            try
            {
                // Write XML file
                XmlTextWriter X = new XmlTextWriter(@"Data\Game Data\Shaders\Profiles.xml", Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                // Base parts
                X.WriteStartDocument();
                X.WriteStartElement("profiles");

                // Write effects
                foreach (ShaderProfile Pr in LoadedProfiles)
                {
                    if (Pr != null)
                    {
                        X.WriteStartElement("profile");

                        X.WriteStartAttribute("id");
                        X.WriteString(Pr.ID.ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("range");
                        X.WriteString(Pr.Range.ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("name");
                        X.WriteString(Pr.Name);
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("hn");
                        X.WriteString(GetShaderID(Pr.HighNear).ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("hf");
                        X.WriteString(GetShaderID(Pr.HighFar).ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("mn");
                        X.WriteString(GetShaderID(Pr.MediumNear).ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("mf");
                        X.WriteString(GetShaderID(Pr.MediumFar).ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("ln");
                        X.WriteString(GetShaderID(Pr.LowNear).ToString());
                        X.WriteEndAttribute();

                        X.WriteStartAttribute("lf");
                        X.WriteString(GetShaderID(Pr.LowFar).ToString());
                        X.WriteEndAttribute();

                        if (DefaultStatic == Pr)
                        {
                            X.WriteStartAttribute("default");
                            X.WriteString("static");
                            X.WriteEndAttribute();
                        }

                        if (DefaultAnimated == Pr)
                        {
                            X.WriteStartAttribute("default");
                            X.WriteString("animated");
                            X.WriteEndAttribute();
                        }

                        X.WriteEndElement();
                    }
                }

                // Close and clean
                X.WriteEndElement();
                X.WriteEndDocument();
                X.Flush();
                X.Close();
            }
            catch (Exception e)
            {
                // Eek an error
                MessageBox.Show("Error: " + e.Message, "SaveProfiles", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected ShaderManagerEdit Editor = null;
        protected ShaderProfileEdit ProfileEditor = null;

        public ShaderManager()
        {
            InitializeComponent();

            // Copy loaded to listbox
            foreach (LoadedEffect Le in LoadedShaders)
            {
                if (Le != null)
                {
                    this.ShaderListBox.Items.Add(Le.Name);
                }
            }

            foreach (ShaderProfile Pr in LoadedProfiles)
            {
                if (Pr != null)
                {
                    this.ProfileListBox.Items.Add(Pr.Name);
                }
            }

            // Create dialogs
            Editor = new ShaderManagerEdit();
            ProfileEditor = new ShaderProfileEdit();
        }

        private void NewShaderButton_Click(object sender, EventArgs e)
        {
            // Setup dialog
            Editor.SelectedName = "";
            Editor.SelectedPath = "";
            Editor.EditMode = ShaderManagerEditMode.Add;
            Editor.Text = "Add Shader...";
            Editor.DResult = DialogResult.Cancel;
            Editor.ShowDialog();
            DialogResult Dr = Editor.DResult;

            if (Dr == DialogResult.Cancel)
            {
                return;
            }

            // Find free slot
            int ID = -1, Pos = 0;
            foreach (LoadedEffect Le in LoadedShaders)
            {
                if (Le == null)
                {
                    ID = Pos;
                    break;
                }
                ++Pos;
            }

            // Error
            if (ID == -1)
            {
                MessageBox.Show("Could not import shader. Maximum shader count exceeded!");
            }

            // Copy
            String Path = Editor.SelectedPath;
            String Name = System.IO.Path.GetFileName(Path);
            if (Path.Length > 0 && Name.Length > 0)
            {
                string FullSrc = System.IO.Path.GetFullPath(Path);
                string FullDest = System.IO.Path.GetFullPath(@"Data\Game Data\Shaders\" + Name);
                if(!FullSrc.Equals(FullDest, StringComparison.CurrentCultureIgnoreCase))
                    System.IO.File.Copy(Path, @"Data\Game Data\Shaders\" + Name, true);

                // Setup effect
                LoadedEffect L = new LoadedEffect();
                L.ID = ID;
                L.Name = Editor.SelectedName;
                L.Path = Name;
                L.bIsWaterShader = Editor.IsWaterShader;
                L.Effect = RenderingServices.Shaders.Load("Data\\Game Data\\Shaders\\" + L.Path);

                // Use/Save
                LoadedShaders[ID] = L;
                ShaderListBox.Items.Add(L.Name);
                SaveShaders();
            }
        }

        private void EditShaderButton_Click(object sender, EventArgs e)
        {
            // Find selected shader
            if (ShaderListBox.SelectedItem == null || ShaderListBox.SelectedItem as String == "")
            {
                return;
            }

            // Find selected shader
            String SelectedName = ShaderListBox.SelectedItem as String;
            LoadedEffect Le = null;
            foreach (LoadedEffect iL in LoadedShaders)
            {
                if (iL != null && iL.Name == SelectedName)
                {
                    Le = iL;
                }
            }

            if (Le == null)
            {
                return;
            }

            // Setup dialog
            Editor.SelectedName  = Le.Name;
            Editor.SelectedPath  = Le.Path;
            Editor.IsWaterShader = Le.bIsWaterShader;
            Editor.EditMode = ShaderManagerEditMode.Edit;
            Editor.Text = "Edit Shader: " + Le.Name;

            Editor.DResult = DialogResult.Cancel;
            Editor.ShowDialog();
            DialogResult Dr = Editor.DResult;

            if (Dr == DialogResult.Cancel)
            {
                return;
            }

            // Update listbox
            int Pos = 0;
            int Index = -1;
            foreach (object I in ShaderListBox.Items)
            {
                String S = I as String;
                if (S != null && S == Le.Name)
                {
                    Index = Pos;
                    break;
                }
                ++Pos;
            }
            if (Index > -1)
            {
                ShaderListBox.Items[Index] = Editor.SelectedName;
            }

            // Save
            Le.Name = Editor.SelectedName;
            Le.bIsWaterShader = Editor.IsWaterShader;
            SaveShaders();
        }

        private void DeleteShaderButton_Click(object sender, EventArgs e)
        {
            // Find selected shader
            if (ShaderListBox.SelectedItem == null || ShaderListBox.SelectedItem as String == "")
            {
                return;
            }

            // Find selected shader
            String SelectedName = ShaderListBox.SelectedItem as String;
            LoadedEffect Le = null;
            foreach (LoadedEffect iL in LoadedShaders)
            {
                if (iL != null && iL.Name == SelectedName)
                {
                    Le = iL;
                }
            }

            if (Le == null)
            {
                return;
            }

            // Check what sort of delete we're doing
            bool FoundDependency = false;

            foreach (ShaderProfile Pr in LoadedProfiles)
            {
                if (Pr != null)
                {
                    if (GetShaderID(Pr.HighNear) == Le.ID)
                    {
                        FoundDependency = true;
                    }
                    if (GetShaderID(Pr.HighFar) == Le.ID)
                    {
                        FoundDependency = true;
                    }
                    if (GetShaderID(Pr.MediumNear) == Le.ID)
                    {
                        FoundDependency = true;
                    }
                    if (GetShaderID(Pr.MediumFar) == Le.ID)
                    {
                        FoundDependency = true;
                    }
                    if (GetShaderID(Pr.LowNear) == Le.ID)
                    {
                        FoundDependency = true;
                    }
                    if (GetShaderID(Pr.LowFar) == Le.ID)
                    {
                        FoundDependency = true;
                    }
                }
            }

            String ConfirmMsg = "Are you sure you want to delete '" + Le.Name + "'?";
            if (FoundDependency)
            {
                ConfirmMsg += "\n\nWarning: Deleting this shader will cause some profiles to function incorrectly!";
            }

            DialogResult Dr = MessageBox.Show(ConfirmMsg, "Delete?", MessageBoxButtons.OKCancel);
            if (Dr == DialogResult.Cancel)
            {
                return;
            }

            // Update listbox
            int Pos = 0;
            int Index = -1;
            foreach (object I in ShaderListBox.Items)
            {
                String S = I as String;
                if (S != null && S == Le.Name)
                {
                    Index = Pos;
                    break;
                }
                ++Pos;
            }
            if (Index > -1)
            {
                ShaderListBox.Items.RemoveAt(Index);
            }

            LoadedShaders[Le.ID] = null;
            SaveShaders();
        }

        private void EditProfileButton_Click(object sender, EventArgs e)
        {
            // Find selected profile
            if (ProfileListBox.SelectedItem == null || ProfileListBox.SelectedItem as String == "")
            {
                return;
            }

            String SelectedName = ProfileListBox.SelectedItem as String;
            ShaderProfile Pr = null;
            foreach (ShaderProfile iL in LoadedProfiles)
            {
                if (iL != null && iL.Name == SelectedName)
                {
                    Pr = iL;
                }
            }

            if (Pr == null)
            {
                return;
            }

            ProfileEditor.SelectedProfile = Pr;
            ProfileEditor.UpdateProfile();
            ProfileEditor.Text = "Edit Profile: " + Pr.Name;

            String OldName = Pr.Name;
            ProfileEditor.EditMode = ShaderManagerEditMode.Edit;
            ProfileEditor.DResult = DialogResult.Cancel;
            ProfileEditor.ShowDialog();
            DialogResult Dr = ProfileEditor.DResult;

            if (Dr == DialogResult.Cancel)
            {
                return;
            }

            // Update listbox
            int Pos = 0;
            int Index = -1;
            foreach (object I in ProfileListBox.Items)
            {
                String S = I as String;
                if (S != null && S == OldName)
                {
                    Index = Pos;
                    break;
                }
                ++Pos;
            }
            if (Index > -1)
            {
                ProfileListBox.Items[Index] = Pr.Name;
            }

            // Save
            SaveShaders();
        }

        private void NewProfileButton_Click(object sender, EventArgs e)
        {
            int ID = -1;
            int Pos = 0;
            foreach (ShaderProfile P in LoadedProfiles)
            {
                if (P == null)
                {
                    ID = Pos;
                    break;
                }
                ++Pos;
            }

            ShaderProfile Pr = new ShaderProfile("New Profile", ID);

            ProfileEditor.DResult = DialogResult.Cancel;
            ProfileEditor.SelectedProfile = Pr;
            ProfileEditor.UpdateProfile();
            ProfileEditor.Text = "Add Profile...";
            ProfileEditor.EditMode = ShaderManagerEditMode.Add;
            ProfileEditor.ShowDialog();
            DialogResult Dr = ProfileEditor.DResult;

            if (Dr == DialogResult.Cancel)
            {
                return;
            }

            ProfileListBox.Items.Add(Pr.Name);
            LoadedProfiles[ID] = Pr;
            SaveShaders();
        }

        private void DeleteProfileButton_Click(object sender, EventArgs e)
        {
            // Find selected profile
            if (ProfileListBox.SelectedItem == null || ProfileListBox.SelectedItem as String == "")
            {
                return;
            }

            String SelectedName = ProfileListBox.SelectedItem as String;
            ShaderProfile Pr = null;
            foreach (ShaderProfile iL in LoadedProfiles)
            {
                if (iL != null && iL.Name == SelectedName)
                {
                    Pr = iL;
                }
            }

            if (Pr == null)
            {
                return;
            }

            DialogResult Dr = MessageBox.Show("Are you sure you want to delete '" + Pr.Name + "'?", "Delete profile",
                                              MessageBoxButtons.OKCancel);
            if (Dr == DialogResult.Cancel)
            {
                return;
            }

            // Update listbox
            int Pos = 0;
            int Index = -1;
            foreach (object I in ProfileListBox.Items)
            {
                String S = I as String;
                if (S != null && S == Pr.Name)
                {
                    Index = Pos;
                    break;
                }
                ++Pos;
            }
            if (Index > -1)
            {
                ProfileListBox.Items.RemoveAt(Index);
            }

            LoadedProfiles[Pr.ID] = null;
            SaveShaders();
        }
    }

    public class ShaderAnnotation
    {
        public string Name;
        public object Value;
    }

    public class ShaderParameter
    {
        public string Name;
        public object Value;
        public Dictionary<string, ShaderAnnotation> Annotations = new Dictionary<string,ShaderAnnotation>();
    }

    /// <summary>
    /// Instance of a loaded shader ready to be used in a profile.
    /// </summary>
    public class LoadedEffect
    {
        public String Name;
        public String Path;
        public bool bIsWaterShader;
        public int ID;
        private uint _Effect;

        public Dictionary<string, ShaderParameter> Parameters = new Dictionary<string, ShaderParameter>();

        public uint Effect
        {
            get { return _Effect; }
            set
            {
                // Clear and copy
                Parameters.Clear();
                _Effect = value;
                if (_Effect == 0)
                    return;

                // Fetch new parameter count
                int PCnt = RenderingServices.RenderWrapper.GetParameterCount(_Effect);

                // Loop them
                for (int i = 0; i < PCnt; ++i)
                {
                    // Fetch annotation count for parameter
                    int ACnt = RenderingServices.RenderWrapper.GetAnnotationCount(_Effect, i);

                    // Generate the parameter
                    ShaderParameter P = new ShaderParameter();
                    P.Name = RenderingServices.RenderWrapper.GetParameterName(_Effect, i);
                    P.Value = RenderingServices.RenderWrapper.GetParameterValue(_Effect, i);

                    // Loop annotations
                    for (int f = 0; f < ACnt; ++f)
                    {
                        // Generate annotation
                        ShaderAnnotation A = new ShaderAnnotation();
                        A.Name = RenderingServices.RenderWrapper.GetAnnotationName(_Effect, i, f);
                        A.Value = RenderingServices.RenderWrapper.GetAnnotationValue(_Effect, i, f);

                        P.Annotations.Add(A.Name, A);
                    }

                    Parameters.Add(P.Name, P);
                }
            }
        }
    }

    public class ShaderProfile
    {
        private uint _Handle = 0;
        private String _Name = "";
        private int _ID = 0;

        public ShaderProfile(String Name, int ID)
        {
            _Handle = RenderingServices.RenderWrapper.CreateProfile(Name);
            _Name = Name;
            _ID = ID;
        }

        ~ShaderProfile()
        {
            // Never free a profile, assigned meshes will die!
            //if (_Handle != 0)
            //    RenderingServices.RenderWrapper.FreeProfile(_Handle);
        }

        public uint Handle
        {
            get { return _Handle; }
        }

        public String Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public int ID
        {
            get { return _ID; }
        }

        public float Range
        {
            get { return RenderingServices.RenderWrapper.GetProfileRange(_Handle); }
            set { RenderingServices.RenderWrapper.SetProfileRange(_Handle, value); }
        }

        public uint HighNear
        {
            get { return RenderingServices.RenderWrapper.GetProfileEffect(_Handle, 2, 1); }
            set { RenderingServices.RenderWrapper.SetProfileEffect(_Handle, 2, 1, value); }
        }

        public uint HighFar
        {
            get { return RenderingServices.RenderWrapper.GetProfileEffect(_Handle, 2, 0); }
            set { RenderingServices.RenderWrapper.SetProfileEffect(_Handle, 2, 0, value); }
        }

        public uint MediumNear
        {
            get { return RenderingServices.RenderWrapper.GetProfileEffect(_Handle, 1, 1); }
            set { RenderingServices.RenderWrapper.SetProfileEffect(_Handle, 1, 1, value); }
        }

        public uint MediumFar
        {
            get { return RenderingServices.RenderWrapper.GetProfileEffect(_Handle, 1, 0); }
            set { RenderingServices.RenderWrapper.SetProfileEffect(_Handle, 1, 0, value); }
        }

        public uint LowNear
        {
            get { return RenderingServices.RenderWrapper.GetProfileEffect(_Handle, 0, 1); }
            set { RenderingServices.RenderWrapper.SetProfileEffect(_Handle, 0, 1, value); }
        }

        public uint LowFar
        {
            get { return RenderingServices.RenderWrapper.GetProfileEffect(_Handle, 0, 0); }
            set { RenderingServices.RenderWrapper.SetProfileEffect(_Handle, 0, 0, value); }
        }
    }
}