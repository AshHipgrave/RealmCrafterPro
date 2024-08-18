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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class NewScriptForm : Form
    {
        public NewScriptForm()
        {
            InitializeComponent();

            ScriptTypeCombo.SelectedIndex = 0;
        }

        public int ScriptType
        {
            get { return ScriptTypeCombo.SelectedIndex; }
        }

        public string ScriptName
        {
            get { return ScriptNameBox.Text; }
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            Close();
        }

        public static string[] DefaultScript = new string[] { 
            "using System;",
            "using System.Collections.Generic;",
            "using System.Text;",
            "using Scripting;",
            "",
            "namespace UserScripts",
            "{",
            "    /// <summary>",
            "    /// Enter description here",
            "    /// </summary>",
            "    public class %ScriptName% : ScriptBase",
            "    {",
            "        ",
            "        public void Main(Actor actor, Actor contextActor)",
            "        {",
            "            ",
            "        }",
            "    }",
            "}" };

        public static string[] DefaultFormScript = new string[] {
            "using System;",
            "using System.Collections.Generic;",
            "using System.Text;",
            "using Scripting.Forms;",
            "using Scripting;",
            "",
            "namespace UserScripts",
            "{",
            "    /// <summary>",
            "    /// Enter description here",
            "    /// </summary>",
            "    public partial class %ScriptName% : Form",
            "    {",
            "    ",
            "        public %ScriptName%()",
            "        {",
            "            InitializeComponent();",
            "        }",
            "        ",
            "    }",
            "}" };

        public static string[] DefaultDesignerScript = new string[] {
            "using System;",
            "using System.Collections.Generic;",
            "using System.Text;",
            "using Scripting;",
            "",
            "namespace UserScripts",
            "{",
            "    public partial class %ScriptName% : Scripting.Forms.Form",
            "    {",
            "        private void InitializeComponent()",
            "        {",
            "            ///",
            "            /// %ScriptName%",
            "            /// ",
            "            this.Location = new Scripting.Math.Vector2(0.5f, 0.5f);",
            "            this.PositionType = Scripting.Forms.PositionType.Centered;",
            "            this.Size = new Scripting.Math.Vector2(300, 300);",
            "        }",
            "    }",
            "}" };

        public static string[] DefaultDesignerData = new string[] {
            "<?xml version=\"1.0\"?>",
            "<userform>",
            "	<control type=\"Form\">",
            "		<property name=\"Location\" value=\"0.5, 0.5\" />",
            "		<property name=\"PositionType\" value=\"Centered\" />",
            "		<property name=\"Size\" value=\"300, 300\" />",
            "	</control>",
            "</userform>" };

    }
}