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
//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.ComponentModel.Design;
//using System.Text;

//namespace RealmCrafter_GE.Property_Interfaces
//{
//    // Associate MyDesigner with this component type using a DesignerAttribute
//    [Designer(typeof(MyDesigner))]
//    public class Component1 : System.ComponentModel.Component
//    {
////    }

//    // This is a designer class which provides designer verb menu commands for 
//    // the associated component. This code is called by the design environment at design-time.
//    internal class MyDesigner : ComponentDesigner
//    {
//        DesignerVerbCollection m_Verbs;

//        // DesignerVerbCollection is overridden from ComponentDesigner
//        public override DesignerVerbCollection Verbs
//        {
//            get
//            {
//                if (m_Verbs == null)
//                {
//                    // Create and initialize the collection of verbs
//                    m_Verbs = new DesignerVerbCollection();

//                    m_Verbs.Add(new DesignerVerb("First Designer Verb", new EventHandler(OnFirstItemSelected)));
//                    m_Verbs.Add(new DesignerVerb("Second Designer Verb", new EventHandler(OnSecondItemSelected)));
//                }
//                return m_Verbs;
//            }
//        }

//        MyDesigner()
//        {
//        }

//        private void OnFirstItemSelected(object sender, EventArgs args)
//        {
//            // Display a message
//            System.Windows.Forms.MessageBox.Show("The first designer verb was invoked.");
//        }

//        private void OnSecondItemSelected(object sender, EventArgs args)
//        {
//            // Display a message
//            System.Windows.Forms.MessageBox.Show("The second designer verb was invoked.");
//        }
//    }
//}
