using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;

namespace RCPServer
{
    public class DamageType
    {
        #region Members
        string type;
        #endregion

        #region Properties
        public string Type
        {
            get { return type; }
        }
        #endregion 

        #region Methods
        public DamageType(string setType)
        {
            type = setType;
        }
        #endregion

        #region Static Members
        public static DamageType[] DamageTypes = new DamageType[20];
        #endregion

        #region Static Methods
        public static bool Load(string path)
        {
            for (int i = 0; i < DamageTypes.Length; ++i)
                DamageTypes[i] = null;

            BBBinaryReader F = new BBBinaryReader(File.OpenRead(path));

            for (int i = 0; i < DamageTypes.Length; ++i)
                DamageTypes[i] = new DamageType(F.ReadBBString());

            F.Close();
            return true;
        }

        public static int FindDamageType(string name)
        {
            for (int i = 0; i < DamageTypes.Length; ++i)
                if (DamageTypes[i].Type.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return i;

            return -1;
        }
        #endregion
    }
}
