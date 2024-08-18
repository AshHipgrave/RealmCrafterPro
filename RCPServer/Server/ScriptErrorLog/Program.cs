using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ScriptErrorLog
{
    static class Program
    {
        public static string ExFile = "";
        public static string ExString = "";
        public static int ExLine = 0;
        public static int ExCollumn = 0;
        
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            

            for (int i = 0; i < args.Length - 1; ++i)
            {
                string Arg = args[i];

                if (Arg.Equals("-f", StringComparison.CurrentCultureIgnoreCase))
                    ExFile = args[i + 1];
                if (Arg.Equals("-e", StringComparison.CurrentCultureIgnoreCase))
                    ExString = args[i + 1];
                if (Arg.Equals("-l", StringComparison.CurrentCultureIgnoreCase))
                    ExLine = Convert.ToInt32(args[i + 1]);
                if (Arg.Equals("-c", StringComparison.CurrentCultureIgnoreCase))
                    ExCollumn = Convert.ToInt32(args[i + 1]);
            }




            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainWin());
        }
    }
}