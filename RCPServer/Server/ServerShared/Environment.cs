using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;

namespace RCPServer
{
    public class WorldEnvironment : Scripting.WorldEnvironment
    {
        public static int TimeH = 0;
        public static int TimeM = 0;
        public static int TimeFactor = 0;
        public static uint TimeUpdate = 0;

        public static void Load(string path)
        {
	        BBBinaryReader F = new BBBinaryReader(File.OpenRead(path));

	        int envYear = F.ReadInt32();
	        envDay = F.ReadInt32();
	        TimeH = F.ReadInt32();
	        TimeM = F.ReadInt32();
	        TimeFactor = F.ReadInt32();
      
            F.Close();

	        TimeUpdate = Server.MilliSecs();   
        }


        public static void Update()
        {
            // Advance by one minute
	        if(Server.MilliSecs() - TimeUpdate > 60000 / TimeFactor)
            {
		        TimeUpdate = Server.MilliSecs();
		        ++TimeM;

		        if(TimeM > 59)
                {
			        ++TimeH;
			        TimeM = 0;

			        if(TimeH > 23)
                    {
				        TimeH = 0;
				        ++envDay;
			        }
		        }
	        } // Advance by one

            envHour = TimeH;
            envMinute = TimeM;
            envTimeFactor = TimeFactor;

        }

    }
}
