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
    public class cMainInterface
    {
        public class cBrowseProperty : System.Drawing.Design.UITypeEditor
        {
            public string selectedFile;

            public cBrowseProperty() { selectedFile = ""; }
            public cBrowseProperty( string initialFile ) { selectedFile = initialFile; }
            public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return System.Drawing.Design.UITypeEditorEditStyle.Modal;
            }
            public override bool GetPaintValueSupported(ITypeDescriptorContext context)
            {
                return true;
            }
        }
        
        public class cGameIntroMovie : cBrowseProperty
        {
            public cGameIntroMovie() { }
            public cGameIntroMovie(string initialFile) : base(initialFile) { }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                selectedFile = value.ToString();

                OpenFileDialog openMovieDialog = new OpenFileDialog();
                openMovieDialog.InitialDirectory = @"Data\Game Data";
                openMovieDialog.Filter =
                    "Windows Media Video Files (*.wmv)|*.wmv|Video Files (*.avi; *.mpg, *.wmv; *.mp2; *.mov)|*.avi; *.mov; *.mpg; *.mp2; *.wmv|All Files (*.*)|*.*";
                openMovieDialog.RestoreDirectory = true;
                if (openMovieDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileInfo movieFile = new FileInfo(openMovieDialog.FileName);
                        File.Copy(openMovieDialog.FileName, @"Data\Game Data\Menu.wmv");
                        return @"Data\Game Data\Menu.wmv";
                    }
                    catch
                    {
                    }
                }

                return selectedFile;
            }
        }

        public class cLoginMusic : cBrowseProperty
        {
            public cLoginMusic() {}
            public cLoginMusic(string initialFile) : base(initialFile) { }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                selectedFile = value.ToString();
                OpenFileDialog openLoginMusicDialog = new OpenFileDialog();
                openLoginMusicDialog.InitialDirectory = @"Data\Music";
                //System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments);
                openLoginMusicDialog.Filter = "Audio Files|*.au; *.mp3; *.mp2; *.wav|All Files (*.*)|*.*";
                openLoginMusicDialog.RestoreDirectory = true;
                if (openLoginMusicDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        FileInfo musicFile = new FileInfo(openLoginMusicDialog.FileName);
                        //string extension = musicFile.Extension;
                        File.Copy(openLoginMusicDialog.FileName, @"Data\Music\Menu.wav");
                        //+ "Login" + extension);
                        return openLoginMusicDialog.FileName;
                    }
                    catch
                    {
                    }
                }
                return selectedFile;
            }
        }

        public class cCustomFile : cBrowseProperty
        {
            string GetDefaultTextEditor()
            {
                RegistryKey r = Registry.ClassesRoot;
                r = r.OpenSubKey(@"SystemFileAssociations\text\shell\open\command");
                string[] registryNames = r.GetValueNames();
                string registryValue = r.GetValue(registryNames[0]).ToString();
                registryValue = registryValue.Replace(" %", "\0");
                return registryValue;
            }
            public cCustomFile() {}
            public cCustomFile(string initialFile) : base(initialFile) { }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                selectedFile = value.ToString();
                Process.Start(GetDefaultTextEditor(), selectedFile);
                return selectedFile;
            }
        }

        public class cClientFontEditor : cBrowseProperty
        {
            public cClientFontEditor() {}
            public cClientFontEditor(string initialFile) : base(initialFile) { }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                selectedFile = value.ToString();
                FontEditor Win = new FontEditor();
                    Win.ShowDialog();
                    Win.Dispose();

                return selectedFile;
            }
        }

        public class cGUI_Elements : cBrowseProperty
        {
            public cGUI_Elements() { }
            public cGUI_Elements(string initialFile) : base(initialFile) { }
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                selectedFile = value.ToString();
                InterfaceGuiElements window = new InterfaceGuiElements();
                    window.Show();

                return selectedFile;
            }
        }

        cGameIntroMovie gameIntroMovie;
        cLoginMusic loginMusic;
        cCustomFile userLicenseEula, gameHelp, language;
        cClientFontEditor fontEditor;
        cGUI_Elements gui_Elements;
        cGUI_Sounds gui_Sounds;

        public cMainInterface()
        {
            gameIntroMovie = new cGameIntroMovie( @"Data\Game Data\Menu.wmv" );
            loginMusic = new cLoginMusic( @"Data\Music\Menu.wav" );

            userLicenseEula = new cCustomFile( @"Data\Game Data\Eula.txt" );
            gameHelp = new cCustomFile( @"Data\Game Data\Help.txt" );
            language = new cCustomFile( @"Data\Game Data\Language.xml" );

            fontEditor = new cClientFontEditor(@"Data\UI\Fonts\Title.bmp");
            gui_Elements = new cGUI_Elements(@"GUI_Elements");
            gui_Sounds = new cGUI_Sounds(@"GUI_Sounds");
        }

        //[Editor(typeof(cGameIntroMovie), typeof(System.Drawing.Design.UITypeEditor))]
        //public string GameIntroMovie
        //{
        //    get { return gameIntroMovie.selectedFile; }
        //    set { gameIntroMovie.selectedFile = value; }
        //}

        //[Editor(typeof(cLoginMusic), typeof(System.Drawing.Design.UITypeEditor))]
        //public string LoginMusic
        //{
        //    get { return loginMusic.selectedFile; }
        //    set { loginMusic.selectedFile = value; }
        //}

        [Editor(typeof(cCustomFile), typeof(System.Drawing.Design.UITypeEditor))]
        public string UserLicenseEula
        {
            get { return userLicenseEula.selectedFile; }
            set { userLicenseEula.selectedFile = value; }
        }

        [Editor(typeof(cCustomFile), typeof(System.Drawing.Design.UITypeEditor))]
        public string GameHelp
        {
            get { return gameHelp.selectedFile; }
            set { gameHelp.selectedFile = value; }
        }

        [Editor(typeof(cCustomFile), typeof(System.Drawing.Design.UITypeEditor))]
        public string Language
        {
            get { return language.selectedFile; }
            set { language.selectedFile = value; }
        }

        //[Editor(typeof(cClientFontEditor), typeof(System.Drawing.Design.UITypeEditor))]
        //public string ClientFont
        //{
        //    get { return fontEditor.selectedFile; }
        //    set { fontEditor.selectedFile = value; }
        //}

        [Editor(typeof(cGUI_Elements), typeof(System.Drawing.Design.UITypeEditor))]
        public string GUI_Elements
        {
            get { return gui_Elements.selectedFile; }
            set { gui_Elements.selectedFile = value; }
        }

        [Editor(typeof(cGUI_Sounds), typeof(System.Drawing.Design.UITypeEditor))]
        public string GUI_Sounds
        {
            get { return gui_Sounds.selectedFile; }
            set { gui_Sounds.selectedFile = value; }
        }

    }
}