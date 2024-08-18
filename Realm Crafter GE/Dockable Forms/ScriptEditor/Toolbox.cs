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
using WeifenLuo.WinFormsUI.Docking;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class Toolbox : DockContent
    {
        protected List<Panel> Sets = new List<Panel>();

        public Toolbox()
        {
            InitializeComponent();
        }

        public void CreateSet(string name)
        {
            Panel P = new Panel();
            P.Dock = DockStyle.Fill;
            P.Name = name;
            P.Visible = false;
            P.AutoScroll = true;
            P.Scroll += new ScrollEventHandler(SetPanelScroll);

            this.Controls.Add(P);
            Sets.Add(P);
        }

        void SetPanelScroll(object sender, ScrollEventArgs e)
        {
            Panel P = (sender as Panel);

            P.Invalidate();

            foreach (Control C in P.Controls)
            {
                C.Invalidate();
                foreach (Control cC in C.Controls)
                    cC.Invalidate();
            }

        }

        public void AddCategory(string provider, string category)
        {
            foreach (Panel P in Sets)
                if (P.Name.Equals(provider, StringComparison.CurrentCultureIgnoreCase))
                    AddHeading(P, category);
        }

        public void Add(string provider, string category, EditorControl command)
        {
            foreach (Panel P in Sets)
                if (P.Name.Equals(provider, StringComparison.CurrentCultureIgnoreCase))
                    foreach (Control C in P.Controls)
                        if (C.GetType() == typeof(ToolListHeading) && (C as ToolListHeading).Text.Equals(category, StringComparison.CurrentCultureIgnoreCase))
                            AddItem(C, command);
        }

        public void AddItem(Control parent, EditorControl command)
        {
            ToolListItem Item = new ToolListItem(parent as ToolListHeading, command.Icon, command);
            Item.Dock = DockStyle.Bottom;
            Item.Size = new Size(100, 20);
            Item.Text = command.Name;
            Item.Tag = command;

            _ToolTip.SetToolTip(Item, command.Name);

            Item.Font = this.Font;

            parent.Controls.Add(Item);

            parent.Height = /*19*/19 + parent.Controls.Count * 20;
        }

        public void ShowPage(string provider)
        {
            foreach (Panel P in Sets)
                P.Visible = false;

            foreach (Panel P in Sets)
                if (P.Name.Equals(provider, StringComparison.CurrentCultureIgnoreCase))
                {
                    P.Visible = true;
                }
        }

        public void TempAddItem(String Txt, ToolListHeading H)
        {
            ToolListItem Item = new ToolListItem(H, null, null);
            Item.Dock = DockStyle.Top;
            Item.Location = new Point(0, 0);
            Item.Size = new Size(100, 20);
            Item.Text = Txt;

            this.Controls.Add(Item);
        }

        public ToolListHeading AddHeading(Panel panel, String txt)
        {
            ToolListHeading H = new ToolListHeading();
            H.Dock = DockStyle.Top;
            H.Location = new Point(0, 0);
            //H.Size = new Size(100, 19);
            H.Text = txt;
            H.Click += new EventHandler(H_Click);

            //this.Controls.Add(H);
            panel.Controls.Add(H);
            return H;
        }

        void H_Click(object sender, EventArgs e)
        {
            foreach (Control C in Controls)
            {
                if (C is ToolListItem)
                {
                    if ((C as ToolListItem).Heading == sender as ToolListHeading)
                    {
                        C.Visible = !C.Visible;
                    }
                }
            }
            //throw new Exception("The method or operation is not implemented.");
        }

        private void PaintTimer_Tick(object sender, EventArgs e)
        {
            foreach (Control C0 in Controls)
            {
                foreach (Control C1 in C0.Controls)
                    C1.Invalidate();
                C0.Invalidate();
            }
        }
    }
}