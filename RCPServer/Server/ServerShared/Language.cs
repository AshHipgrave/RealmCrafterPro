using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace RCPServer
{
    public class Language
    {        
        #region Members
        
        #endregion

        #region Properties
        #endregion 

        #region Methods
        #endregion

        #region Static Members
        static List<string> LanguageStrings = new List<string>();
        static string[] LanguageStringNames = new string[]
            {
                "weapon",
		        "armour",
		        "ring",
		        "amulet",
		        "potion",
		        "ingredient",
		        "image",
		        "miscellaneous",
		        "onehanded",
		        "twohanded",
		        "ranged",
		        "xhasjoinedparty",
		        "youhavejoinedparty",
		        "couldnotjoinparty",
		        "partyinvite",
		        "partyinviteinstruction",
		        "couldnotinviteparty",
		        "tradeinvite",
		        "tradeinviteinstruction",
		        "xisoffline",
		        "playernotfound",
		        "playersingame",
		        "playersinzone",
		        "season",
		        "abilitynotrecharged",
		        "ignoring",
		        "unignoring",
		        "skick",
		        "sunignore",
		        "signore",
		        "snetdump",
		        "spet",
		        "sleave",
		        "sok",
		        "sinvite",
		        "sxp",
		        "sgold",
		        "ssetattribute",
		        "ssetattributemax",
		        "sscript",
		        "sme",
		        "syell",
		        "sgm",
		        "sg",
		        "sp",
		        "spm",
		        "strade",
		        "sallplayers",
		        "splayers",
		        "swarp",
		        "swarpother",
		        "sability",
		        "sgive",
		        "sweather",
		        "stime",
		        "sdate",
		        "sseason",

                "criticaldamage",
                "weapondamaged"
            };
        #endregion

        #region Static Methods
        public static string Get(LanguageString id)
        {
            return LanguageStrings[(int)id];
        }

        public static bool Load(string path)
        {
            XmlTextReader X = null;

            X = new XmlTextReader(File.OpenRead(path));

            LanguageStrings.Clear();
            foreach (string S in LanguageStringNames)
                LanguageStrings.Add(S);

            while (X.Read())
            {
                if (X.NodeType == XmlNodeType.Element && X.Name.Equals("entry", StringComparison.CurrentCultureIgnoreCase))
                {
                    for (int i = 0; i < LanguageStringNames.Length; ++i)
                        if (LanguageStringNames[i].Equals(X.GetAttribute("name"), StringComparison.CurrentCultureIgnoreCase))
                            LanguageStrings[i] = X.GetAttribute("value");
                }
            }

            X.Close();
            
            return true;
        }
        #endregion
    }
}
