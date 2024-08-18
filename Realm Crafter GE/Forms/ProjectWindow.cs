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
using System.Diagnostics;

namespace RealmCrafter_GE.Forms
{
    public partial class ProjectWindow : Form
    {
        GE GEForm;

        private readonly string[] UpdatesList = {
													@"Data\Areas", @"Data\Emitter Configs", @"Data\Game Data",
													@"Data\UI", @"Data\Meshes", @"Data\Music", @"Data\Sounds",
													@"Data\Textures", @"Data\Skins", @"Data\Fonts", @"Data\Terrains",
													@"Data\Trees"
												};

        public ProjectWindow(GE geForm)
        {
            InitializeComponent();
            GEForm = geForm;
        }

        private void ProjectWindowcs_Load(object sender, EventArgs e)
        {
            // Populate controls initially
            // Load host data
            BinaryReader file = Blitz.ReadFile(@"Data\Game Data\Hosts.dat");
            if (file == null)
            {
                MessageBox.Show(@"File not found: Data\Game Data\Hosts.dat!", "Error");
                return;
            }

            string ip = Blitz.ReadLine(file), 
                updateAddress = Blitz.ReadLine(file);

            file.Close();

            // Assign strings after so it doesnt trigger text changed with file open
            IPTextBox.Text = ip;
            UpdateTextBox.Text = updateAddress;

            // Load max accounts amount - by seeking bytes because we live in 1985
            FileStream fileStream = new FileStream(@"Data\Server Data\Misc.dat",
                                    FileMode.Open, FileAccess.Read);
            BinaryReader bReader = new BinaryReader(fileStream);
            if (bReader == null)
            {
                MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
                return;
            }
            bReader.BaseStream.Seek(16, SeekOrigin.Begin);
            byte maxAccounts = bReader.ReadByte();
            bReader.Close();

            // Assign after so it doesnt trigger changed event with file open
            MaxCharsPerAccount.Value = maxAccounts;
         

            // Load account creation bool
            fileStream = new FileStream(@"Data\Server Data\Misc.dat",
                                    FileMode.Open, FileAccess.Read);
            bReader = new BinaryReader(fileStream);
            if (bReader == null)
            {
                MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
                return;
            }
            bReader.BaseStream.Seek(15, SeekOrigin.Begin);
            bool accountCreation = bReader.ReadBoolean();
            bReader.Close();
            // Assign after so it doesnt trigger changed event with file open
            AllowAccountCreation.Checked = accountCreation;

            ProjectMeshes.Text = "Meshes: " + MediaDialogs.TotalMeshes;
            ProjectTextures.Text = "Textures: " + MediaDialogs.TotalTextures;
            ProjectSounds.Text = "Sounds: " + MediaDialogs.TotalSounds;
            ProjectMusic.Text = "Music: " + MediaDialogs.TotalMusic;
            ProjectActors.Text = "Actors: " + GEForm.TotalActors;
            ProjectItems.Text = "Items: " + GEForm.TotalItems;
            ProjectZones.Text = "Zones: " + GEForm.TotalZones;


        }

