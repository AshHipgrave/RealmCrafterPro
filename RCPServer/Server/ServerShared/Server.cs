using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Scripting;
using System.Xml;

namespace RCPServer
{
    public class Server
    {
        // Flag to keep the server open (or to close it)
        public static bool IsRunning = true;

        public static Random Random = new Random((int)Server.MilliSecs());
        public static object ChatProcessor;
        public static object AccountDatabase;

        public static float SectorSize = 768.0f;

        public static int StartGold = 0;
        public static int StartReputation = 0;
        public static bool ForcePortals = false;
        public static int CombatDelay = 0;
        public static byte CombatFormula = 0;
        public static int WeaponDamage = 0;
        public static int ArmourDamage = 0;
        public static int CombatRatingAdjust = 0;
        public static bool AllowAccountCreation = true;
        public static int MaxAccountChars = 10;


        public static System.Net.IPAddress MasterAddr = System.Net.IPAddress.Loopback;
        public static int MasterPort = 25002;
        public static int MasterConnection = 0;
        public static uint MasterReconnectTime = 0;
        public static bool MasterAck = false;
        public static bool MasterLogAck = false;
        public static uint MasterAckTime = 0;

        public static string AdminUsername = "";
        public static string AdminPassword = "";

        public static System.Net.IPAddress MasterProxyAddr = System.Net.IPAddress.Loopback;
        public static int MasterProxyPort = 25000;
        public static bool IAmMasterProxy = false;

        public static string HostName = "SUnknown";
        public static System.Net.IPAddress PublicAddr = System.Net.IPAddress.Loopback;
        public static int PublicPort = 25000;
        public static System.Net.IPAddress PrivateAddr = System.Net.IPAddress.Loopback;

        public static int PrivatePort = 25000;

        public static bool RequireMemorise = false;
        public static int HealthStat = 65535;
        public static int EnergyStat = 65535;
        public static int BreathStat = 65535;
        public static int ToughnessStat = 65535;
        public static int StrengthStat = 65535;
        public static int SpeedStat = 65535;
        public static float InteractDist = 400.0f;

        public static string LoginMessage = "";

        public static LinkedListNode<Area> UpdateArea;
        public const int WorldUpdateMS = 1000 / 30; //Change here from 30 to 10
        public const int BroadcastMS = 1000 / 5;
        public const int CompleteUpdateMS = 1000;
        public static uint LastWorldUpdate = 0;
        public static uint LastBroadcast = 0;
        public static uint LastCompleteUpdate = 0;
        public static uint LastSpellRecharge = 0;

        public const int UpdateDistance = 250000;
        public const int UpdateFarDistance = 1000000;


        public const int PortalLockTime = 35000;

        // RCEnet handle
        public static int Host = 0;

        protected static List<Regex> NameFilters = new List<Regex>();

        // Try to parse an IP
        private static System.Net.IPAddress Parse(string addr)
        {
            System.Net.IPAddress Out = System.Net.IPAddress.Any;
            if (System.Net.IPAddress.TryParse(addr, out Out))
                return Out;
            return System.Net.IPAddress.Any;
        }

        private static int ToInt32(string port)
        {
            try
            {
                return Convert.ToInt32(port);
            }
            catch (System.Exception ex)
            {
                return 0;
            }
        }

        // Change this to load real data
        public static void LoadClusterInfo(string path)
        {

                XmlTextReader X = new XmlTextReader(path);

                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.Equals("config", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string Name = X.GetAttribute("name");
                        string Value = X.GetAttribute("value");
                        if (Name == null)
                            Name = "";
                        if (Value == null)
                            Value = "";

                        if(Name.Equals("privateaddress", StringComparison.CurrentCultureIgnoreCase))
                            PrivateAddr = Parse(Value);
                        else if(Name.Equals("privateport", StringComparison.CurrentCultureIgnoreCase))
                            PrivatePort = ToInt32(Value);
                        else if(Name.Equals("publicaddress", StringComparison.CurrentCultureIgnoreCase))
                            PublicAddr = Parse(Value);
                        else if(Name.Equals("publicport", StringComparison.CurrentCultureIgnoreCase))
                            PublicPort = ToInt32(Value);
                        else if(Name.Equals("masterproxyaddress", StringComparison.CurrentCultureIgnoreCase))
                            MasterProxyAddr = Parse(Value);
                        else if(Name.Equals("masterproxyport", StringComparison.CurrentCultureIgnoreCase))
                            MasterProxyPort =ToInt32(Value);
                        else if(Name.Equals("masteraddress", StringComparison.CurrentCultureIgnoreCase))
                            MasterAddr = Parse(Value);
                        else if(Name.Equals("masterport", StringComparison.CurrentCultureIgnoreCase))
                            MasterPort = ToInt32(Value);
                        else if(Name.Equals("adminusername", StringComparison.CurrentCultureIgnoreCase))
                            AdminUsername = Value;
                        else if(Name.Equals("adminpassword", StringComparison.CurrentCultureIgnoreCase))
                            AdminPassword = Value;

                    }
                }

