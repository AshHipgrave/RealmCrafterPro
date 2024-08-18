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
using System.IO;
using RenderingServices;
using RealmCrafter;
using IrrlichtSound;
using RottParticles;

namespace RealmCrafter_GE.Forms
{
    public partial class MediaManager : Form
    {
        Panel RenderingPanel;
        GE GEForm;
        Channel MediaAudioPreview;
        ConfigureParameter ParametersForm = null;
        int SelectedMediaID = 65535;
        string SelectedMediaType;
        bool MediaTreeSelectedItem = false;


        public MediaManager(GE ge, Panel panel)
        {
            InitializeComponent();

            RenderingPanel = panel;
            GEForm = ge;

            MediaDialogs.Init(MediaTree);
        }

        public void SetupRenderPanel()
        {           

            GEForm.UpdateRenderingPanel((int)RealmCrafter_GE.GE.GETab.MEDIA);

            RenderingPanel.Focus();

            UpdateShaderCombo();

        }

        public void UpdateMedia(object sender, EventArgs e)
        {
            while (Program.AppStillIdle)
            {
                MLib.Timer.Update();
                GE.DeltaTime = MLib.Timer.DeltaTime;
                GE.MilliSecs = MLib.Timer.Ticks;

                // Store delta time
                GE.DeltaBuffer[GE.DeltaBufferIndex] = GE.DeltaTime;
                GE.DeltaBufferIndex++;
                if (GE.DeltaBufferIndex == GE.DeltaFrames)
                {
                    GE.DeltaBufferIndex = 0;
                }

                Program.GE.SpeedFrames = 1;


                if (true)
                {
                    #region Mouse rotation
                    if (Program.GE.MediaMeshEN != null)
                    {
                        // Scale mouse position to back-buffer co-ordinates
                        Point MousePos = RenderingPanel.PointToClient(Cursor.Position);
                        int MouseX = MousePos.X;// (MousePos.X * 1024) / RenderingPanel.Size.Width;

                        // Mouse button down?
                        if (KeyState.Get(Keys.LButton))
                        {
                            if (!Program.GE.MouseDragging)
                            {
                                Program.GE.OldMouseX = Cursor.Position.X;
                                Program.GE.OldMouseY = Cursor.Position.Y;
                                Program.GE.MouseDragging = true;
                            }
                            else
                            {
                                // Rotate whit ... ( Marian Voicu )
                                int MouseMoveX = Cursor.Position.X - Program.GE.OldMouseX;
                                int MouseMoveY = Cursor.Position.Y - Program.GE.OldMouseY;
                                Program.GE.OldMouseX = Cursor.Position.X;
                                Program.GE.OldMouseY = Cursor.Position.Y;
                                Program.GE.MediaMeshEN.Turn(GE.Delta * MouseMoveY * 2f, GE.Delta * (float)MouseMoveX * 2f, 0f);
                                // end ( MV )
                            }
                        }
                        else
                        {
                            Program.GE.MouseDragging = false;
                        }
                    }
                    #endregion

                    //TabMedia.Focus();
                }


                General.Update(GE.Delta);
                Render.RenderWorld();
                Collision.UpdateWorld();

                System.Threading.Thread.Sleep(10);
            }
        }

   


        private void MediaTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            // Ignore collapse/expand events
            if (e.Action != TreeViewAction.Collapse && e.Action != TreeViewAction.Expand)
            {
                // Get type and media ID of selected node
                SelectedMediaType = e.Node.FullPath.Substring(0, 2);
                int NewMediaID = 65535;
                if (e.Node.Tag != null)
                {
                    NewMediaID = (ushort)e.Node.Tag;
                }

                // Update preview
                if (NewMediaID < 65535)
                {
                    MediaPreviewIDLabel.Text = "Media ID: " + NewMediaID.ToString();
                }
                else
                {
                    MediaPreviewIDLabel.Text = "Media ID: Nothing selected";
                }
                if (SelectedMediaID < 65535 && GEForm.MediaMeshEN != null)
                {
                    GEForm.MediaMeshEN.Free();
                    GEForm.MediaMeshEN = null;
                }
                GEForm.MediaTextureEN.Visible = false;
                MediaMeshScaleSpinner.Enabled = false;
                MediaMeshShaderCombo.Enabled = false;

                MeshPropertiesGroup.Visible = false;
                SoundPropertiesGroup.Visible = false;
                switch (SelectedMediaType)
                {
                    // Mesh
                    case "Me":
                        if (NewMediaID < 65535)
                        {
                            GEForm.MediaMeshEN = Media.GetMesh(NewMediaID);
                            if (GEForm.MediaMeshEN != null)
                            {
                                GEForm.MediaMeshEN.Position(0, -20f, 350f);
                                // Set scale ( Marian Voicu )
                                float meshScales = Media.LoadedMeshScales[NewMediaID];
                                if ((decimal)meshScales == 0)
                                {
                                    meshScales = 1.0F;
                                }
                                Media.SizeEntity(GEForm.MediaMeshEN, 150f * meshScales, 150f * meshScales, 150f * meshScales,
                                                 true);
                                // end ( MV )
                                GEForm.MediaMeshEN.Visible = true;
                                // Scale and shader
                                MediaMeshScaleSpinner.Enabled = true;
                                MediaMeshScaleSpinner.Value = (decimal)meshScales;
                                ListBoxItem LBI;
                                MediaMeshShaderCombo.Enabled = true;

                                // Flags
                                CheckMeshTangents.Checked = (GEForm.MediaMeshEN.Flags & 2) > 0;
                                CheckReflectEnvironment.Checked = (GEForm.MediaMeshEN.Flags & 4) > 0;
                                CheckRefractEnvironment.Checked = (GEForm.MediaMeshEN.Flags & 8) > 0;

                                // LOD
                                btnMeshHigh.Text = GE.NiceMeshName(GEForm.MediaMeshEN.ID);
                                btnMeshMedium.Text = GE.NiceMeshName(GEForm.MediaMeshEN.MeshLOD_Medium);
                                btnMeshLow.Text = GE.NiceMeshName(GEForm.MediaMeshEN.MeshLOD_Low);
                                nDistHigh.Value = (decimal)GEForm.MediaMeshEN.distLOD_High;
                                nDistMedium.Value = (decimal)GEForm.MediaMeshEN.distLOD_Medium;
                                nDistLow.Value = (decimal)GEForm.MediaMeshEN.distLOD_Low;

                                MeshPropertiesGroup.Visible = true;

                                //if (Media.LoadedMeshShaders[NewMediaID] == 65535)
                                //{
                                //    MediaMeshShaderCombo.SelectedIndex = 0;
                                //}
                                //else
                                //{
                                for (int i = 0; i < MediaMeshShaderCombo.Items.Count; ++i)
                                {
                                    LBI = (ListBoxItem)MediaMeshShaderCombo.Items[i];
                                    if (LBI.Value == Media.LoadedMeshShaders[NewMediaID])
                                    {
                                        MediaMeshShaderCombo.SelectedIndex = i;
                                    }
                                }
                                //}
                                /*
                                                                MediaMeshShaderCombo.SelectedIndex = 0;
                                                                for (int i = 0; i < MediaMeshShaderCombo.Items.Count; ++i)
                                                                {
                                                                    LBI = (ListBoxItem)MediaMeshShaderCombo.Items[i];
                                                                    if (LBI.Value == Media.LoadedMeshShaders[NewMediaID])
                                                                        MediaMeshShaderCombo.SelectedIndex = i;
                                                                }
                                 */
                            }
                            else
                            {
                                SelectedMediaType = "";
                            }
                        }
                        MediaPreviewPlaySound.Enabled = false;
                        MediaPreviewStopSound.Enabled = false;
                        break;
                    // Texture
                    case "Te":
                        if (NewMediaID < 65535)
                        {
                            GEForm.MediaTextureEN.Visible = true;
                            uint Tex = Media.GetTexture(NewMediaID, false);
                            if (Tex != 0)
                            {
                                GEForm.MediaTextureEN.Texture(Tex);
                                //Media.UnloadTexture(NewMediaID);
                            }
                            else
                            {
                                SelectedMediaType = "";
                            }
                        }
                        MediaPreviewPlaySound.Enabled = false;
                        MediaPreviewStopSound.Enabled = false;
                        break;
                    // Sound
                    case "So":
                        MediaPreviewPlaySound.Enabled = true;
                        MediaPreviewStopSound.Enabled = true;
                        SoundPropertiesGroup.Visible = true;
                        break;
                    // Music
                    case "Mu":
                        MediaPreviewPlaySound.Enabled = true;
                        MediaPreviewStopSound.Enabled = true;
                        SoundPropertiesGroup.Visible = true;
                        break;
                }

                SelectedMediaID = NewMediaID;
            }
        }



