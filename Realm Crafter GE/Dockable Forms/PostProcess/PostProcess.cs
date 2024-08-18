//##############################################################################################################################
// Realm Crafter Professional																									
// Copyright (C) 2013 Solstar Games, LLC. All rights reserved																	
// contact@solstargames.com																																																		
//
// Grand Poohbah: Mark Bryant
// Programmer: Jared Belkus
// Programmer: Frank Puig Placeres
// Programmer: Rob Williams
// 																										
// Program: 
//																																
//This is a licensed product:
//BY USING THIS SOURCECODE, YOU ARE CONFIRMING YOUR ACCEPTANCE OF THE SOFTWARE AND AGREEING TO BECOME BOUND BY THE TERMS OF 
//THIS AGREEMENT. IF YOU DO NOT AGREE TO BE BOUND BY THESE TERMS, THEN DO NOT USE THE SOFTWARE.
//																		
//Licensee may NOT: 
// (i)   create any derivative works of the Engine, including translations Or localizations, other than Games;
// (ii)  redistribute, encumber, sell, rent, lease, sublicense, Or otherwise transfer rights To the Engine// or
// (iii) remove Or alter any trademark, logo, copyright Or other proprietary notices, legends, symbols Or labels in the Engine.
// (iv)   licensee may Not distribute the source code Or documentation To the engine in any manner, unless recipient also has a 
//       license To the Engine.													
// (v)  use the Software to develop any software or other technology having the same primary function as the Software, 
//       including but not limited to using the Software in any development or test procedure that seeks to develop like 
//       software or other technology, or to determine if such software or other technology performs in a similar manner as the
//       Software																																
//##############################################################################################################################

namespace RealmCrafter_GE.PostProcess
{
    using System;
    using System.Collections.Generic;
    using System.Xml;

    class cParam
    {
        public string Name;
        public string Type, Value;

        public void Load(XmlTextReader X)
        {
            Name    = X.GetAttribute("name"); 
            Type    = X.GetAttribute("type");
            Value   = X.GetAttribute("value");
        }
        public void Save(XmlTextWriter X)
        {
            X.WriteStartElement("Param");

            X.WriteStartAttribute("name");
            X.WriteString(Name);
            X.WriteEndAttribute();

            X.WriteStartAttribute("type");
            X.WriteString(Type);
            X.WriteEndAttribute();

            X.WriteStartAttribute("value");
            X.WriteString(Value);
            X.WriteEndAttribute();

            X.WriteEndElement();
        }
    }

    class cEffect
    {
        public string Name, Shader;
        public List<cParam> lParams;

        public cEffect()
        {
            lParams = new List<cParam>();
        }

        public void Load(XmlTextReader X)
        {
            Name = X.GetAttribute("name");
            Shader = X.GetAttribute("shader");
            while (X.Read() && X.NodeType != XmlNodeType.EndElement)
                if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "param")
                {
                    cParam pParam = new cParam();
                    pParam.Load(X);
                    lParams.Add(pParam);
                }
        }
        public void Save(XmlTextWriter X)
        {
            X.WriteStartElement("Effect");
            X.WriteStartAttribute("name");
            X.WriteString(Name);
            X.WriteEndAttribute();
            X.WriteStartAttribute("shader");
            X.WriteString(Shader);
            X.WriteEndAttribute();

            foreach (cParam param in lParams) param.Save(X);

            X.WriteFullEndElement();
        }
    }

    class cPP_Effect
    {
        public string Name;
        public List<cEffect> lEffects;

        public cPP_Effect()
        {
            lEffects = new List<cEffect>();
        }

        public void AddEffect(cEffect e) { lEffects.Add(e); }
        public void DeleteEffect(int iEffect) { lEffects.RemoveAt(iEffect); }
        public void SwapEffects(int indexA, int indexB)
        {
            cEffect e = lEffects[indexA];
            lEffects[indexA] = lEffects[indexB];
            lEffects[indexB] = e;
        }

        public void Load(XmlTextReader X, List<cEffect> lBasicEffect)
        {
            Name = X.GetAttribute("name");
            while (X.Read() && X.NodeType != XmlNodeType.EndElement)
                if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "effect")
                {
                    cEffect pEffect = new cEffect();
                    pEffect.Load(X);
                    lEffects.Add(pEffect);
                }
        }
        public void Save(XmlTextWriter X)
        {
            X.WriteStartElement("PostProcess");
            X.WriteStartAttribute("name");
            X.WriteString(Name);
            X.WriteEndAttribute();

            foreach (cEffect e in lEffects) e.Save(X);

            X.WriteEndElement();
        }
    }
};