                X.Close();
            
            
            HostName = System.Net.Dns.GetHostName();


            if (PublicAddr.Equals(MasterProxyAddr) && PublicPort == MasterProxyPort)
            {
                IAmMasterProxy = true;
                Log.WriteLine("MASTER PROXY", ConsoleColor.Red);
            }
        }

        static uint ClientIndices = 1;
        public static uint AllocateGlobalClientID()
        {
            return ++ClientIndices;
        }

        public static void LoadData()
        {
            BBBinaryReader F = new BBBinaryReader(File.OpenRead("Data/Server Data/Misc.dat"));

            StartGold = F.ReadInt32();
	        StartReputation = F.ReadInt32();
	        ForcePortals = F.ReadBoolean();
	        CombatDelay = (int)(F.ReadUInt16());
	        CombatFormula = F.ReadByte();
	        WeaponDamage = (int)(F.ReadByte());
	        ArmourDamage = (int)(F.ReadByte());
	        CombatRatingAdjust = (int)(F.ReadByte());
	        AllowAccountCreation = F.ReadBoolean();
	        MaxAccountChars = (int)(F.ReadByte());
	        
            //PublicPort = F.ReadInt32();
	        //if(PublicPort == 0)
            //    PublicPort = 25000;

	        RequireMemorise = F.ReadBoolean();
            F.Close();

            F = new BBBinaryReader(File.OpenRead("Data/Server Data/Fixed Attributes.dat"));
            HealthStat = (int)(F.ReadUInt16());
            EnergyStat = (int)(F.ReadUInt16());
            BreathStat = (int)(F.ReadUInt16());
            ToughnessStat = (int)(F.ReadUInt16());
            StrengthStat = (int)(F.ReadUInt16());
            SpeedStat = (int)(F.ReadUInt16());
            F.Close();

            if(HealthStat == 65535)
                throw new Exception("A valid Health attribute must be selected!");
            if(EnergyStat == 65535)
                EnergyStat = -1;
            if(BreathStat == 65535)
                BreathStat = -1;
            if(ToughnessStat == 65535)
                throw new Exception("A valid Toughness attribute must be selected!");
            if(StrengthStat == 65535)
                throw new Exception("A valid Strength attribute must be selected!");
            if (SpeedStat == 65535)
                throw new Exception("A valid Speed attribute must be selected!");

            try
            {
                StreamReader TR = new StreamReader(File.OpenRead("Data/Server Data/Names Filter.txt"));

                while (!TR.EndOfStream)
                {
                    string Txt = TR.ReadLine();
                    if(Txt.Length > 0 && !Txt.StartsWith(";"))
                        NameFilters.Add(new Regex(Txt));
                }

                TR.Close();
            }catch(FileNotFoundException)
            {

            }
        }

        public static bool IsNameValid(string name)
        {
            foreach (Regex R in NameFilters)
            {
                if (R.IsMatch(name))
                    return false;
            }
            return true;
        }

        public static uint MilliSecs()
        {
            return (uint)(DateTime.Now.Ticks / 10000);
        }

        public static float Rnd(float max)
        {
            return Rnd(0.0f, max);
        }

        public static float Rnd(float min, float max)
        {
            float R = (float)Random.NextDouble();
            R *= (max - min);
            R += min;
            return R;
        }
    }
}
