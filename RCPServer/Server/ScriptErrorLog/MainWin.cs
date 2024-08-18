using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ScriptErrorLog
{


    public partial class MainWin : Form
    {
        public MainWin()
        {
            InitializeComponent();
        }

        public void This_Activated(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("TRIGGER");
        }


        public void This_SizeChanged(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("CHANGE");
            //B.ClearCallbacks();
        }


        //Scripting.ScriptBase B;

        private void MainWin_Load(object sender, EventArgs e)
        {
            //B = new ScriptBase();
            //B.RegisterCallback(this, "Activated", new EventHandler(This_Activated));
            //B.RegisterCallback(this, "SizeChanged", new EventHandler(This_SizeChanged));

            ScriptText.Text = "";
            ScriptText.ReadOnly = true;
            StreamReader Reader = new StreamReader(File.OpenRead(Program.ExFile));

            string DataRead = "";
            int Line = 1;
            int SelStart = 0;
            int SelEnd = 0;

            try
            {
                while (!Reader.EndOfStream)
                {
                    string D = Reader.ReadLine();
                    if (Line == Program.ExLine)
                        SelStart = DataRead.Length - (Line - 1);
                    if (Line == Program.ExLine + 1)
                        SelEnd = DataRead.Length - Line;
                    DataRead += D + Environment.NewLine;
                    ++Line;
                }

            }
            catch
            {

            }

            ScriptText.Text = DataRead;// Reader.ReadToEnd();
            ScriptText.SelectionStart = SelStart;
            ScriptText.SelectionLength = SelEnd - SelStart;

            ScriptText.SelectionFont = new Font("Courier New", 10, FontStyle.Regular);
            ScriptText.SelectionBackColor = Color.Red;
            ScriptText.SelectionColor = Color.White;

            Reader.Close();

            

            ExceptionStatus.Text = Program.ExString;
        }
    }
}