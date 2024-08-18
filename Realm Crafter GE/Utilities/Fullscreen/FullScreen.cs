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
using System.Drawing;
using System.Windows.Forms;

namespace RealmCrafter_GE.Utilities.Fullscreen
{
	public class FullScreen
	{
		private Form                _Form;
		private FormWindowState     _cWindowState;
		private FormBorderStyle     _cBorderStyle;
		private Rectangle           _cBounds;
		private bool                _FullScreen;

		public FullScreen(Form form)
		{
			this._Form           = form;            
			this._FullScreen     = false;
		}

		/// <summary>
		/// Show or Hide WinForm in full screen mode.
		/// </summary>
		private void ScreenMode()
		{
			if (!this._FullScreen)
			{
				this._cBorderStyle               = this._Form.FormBorderStyle;
				this._cBounds	                = this._Form.Bounds;
				this._cWindowState               = this._Form.WindowState;

				this._Form.Visible			    = false;

				HandleTaskBar.hideTaskBar();
				
				this._Form.FormBorderStyle		= FormBorderStyle.None;
				this._Form.WindowState			= FormWindowState.Maximized;
				
				this._Form.Visible				= true;
				this._FullScreen                 = true;
			}
			else 
			{
				this._Form.Visible               = false;
				this._Form.WindowState           = this._cWindowState;
				this._Form.FormBorderStyle       = this._cBorderStyle;
				this._Form.Bounds                = this._cBounds;

				HandleTaskBar.showTaskBar();

				this._Form.Visible               = true;

				this._FullScreen                 = false;
			}
		}

		/// <summary>
		/// Show or hide full screen mode
		/// </summary>
		public void ShowFullScreen()
		{
			this.ScreenMode();
		}
		/// <summary>
		/// Reset taskbar visibility
		/// </summary>
		public void ResetTaskBar()
		{
			HandleTaskBar.showTaskBar();
		}
	}
}