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
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class ToolListHeading : UserControl
    {
        protected static Image ToolMinus = null, ToolPlus = null;
        protected bool _IsOpen = true;

        public ToolListHeading()
        {
            InitializeComponent();

            if (ToolMinus == null)
                ToolMinus = Bitmap.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("RealmCrafter_GE.Resources.Toolminus.png"));
            
            if (ToolPlus == null)
                ToolPlus = Bitmap.FromStream(Assembly.GetExecutingAssembly().GetManifestResourceStream("RealmCrafter_GE.Resources.Toolplus.png"));

            //ToolListItem Item = new ToolListItem(this);
            //Item.Dock = DockStyle.Bottom;
            //Item.Location = new Point(0, 0);
            //Item.Size = new Size(100, 20);
            //Item.Text = "woo wee";

            //this.Controls.Add(Item);

            this.Height = 19 + Controls.Count * 21;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Rectangle BorderRectangle = new Rectangle(2, 0, e.ClipRectangle.Width - 4, Height - 1);
            Rectangle BackRectangle = new Rectangle(2, 0, e.ClipRectangle.Width - 3, Height);

            //Rectangle BorderRectangle = new Rectangle(e.ClipRectangle.X + 2, e.ClipRectangle.Y, e.ClipRectangle.Width - 4, e.ClipRectangle.Height - 1);
            //Rectangle BackRectangle = new Rectangle(e.ClipRectangle.X + 2, e.ClipRectangle.Y, e.ClipRectangle.Width - 3, e.ClipRectangle.Height);

            e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), BackRectangle);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(225, 225, 225)), BackRectangle.X, 17, BackRectangle.Width, 17);
            e.Graphics.DrawLine(new Pen(Color.FromArgb(225, 225, 225)), BackRectangle.X, 0, BackRectangle.Width, 0);

            // 7, 5
            if(_IsOpen)
                e.Graphics.DrawImage(ToolMinus, new Point(7, 5));
            else
                e.Graphics.DrawImage(ToolPlus, new Point(7, 5));

            // 22
            SizeF StringSize = e.Graphics.MeasureString(this.Text, this.Font);
            e.Graphics.DrawString(this.Text, this.Font, Brushes.Black, new Point(22, (19 / 2) - ((int)StringSize.Height / 2)));


            //e.Graphics.FillRectangle(Brushes.Green, e.ClipRectangle);
        }

        protected override void OnClick(EventArgs e)
        {
            _IsOpen = !_IsOpen;
            Invalidate();

            foreach (Control C in Controls)
            {
                if (C is ToolListItem)
                {
                    //if ((C as ToolListItem).Heading == sender as ToolListHeading)
                    {
                        C.Visible = !C.Visible;
                    }
                }
            }

            if (_IsOpen)
                this.Height = 19 + Controls.Count * 20;
            else
                this.Height = 19;

            base.OnClick(e);
        }
    }
}
