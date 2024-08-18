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
using RealmCrafter.ClientZone;

namespace RealmCrafter_GE
{
    public partial class TreeCollectionEditor : Form//, System.Drawing.Design.UITypeEditor
    {
        List<TreePlacerType> PlacerTrees = null;

        public TreeCollectionEditor()
        {
            InitializeComponent();
        }

        public System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        public bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value is List<TreePlacerType>)
            {
                PlacerTrees = value as List<TreePlacerType>;

                List<TreePlacerType> Old = new List<TreePlacerType>();
                foreach (TreePlacerType T in PlacerTrees)
                {
                    Old.Add(T);
                }


                CollectionList.Items.Clear();
                CollectionList.Items.AddRange(PlacerTrees.ToArray());

                ShowDialog();
                if (this.DialogResult == DialogResult.Cancel)
                {
                    PlacerTrees.Clear();

                    foreach (TreePlacerType T in Old)
                    {
                        PlacerTrees.Add(T);
                    }

                    return PlacerTrees;
                }
                else
                {
                    return PlacerTrees;
                }
            }
            else
            {
                MessageBox.Show("Error: List<TreePlacerType> was not passer to EditValue!");
                return value;
            }

        }

        private void ButtonCancel_Click(object sender, EventArgs e)
        {
            Close();
            this.DialogResult = DialogResult.Cancel;
        }

        private void ButtonOK_Click(object sender, EventArgs e)
        {
            Close();
            this.DialogResult = DialogResult.OK;
        }

        private void ButtonAdd_Click(object sender, EventArgs e)
        {
            TreeCollectionEditorList ListAdd = new TreeCollectionEditorList();
            ListAdd.Setup(PlacerTrees);
            ListAdd.ShowDialog();
            PlacerTrees.AddRange(ListAdd.AddedPlacers.ToArray());
            CollectionList.Items.AddRange(ListAdd.AddedPlacers.ToArray());
        }

        private void ButtonRemove_Click(object sender, EventArgs e)
        {
            object O = CollectionList.SelectedItem;
            if (!(O is TreePlacerType))
                return;

            CollectionList.Items.Remove(O);
            PlacerTrees.Remove(O as TreePlacerType);
            Properties.SelectedObject = null;
        }

        private void CollectionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            object O = CollectionList.SelectedItem;
            if (!(O is TreePlacerType))
                return;

            Properties.SelectedObject = O;
        }
    }

    public class TreeCollectionTypeEditor : System.Drawing.Design.UITypeEditor
    {
        TreeCollectionEditor Editor = new TreeCollectionEditor();

        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return Editor.GetEditStyle(context);
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return Editor.GetPaintValueSupported(context);
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            return Editor.EditValue(context, provider, value);
        }
    }

    public class TreePlacerAreaFinalizer : System.Drawing.Design.UITypeEditor
    {
        public override System.Drawing.Design.UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return System.Drawing.Design.UITypeEditorEditStyle.Modal;
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (!(value is TreePlacerArea))
            {
                MessageBox.Show("Error: value is not TreePlacerArea in EditValue!");
                return value;
            }

            if (MessageBox.Show("Finalizing this tree placer will fill the marked areas with trees. In doing so, you will no longer be able to edit this tree placer; are you sure you want to continue?", "Tree Finalizer", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                (value as TreePlacerArea).BuildFinal();
            }

            return null;
        }
    }
}