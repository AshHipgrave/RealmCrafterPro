using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace RCPServer.HTTPModules
{
    public class ScriptExceptions : IHTTPModule
    {
        public class ExceptionInstance
        {
            public int ID = 0;
            public DateTime Time;
            public string HostName;
            public string ActorName;
            public string FileName;
            public int LineNumber = 0;
            public int CollumnNumber = 0;
            public string ExceptionMessage;

            public static int AllocIDs = 0;

            public ExceptionInstance(string hostName, string actorName, string fileName, int lineNumber, int collumnNumber, string exceptionMessage)
            {
                ID = ++AllocIDs;

                Time = DateTime.Now;
                HostName = hostName;
                ActorName = actorName;
                FileName = fileName;
                LineNumber = lineNumber;
                CollumnNumber = collumnNumber;
                ExceptionMessage = exceptionMessage;
            }
        }

        public static List<ExceptionInstance> Instances = new List<ExceptionInstance>();

        public ScriptExceptions()
        {
            //Instances.Insert(0, new ExceptionInstance("Bender", "Hizzle", @"Data\Server Data\Scripts\InstanceTest.cs", 15, 0, "Object reference not set to an instance of an object."));
            //Instances.Insert(0, new ExceptionInstance("Bender", "None", @"Data\Server Data\Scripts\AccountDatabase.cs", 160, 0, "Object reference not set to an instance of an object."));
        }

        public bool CheckRequestURL(string c)
        {
            if (c.Equals("/scriptexceptions"))
                return true;
            return false;
        }

        public byte[] Process(string u, Dictionary<string, string> args)
        {
            string HTML = "";

            if (args.ContainsKey("clearall"))
            {
                lock (Instances)
                {
                    Instances.Clear();
                }

                HTML = "JSZoneSelected = -1;contextSwitch('/scriptexceptions');|Directing...";
            }
            else if (args.ContainsKey("delid"))
            {
                int ID = Convert.ToInt32(args["delid"]);
                ExceptionInstance Ex = null;

                lock (Instances)
                {
                    foreach (ExceptionInstance tEx in Instances)
                    {
                        if (tEx.ID == ID)
                        {
                            Ex = tEx;
                            break;
                        }
                    }

                    if (Ex != null)
                    {
                        Instances.Remove(Ex);

                        HTML = "JSZoneSelected = -1;contextSwitch('/scriptexceptions');|Directing...";
                    }

                }
            }
            else if (args.ContainsKey("exid"))
            {
                int ID = Convert.ToInt32(args["exid"]);
                ExceptionInstance Ex = null;

                lock (Instances)
                {
                    if (Instances.Count > ID && ID >= 0)
                        Ex = Instances[ID];
                }

                if(Ex != null)
                {
                    lock (Ex)
                    {
                        HTML = "<table style=\"width: 100%; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" style=\"padding-left: 50px;\" valign=\"top\"><br /><br /><br /><br />";
                        HTML += "<input type=\"button\" value=\"Return to List\" onclick=\"contextSwitch('/scriptexceptions');\" /><br />";
                        HTML += "<table cellpadding=\"0\" cellspacing=\"0\" style=\"width: 800px;\"><tr><td style=\"background-image: url('/img/menugrad.jpg'); height: 21px; padding-left: 10px; border-top: 2px solid #DC1300; border-left: 1px solid #DC1300; border-bottom: 1px solid #390909; border-right: 1px solid #390909;\" valign=\"middle\" class=\"VertMenuTitle\"><b>Script Exception</b></td></tr><tr><td style=\"border: 1px solid #000000; padding: 5px;\">";
                        HTML += "<table style=\"width: 100%; border: 0px; \">";

                        //HTML += "<tr><td style=\"width: 100px;\">&nbsp;</td><td></td></tr>";
                        HTML += string.Format("<tr><td style=\"width: 80px;\">{0}</td><td>{1}</td></tr>", "Time:", Ex.Time.ToString("yyyy-MM-dd HH:mm:ss"));
                        HTML += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Server:", Ex.HostName);
                        HTML += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Actor:", Ex.ActorName);
                        HTML += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "File:", Ex.FileName);
                        HTML += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Line:", Ex.LineNumber);
                        HTML += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Collumn:", Ex.CollumnNumber);
                        HTML += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Message:", Ex.ExceptionMessage);
                        HTML += string.Format("<tr><td>&nbsp;</td><td><input type=\"button\" value=\"Delete Entry\" onclick=\"if(confirm('Are you sure you want to delete this entry?')) {{ contextSwitch('/scriptexceptions?delid={0}'); }}\" /></td></tr>", Ex.ID);
                        HTML += string.Format("<tr><td>{0}</td><td>{1}</td></tr>", "Script:", "");

                        HTML += "</table>";

                        HTML += "<table style=\"width: 100%; border: 1px solid #B5B5B5;\" cellspacing=\"0\" cellpadding=\"0\">";

                        if (File.Exists(Ex.FileName))
                        {
                            StreamReader Reader = new StreamReader(Ex.FileName);

                            int LineNumber = 1;
                            while (!Reader.EndOfStream)
                            {
                                string Txt = Reader.ReadLine();

                                Txt = Txt.Replace(" ", "&nbsp;");
                                Txt = Txt.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                                Txt = Txt.Replace("<", "&lt;");
                                Txt = Txt.Replace(">", "&gt;");

                                if (LineNumber == Ex.LineNumber)
                                    HTML += "<tr><td style=\"border-right: 1px solid #B5B5B5; font-family: 'Courier New'; background-color: #bb0000; color: #ffffff;\">&nbsp;" + LineNumber.ToString() + "&nbsp;</td><td style=\"font-family: 'Courier New'; width: 100%; padding-left: 5px; background-color: #bb0000; color: #ffffff;\">" + Txt + "</td></tr>";
                                else
                                    HTML += "<tr><td style=\"border-right: 1px solid #B5B5B5; font-family: 'Courier New';\">&nbsp;" + LineNumber.ToString() + "&nbsp;</td><td style=\"font-family: 'Courier New'; width: 100%; padding-left: 5px;\">" + Txt + "</td></tr>";

                                ++LineNumber;
                            }

                            Reader.Close();

                            //HTML += "<tr><td style=\"border-right: 1px solid #B5B5B5; font-family: 'Courier New';\">&nbsp;1024&nbsp;</td><td style=\"font-family: 'Courier New'; width: 100%; padding-left: 5px;\">dfsdfsd</td></tr>";
                            //HTML += "<tr><td style=\"border-right: 1px solid #B5B5B5; font-family: 'Courier New';\">&nbsp;1024&nbsp;</td><td style=\"font-family: 'Courier New'; width: 100%; padding-left: 5px;\">dfsdfsd</td></tr>";
                        }
                        else
                        {
                            HTML += "<tr><td style=\"border-right: 1px solid #B5B5B5; font-family: 'Courier New';\">&nbsp;1&nbsp;</td><td style=\"font-family: 'Courier New'; width: 100%; padding-left: 5px;\">File not found!</td></tr>";
                        }

                    }
                    

                    HTML += "</table>";

                    HTML += "</td></tr></table></td></tr></table>";
                }else
                {
                    HTML = "Something went wrong!";
                }
            }
            else
            {
                HTML = "<table style=\"width: 100%; height: 100%; border: 0px;\" cellpadding=\"0\" cellspacing=\"0\"><tr><td align=\"left\" valign=\"top\" style=\"padding-left: 50px;\"><br /><br /><br /><br />Recent Exceptions:&nbsp;&nbsp;&nbsp;";

                HTML += "<input type=\"button\" value=\"Properties\" onclick=\"if(ZoneSelected >= 0) { contextSwitch('/scriptexceptions?exid=' + ZoneSelected); }\" />";
                HTML += "<input type=\"button\" value=\"Clear All\" onclick=\"if(confirm('Are you sure you want to remove all entries?')) { contextSwitch('/scriptexceptions?clearall=a'); }\" />";

                HTML += "<br />";
                HTML += "<div id=\"tableContainer\" class=\"tableContainerLG\"><table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" class=\"scrollTable\"><thead class=\"fixedHeader\"><tr><th style=\"width: 90px;\"><a href=\"#\">Time</a></th><th style=\"width: 120px;\"><a href=\"#\">Server</a></th><th style=\"width: 120px;\"><a href=\"#\">Actor</a></th><th style=\"width: 120px;\"><a href=\"#\">File</a></th><th style=\"width: 40px;\"><a href=\"#\">Line</a></th><th style=\"width: 310px;\"><a href=\"#\">Message</a></th></tr></thead><tbody id=\"tableContainer_scrollContent\" class=\"scrollContentLG\">";

                int Idx = 0;
                foreach (ExceptionInstance Ex in Instances)
                {
                    string FilePath = System.IO.Path.GetFileName(Ex.FileName);

                    HTML += string.Format("<tr class=\"onlineRow\" id=\"d{0}_r{1}\"><td onclick=\"SelectRow({0}, {1});\" style=\"width: 90px;\">{2}</td><td onclick=\"SelectRow({0}, {1});\" style=\"width: 120px;\">{3}</td><td onclick=\"SelectRow({0}, {1});\" id=\"d{0}_{2}\" style=\"width: 120px;\">{4}</td><td onclick=\"SelectRow({0}, {1});\" style=\"width: 120px;\">{5}</td><td onclick=\"SelectRow({0}, {1});\" style=\"width: 40px;\">{6}</td><td onclick=\"SelectRow({0}, {1});\" style=\"width: 310px;\">{7}</td></tr>",
                        new object[] { "3", Idx.ToString(), Ex.Time.ToString("MM-dd HH:mm"),
                        (Ex.HostName.Length > 15) ? Ex.HostName.Substring(0, 15) + "..." : Ex.HostName,
                        (Ex.ActorName.Length > 15) ? Ex.ActorName.Substring(0, 15) + "..." : Ex.ActorName,
                        (FilePath.Length > 15) ? FilePath.Substring(0, 15) + "..." : FilePath,
                        Ex.LineNumber,
                        (Ex.ExceptionMessage.Length > 36) ? Ex.ExceptionMessage.Substring(0, 36) + "..." : Ex.ExceptionMessage
                    });

                    ++Idx;
                }

                HTML += "</tbody></table></div></td></tr></table><br/><br/><script language=\"javascript\"></script>";
            }

            StringBuilder Packet = new StringBuilder();
            Packet.Append("HTTP/1.1 200 OK\r\n");
            Packet.Append("Server: RCP/2.50\r\n");
            Packet.Append("Content-Type: text/html\r\n");
            Packet.Append("Content-Length: " + HTML.Length.ToString() + "\r\n");
            Packet.Append("\r\n");
            Packet.Append(HTML);

            return ASCIIEncoding.ASCII.GetBytes(Packet.ToString());
        }
    }
}
