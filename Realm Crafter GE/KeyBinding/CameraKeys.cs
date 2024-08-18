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
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RealmCrafter_GE.KeyBinding
{
    public partial class CameraKeys : Form
    {
        KeyBinding.Action SelectedAction;
        bool SelectedKey;

        public CameraKeys()
        {
            InitializeComponent();
            SelectedKey = false;
            Program.KeyBindings.Load();
            RefreshButtons();
        }

        private void Apply_Click(object sender, EventArgs e)
        {
            Program.KeyBindings.Save();
            Close();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            // Load up old ones
            Program.KeyBindings.Load();
            Close();
        }

        private void KeyBind_Click(object sender, EventArgs e)
        {
            Button button = sender as Button;
            // Note each buttons tag is string name of a KeyBinding.Action enum element
            SelectedAction = (KeyBinding.Action)Enum.Parse(typeof(KeyBinding.Action), button.Tag as string);
            SelectedKey = true;
    
        }

        private void CameraKeys_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void RefreshButtons()
        {
            // Setup name of key for each button - uses tag's to work out which enum is represents
            // Recurse through controls and label buttons - this is because of group boxes being parents
            Queue<Control> controlQueue = new Queue<Control>();
            controlQueue.Enqueue(this);

            while (controlQueue.Count > 0)
            {
                Control child = controlQueue.Dequeue();

                if (child is Button ? (child as Button).Tag != null : false)
                {
                 
                    Button button = child as Button;
                    button.Text = Program.KeyBindings.KeyMap[(KeyBinding.Action)Enum.Parse(typeof(KeyBinding.Action), button.Tag as string)].ToString();
                }
                else
                    foreach (Control c in child.Controls)
                        controlQueue.Enqueue(c);
            }

        }

        private void CameraKeys_KeyPress(object sender, KeyEventArgs e)
        {
            if (SelectedKey)
            {
                // Assign keypress to keybinding
                Program.KeyBindings.KeyMap[SelectedAction] = e.KeyCode;

                SelectedKey = false;
                RefreshButtons();
            }
        }

        private void Bind_MouseDown(object sender, MouseEventArgs e)
        {
            if (SelectedKey && e.Button == System.Windows.Forms.MouseButtons.Middle)
            {
                // Assign keypress to keybinding
                Program.KeyBindings.KeyMap[SelectedAction] = Keys.MButton;

                SelectedKey = false;
                RefreshButtons();
            }
        }


        private void CameraKeys_Load(object sender, EventArgs e)
        {

        }

        private void DefaultButton_Click(object sender, EventArgs e)
        {
            Program.KeyBindings.Default();
            RefreshButtons();
        }


    }
}
