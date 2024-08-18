using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ServerShared
{
    public class SecretString
    {
        const string Default = "PlayaGonnaConnectSon";
        const string Filename = "password.txt";
        public static string LoadSecretString()
        {
            if (File.Exists(Filename))
            {
                try
                {
                    using (TextReader reader = new StreamReader(Filename))
                    {
                        return reader.ReadLine();
                    }
                }
                catch
                {

                }

            }

            return Default;
        }
    }
}