        		bool SuspendBuildFull = false;
		private void BProjectBuildFull_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion)
			{
				// Warn user
				DialogResult Result = DialogResult.OK;
				
				if(!SuspendBuildFull)
					MessageBox.Show("Building full client may take some time, please be patient.",
													  "Warning", MessageBoxButtons.OKCancel);
				if (Result == DialogResult.OK)
				{
					// Clear the output folder and recreate new folder structure
                    GEForm.MainProgress.Visible = true;
					GEForm.MainProgress.Value = 0;
					if (Directory.Exists("Game"))
					{
						try
						{
							Directory.Delete("Game", true);
						}
						catch (System.Exception)
						{
							
						}
						
					}
					Program.CreateDirectory("Game");
					Program.CreateDirectory(@"Game\Data");
					Program.CreateDirectory(@"Game\Data\Logs");
					foreach (string S in UpdatesList)
					{
						Program.CreateDirectory(@"Game\" + S);
					}

					// Copy required files
					GEForm.MainProgress.Value = 10;
					Application.DoEvents();
                    GE.SafeCopyFile(GEForm.GameName + ".exe", @"Game\" + GEForm.GameName + ".exe");
					GE.SafeCopyFile("libbz2w.dll", @"Game\libbz2w.dll");
					GE.SafeCopyFile("blitzsys.dll", @"Game\blitzsys.dll");
					GE.SafeCopyFile("rc64.dll", @"Game\rc64.dll");
					GE.SafeCopyFile("QuickCrypt.dll", @"Game\QuickCrypt.dll");
					GE.SafeCopyFile("BBDX.dll", @"Game\BBDX.dll");
					GE.SafeCopyFile("SDK.dll", @"Game\SDK.dll");
					GE.SafeCopyFile("LT.dll", @"Game\LT.dll");
					GE.SafeCopyFile("NGUID3D9.dll", @"Game\NGUID3D9.dll");
					GE.SafeCopyFile("NxCharacter.dll", @"Game\NxCharacter.dll");
					GE.SafeCopyFile("irrKlang.dll", @"Game\irrKlang.dll");
					GE.SafeCopyFile("Game.exe", @"Game\Game.exe");
					GE.SafeCopyFile("libcurl.dll", @"Game\libcurl.dll");
					GE.SafeCopyFile("rcui2.dll", @"Game\rcui2.dll");
					GE.SafeCopyFile("RCEnet.dll", @"Game\RCEnet.dll");
                    GE.SafeCopyFile("References.txt", @"Game\References.txt");
					//GE.SafeCopyFile(@"Data\Last Username.dat", @"Game\Data\Last Username.dat");
					GE.SafeCopyFile(@"Data\Options.xml", @"Game\Data\Options.xml");
					GE.SafeCopyFile(@"Data\Controls.dat", @"Game\Data\Controls.dat");
					GE.SafeCopyFile(@"Data\Patch.exe", @"Game\Data\Patch.exe");
					GE.SafeCopyFile(@"Data\DefaultTex.PNG", @"Game\Data\DefaultTex.PNG");
					GE.SafeCopyFile(@"Data\Sphere.b3d", @"Game\Data\Sphere.b3d");
					GE.SafeCopyFile(@"Data\Box.b3d", @"Game\Data\Box.b3d");

                    // Copy sqlite
                    GE.SafeCopyFile(@"sqlite3.dll", @"Game\sqlite3.dll");
                    GE.SafeCopyFile(@"System.Data.SQLite.DLL", @"Game\System.Data.SQLite.DLL");

                    // Copy physx dll
                    GE.SafeCopyFile(@"PhysXCooking.dll", @"Game\PhysXCooking.dll");
                    GE.SafeCopyFile(@"PhysXCore.dll", @"Game\PhysXCore.dll");
                    GE.SafeCopyFile(@"PhysXDevice.dll", @"Game\PhysXDevice.dll");
                    GE.SafeCopyFile(@"PhysXLoader.dll", @"Game\PhysXLoader.dll");

                    // Copy Launcher
                    GE.SafeCopyFile(@"Launcher.exe", @"Game\Launcher.exe");
                    GE.SafeCopyFile(@"Updater.xml", @"Game\Updater.xml");

                    GEForm.MainProgress.Value = 0;
                    GEForm.MainProgress.Maximum = UpdatesList.Length;
					Application.DoEvents();
					foreach (string S in UpdatesList)
					{
						GE.CopyTree(S, @"Game\" + S);

						// We don't really care if the progressbar is not exact
						try
						{
							GEForm.MainProgress.Value += 1;
						}
						catch (System.Exception ex)
						{
							
						}
						
						Application.DoEvents();
					}

					// Change to non-development version
					FileStream F = new FileStream(@"Game\Data\Game Data\Misc.dat",
												  FileMode.Create,
												  FileAccess.Write);
					BinaryWriter BW = new BinaryWriter(F);
                    Blitz.WriteLine(GEForm.GameName, BW);
					Blitz.WriteLine("Normal", BW);
					Blitz.WriteLine("1", BW);
					BW.Close();

					// Complete
                    GEForm.MainProgress.Visible = false;

					if(!SuspendBuildFull)
						MessageBox.Show(@"Complete! Required files are in the \Game folder.", "Build Client");
				}
			}
		}

		private void BProjectBuildMinimum_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion)
			{
				// Clear the output folder and recreate new folder structure
				if (Directory.Exists("Game"))
				{
					Directory.Delete("Game", true);
				}
				Directory.CreateDirectory("Game");
				Directory.CreateDirectory(@"Game\Data");
				Directory.CreateDirectory(@"Game\Data\Textures");
				Directory.CreateDirectory(@"Game\Data\Logs");

				// Copy required files
                GE.SafeCopyFile(GEForm.GameName + ".exe", @"Game\" + GEForm.GameName + ".exe");
				GE.SafeCopyFile("Game.exe", @"Game\Game.exe");
				//GE.SafeCopyFile("libbz2w.dll", @"Game\libbz2w.dll");
				//GE.SafeCopyFile("blitzsys.dll", @"Game\blitzsys.dll");
				GE.SafeCopyFile("rc64.dll", @"Game\rc64.dll");
				GE.SafeCopyFile("QuickCrypt.dll", @"Game\QuickCrypt.dll");
				GE.SafeCopyFile("BBDX.dll", @"Game\BBDX.dll");
				GE.SafeCopyFile("irrKlang.dll", @"Game\irrKlang.dll");
				GE.SafeCopyFile("libcurl.dll", @"Game\libcurl.dll");
				GE.SafeCopyFile("NxCharacter.dll", @"Game\NxCharacter.dll");
				GE.SafeCopyFile("RCEnet.dll", @"Game\RCEnet.dll");
				//  GE.SafeCopyFile(@"Data\Last Username.dat", @"Game\Data\Last Username.dat");
				GE.SafeCopyFile(@"Data\Options.xml", @"Game\Data\Options.xml");
				GE.SafeCopyFile(@"Data\Controls.dat", @"Game\Data\Controls.dat");
				GE.SafeCopyFile(@"Data\Patch.exe", @"Game\Data\Patch.exe");
				GE.SafeCopyFile(@"Data\DefaultTex.PNG", @"Game\Data\DefaultTex.PNG");
				GE.SafeCopyFile(@"Data\Sphere.b3d", @"Game\Data\Sphere.b3d");
				GE.SafeCopyFile(@"Data\Box.b3d", @"Game\Data\Box.b3d");
				GE.CopyTree(@"Data\Game Data", @"Game\Data\Game Data");
                GE.CopyTree(@"Data\UI", @"Game\Data\UI");
				//CopyTree(@"Data\Textures\Menu", @"Game\Data\Textures\Menu");

				// Change to non-development version
				FileStream F = new FileStream(@"Game\Data\Game Data\Misc.dat",
											  FileMode.Create, FileAccess.Write);
				BinaryWriter BW = new BinaryWriter(F);
                Blitz.WriteLine(GEForm.GameName, BW);
				Blitz.WriteLine("Normal", BW);
				Blitz.WriteLine("1", BW);
				BW.Close();

				// Complete
				MessageBox.Show(@"Complete! Required files are in the \Game folder.", "Build Client");
			}
		}

		private void BProjectBuildServer_Click(object sender, EventArgs e)
		{
			if (!Program.DemoVersion)
			{
				BuildManager Mgr = new BuildManager();
				Mgr.BuildBases("Server\\");
//                 // User options
//                 DialogResult DynamicDataResult = MessageBox.Show("Include dynamic data (e.g. accounts)?", "Build Server",
//                                                                  MessageBoxButtons.YesNo);
//                 DialogResult SQLResult = MessageBox.Show("Build a MySQL server?", "Build Server",
//                                                          MessageBoxButtons.YesNo);
// 
//                 // Clear the output folder and recreate new folder structure
//                 if (Directory.Exists("Server"))
//                 {
//                     try
//                     {
//                         Directory.Delete("Server", true);
//                     }
//                     catch (System.Exception)
//                     {
//                     	
//                     }
//                 }
//                 Program.CreateDirectory("Server");
//                 Program.CreateDirectory(@"Server\Data");
//                 Program.CreateDirectory(@"Server\Data\Logs");
//                 Program.CreateDirectory(@"Server\Data\Server Data");
// 
//                 // Copy required files
//                 if (SQLResult == DialogResult.No)
//                 {
//                     GE.SafeCopyFile("Server.exe", @"Server\Server.exe");
//                 }
//                 else
//                 {
//                     GE.SafeCopyFile("MySQL Server.exe", @"Server\MySQL Server.exe");
//                     GE.SafeCopyFile("MySQL Configure.exe", @"Server\MySQL Configure.exe");
//                     GE.SafeCopyFile("libmySQL.dll", @"Server\libmySQL.dll");
//                     GE.SafeCopyFile("SQLDLL.dll", @"Server\SQLDLL.dll");
//                     GE.SafeCopyFile("BlitzSQL.dll", @"Server\BlitzSQL.dll");
//                     GE.SafeCopyFile("MySql.Data.dll", @"Server\MySql.Data.dll");
//                     GE.SafeCopyFile("rcsql.sql", @"Server\rcsql.sql");
//                     GE.SafeCopyFile("rcsql_flat.sql", @"Server\rcsql_flat.sql");
//                     GE.SafeCopyFile("mini.exe", @"Server\mini.exe");
//                 }
//                 GE.SafeCopyFile("ggTray.dll", @"Server\ggTray.dll");
//                 GE.SafeCopyFile("briskvm.dll", @"Server\briskvm.dll");
//                 GE.SafeCopyFile("RCEnet.dll", @"Server\RCEnet.dll");
//                 CopyTree(@"Data\Server Data", @"Server\Data\Server Data\");
// 
//                 // If it's only an update, delete accounts etc.
//                 if (DynamicDataResult == DialogResult.No)
//                 {
//                     File.Delete(@"Server\Data\Server Data\Accounts.dat");
//                     File.Delete(@"Server\Data\Server Data\Dropped Items.dat");
//                     File.Delete(@"Server\Data\Server Data\Superglobals.dat");
//                     try
//                     {
//                         Directory.Delete(@"Server\Data\Server Data\Areas\Ownerships", true);
//                         Program.CreateDirectory(@"Server\Data\Server Data\Areas\Ownerships");
//                     }
//                     catch (System.Exception)
//                     {
//                     	
//                     }
//                     
//                }

				// Complete
				MessageBox.Show(@"Complete! Required files are in the \Server folder.", "Build Server");
			}
		}

		private class UpdateFile
		{
			public string Name;
		}

		private List<UpdateFile> UpdateFiles;

		private void BProjectBuildUpdate_Click(object sender, EventArgs e)
		{
			if (Program.DemoVersion)
				return;

			if (MessageBox.Show("Realm Crafter must build a full client to generate update files. Press yes to build a new client.", "Generate Update", MessageBoxButtons.YesNo) == DialogResult.No)
				return;
            
			SuspendBuildFull = true;
			BProjectBuildFull_Click(sender, e);
			SuspendBuildFull = false;

            Process.Start(new ProcessStartInfo("ChecksumGenerator.exe", "Game Game/patchsums.txt"));

            #region Old update generater
            /*
			try
			{
				Directory.Delete("Patches", true);
			}
			catch (System.Exception ex)
			{
				
			}
			
			try { Program.CreateDirectory("Patches\\"); } catch (Exception ex) { MessageBox.Show("1: " + ex.Message); }
			try { Program.CreateDirectory("Patches\\Files\\"); } catch (Exception ex) { MessageBox.Show("2: " + ex.Message); }

			UpdateFiles = new List<UpdateFile>();
			SearchDir("Game\\");

			foreach (UpdateFile F in UpdateFiles)
			{
				string TName = F.Name.Substring(5);
				TName = TName.Replace("\\", "!!");
				TName = TName.Replace(" ", "!!!");
				TName = TName.ToUpper();


				try
				{
					File.Copy(F.Name, Path.Combine(@"Patches\Files\", TName) + ".DAT");
				}
				catch (System.Exception ex)
				{
					MessageBox.Show("Exception thrown when copying file: " + F.Name + ".\nThis may cause problems when running the built project\n\nException Info:\n" + ex.Message);
				}
			}

			// Copy PHP file
			try
			{
				File.Copy("UpdateServer.php", @"Patches\Files\UpdateServer.php");
			}
			catch (System.Exception)
			{
				
			}
			

			// Give instructions to user:
			MessageBox.Show("Please follow these steps:\nCopy the files from \\Patches\\Files to your webserver making sure that you overwrite any newer files (Unchanged files can be left to save upload time).\nWhen your clients next launch your game patcher, they will download the latest files.");

			UpdateFiles.Clear();
            */
            #endregion
            UpdateFiles = null;
		}

		private void SearchDir(string path)
		{
			string[] Files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

			foreach (string Filename in Files)
			{
				if (File.Exists(Filename) && Filename.Length > 2)
				{
                    if (Filename.Equals(GEForm.GameName, StringComparison.CurrentCultureIgnoreCase) == false)
					{
						UpdateFile F = new UpdateFile();
						F.Name = Filename;
						UpdateFiles.Add(F);
					}
				}
			}
		}

		private void HostDataChanged(object sender, EventArgs e)
		{
			BinaryWriter F = Blitz.WriteFile(@"Data\Game Data\Hosts.dat");
			if (F == null)
			{
				MessageBox.Show(@"Could not open Data\Game Data\Hosts.dat!");
				return;
			}
			Blitz.WriteLine(IPTextBox.Text, F);
			Blitz.WriteLine(UpdateTextBox.Text, F);
			if (AllowAccountCreation.Checked)
			{
				Blitz.WriteLine("1", F);
			}
			else
			{
				Blitz.WriteLine("0", F);
			}
			F.Close();
		}

		private void ServerPortSpinner_ValueChanged(object sender, EventArgs e)
		{
			FileStream FStream;
			BinaryWriter F;
			// Server side
			int Port = (int)Program.ServerPort;

			FStream = new FileStream(@"Data\Server Data\Misc.dat", FileMode.Open,
									 FileAccess.Write);
			F = new BinaryWriter(FStream);
			if (F == null)
			{
				MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
				return;
			}
			F.BaseStream.Seek(17, SeekOrigin.Begin);
			F.Write(Port);
			F.Close();

			//client side
			FStream = new FileStream(@"Data\Game Data\Other.dat", FileMode.Open,
									 FileAccess.Write);
			F = new BinaryWriter(FStream);
			if (F == null)
			{
				MessageBox.Show(@"Could not open Data\Game Data\Other.dat!");
				return;
			}
			F.BaseStream.Seek(3, SeekOrigin.Begin);
			F.Write(Port);
			F.Close();
		}

		private void AllowAccountCreation_CheckedChanged(object sender, EventArgs e)
		{
			// Client side
			HostDataChanged(sender, e);

			// Server side
			FileStream FStream = new FileStream(@"Data\Server Data\Misc.dat",
												FileMode.Open, FileAccess.Write);
			BinaryWriter F = new BinaryWriter(FStream);
			if (F == null)
			{
				MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
				return;
			}
			F.BaseStream.Seek(15, SeekOrigin.Begin);
			F.Write(AllowAccountCreation.Checked);
			F.Close();
		}

		private void MaxCharsPerAccount_ValueChanged(object sender, EventArgs e)
		{
			FileStream FStream = new FileStream(@"Data\Server Data\Misc.dat",
												FileMode.Open, FileAccess.Write);
			BinaryWriter F = new BinaryWriter(FStream);
			if (F == null)
			{
				MessageBox.Show(@"Could not open Data\Server Data\Misc.dat!");
				return;
			}
			F.BaseStream.Seek(16, SeekOrigin.Begin);
			F.Write((byte) MaxCharsPerAccount.Value);
			F.Close();
		}

        private void ConfigManagerButton_Click(object sender, EventArgs e)
        {
            BuildManager Mgr = new BuildManager();
            Mgr.ShowDialog();
        }

        private void ProjectMusic_Click(object sender, EventArgs e)
        {

        }
		

    }
}
