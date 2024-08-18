using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace RCPServer
{
    public class Log
    {
        static StreamWriter writer = null;
        static LinkedList<string> HTMLBuffer = null;

        public static void StartLog(string path, bool useHTMLBuffer)
        {
            writer = new StreamWriter(File.OpenWrite(path));

            if (useHTMLBuffer)
            {
                HTMLBuffer = new LinkedList<string>();
                for (int i = 0; i < 40; ++i)
                    HTMLBuffer.AddLast("");
            }
        }

        public static void StopLog()
        {
            if (writer != null)
                writer.Close();
            HTMLBuffer = null;
        }

        static string HTMLColorFromConsoleColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Black:
                    return "#000000";
                case ConsoleColor.Blue:
                    return "#0000ff";
                case ConsoleColor.Cyan:
                    return "#00ffff";
                case ConsoleColor.DarkBlue:
                    return "#000080";
                case ConsoleColor.DarkCyan:
                    return "#008080";
                case ConsoleColor.DarkGray:
                    return "#808080";
                case ConsoleColor.DarkGreen:
                    return "#008000";
                case ConsoleColor.DarkMagenta:
                    return "#800080";
                case ConsoleColor.DarkRed:
                    return "#80000";
                case ConsoleColor.DarkYellow:
                    return "#808000";
                case ConsoleColor.Gray:
                    return "#c0c0c0";
                case ConsoleColor.Green:
                    return "#00ff00";
                case ConsoleColor.Magenta:
                    return "#ff00ff";
                case ConsoleColor.Red:
                    return "#ff0000";
                case ConsoleColor.White:
                    return "#ffffff";
                case ConsoleColor.Yellow:
                    return "#ffff00";
            }

            return "#ffffff";
        }

        public static void WriteLine(string data)
        {
            WriteLine(data, ConsoleColor.White);
        }

        public static void WriteLine(string data, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(data);
            Console.ForegroundColor = ConsoleColor.White;
            //System.Diagnostics.Trace.WriteLine(data);

            if (writer != null)
                writer.WriteLine(data);

            if (HTMLBuffer != null)
            {
                HTMLBuffer.AddLast("<span style=\"color: " + HTMLColorFromConsoleColor(color) + "\">" + System.Web.HttpUtility.HtmlEncode(data).Replace(" ", "&nbsp;") + "&nbsp;</span><br />");
                HTMLBuffer.RemoveFirst();
            }
        }

        public static void Write(string data)
        {
            Console.Write(data);
            //System.Diagnostics.Trace.Write(data);

            if (writer != null)
                writer.Write(data);

            if (HTMLBuffer != null)
            {
                HTMLBuffer.Last.Value += ("<span style=\"color: #ffffff\">" + System.Web.HttpUtility.HtmlEncode(data).Replace(" ", "&nbsp;") + "</span>");
            }
        }

        public static void WriteOK()
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("OK");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("]");

            if (writer != null)
                writer.WriteLine("[OK]");

            if (HTMLBuffer != null)
            {
                HTMLBuffer.Last.Value += ("<span style=\"color: #ffffff\">[</span><span style=\"color: #00ff00\">OK</span><span style=\"color: #ffffff\">]</span><br />");
                HTMLBuffer.RemoveFirst();
            }
        }

        public static void WriteOK(string text)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("OK");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" " + text + "]");

            if (writer != null)
                writer.WriteLine("[OK " + text + "]");

            if (HTMLBuffer != null)
            {
                HTMLBuffer.Last.Value += ("<span style=\"color: #ffffff\">[</span><span style=\"color: #00ff00\">OK</span><span style=\"color: #ffffff\"> " + System.Web.HttpUtility.HtmlEncode(text).Replace(" ", "&nbsp;") + "]</span><br />");
                HTMLBuffer.RemoveFirst();
            }
        }

        public static void WriteFail(string text)
        {
            Console.Write("[");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("FAILED");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(" (" + text + ")]");

            if (writer != null)
                writer.WriteLine("[FAILED " + text + "]");

            if (HTMLBuffer != null)
            {
                HTMLBuffer.Last.Value += ("<span style=\"color: #ffffff\">[</span><span style=\"color: #ff0000\">FAILED</span><span style=\"color: #ffffff\"> " + System.Web.HttpUtility.HtmlEncode(text).Replace(" ", "&nbsp;") + "]</span><br />");
                HTMLBuffer.RemoveFirst();
            }
        }

        public static string HTMLLog()
        {
            if(HTMLBuffer == null)
                return "Error: No Buffer!";

            string Out = "";

            foreach (string S in HTMLBuffer)
            {
                Out += S;
            }

            return Out;
        }
    }
}
