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

namespace RealmCrafter_GE.Dockable_Forms.ScriptEditor
{
    public partial class ToolListItem : UserControl
    {
        ToolListHeading _Heading = null;
        Image _Icon = null;
        EditorControl _Command;

        public ToolListItem(ToolListHeading heading, Image icon, EditorControl command)
        {
            _Icon = icon;
            _Heading = heading;
            _Command = command;
            InitializeComponent();
        }

        public ToolListHeading Heading
        {
            get { return _Heading; }
            set { _Heading = value; }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Point M = PointToClient(Cursor.Position);

            Rectangle BorderRectangle = new Rectangle(5, 0, e.ClipRectangle.Width - 7, Height - 1);
            Rectangle BackRectangle = new Rectangle(5, 0, e.ClipRectangle.Width - 6, Height);

            //Rectangle BorderRectangle = new Rectangle(e.ClipRectangle.X + 5, e.ClipRectangle.Y, e.ClipRectangle.Width - 7, e.ClipRectangle.Height - 1);
            //Rectangle BackRectangle = new Rectangle(e.ClipRectangle.X + 5, e.ClipRectangle.Y, e.ClipRectangle.Width - 6, e.ClipRectangle.Height);

            if (e.ClipRectangle.Contains(M))
            {
                e.Graphics.FillRectangle(new System.Drawing.SolidBrush(Color.FromArgb(192, 221, 252)), BackRectangle);
                e.Graphics.DrawRectangle(new Pen(Color.FromArgb(51, 163, 255)), BorderRectangle);
            }
            else
            {
                e.Graphics.FillRectangle(new SolidBrush(Color.FromArgb(225, 225, 225)), BackRectangle);
            }

            SizeF StringSize = e.Graphics.MeasureString(this.Text, this.Font);

            e.Graphics.DrawString(this.Text, this.Font, Brushes.Black, new Point(29, (Height / 2) - ((int)StringSize.Height / 2)));

            if (_Icon != null)
                e.Graphics.DrawImage(_Icon, 8, 2, 16, 16);

        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);

            this.DoDragDrop(_Command, DragDropEffects.All);
        }

        protected override void OnMouseEnter(EventArgs e)
        {
            Invalidate();
            base.OnMouseEnter(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            Invalidate();
            base.OnMouseLeave(e);
        }
    }
}