        private void ImportFolder_Click(object sender, EventArgs e)
        {
            List<string> meshFormats = new List<string>()
            {
                ".b3d",
                ".eb3d", 
                ".3ds", 
                ".x"
            };


            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = System.Environment.CurrentDirectory + @"\Data\Meshes\";
            DialogResult result = dialog.ShowDialog();

            bool useSubFolders = MessageBox.Show("Import sub directories as well?", "Sub Directories", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Queue<string> folderQueue = new Queue<string>();
                folderQueue.Enqueue(dialog.SelectedPath);

                while (folderQueue.Count > 0)
                {
                    string folder = folderQueue.Dequeue();

                    List<string> meshFiles = new List<string>();
                    string[] files = Directory.GetFiles(folder);

                    // Get all mesh files in folder
                    foreach (string f in files)
                        if (meshFormats.Contains(Path.GetExtension(f)))
                        {
                            meshFiles.Add(f);
                        }

                    // Import them
                    if (meshFiles.Count > 0)
                    {
                        ImportMesh(meshFiles.ToArray());
                    }

                    if (useSubFolders)
                    {
                        // Recurse through sub folders 
                        string[] folders = Directory.GetDirectories(folder);
                        foreach (string f in folders)
                            folderQueue.Enqueue(f);
                    }
                }
            }
        }

        private void ImportTextureFolder_Click(object sender, EventArgs e)
        {
            List<string> textureFormats = new List<string>()
            {
                ".bmp",
                ".jpg",
                ".png",
                ".dds",
                ".tga",
                ".jpeg",
                ".dds",
            };

            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = System.Environment.CurrentDirectory + @"\Data\Textures\";
            DialogResult result = dialog.ShowDialog();
            bool useSubFolders = MessageBox.Show("Import sub directories as well?", "Sub Directories", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Queue<string> folderQueue = new Queue<string>();
                folderQueue.Enqueue(dialog.SelectedPath);

                while (folderQueue.Count > 0)
                {
                    string folder = folderQueue.Dequeue();

                    List<string> textureFiles = new List<string>();
                    string[] files = Directory.GetFiles(folder);

                    // Get all mesh files in folder
                    foreach (string f in files)
                        if (textureFormats.Contains(Path.GetExtension(f)))
                        {
                            textureFiles.Add(f);
                        }

                    // Impoty tjhem
                    if (textureFiles.Count > 0)
                    {
                        ImportTextures(textureFiles.ToArray());
                    }
                    if (useSubFolders)
                    {
                        // Recurse through sub folders 
                        string[] folders = Directory.GetDirectories(folder);
                        foreach (string f in folders)
                            folderQueue.Enqueue(f);
                    }
                }
            }
        }

        private void ImportSoundFolder_Click(object sender, EventArgs e)
        {
            List<string> soundFormats = new List<string>()
            {
                ".wav",
                ".ogg"
            };


            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = System.Environment.CurrentDirectory + @"\Data\Sounds\";
            DialogResult result = dialog.ShowDialog();
            bool useSubFolders = MessageBox.Show("Import sub directories as well?", "Sub Directories", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Queue<string> folderQueue = new Queue<string>();
                folderQueue.Enqueue(dialog.SelectedPath);

                while (folderQueue.Count > 0)
                {
                    string folder = folderQueue.Dequeue();

                    List<string> soundFiles = new List<string>();
                    string[] files = Directory.GetFiles(folder);

                    // Get all mesh files in folder
                    foreach (string f in files)
                        if (soundFormats.Contains(Path.GetExtension(f)))
                        {
                            soundFiles.Add(f);
                        }

                    // Import tjhem
                    if (soundFiles.Count > 0)
                    {
                        ImportSounds(soundFiles.ToArray());
                    }
                    if (useSubFolders)
                    {
                        // Recurse through sub folders 
                        string[] folders = Directory.GetDirectories(folder);
                        foreach (string f in folders)
                            folderQueue.Enqueue(f);
                    }
                }
            }

        }

