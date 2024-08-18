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
//	Realmcrafter postprocess window interface
//	Author: Yeisnier Dominguez Silva, March 2009

namespace RealmCrafter_GE.PostProcess
{
    using System;
    using System.Drawing;
    using System.IO;
    using System.Text;
    using System.Windows.Forms;
    using ScintillaNet;
    using WeifenLuo.WinFormsUI.Docking;
    using System.Collections.Generic;
    using System.Xml;
    using RealmCrafter_GE.Property_Interfaces;
    using RealmCrafter_GE.Dockable_Forms.PostProcess.ParamTypes;
    using RenderingServices;
    using System.ComponentModel;
         
    public partial class PostProcessWindows : DockContent
    {
        List<cEffect> lEffects;
        List<cPP_Effect> lUserPP_Effects, lPrePP_Effects;
        int activePP_Effect;

        public PostProcessWindows()
        {
            InitializeComponent();

            activePP_Effect = -1;
            LoadBasicEffects(@"Data\Game Data\PostProcess\Effects.xml");
            LoadPP_Effects(@"Data\Game Data\PostProcess\PrePostProcess.xml", ref lPrePP_Effects);
            LoadPP_Effects(@"Data\Game Data\PostProcess\PostProcess.xml", ref lUserPP_Effects);

            if ( lUserPP_Effects.Count == 0 )
            {
                cPP_Effect pPP_Effect = new cPP_Effect();
                pPP_Effect.Name = "Standard";
                lUserPP_Effects.Add(pPP_Effect);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        void LoadBasicEffects( string name )
        {
            lEffects = new List<cEffect>();

            // Reader
            XmlTextReader X = null;
            try
            {
                // Open file
                X = new XmlTextReader(name);

                // Read next element
                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "effect")
                    {
                        cEffect e = new cEffect();
                        e.Load(X);
                        lEffects.Add(e);
                    }
                }
            }
            catch (Exception except)
            {
                // Eek an error
                MessageBox.Show("Error: " + except.Message, "PostProcess:LoadBasicEffects", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                // Cleanup
                X.Close();
            }
        }
        void LoadPP_Effects( string name, ref List<cPP_Effect> lPP_Effects )
        {
            lPP_Effects = new List<cPP_Effect>();

            // Reader
            XmlTextReader X = null;
            try
            {
                // Open file
                X = new XmlTextReader(name);

                // Read next element
                while (X.Read())
                {
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "postprocess")
                    {
                        cPP_Effect pPP_Effect = new cPP_Effect();
                        pPP_Effect.Load(X, lEffects);
                        lPP_Effects.Add(pPP_Effect);
                    }
                }
            }
            catch (Exception except)
            {
                // Eek an error
                MessageBox.Show("Error: " + except.Message, "PostProcess:LoadUserPP_Effects", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                // Cleanup
                X.Close();
            }
        }
        public void SaveUserPP_Effects(string name)
        {
            XmlTextWriter X = null;
            try
            {
                // Create file
                X = new XmlTextWriter(name, Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                // Base parts
                X.WriteStartDocument();
                X.WriteStartElement("UserDefined_PP");
                foreach (cPP_Effect p in lUserPP_Effects) p.Save(X);
                X.WriteEndElement();
                X.WriteEndDocument();
            }
            catch (Exception except)
            {
                // Eek an error
                MessageBox.Show("Error: " + except.Message, "cPostProcess:SaveUserPP_Effects", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }
            finally
            {
                X.Flush();
                X.Close();
            }
        }
 
        private void PostProcessWindows_Load(object sender, EventArgs e)
        {
            lbxEffects.Items.Clear();
            foreach (cEffect ef in lEffects) lbxEffects.Items.Add(ef.Name);

            cbPrePP.Items.Clear();
            foreach (cPP_Effect pp in lPrePP_Effects) cbPrePP.Items.Add(pp.Name);
            if (lPrePP_Effects.Count > 0) cbPrePP.SelectedIndex = 0;

            cbUserPP.Items.Clear();
            foreach (cPP_Effect pp in lUserPP_Effects) cbUserPP.Items.Add(pp.Name);
            if (lUserPP_Effects.Count > 0) cbUserPP.SelectedIndex = 0;
        }

        cEffect FindEffect(string Name)
        {
            foreach (cEffect ef in lEffects) 
                if (ef.Name == Name) return ef;

            return null;
        }
        private void lbxEffects_DoubleClick(object sender, EventArgs e)
        {
            int i = lbxEffects.SelectedIndex;
            if (i == -1) return;

            cEffect pEffect = FindEffect( lbxEffects.Items[i].ToString() );
            if (pEffect != null) AddEffect(pEffect, true);
        }

        private void btMoveUp_Click(object sender, EventArgs e)
        {
            int i = lbxActiveEffects.SelectedIndex;
            if (i <= 0) return;

            lbxActiveEffects.SelectedIndex = i - 1;
            RenderWrapper.SwapPP_Effect(i, i - 1);

            cPP_Effect pPP_Effect = lUserPP_Effects[activePP_Effect];
            cEffect ef = pPP_Effect.lEffects[i];
            pPP_Effect.lEffects[i] = pPP_Effect.lEffects[i - 1];
            pPP_Effect.lEffects[i - 1] = ef;

            object item = lbxActiveEffects.Items[i];
            lbxActiveEffects.Items[i] = lbxActiveEffects.Items[i - 1];
            lbxActiveEffects.Items[i - 1] = item;

        }
        private void btMoveDown_Click(object sender, EventArgs e)
        {
            int i = lbxActiveEffects.SelectedIndex;
            if (i == -1 || i == lbxActiveEffects.Items.Count - 1) return;

            lbxActiveEffects.SelectedIndex = i + 1;
            RenderWrapper.SwapPP_Effect(i, i + 1);

            cPP_Effect pPP_Effect = lUserPP_Effects[activePP_Effect];
            cEffect ef = pPP_Effect.lEffects[i];
            pPP_Effect.lEffects[i] = pPP_Effect.lEffects[i + 1];
            pPP_Effect.lEffects[i + 1] = ef;

            object item = lbxActiveEffects.Items[i];
            lbxActiveEffects.Items[i] = lbxActiveEffects.Items[i + 1];
            lbxActiveEffects.Items[i + 1] = item;
        }
        private void btEffectDelete_Click(object sender, EventArgs e)
        {
            int i = lbxActiveEffects.SelectedIndex;
            if (i == -1) return;

            RenderWrapper.DeletePP_Effect(i);

            lUserPP_Effects[activePP_Effect].lEffects.RemoveAt(i);
            lbxActiveEffects.Items.RemoveAt(i);

            if ( i < lbxActiveEffects.Items.Count && lbxActiveEffects.Items.Count > 0 ) 
                lbxActiveEffects.SelectedIndex = i;
        }

        void AddEffect(cEffect pEffect, bool bAddUserPP)
        {
            RenderWrapper.AddPP_Effect(pEffect.Name, pEffect.Shader);
            foreach (cParam p in pEffect.lParams)
                RenderWrapper.AddEffect_Param(pEffect.Name, p.Name, p.Type, p.Value);

            if ( bAddUserPP ) lUserPP_Effects[activePP_Effect].lEffects.Add(pEffect);
            lbxActiveEffects.Items.Add(pEffect.Name);
        }

        private void btApply_Click(object sender, EventArgs e)
        {
            if (cbPrePP.SelectedIndex == -1) return;

            int i = lbxActiveEffects.SelectedIndex;
            cPP_Effect pPP_Effect = lPrePP_Effects[cbPrePP.SelectedIndex];
            foreach (cEffect ef in pPP_Effect.lEffects) AddEffect(ef, true);
            lbxActiveEffects.SelectedIndex = i;
        }

        private void cbPrePP_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.KeyChar = (char)0;
        }

        void OnParamChange(string value, string paramName)
        {
            int i = lbxActiveEffects.SelectedIndex;
            if ( i == -1) return;

            cPP_Effect pPP_Effect = lUserPP_Effects[activePP_Effect];
            foreach (cParam p in pPP_Effect.lEffects[i].lParams)
                if (p.Name == paramName)
                {
                    p.Value = value;
                    RenderWrapper.SetEffect_Param(pPP_Effect.lEffects[i].Name, paramName, value);
                }
        }
        private void lbxActiveEffects_SelectedIndexChanged(object sender, EventArgs e)
        {
            gbParam.Controls.Clear();
            int i = lbxActiveEffects.SelectedIndex;
            if ( i == -1) return;

            cPP_Effect pPP_Effect = lUserPP_Effects[activePP_Effect];
            if (pPP_Effect.lEffects[i].lParams.Count > 0)
            {
                foreach (cParam p in pPP_Effect.lEffects[i].lParams)
                {
                    if (p.Type == "int")
                        new cParam_Int(this.gbParam, p.Name, p.Value, new OnParamChangeCallback(this.OnParamChange));
                    else
                        if (p.Type == "float")
                            new cParam_Float(this.gbParam, p.Name, p.Value, new OnParamChangeCallback(this.OnParamChange));
                        else
                            if (p.Type == "float4")
                                new cParam_Float4(this.gbParam, p.Name, p.Value, new OnParamChangeCallback(this.OnParamChange));
                }
            }

        }

        private void btNew_Click(object sender, EventArgs e)
        {
            cPP_Effect pPP_Effect = new cPP_Effect();
            pPP_Effect.Name = "UnNamed";
            lbxActiveEffects.Items.Clear();

            bool bExistPP_Name = false;
            int i = 1;
            do
            {
                bExistPP_Name = false;
                foreach (cPP_Effect p in lUserPP_Effects)
                    if (p.Name == pPP_Effect.Name) { bExistPP_Name = true; break; }

                if (bExistPP_Name)
                {
                    pPP_Effect.Name = "UnNamed" + i.ToString();
                    i++;
                }
            } while (bExistPP_Name);
            lUserPP_Effects.Add(pPP_Effect);

            cbUserPP.SelectedIndex = cbUserPP.Items.Add(pPP_Effect.Name);
        }
        private void cbUserPP_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbUserPP.SelectedIndex == activePP_Effect ) return;
            RenderWrapper.CleanPP_Pipeline();
            lbxActiveEffects.Items.Clear();

            activePP_Effect = cbUserPP.SelectedIndex;
            foreach (cEffect ef in lUserPP_Effects[activePP_Effect].lEffects) AddEffect(ef, false);
        }
        private void btRename_Click(object sender, EventArgs e)
        {
            if (cbUserPP.Text != lUserPP_Effects[activePP_Effect].Name)
            {
                lUserPP_Effects[activePP_Effect].Name = cbUserPP.Text;
                cbUserPP.Items.Clear();
                foreach (cPP_Effect pp in lUserPP_Effects) cbUserPP.Items.Add(pp.Name);
            }
        }
        private void btDelete_Click(object sender, EventArgs e)
        {
            int i = cbUserPP.SelectedIndex;
            if (i == -1 || cbUserPP.Items.Count <= 1) return;

            lUserPP_Effects.RemoveAt(i);
            cbUserPP.Items.RemoveAt(i);

            activePP_Effect = -1;
            if (i >= cbUserPP.Items.Count) cbUserPP.SelectedIndex = i-1;
            else                          cbUserPP.SelectedIndex = i;
        }

        private void chbxEnable_CheckedChanged(object sender, EventArgs e)
        {
            RenderWrapper.EnablePP_Pipeline(chbxEnable.Checked);
        }
    }
}