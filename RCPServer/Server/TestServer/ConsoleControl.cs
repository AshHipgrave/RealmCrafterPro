using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace TestServer
{
    public partial class ConsoleControl : UserControl
    {
        Font ConsoleFont;
        string[] ConsoleText = new string[24];

        public ConsoleControl()
        {
            InitializeComponent();

            ConsoleFont = new Font("Courier New", 10.0f);

            for (int i = 0; i < 24; ++i)
            {
                ConsoleText[i] = "01234567890123456789012345678901234567890123456789012345678901234567890123456789";
            }

            ConsoleText[0] = "Starting Realm Crafter Server";
            ConsoleText[1] = "Version: 2.50 <PR-Core>";
            ConsoleText[2] = "Build Type: Master Server";
            ConsoleText[3] = "";
            ConsoleText[4] = "MASTER PROXY";
            ConsoleText[5] = "Loading Areas                  [OK (4 Instances)]";
            ConsoleText[6] = "Starting Network               [OK]";
            ConsoleText[7] = "";
            ConsoleText[8] = "Server Started";
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.FillRectangle(Brushes.Black, e.ClipRectangle);

            for (int i = 0; i < ConsoleText.Length; ++i)
            {
                e.Graphics.DrawString(ConsoleText[i], ConsoleFont, Brushes.White, 0, i * 11.9f);
            }
        }
    }
}
