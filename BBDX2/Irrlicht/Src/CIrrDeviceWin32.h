// Copyright (C) 2002-2006 Nikolaus Gebhardt
// This file is part of the "Irrlicht Engine".
// For conditions of distribution and use, see copyright notice in irrlicht.h

#ifndef __C_IRR_DEVICE_WIN32_H_INCLUDED__
#define __C_IRR_DEVICE_WIN32_H_INCLUDED__

#include "IrrCompileConfig.h"
#ifdef _IRR_WINDOWS_

#include "CIrrDeviceStub.h"
#include "IrrlichtDevice.h"
#include "IImagePresenter.h"

#define WIN32_LEAN_AND_MEAN
#include <windows.h>

namespace irr
{

	class CIrrDeviceWin32 : public CIrrDeviceStub, video::IImagePresenter
	{
	public:

		//! constructor
		CIrrDeviceWin32(video::E_DRIVER_TYPE deviceType, 
			const core::dimension2d<s32> windowSize, u32 bits,
			bool fullscreen, bool stencilbuffer, bool vsync, 
			int antiAlias, bool highPrecisionFPU,
			IEventReceiver* receiver,
			HWND window,
			const char* version);

		//! destructor
		virtual ~CIrrDeviceWin32();

		//! runs the device. Returns false if device wants to be deleted
		virtual bool run();

		//! sets the caption of the window
		virtual void setWindowCaption(const wchar_t* text);

		//! returns if window is active. if not, nothing need to be drawn
		virtual bool isWindowActive();

		//! presents a surface in the client area
		virtual void present(video::IImage* surface, s32 windowId = 0, core::rect<s32>* src=0 );

		//! notifies the device that it should close itself
		virtual void closeDevice();

		//! \return Returns a pointer to a list with all video modes supported
		//! by the gfx adapter.
		video::IVideoModeList* getVideoModeList();

		//! Notifies the device, that it has been resized
		void OnResized();

		//! Sets if the window should be resizeable in windowed mode.
		virtual void setResizeAble(bool resize=false);

	private:



		//! create the driver
		void createDriver(video::E_DRIVER_TYPE driverType,
			const core::dimension2d<s32>& windowSize, u32 bits, bool fullscreen,
			bool stencilbuffer, bool vsync, int antiAlias, bool highPrecisionFPU);

		//! switchs to fullscreen
		bool switchToFullScreen(s32 width, s32 height, s32 bits);

		void getWindowsVersion(core::stringc& version);

		void resizeIfNecessary();

		HWND HWnd;

		bool ChangedToFullScreen;
		bool FullScreen;
		bool IsNonNTWindows;
		bool Resized;
		bool ExternalWindow;
	};


} // end namespace irr

#endif
#endif