        private void ImportMusicFolder_Click(object sender, EventArgs e)
        {

            List<string> soundFormats = new List<string>()
            {
                ".ogg",
                ".mid",
                ".wav",
                ".mod",
                ".s3m",
                ".xm",
                ".it"
            };


            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.SelectedPath = System.Environment.CurrentDirectory + @"\Data\Music\";
            DialogResult result = dialog.ShowDialog();
            bool useSubFolders = MessageBox.Show("Import sub directories as well?", "Sub Directories", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes;

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Queue<string> folderQueue = new Queue<string>();
                folderQueue.Enqueue(dialog.SelectedPath);

                while (folderQueue.Count > 0)
                {
                    string folder = folderQueue.Dequeue();

                    List<string> soundFiles = new List<string>();
                    string[] files = Directory.GetFiles(folder);

                    // Get all mesh files in folder
                    foreach (string f in files)
                        if (soundFormats.Contains(Path.GetExtension(f)))
                        {
                            soundFiles.Add(f);
                        }

                    // Impoty tjhem
                    if (soundFiles.Count > 0)
                    {
                        ImportMusic(soundFiles.ToArray());
                    }
                    if (useSubFolders)
                    {
                        // Recurse through sub folders 
                        string[] folders = Directory.GetDirectories(folder);
                        foreach (string f in folders)
                            folderQueue.Enqueue(f);
                    }
                }
            }
        }



