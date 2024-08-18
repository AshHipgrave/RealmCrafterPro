using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class EditorProperties : DockContent
    {
        public EditorProperties()
        {
            InitializeComponent();
        }

        private void ControlsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (Program.GE.m_ScriptView.CurrentForm != null)
            {
                Program.GE.m_ScriptView.CurrentForm.SelectControl(ControlsList.SelectedIndex);
            }
        }
    }
}