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
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;

namespace RealmCrafter
{
    public partial class LightFunctionList : Form
    {
        public const string FunctionsXMLPath = @"Data\Game Data\LightFunctions.xml";

        public LightFunctionList()
        {
            InitializeComponent();
        }

        private void LightFunctionList_Load(object sender, EventArgs e)
        {
            foreach (LightFunction F in Functions)
            {
                FunctionsList.Items.Add(F);
            }
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        public static List<LightFunction> Functions = new List<LightFunction>();

        public static void SaveFunctions()
        {
            try
            {
                // Write
                XmlTextWriter X = new XmlTextWriter(FunctionsXMLPath, Encoding.ASCII);
                X.Formatting = Formatting.Indented;

                // Base parts
                X.WriteStartDocument();
                X.WriteStartElement("functions");

                foreach (LightFunction F in Functions)
                {
                    F.Save(X);
                }

                X.WriteEndElement();
                X.WriteEndDocument();
                X.Close();
            }
            catch (Exception e)
            {

            }
        }

        public static void LoadFunctions()
        {
            // Reader
            XmlTextReader X = null;
            Functions.Clear();

            try
            {
                // Open file
                X = new XmlTextReader(FunctionsXMLPath);

                // Read next element
                while (X.Read())
                {
                    // If its a <function> element
                    if (X.NodeType == XmlNodeType.Element && X.Name.ToLower() == "function")
                    {
                        LightFunction Function = new LightFunction();
                        Function.Name = X.GetAttribute("name");

                        if (!X.IsEmptyElement)
                            Function.Compile(X);
                        Functions.Add(Function);
                    }
                }
            }
            catch (System.IO.FileNotFoundException e)
            {
                return;
            }
            //catch (Exception e)
            //{
            //    // Eek an error
            //    MessageBox.Show("Error: " + e.Message, "LoadShaders", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    Application.Exit();
            //}
            finally
            {
                // Cleanup
                X.Close();
            }
        }

        public static void Update()
        {
            foreach (LightFunction F in Functions)
            {
                //F.Update(Environment3D.TimeH, Environment3D.TimeM, 0);
                //F.Update(0, 0, 0);
                F.Update(Environment.TimeH, Environment.TimeM, 0);
            }
        }

        private void NewButton_Click(object sender, EventArgs e)
        {
            LightFunctionNamePrompt Prompt = new LightFunctionNamePrompt();
            Prompt.Data = "";

            if (Prompt.ShowDialog() == DialogResult.Cancel)
                return;

            if (Prompt.Text.Length == 0)
                return;

            LightFunction F = new LightFunction();
            F.Name = Prompt.Data;
            Functions.Add(F);
            FunctionsList.Items.Add(F);

            SaveFunctions();
        }

        private void DeleteButton_Click(object sender, EventArgs e)
        {
            if (FunctionsList.SelectedItem == null || !(FunctionsList.SelectedItem is LightFunction))
                return;

            LightFunction Del = FunctionsList.SelectedItem as LightFunction;
            Functions.Remove(Del);
            FunctionsList.Items.Remove(Del);

            SaveFunctions();
        }

        private void FunctionsList_DoubleClick(object sender, EventArgs e)
        {
            if (FunctionsList.SelectedItem == null || !(FunctionsList.SelectedItem is LightFunction))
                return;

            LightFunction Function = FunctionsList.SelectedItem as LightFunction;
            LightFunctionDialog D = new LightFunctionDialog();
            D.LightFunction = Function.Clone() as LightFunction;

            if (D.ShowDialog() == DialogResult.Cancel)
                return;

            Function.ResetFrom(D.LightFunction);
            SaveFunctions();
        }
    }
}