        private void BMediaAddMesh_Click(object sender, EventArgs e)
        {
            // Open file dialog
            MediaAddDialog.InitialDirectory = @"Data\Meshes";
            MediaAddDialog.Filter = @"Supported Formats(*.b3d, *.eb3d, *.3ds, *.x)|*.b3d;*.eb3d;*.3ds;*.x";
            MediaAddDialog.FilterIndex = 1;

            DialogResult Result = MediaAddDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                ImportMesh(MediaAddDialog.FileNames);
            }
        }

        private void ImportMesh(string[] files)
        {
            DialogResult Result;

            // Add settings
            string BaseFolder = System.Environment.CurrentDirectory + @"\Data\Meshes\";


            // For each file selected
            string Extension;
            foreach (string File in files)
            {
                Result = MessageBox.Show("Import " + Path.GetFileName(File) + " as animated?", "Add Meshes", MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question);
                bool IsAnim = (Result == DialogResult.Yes);
                Result = MessageBox.Show("Do you wish to encrypt " + Path.GetFileName(File) + " where possible?", "Add Meshes", MessageBoxButtons.YesNo,
                                         MessageBoxIcon.Question);
                bool Encrypt = (Result == DialogResult.Yes);

                // Copy file if required
                string Name;
                if (!File.Contains(BaseFolder))
                {
                    Name = Path.GetFileName(File);
                    if (!System.IO.File.Exists(@"Data\Meshes\" + Name))
                    {
                        System.IO.File.Copy(File, @"Data\Meshes\" + Name);
                    }
                }
                else
                {
                    Name = File.Remove(0, BaseFolder.Length);
                }

                // Get file extension
                Extension = Path.GetExtension(Name).ToUpper();

                // Convert file to B3D format if it's .X or .3DS
                if (Extension == ".X")
                {
                    string NewName = Render.ConvertXToB3D(@"Data\Meshes\" + Name);
                    if (!string.IsNullOrEmpty(NewName))
                    {
                        System.IO.File.Delete(@"Data\Meshes\" + Name);
                        Name = NewName.Remove(0, 12);
                        Extension = ".B3D";
                    }
                }
                else if (Extension == ".3DS")
                {
                    string NewName = Render.Convert3DSToB3D(@"Data\Meshes\" + Name);
                    if (!string.IsNullOrEmpty(NewName))
                    {
                        System.IO.File.Delete(@"Data\Meshes\" + Name);
                        Name = NewName.Remove(0, 12);
                        Extension = ".B3D";
                    }
                }

                // Encrypt file if required
                if (Encrypt && Extension == ".B3D")
                {
                    Encryption.EncryptFile(@"Data\Meshes\" + Name,
                                           @"Data\Meshes\" + Name.Insert((Name.Length - Extension.Length) + 1, "e"));
                    Name = Name.Insert((Name.Length - Extension.Length) + 1, "e");
                }

                // Add file to database
                int ID = Media.AddMeshToDatabase(Name, IsAnim);
                // Add to media manager display
                if (ID == -1)
                {
                    MessageBox.Show("No free mesh ID found or mesh already added!", "Error");
                }
                else
                {
                    if (IsAnim)
                    {
                        MediaDialogs.AddMesh(Name + "1", (ushort)ID, MediaTree.Nodes[0]);
                    }
                    else
                    {
                        MediaDialogs.AddMesh(Name + "0", (ushort)ID, MediaTree.Nodes[0]);
                        Program.MeshList.Insert(ID, Media.GetMeshName(ID) + " ID: " + ID);
                    }
                }
            }
        }

        private void BMediaAddTexture_Click(object sender, EventArgs e)
        {
            MediaAddDialog.InitialDirectory = @"Data\Textures";
            //MediaAddDialog.Filter = @"Bitmap (*.bmp)|*.bmp|JPEG (*.jpg)|*.jpg;*.jpeg|PNG (*.png)|*.png|DirectX (*.dds)|*.dds|Targa (*.tga)|*.tga";
            MediaAddDialog.Filter =
                @"Supported Formats(*.bmp, *.jpg, *.png, *.dds, *.tga)|*.bmp*;*.jpg;*.jpeg;*.png;*.dds;*.tga";
            MediaAddDialog.FilterIndex = 1;
            DialogResult Result = MediaAddDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                ImportTextures(MediaAddDialog.FileNames);
            }
        }

        private void ImportTextures(string[] fileNames)
        {
            // Add settings
            string BaseFolder = System.Environment.CurrentDirectory + @"\Data\Textures\";

            // For each file selected
            foreach (string File in fileNames)
            {
                TextureFlags T = new TextureFlags();
                T.Text = Path.GetFileName(File);
                T.ShowDialog();
                ushort Flags = 0;
                if (T.TextureSettingsColour.Checked)
                {
                    Flags += 1;
                }
                if (T.TextureSettingsAlpha.Checked)
                {
                    Flags += 2;
                }
                if (T.TextureSettingsMasked.Checked)
                {
                    Flags += 4;
                }
                if (T.TextureSettingsMipmapped.Checked)
                {
                    Flags += 8;
                }
                if (T.TextureSettingsClampU.Checked)
                {
                    Flags += 16;
                }
                if (T.TextureSettingsClampV.Checked)
                {
                    Flags += 32;
                }
                if (T.TextureSettingsSphere.Checked)
                {
                    Flags += 64;
                }
                if (T.TextureSettingsCube.Checked)
                {
                    Flags += 128;
                }
                T.Dispose();

                // Copy file if required
                string Name;
                if (!File.Contains(BaseFolder))
                {
                    Name = Path.GetFileName(File);
                    if (!System.IO.File.Exists(@"Data\Textures\" + Name))
                    {
                        System.IO.File.Copy(File, @"Data\Textures\" + Name);
                    }
                }
                else
                {
                    Name = File.Remove(0, BaseFolder.Length);
                }
                // Add file to database
                int ID = Media.AddTextureToDatabase(Name, Flags);
                // Add to media manager display
                if (ID == -1)
                {
                    MessageBox.Show("No free texture ID found!", "Error");
                }
                else if (ID == -2)
                {
                    MessageBox.Show("Texture already exists in database!", "Error");
                }
                else if (ID < 0) //Shouldn't be seen
                {
                    MessageBox.Show("Undefined Error adding texture!", "Error");
                }
                else
                {
                    MediaDialogs.AddTexture(Name + Blitz.StrFromInt(Flags, 1), (ushort)ID, MediaTree.Nodes[1]);

                    Program.TextureList.Add(Media.GetTextureName(ID) + " ID: " + ID);
                }
            }

        }

        private void BMediaAddSound_Click(object sender, EventArgs e)
        {
            MediaAddDialog.InitialDirectory = @"Data\Sounds";
            // MediaAddDialog.Filter = @"Wave (*.wav)|*.wav|Raw (*.raw)|*.raw|MP3 (*.mp3)|*.mp3|OGG (*.ogg)|*.ogg";
            MediaAddDialog.Filter = @"Supported formats (*.wav,*.ogg)|*.wav;*.ogg";
            MediaAddDialog.FilterIndex = 1;
            DialogResult Result = MediaAddDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                ImportSounds(MediaAddDialog.FileNames);
            }

        }

        private void ImportSounds(string[] fileNames)
        {
            // Add settings
            DialogResult result;
            string BaseFolder = System.Environment.CurrentDirectory + @"\Data\Sounds\";


            // For each file selected
            foreach (string File in fileNames)
            {
                result = MessageBox.Show("Import " + Path.GetFileName(File) + " sound as 3D?", "Add Sound", MessageBoxButtons.YesNo,
                         MessageBoxIcon.Question);
                bool Is3D = (result == DialogResult.Yes);


                // Copy file if required
                string Name;
                if (!File.Contains(BaseFolder))
                {
                    Name = Path.GetFileName(File);
                    if (!System.IO.File.Exists(@"Data\Sounds\" + Name))
                    {
                        System.IO.File.Copy(File, @"Data\Sounds\" + Name);
                    }
                }
                else
                {
                    Name = File.Remove(0, BaseFolder.Length);
                }
                // Add file to database
                int ID = Media.AddSoundToDatabase(Name, Is3D);
                // Add to media manager display
                if (ID < 0)
                {
                    MessageBox.Show("No free sound ID found!", "Error");
                }
                else
                {
                    if (Is3D)
                    {
                        MediaDialogs.AddSound(Name + "1", (ushort)ID, MediaTree.Nodes[2]);
                        Program.SoundsList.Add(Media.GetSoundName(ID) + " ID: " + ID);
                    }
                    else
                    {
                        MediaDialogs.AddSound(Name + "0", (ushort)ID, MediaTree.Nodes[2]);
                        Program.SoundsList.Add(Media.GetSoundName(ID) + " ID: " + ID);
                    }
                }
            }
        }

        private void BMediaAddMusic_Click(object sender, EventArgs e)
        {
            MediaAddDialog.InitialDirectory = @"Data\Music";
            //MediaAddDialog.Filter = @"MP3 (*.mp3)|*.mp3|OGG (*.ogg)|*.ogg|MIDI (*.mid)|*.mid|Wave (*.wav)|*.wav|Mod (*.mod)|*.mod|" +
            //                        @"S3M (*.s3m)|*.s3m|XM (*.xm)|*.xm|IT (*.it)|*.it";
            MediaAddDialog.Filter = @"All formats (*.ogg,*.mid,*.wav,*.mod,*.s3m,*.xm,*.it)|" +
                                    @"*.ogg;*.mid;*.wav;*.mod;*.s3m;*.xm;*.it";
            MediaAddDialog.FilterIndex = 1;
            DialogResult Result = MediaAddDialog.ShowDialog();
            if (Result == DialogResult.OK)
            {
                ImportMusic(MediaAddDialog.FileNames);
            }
        }

        private void ImportMusic(string[] fileNames)
        {
            // Add settings
            string BaseFolder = System.Environment.CurrentDirectory + @"\Data\Music\";

            // For each file selected
            foreach (string File in fileNames)
            {
                // Copy file if required
                string Name;
                if (!File.Contains(BaseFolder))
                {
                    Name = Path.GetFileName(File);
                    if (!System.IO.File.Exists(@"Data\Music\" + Name))
                    {
                        System.IO.File.Copy(File, @"Data\Music\" + Name);
                    }
                }
                else
                {
                    Name = File.Remove(0, BaseFolder.Length);
                }

                // Add file to database
                int ID = Media.AddMusicToDatabase(Name);
                // Add to media manager display
                if (ID < 0)
                {
                    MessageBox.Show("No free music ID found!", "Error");
                }
                else
                {
                    MediaDialogs.AddMusic(Name, (ushort)ID, MediaTree.Nodes[3]);
                    Program.MusicList.Add(Media.GetMusicName(ID) + " ID: " + ID);
                }
            }
        }

        // added by them
        private void BMediaShadersEdit_Click(object sender, EventArgs e)
        {
            //ShaderDatabase.RunShaderDatabaseEditor(this);
            //int CurrentID = 65535;
            //if (MediaMeshShaderCombo.SelectedItem != null)
            //    CurrentID = ((ListBoxItem)MediaMeshShaderCombo.SelectedItem).Value;
            //MediaMeshShaderCombo.Items.Clear();
            //MediaMeshShaderCombo.Items.Add(new ListBoxItem("(Default lit object)", 65535));

            //int Idx;
            //for (int ID = 0; ID < 65535; ++ID)
            //    if (!string.IsNullOrEmpty(Media.Shaders[ID]))
            //    {
            //        Idx = MediaMeshShaderCombo.Items.Add(new ListBoxItem(Media.Shaders[ID], ID));
            //        if (ID == CurrentID)
            //            MediaMeshShaderCombo.SelectedIndex = Idx;
            //    }

            //if (CurrentID == 65535)
            //    MediaMeshShaderCombo.SelectedIndex = 0;
            Program.ShaderManager.ShowDialog();
            UpdateShaderCombo();
        }

        public void UpdateShaderCombo()
        {
            int CurrentID = 65535;
            if (MediaMeshShaderCombo.SelectedItem != null)
            {
                CurrentID = (int)((ListBoxItem)MediaMeshShaderCombo.SelectedItem).Value;
            }

            MediaMeshShaderCombo.Items.Clear();
            MediaMeshShaderCombo.Items.Add(new ListBoxItem("Default (None)", 65535));

            foreach (ShaderProfile Pr in ShaderManager.LoadedProfiles)
            {
                if (Pr != null)
                {
                    int Index = MediaMeshShaderCombo.Items.Add(new ListBoxItem(Pr.Name, (uint)Pr.ID));
                    if (Pr.ID == CurrentID)
                    {
                        MediaMeshShaderCombo.SelectedIndex = Index;
                    }
                }
            }


        }

        private void MediaMeshScaleSpinner_ValueChanged(object sender, EventArgs e)
        {
            // Get type and media ID of selected node
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null)
            {
                if (MediaTree.SelectedNode.Tag != null)
                {
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;
                }
                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);

                if (MediaID < 65535 && MediaType == "Me")
                {
                    Media.SetMeshScale(MediaID, (float)MediaMeshScaleSpinner.Value);
                    Media.SizeEntity(GEForm.MediaMeshEN, 150f * (float)MediaMeshScaleSpinner.Value,
                                     150f * (float)MediaMeshScaleSpinner.Value,
                                     150f * (float)MediaMeshScaleSpinner.Value, true);
                }
            }
        }

        public void EntityProfileAll(ushort MediaID, ShaderProfile Pr)
        {
            if (MediaID == 65535)
            {
                return;
            }

            foreach (Entity E in Entity.EntitiesList)
            {
                if (E.ID == MediaID)
                {
                    RenderWrapper.EntityProfile(E.Handle, Pr.Handle);
                }
            }
        }

        public void EntityShaderAll(ushort MediaID, uint Pr)
        {
            if (MediaID == 65535)
            {
                return;
            }

            foreach (Entity E in Entity.EntitiesList)
            {
                if (E.ID == MediaID)
                {
                    E.Shader = Pr;
                }
            }
        }

        private void MediaMeshShaderCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Get type and media ID of selected node
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null && MediaMeshShaderCombo.SelectedIndex >= 0)
            {
                if (MediaTree.SelectedNode.Tag != null)
                {
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;
                }
                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);

                if (MediaID < 65535 && MediaType == "Me")
                {
                    //backup
                    /*
                                        ListBoxItem LBI = (ListBoxItem)MediaMeshShaderCombo.SelectedItem;
                                        Media.SetMeshShader(MediaID, (ushort)LBI.Value);
                     */
                    //added by them
                    ListBoxItem LBI = (ListBoxItem)MediaMeshShaderCombo.SelectedItem;

                    if (LBI != null && LBI.Value != 65535)
                    {
                        Media.SetMeshShader(MediaID, (ushort)LBI.Value);
                        if (GEForm.MediaMeshEN != null)
                        {
                            ShaderProfile Pr = ShaderManager.GetProfile((int)LBI.Value);

                            if (Pr != null)
                            {
                                EntityProfileAll(MediaID, Pr);
                            }
                            else
                            {
                                MessageBox.Show("Error: Could not reselect ShaderProfile!");
                            }
                        }
                    }
                    else
                    {
                        Media.SetMeshShader(MediaID, 65535);
                        if (GEForm.MediaMeshEN != null)
                        {
                            EntityShaderAll(MediaID, Shaders.LitObject1);

                            if (GEForm.MediaMeshEN.IsAnim == 0 && ShaderManager.DefaultStatic != null)
                            {
                                EntityProfileAll(MediaID, ShaderManager.DefaultStatic);
                            }

                            if (GEForm.MediaMeshEN.IsAnim == 1 && ShaderManager.DefaultAnimated != null)
                            {
                                EntityProfileAll(MediaID, ShaderManager.DefaultAnimated);
                            }
                        }
                    }
                    // end by them
                }
            }
        }

        private void MediaPreviewPlaySound_Click(object sender, EventArgs e)
        {
            // Get type and media ID of selected node
            if (MediaTree.SelectedNode != null)
            {
                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);

                if (SelectedMediaID < 65535 && (MediaType == "So" || MediaType == "Mu"))
                {
                    // Stop old sound
                    if (MediaAudioPreview != null)
                    {
                        MediaAudioPreview.Stop();
                        MediaAudioPreview = null;
                        Media.UnloadSound(SelectedMediaID);
                    }

                    // Play new
                    if (MediaType == "So")
                    {
                        try
                        {
                            Sound S = Media.GetSound(SelectedMediaID);
                            if (S != null)
                            {
                                MediaAudioPreview = S.Play();
                            }
                        }
                        catch (Exception)
                        {
                            MediaAudioPreview = null;
                            MessageBox.Show("Error playing sound", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        try
                        {
                            MediaAudioPreview = Sound.PlayMusic(@"Data\Music\" + Media.GetMusicName(SelectedMediaID));
                        }
                        catch (Exception)
                        {
                            MediaAudioPreview = null;
                            MessageBox.Show("Error playing music", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                        }
                    }
                    if (MediaAudioPreview != null)
                    {
                        MediaAudioPreview.Volume = 1; // = MediaPreviewSoundVolume.Value / 100f;
                    }
                }
            }
        }

        private void MediaTree_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteToolStripMenuItem_Click(null, null);

            }
        }

        private void MediaPreviewStopSound_Click(object sender, EventArgs e)
        {
            if (MediaAudioPreview != null)
            {
                MediaAudioPreview.Stop();
                MediaAudioPreview = null;
                Media.UnloadSound(SelectedMediaID);
            }
        }

        private void MediaPreviewSoundVolume_Scroll(object sender, EventArgs e)
        {
            if (MediaAudioPreview != null)
            {
                MediaAudioPreview.Volume = MediaPreviewSoundVolume.Value / 100f;
            }
        }

        #region Media Post Processing controls events
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //delete selected item from media tree
            // Get type and media ID of selected node
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null)
            {
                if (MediaTree.SelectedNode.Tag != null)
                {
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;
                }
                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);

                // Valid media
                if (MediaID < 65535)
                {
                    // Ask user whether to also delete the file from disk
                    DialogResult Result = MessageBox.Show("Also delete file from disk?", "Remove Media",
                                                          MessageBoxButtons.YesNoCancel);
                    if (Result != DialogResult.Cancel)
                    {
                        // Remove from database and delete file if necessary
                        switch (MediaType)
                        {
                            case "Me":
                                // Delete file
                                if (Result == DialogResult.Yes)
                                {
                                    if (GEForm.PlaceSceneryID == MediaID)
                                    {
                                        GEForm.PlaceSceneryID = 65535;
                                        GEForm.m_CreateWindow.WorldPlaceSceneryButton.Text = "";
                                    }
                                    string Name = Media.GetMeshName(MediaID);
                                    if (Name.Length > 1)
                                    {
                                        if (File.Exists(@"Data\Meshes\" + Name.Substring(0, Name.Length - 1)))
                                        //Ensure file exists before deleting
                                        {
                                            try
                                            {
                                                File.Delete(@"Data\Meshes\" +
                                                            Name.Substring(0, Name.Length - 1));
                                            }
                                            catch (IOException exception)
                                            {
                                                MessageBox.Show(@"Unable to remove file from disc: \Data\Meshes\" +
                                                                Name.Substring(0, Name.Length - 1) +
                                                                " file is locked or in use, continuing to remove from database");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show(
                                                @"Unable to remove file from disc, does not exist at path \Data\Meshes\" +
                                                Name.Substring(0, Name.Length - 1) +
                                                " continuing to remove from database");
                                        }
                                    }
                                }
                                // Remove
                                Media.RemoveMeshFromDatabase(MediaID);
                                MediaDialogs.TotalMeshes--;
                                //GEForm.ProjectMeshes.Text = "Meshes: " + MediaDialogs.TotalMeshes.ToString();
                                RemoveTreeViewNode(MediaTree.SelectedNode);
                                break;
                            case "Te":
                                // Delete file
                                if (Result == DialogResult.Yes)
                                {
                                    string Name = Media.GetTextureName(MediaID);
                                    if (Name.Length > 1)
                                    {
                                        if (File.Exists(@"Data\Textures\" + Name.Substring(0, Name.Length - 1)))
                                        //Ensure file exists before deleting
                                        {
                                            try
                                            {
                                                File.Delete(@"Data\Textures\" +
                                                            Name.Substring(0, Name.Length - 1));
                                            }
                                            catch (IOException exception)
                                            {
                                                MessageBox.Show(@"Unable to remove file from disc: \Data\Textures\" +
                                                                Name.Substring(0, Name.Length - 1) +
                                                                " file is locked or in use, continuing to remove from database");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show(
                                                @"Unable to remove file from disc, does not exist at path \Data\Textures\" +
                                                Name.Substring(0, Name.Length - 1) +
                                                " continuing to remove from database");
                                        }
                                    }
                                }
                                // Remove
                                Media.RemoveTextureFromDatabase(MediaID);
                                MediaDialogs.TotalTextures--;
                                //GEForm.ProjectTextures.Text = "Textures: " + MediaDialogs.TotalTextures.ToString();
                                RemoveTreeViewNode(MediaTree.SelectedNode);
                                break;
                            case "So":
                                // Delete file
                                if (Result == DialogResult.Yes)
                                {
                                    string Name = Media.GetSoundName(MediaID);
                                    if (Name.Length > 1)
                                    {
                                        if (File.Exists(@"Data\Sounds\" + Name.Substring(0, Name.Length - 1)))
                                        //Ensure file exists before deleting
                                        {
                                            try
                                            {
                                                File.Delete(@"Data\Sounds\" +
                                                            Name.Substring(0, Name.Length - 1));
                                            }
                                            catch (IOException exception)
                                            {
                                                MessageBox.Show(@"Unable to remove file from disc: \Data\Sounds\" +
                                                                Name.Substring(0, Name.Length - 1) +
                                                                " file is locked or in use, continuing to remove from database");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show(
                                                @"Unable to remove file from disc, does not exist at path \Data\Sounds\" +
                                                Name.Substring(0, Name.Length - 1) +
                                                " continuing to remove from database");
                                        }
                                    }
                                }
                                // Remove
                                Media.RemoveSoundFromDatabase(MediaID);
                                MediaDialogs.TotalSounds--;
                                //GEForm.ProjectSounds.Text = "Sounds: " + MediaDialogs.TotalSounds.ToString();
                                RemoveTreeViewNode(MediaTree.SelectedNode);
                                break;
                            case "Mu":
                                // Delete file
                                if (Result == DialogResult.Yes)
                                {
                                    string Name = Media.GetMusicName(MediaID);
                                    if (Name.Length > 0)
                                    {
                                        if (File.Exists(@"Data\Music\" + Name.Substring(0, Name.Length - 1)))
                                        //Ensure file exists before deleting
                                        {
                                            try
                                            {
                                                File.Delete(@"Data\Music\" + Name);
                                            }
                                            catch (IOException exception)
                                            {
                                                MessageBox.Show(@"Unable to remove file from disc: \Data\Sounds\" +
                                                                Name.Substring(0, Name.Length - 1) +
                                                                " file is locked or in use, continuing to remove from database");
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show(
                                                @"Unable to remove file from disc, does not exist at path \Data\Music\" +
                                                Name.Substring(0, Name.Length - 1) +
                                                " continuing to remove from database");
                                        }
                                    }
                                    // Remove
                                    Media.RemoveMusicFromDatabase(MediaID);
                                    MediaDialogs.TotalMusic--;
                                    //GEForm.ProjectMusic.Text = "Music: " + MediaDialogs.TotalMusic.ToString();
                                }
                                // Remove the item from the main tree view
                                RemoveTreeViewNode(MediaTree.SelectedNode);
                                break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Removing a folder is not currently supported", "Remove Media");
                }
            }
        }

        private void MediaTree_MouseDown(object sender, MouseEventArgs e)
        {
            return;
            ListView lv = (ListView)sender;
            TreeView tv = (TreeView)sender;
            if (tv.HitTest(e.X, e.Y).Node != null)
            {
                TreeNode temp = tv.HitTest(e.X, e.Y).Node;
                tv.SelectedNode = temp;
                if (temp.GetNodeCount(false) == 0)
                {
                    //if (e.Action != TreeViewAction.Collapse && e.Action != TreeViewAction.Expand)
                    {
                        // Get type and media ID of selected node
                        SelectedMediaType = temp.FullPath.Substring(0, 2);
                        int NewMediaID = 65535;
                        if (temp.Tag != null)
                        {
                            NewMediaID = (ushort)temp.Tag;
                        }

                        // Update preview
                        if (NewMediaID < 65535)
                        {
                            MediaPreviewIDLabel.Text = "Media ID: " + NewMediaID.ToString();
                            MediaTreeSelectedItem = true;
                        }
                        else
                        {
                            MediaPreviewIDLabel.Text = "Media ID: Nothing selected";
                            MediaTreeSelectedItem = false;
                        }
                        if (SelectedMediaID < 65535 && GEForm.MediaMeshEN != null)
                        {
                            GEForm.MediaMeshEN.Free();
                            GEForm.MediaMeshEN = null;
                        }
                        GEForm.MediaTextureEN.Visible = false;
                        MediaMeshScaleSpinner.Enabled = false;
                        MediaMeshShaderCombo.Enabled = false;

                        MeshPropertiesGroup.Visible = false;
                        SoundPropertiesGroup.Visible = false;

                        switch (SelectedMediaType)
                        {
                            // Mesh
                            case "Me":
                                if (NewMediaID < 65535)
                                {
                                    GEForm.MediaMeshEN = Media.GetMesh(NewMediaID);
                                    if (GEForm.MediaMeshEN != null)
                                    {
                                        GEForm.MediaMeshEN.Position(0, -20f, 350f);
                                        // Set scale ( Marian Voicu )
                                        float meshScales = Media.LoadedMeshScales[NewMediaID];
                                        if ((decimal)meshScales == 0)
                                        {
                                            meshScales = 1.0F;
                                        }
                                        Media.SizeEntity(GEForm.MediaMeshEN, 150f * meshScales, 150f * meshScales,
                                                         150f * meshScales, true);
                                        // end ( MV )
                                        GEForm.MediaMeshEN.Visible = true;
                                        // Scale and shader
                                        MediaMeshScaleSpinner.Enabled = true;
                                        MediaMeshScaleSpinner.Value = (decimal)meshScales;
                                        ListBoxItem LBI;
                                        MediaMeshShaderCombo.Enabled = true;

                                        // Flags
                                        CheckMeshTangents.Checked = (GEForm.MediaMeshEN.Flags & 2) > 0;
                                        CheckReflectEnvironment.Checked = (GEForm.MediaMeshEN.Flags & 4) > 0;
                                        CheckRefractEnvironment.Checked = (GEForm.MediaMeshEN.Flags & 8) > 0;

                                        MeshPropertiesGroup.Visible = true;
                                        //if (Media.LoadedMeshShaders[NewMediaID] == 65535)
                                        //{
                                        //    MediaMeshShaderCombo.SelectedIndex = 0;
                                        //}
                                        //else
                                        //{
                                        for (int i = 0; i < MediaMeshShaderCombo.Items.Count; ++i)
                                        {
                                            LBI = (ListBoxItem)MediaMeshShaderCombo.Items[i];
                                            if (LBI.Value == Media.LoadedMeshShaders[NewMediaID])
                                            {
                                                MediaMeshShaderCombo.SelectedIndex = i;
                                            }
                                        }
                                        //}
                                    }
                                    else
                                    {
                                        SelectedMediaType = "";
                                    }
                                }
                                MediaPreviewPlaySound.Enabled = false;
                                MediaPreviewStopSound.Enabled = false;
                                break;
                            // Texture
                            case "Te":
                                if (NewMediaID < 65535)
                                {
                                    GEForm.MediaTextureEN.Visible = true;
                                    uint Tex = Media.GetTexture(NewMediaID, false);
                                    if (Tex != 0)
                                    {
                                        GEForm.MediaTextureEN.Texture(Tex);
                                        Media.UnloadTexture(NewMediaID);
                                    }
                                    else
                                    {
                                        SelectedMediaType = "";
                                    }
                                }
                                MediaPreviewPlaySound.Enabled = false;
                                MediaPreviewStopSound.Enabled = false;
                                break;
                            // Sound
                            case "So":
                                MediaPreviewPlaySound.Enabled = true;
                                MediaPreviewStopSound.Enabled = true;
                                SoundPropertiesGroup.Visible = true;
                                break;
                            // Music
                            case "Mu":
                                MediaPreviewPlaySound.Enabled = true;
                                MediaPreviewStopSound.Enabled = true;
                                SoundPropertiesGroup.Visible = true;
                                break;
                        }

                        SelectedMediaID = NewMediaID;
                    }
                }
            }
            else
            {
                MediaTreeSelectedItem = false;
            }
        }

        private void MediaTreeMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            if (!MediaTreeSelectedItem)
            {
                //e.Cancel = true;
                return;
            }
        }
        #endregion

        private void CheckMeshTangents_CheckedChanged(object sender, EventArgs e)
        {
            if (GEForm.MediaMeshEN != null)
            {
                if (CheckMeshTangents.Checked)
                {
                    GEForm.MediaMeshEN.Flags |= 2;
                }
                else
                {
                    GEForm.MediaMeshEN.Flags &= BitConverter.GetBytes(~2)[0];
                }
                Media.MeshFlags[GEForm.MediaMeshEN.ID] = GEForm.MediaMeshEN.Flags;
                Media.SetMeshFlags(GEForm.MediaMeshEN.ID, GEForm.MediaMeshEN.Flags);
            }
        }

        private void CheckReflectEnvironment_CheckedChanged(object sender, EventArgs e)
        {
            if (GEForm.MediaMeshEN != null)
            {
                if (CheckMeshTangents.Checked)
                {
                    GEForm.MediaMeshEN.Flags |= 4;
                }
                else
                {
                    GEForm.MediaMeshEN.Flags &= BitConverter.GetBytes(~4)[0];
                }
                Media.MeshFlags[GEForm.MediaMeshEN.ID] = GEForm.MediaMeshEN.Flags;
                Media.SetMeshFlags(GEForm.MediaMeshEN.ID, GEForm.MediaMeshEN.Flags);
            }
        }

        private void CheckRefractEnvironment_CheckedChanged(object sender, EventArgs e)
        {
            if (GEForm.MediaMeshEN != null)
            {
                if (CheckMeshTangents.Checked)
                {
                    GEForm.MediaMeshEN.Flags |= 8;
                }
                else
                {
                    GEForm.MediaMeshEN.Flags &= BitConverter.GetBytes(~8)[0];
                }
                Media.MeshFlags[GEForm.MediaMeshEN.ID] = GEForm.MediaMeshEN.Flags;
                Media.SetMeshFlags(GEForm.MediaMeshEN.ID, GEForm.MediaMeshEN.Flags);
            }
        }

        private void ConfigureParametersButton_Click(object sender, EventArgs e)
        {
            ListBoxItem LBI = (ListBoxItem)MediaMeshShaderCombo.SelectedItem;

            if (LBI == null)
            {
                return;
            }

            int Value = (int)LBI.Value;

            if (GEForm.MediaMeshEN != null)
            {
                ShaderProfile Pr = null;

                if (Value == 65535)
                {
                    if (GEForm.MediaMeshEN.IsAnim == 0)
                    {
                        Pr = ShaderManager.DefaultStatic;
                    }
                    else
                    {
                        Pr = ShaderManager.DefaultAnimated;
                    }
                }
                else
                {
                    Pr = ShaderManager.GetProfile(Value);
                }

                if (ParametersForm == null)
                {
                    ParametersForm = new ConfigureParameter();
                }
                ParametersForm.Disposed += new EventHandler(ParametersForm_Disposed);
                ParametersForm.Entity = GEForm.MediaMeshEN;
                ParametersForm.Effect = Pr;
                ParametersForm.Show();
            }
        }

        private void ParametersForm_Disposed(object sender, EventArgs e)
        {
            ParametersForm = null;
        }

        private void nDistHigh_ValueChanged(object sender, EventArgs e)
        {
            // Get type and media ID of selected node
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null)
            {
                if (MediaTree.SelectedNode.Tag != null)
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;

                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);
                if (MediaID < 65535 && MediaType == "Me")
                    Media.SetMeshLOD_Distance(MediaID, 0, (float)nDistHigh.Value);
            }
        }

        private void nDistMedium_ValueChanged(object sender, EventArgs e)
        {
            // Get type and media ID of selected node
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null)
            {
                if (MediaTree.SelectedNode.Tag != null)
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;

                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);
                if (MediaID < 65535 && MediaType == "Me")
                    Media.SetMeshLOD_Distance(MediaID, 1, Convert.ToSingle(nDistMedium.Value));
            }
        }

        private void nDistLow_ValueChanged(object sender, EventArgs e)
        {
            // Get type and media ID of selected node
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null)
            {
                if (MediaTree.SelectedNode.Tag != null)
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;

                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);
                if (MediaID < 65535 && MediaType == "Me")
                    Media.SetMeshLOD_Distance(MediaID, 2, Convert.ToSingle(nDistLow.Value));
            }
        }


        private void btnMeshMedium_Click(object sender, EventArgs e)
        {
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null)
            {
                if (MediaTree.SelectedNode.Tag != null)
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;

                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);
                if (MediaID < 65535 && MediaType == "Me")
                {
                    ushort LastID = 65535;
                    if (GEForm.MediaMeshEN != null)
                        LastID = GEForm.MediaMeshEN.MeshLOD_Medium;
                    ushort MeshID = MediaDialogs.GetMesh(true, LastID);
                    //if (MeshID < 65535)
                    //{
                    Media.SetMeshLOD(MediaID, 0, MeshID);
                    btnMeshMedium.Text = GE.NiceMeshName(MeshID);
                    //}
                }
            }
        }

        private void btnMeshLow_Click(object sender, EventArgs e)
        {
            ushort MediaID = 65535;
            if (MediaTree.SelectedNode != null)
            {
                if (MediaTree.SelectedNode.Tag != null)
                    MediaID = (ushort)MediaTree.SelectedNode.Tag;

                string MediaType = MediaTree.SelectedNode.FullPath.Substring(0, 2);
                if (MediaID < 65535 && MediaType == "Me")
                {
                    ushort LastID = 65535;
                    if (GEForm.MediaMeshEN != null)
                        LastID = GEForm.MediaMeshEN.MeshLOD_Low;
                    ushort MeshID = MediaDialogs.GetMesh(true, LastID);
                    //if (MeshID < 65535)
                    //{
                    Media.SetMeshLOD(MediaID, 1, MeshID);
                    btnMeshLow.Text = GE.NiceMeshName(MeshID);
                    //}
                }
            }
        }

        private void btnMeshHigh_Click(object sender, EventArgs e)
        {

        }

        // Removes a node from a treeview, along with all its parents which no longer have children
        private void RemoveTreeViewNode(TreeNode T)
        {
            if (T == null)
                return;

            TreeNode OldParent;
            TreeNode Parent = T.Parent;
            T.Remove();

            if (Parent == null)
                return;

            if (Parent.Parent != null)
            {
                while (Parent.Nodes.Count == 0)
                {
                    OldParent = Parent;
                    Parent = OldParent.Parent;
                    if (Parent == null)
                        return;

                    Parent.Nodes.Remove(OldParent);
                }
            }
        }

        private void MediaManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Unparent before window closes to prevent disposing
            Controls.Remove(RenderingPanel);
            RenderingPanel.Parent = null;

        }
    }
}
