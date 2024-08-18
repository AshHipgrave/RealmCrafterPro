using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Scripting;

namespace RCPServer
{
    public class Attribute
    {
        #region Members
        string name;
        bool isSkill;
        bool isHidden;
        #endregion

        #region Properties
        public string Name
        {
            get { return name; }
        }

        public bool IsSkill
        {
            get { return isSkill; }
        }

        public bool IsHidden
        {
            get { return isHidden; }
        }
        #endregion

        #region Methods
        public Attribute(string setName, bool setIsSkill, bool setIsHidden)
        {
            name = setName;
            isSkill = setIsSkill;
            isHidden = setIsHidden;
        }
        #endregion

        #region Static Members
        public static Attribute[] Attributes = new Attribute[40];
        public static byte AttributeAssignment = 0;
        #endregion

        #region Static Methods
        public static bool Load(string path)
        {
            for (int i = 0; i < Attributes.Length; ++i)
                Attributes[i] = null;

            BBBinaryReader F = new BBBinaryReader(File.OpenRead(path));

            AttributeAssignment = F.ReadByte();

            for (int i = 0; i < Attributes.Length; ++i)
            {
                Attributes[i] = new Attribute(F.ReadBBString(), F.ReadBoolean(), F.ReadBoolean());
            }

            F.Close();
            return true;
        }

        public static int FindAttribute(string name)
        {
            for (int i = 0; i < Attributes.Length; ++i)
                if (Attributes[i].Name.Equals(name, StringComparison.CurrentCultureIgnoreCase))
                    return i;

            return -1;
        }
        #endregion
    }
}
