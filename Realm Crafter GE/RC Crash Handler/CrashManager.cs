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
//Generic RC crash manager.
//Author: Shane Smith Jan 2009

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Management;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RealmCrafter_GE.RC_Crash_Handler
{
    public partial class CrashManager : Form
    {
        private readonly ThreadExceptionEventArgs exc;

        public CrashManager(ThreadExceptionEventArgs e)
        {
            this.InitializeComponent();
            this.exc = e;
        }

        public static void copyDirectory(string Src, string Dst)
        {
            if (Dst[Dst.Length - 1] != Path.DirectorySeparatorChar)
            {
                Dst += Path.DirectorySeparatorChar;
            }
            if (!Directory.Exists(Dst))
            {
                Directory.CreateDirectory(Dst);
            }
            string[] Files = Directory.GetFileSystemEntries(Src);
            foreach (string Element in Files)
            {
                if (Directory.Exists(Element))
                {
                    copyDirectory(Element, Dst + Path.GetFileName(Element));
                }
                else
                {
                    File.Copy(Element, Dst + Path.GetFileName(Element), true);
                }
            }
        }

        public static bool MinimalBackup()
        {
            try
            {
                if (Directory.Exists(@"Minimal Backup Data"))
                {
                    Directory.Delete(@"Minimal Backup Data", true);
                }
                copyDirectory(@"Data\Areas", @"Minimal Backup Data\Areas");
                copyDirectory(@"Data\Game Data", @"Minimal Backup Data\Game Data");
                copyDirectory(@"Data\Server Data", @"Minimal Backup Data\Server Data");
            }
            catch
            {
                MessageBox.Show("Backup failed");
                return false;
            }
            return true;
        }

        public static bool MinimalBackupRestore()
        {
            try
            {
                copyDirectory(@"Minimal Backup Data\Areas", @"Data\Areas");
                copyDirectory(@"Minimal Backup Data\Game Data", @"Data\Game Data");
                copyDirectory(@"Minimal Backup Data\Server Data", @"Data\Server Data");
            }
            catch
            {
                return false;
            }
            return true;
        }

        private void CrashManager_Load(object sender, EventArgs e)
        {
            Thread t = new Thread(this.SetCrashText);
            this.CrashText.Text = "Please wait. Obtaining system and crash information . . .";
            t.Start();
            this.Update();
            
            //string CrashStr = this.GetCrashText();
            //this.CrashText.Text = CrashStr;
        }

        // Thread obtaining crash text
        private void SetCrashText()
        {
            try
            {
                string CrashStr = this.GetCrashText();
                this.Invoke((MethodInvoker)delegate
                {
                    this.CrashText.Text = CrashStr; // runs on UI thread
                });
            }
            catch
            {
                CrashText.Text = "Unable to retrieve system information \n";
                CrashText.Text += this.exc.Exception;
            }
            using (StreamWriter w = File.AppendText("crashlog.log"))
            {
                Log(this.CrashText.Text, w);
                // Close the writer and underlying file.
                w.Close();
            }
        }

        public static void Log(String logMessage, TextWriter w)
        {
            w.Write("\r\nCrash : ");
            w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                        DateTime.Now.ToLongDateString());
            w.WriteLine("{0}", logMessage);
            w.WriteLine("-------------------------------");
            // Update the underlying file.
            w.Flush();
        }

        private string GetCrashText()
        {
            StringBuilder error = new StringBuilder();

            error.AppendLine("Application:       " + Application.ProductName);
            error.AppendLine("App Version:       " + Application.ProductVersion);
            error.AppendLine("RC Version:        " + Program.Version);
            error.AppendLine("Date:              " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"));
            error.AppendLine("Computer name:     " + SystemInformation.ComputerName);
            error.AppendLine("User name:         " + SystemInformation.UserName);
            error.AppendLine("OS:                " + Environment.OSVersion);
            error.AppendLine("Culture:           " + CultureInfo.CurrentCulture.Name);
            error.AppendLine("Resolution:        " + SystemInformation.PrimaryMonitorSize);
            error.AppendLine("System up time:    " + GetSystemUpTime());
            error.AppendLine("App up time:       " +
                             (DateTime.Now - Process.GetCurrentProcess().StartTime));
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("select Caption from Win32_VideoController");
            ManagementObjectCollection oReturnCollection = searcher.Get();
            int x = 1 ;
            foreach (ManagementObject oReturn in oReturnCollection)
            {
                error.AppendLine("Graphics card " + x + ": " + oReturn["Caption"]);
                x++;
            }
            searcher = new ManagementObjectSearcher("select Name from Win32_Processor");
            oReturnCollection = searcher.Get();
            x = 1;
            foreach (ManagementObject oReturn in oReturnCollection)
            {
                error.AppendLine("Processor " + x + ": " + oReturn["Name"]);
                x++;
            }
            

            MEMORYSTATUSEX memStatus = new MEMORYSTATUSEX();
            if (GlobalMemoryStatusEx(memStatus))
            {
                error.AppendLine("Total memory:      " + memStatus.ullTotalPhys / (1024 * 1024) + "Mb");
                error.AppendLine("Available memory:  " + memStatus.ullAvailPhys / (1024 * 1024) + "Mb");
            }

            error.AppendLine("");
            error.Append(this.exc.Exception);
            error.AppendLine("Exception classes:   ");
            error.Append(this.GetExceptionTypeStack(this.exc.Exception));
            error.AppendLine("");
            error.AppendLine("Exception messages: ");
            error.Append(this.GetExceptionMessageStack(this.exc.Exception));

            error.AppendLine("");
            error.AppendLine("Stack Traces:");
            error.Append(this.GetExceptionCallStack(this.exc.Exception));
            error.AppendLine("");
            error.AppendLine("Loaded Modules:");
            Process thisProcess = Process.GetCurrentProcess();
            foreach (ProcessModule module in thisProcess.Modules)
            {
                error.AppendLine(module.FileName + " " + module.FileVersionInfo.FileVersion);
            }

            return error.ToString();
        }

        private void ButtonQuit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private static long GetDirectorySize(string p)
        {
            List<string> result = GetFilesRecursive(p);
            long b = 0;
            foreach (string name in result)
            {
                FileInfo info = new FileInfo(name);
                b += info.Length;
            }
            return b;
        }

        public static List<string> GetFilesRecursive(string b)
        {
            List<string> result = new List<string>();
            Stack<string> stack = new Stack<string>();
            stack.Push(b);
            while (stack.Count > 0)
            {
                string dir = stack.Pop();
                try
                {
                    result.AddRange(Directory.GetFiles(dir, "*.*"));
                    foreach (string dn in Directory.GetDirectories(dir))
                    {
                        stack.Push(dn);
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        private void ButtonBackupSave_Click(object sender, EventArgs e)
        {
            DialogResult ContinueResult;
            try
            {
                long size = GetDirectorySize(@"Data");
                size = (size / 1024) / 1024;
                ContinueResult =
                    MessageBox.Show(
                        @"RealmCrafter will attempt to backup your existing \Data folder to \Backup Data the application may seem unresponsive during this time. " +
                        "\nWe estimate this to be " + size + " MB of data are you sure you wish to continue?",
                        "Continue with backup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            catch
            {
                ContinueResult = MessageBox.Show(
                    @"RealmCrafter will attempt to backup your existing \Data folder to \Backup Data the application may seem unresponsive during this time. " +
                    "\nThis could be a large amount of data, are you sure you wish to continue?",
                    "Continue with backup", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            }
            if (ContinueResult == DialogResult.No)
            {
                return;
            }
            try
            {
                if (Directory.Exists(@"Backup Data"))
                {
                    Directory.Delete(@"Backup Data", true);
                }
                copyDirectory(@"Data", @"Backup Data");
            }
            catch
            {
                MessageBox.Show("Backup failed, cancelling save.");
                return;
            }
            try
            {
                Program.GE.SaveAll();
            }
            catch
            {
                DialogResult Err = MessageBox.Show("Fatal error saving, attempt to restore backup?", "Fatal error",
                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (Err == DialogResult.Yes)
                {
                    try
                    {
                        copyDirectory(@"Backup Data", @"Data");
                    }
                    catch
                    {
                        MessageBox.Show("Backup restore failed");
                        return;
                    }
                }
                return;
            }
            MessageBox.Show(
                "Project succesfully saved. If you encounter any issues, please restore the Backup Data folder");
        }

        private void MinBackupSave_Click(object sender, EventArgs e)
        {
            if (!MinimalBackup())
            {
                MessageBox.Show("Error backing up files. Cancelling save.");
                return;
            }
            try
            {
                Program.GE.SaveAll();
            }
            catch
            {
                DialogResult Err = MessageBox.Show("Fatal error saving, attempt to restore backup?", "Fatal error",
                                                   MessageBoxButtons.YesNo, MessageBoxIcon.Error);
                if (Err == DialogResult.Yes)
                {
                    if (!MinimalBackupRestore())
                    {
                        MessageBox.Show("Critical failure restoring backup files.");
                        return;
                    }
                }
                return;
            }
            MessageBox.Show(
                "Project succesfully saved. If you encounter any issues, please restore the Minimal Backup Data folder","Minimal Backup",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }



        private string GetExceptionTypeStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(this.GetExceptionTypeStack(e.InnerException));
                return (message.ToString());
            }
            else
            {
                return ("   " + e.GetType());
            }
        }

        private string GetExceptionMessageStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(this.GetExceptionMessageStack(e.InnerException));
                return (message.ToString());
            }
            else
            {
                return ("   " + e.Message);
            }
        }

        private string GetExceptionCallStack(Exception e)
        {
            if (e.InnerException != null)
            {
                StringBuilder message = new StringBuilder();
                message.AppendLine(this.GetExceptionCallStack(e.InnerException));
                message.AppendLine("--- Next Call Stack:");
                return (message.ToString());
            }
            else
            {
                return (e.StackTrace);
            }
        }

        private static TimeSpan GetSystemUpTime()
        {
            PerformanceCounter upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            return TimeSpan.FromSeconds(upTime.NextValue());
        }

        private void CrashText_Click(object sender, EventArgs e)
        {
        }

        private void MinBackupSave_MouseLeave(object sender, EventArgs e)
        {
            this.ButtonTooltip.Text = "";
        }

        private void MinBackupSave_MouseEnter(object sender, EventArgs e)
        {
            this.ButtonTooltip.Text = "Backup game data (recommended)";
        }

        private void ButtonBackupSave_MouseEnter(object sender, EventArgs e)
        {
            this.ButtonTooltip.Text = "Backup all data";
        }

        private void ButtonBackupSave_MouseLeave(object sender, EventArgs e)
        {
            this.ButtonTooltip.Text = "";
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool GlobalMemoryStatusEx([In, Out] MEMORYSTATUSEX lpBuffer);

        #region Nested type: MEMORYSTATUSEX
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private class MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;

            public MEMORYSTATUSEX()
            {
                this.dwLength = (uint) Marshal.SizeOf(typeof(MEMORYSTATUSEX));
            }
        }
        #endregion
    }
}