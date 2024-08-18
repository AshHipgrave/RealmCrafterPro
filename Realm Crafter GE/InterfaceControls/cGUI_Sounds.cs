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

using System.ComponentModel;
using NGUINet;
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using Microsoft.Win32;


namespace RealmCrafter_GE.Property_Interfaces
{
    public class cGUI_Sounds : System.Drawing.Design.UITypeEditor  
    {
        Form tempForm;
        public string selectedFile;

        public cGUI_Sounds()
        {
            int firstRowCoordinateX = 5, firstRowCoordinateY = 20, secondRowCoordinateY;
            tempForm = new Form();

            GroupBox tempGroupbox = new GroupBox();
            tempGroupbox.Text = "Choose files for menu \"Click\" and \"Beep\" sounds:";
            tempGroupbox.Left = 5;
            tempGroupbox.Top = 1;
            tempForm.Controls.Add(tempGroupbox);

            //add label info for "Click"
            Label tempLabel = new Label();
            tempLabel.Text = "File for \"Click\":";
            tempLabel.Left = firstRowCoordinateX;
            tempLabel.Top = firstRowCoordinateY;
            tempLabel.Width = 1;
            tempLabel.AutoSize = true;

            tempGroupbox.Controls.Add(tempLabel);

            //add textbox for "Click"
            TextBox tempTextbox = new TextBox();
            tempTextbox.Left = tempLabel.Left + tempLabel.Width;
            tempTextbox.Top = tempLabel.Top - ((tempTextbox.Height / 2) - (tempLabel.Height / 2));
            tempTextbox.Width = 150;
            tempTextbox.Name = "ClickTextbox";
            tempGroupbox.Controls.Add(tempTextbox);

            //add button for "Click"
            Button tempButton = new Button();
            tempButton.Text = "...";
            tempButton.Left = tempTextbox.Left + tempTextbox.Width + 2;
            tempButton.Top = tempLabel.Top - ((tempButton.Height / 2) - (tempLabel.Height / 2));
            tempButton.Width = 30;
            tempButton.Name = "ClickButton";
            tempButton.Click += new EventHandler(this.FindSoundFile_Click);
            tempGroupbox.Controls.Add(tempButton);

            secondRowCoordinateY = firstRowCoordinateY + tempButton.Height + 5;

            //add label info for "Beep"
            tempLabel = new Label();
            tempLabel.Text = "File for \"Beep\":";
            tempLabel.Left = firstRowCoordinateX;
            tempLabel.Top = secondRowCoordinateY;
            tempLabel.Width = 1;
            tempLabel.AutoSize = true;
            tempGroupbox.Controls.Add(tempLabel);

            //add textbox for "Beep"
            tempTextbox = new TextBox();
            tempTextbox.Left = tempLabel.Left + tempLabel.Width;
            tempTextbox.Top = tempLabel.Top - ((tempTextbox.Height / 2) - (tempLabel.Height / 2));
            tempTextbox.Width = 150;
            tempTextbox.Name = "BeepTextbox";
            tempGroupbox.Controls.Add(tempTextbox);

            //add button for "Click"
            tempButton = new Button();
            tempButton.Text = "...";
            tempButton.Left = tempTextbox.Left + tempTextbox.Width + 2;
            tempButton.Top = tempLabel.Top - ((tempButton.Height / 2) - (tempLabel.Height / 2));
            tempButton.Width = 30;
            tempButton.Name = "BeepButton";
            tempButton.Click += new EventHandler(this.FindSoundFile_Click);
            tempGroupbox.Controls.Add(tempButton);

            tempGroupbox.Height = tempButton.Top + tempButton.Height + 8;
            tempGroupbox.Width = tempButton.Left + tempButton.Width + 8;

            tempForm.Height = 60 + tempGroupbox.Top + tempGroupbox.Height;
            tempForm.Width = 12 + tempGroupbox.Left + tempGroupbox.Width;
            tempForm.Text = "GUI Sound Effects";
            tempForm.FormBorderStyle = FormBorderStyle.FixedDialog;
            tempForm.MinimizeBox = false;
            tempForm.MaximizeBox = false;
            tempForm.Deactivate += new EventHandler(this.soundsForm_Deactivate);
            tempForm.Name = "FreeToAutoClose";

            //add button for "OK"
            tempButton = new Button();
            tempButton.Text = "OK";
            tempButton.Left = tempLabel.Left + tempLabel.Width + 70;
            tempButton.Top = tempLabel.Top + tempLabel.Height + 17;
            //- ((tempButton.Height / 2) - (tempLabel.Height / 2));
            tempButton.Width = 60;
            tempButton.Name = "OKButton";
            tempButton.Click += new EventHandler(this.OK_Click);
            tempForm.Controls.Add(tempButton);

            //add button for "Cancel"
            tempButton = new Button();
            tempButton.Text = "Cancel";
            tempButton.Left = tempLabel.Left + tempLabel.Width + 136;
            tempButton.Top = tempLabel.Top + tempLabel.Height + 17;
            tempButton.Width = 60;
            tempButton.Name = "CancelButton";
            tempButton.Click += new EventHandler(this.Cancel_Click);
            tempForm.Controls.Add(tempButton);

            //tempForm.Show();
        }
        public cGUI_Sounds( string initialFile ) { selectedFile = initialFile; }
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            selectedFile = value.ToString();
            tempForm.Show();

            return selectedFile;
        }
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        private void FindSoundFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openSoundsDialog = new OpenFileDialog();
            //openMovieDialog.InitialDirectory = System.Environment.SpecialFolder.MyDocuments.;
            openSoundsDialog.InitialDirectory = @"Data\UI";
            //System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
            openSoundsDialog.Filter = "Sound Wav Files|*.wav|All Files (*.*)|*.*";
            openSoundsDialog.RestoreDirectory = true;

            ((Form) ((Control) sender).Parent.Parent).Name = "DoNotAutoClose";

            if (openSoundsDialog.ShowDialog() == DialogResult.OK)
            {
                if (((Control) sender).Name == "BeepButton")
                {
                    foreach (Control c in ((Control) sender).Parent.Controls)
                    {
                        if (c.Name == "BeepTextbox")
                        {
                            c.Text = openSoundsDialog.FileName;
                        }
                    }
                }
                //System.IO.File.Copy(openSoundsDialog.FileName, @"Data\UI\" + "Beep.wav");

                if (((Control) sender).Name == "ClickButton")
                {
                    foreach (Control c in ((Control) sender).Parent.Controls)
                    {
                        if (c.Name == "ClickTextbox")
                        {
                            c.Text = openSoundsDialog.FileName;
                        }
                    }
                }
                //System.IO.File.Copy(openSoundsDialog.FileName, @"Data\UI\" + "Beep.wav");
            }
            ((Form) ((Control) sender).Parent.Parent).Name = "FreeToAutoClose";
        }

        private void soundsForm_Deactivate(object sender, EventArgs e)
        {
            Form current = (Form) sender;
            if (current.Name != "DoNotAutoClose") current.Hide();
        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            Form current = (Form) ((Control) sender).Parent;
            current.Hide();
        }

        private void OK_Click(object sender, EventArgs e)
        {
            Form current = (Form) ((Control) sender).Parent;
            TextBox ClickTextbox, BeepTextbox;
            try
            {
                ClickTextbox = (TextBox) (((Control) sender).Parent.Controls.Find("ClickTextbox", true))[0];
                BeepTextbox = (TextBox) (((Control) sender).Parent.Controls.Find("BeepTextbox", true))[0];
                if (ClickTextbox.Text.Length > 0)
                {
                    File.Copy(ClickTextbox.Text, @"Data\UI\" + "Click.wav", true);
                }
                if (BeepTextbox.Text.Length > 0)
                {
                    File.Copy(BeepTextbox.Text, @"Data\UI\" + "Beep.wav", true);
                }
            }
            catch //(Exception ex)
            {
                current.Name = "DoNotAutoClose";
                //MessageBox.Show(ex.ToString());
                MessageBox.Show("Error on performing copy operations for specified files!", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            current.Hide();
        }
}
}