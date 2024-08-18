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
#pragma once

#include <IGUITexture.h>
#include <IGUIMeshBuffer.h>
#include <SGUIVertex.h>
#include <IParameter.h>
#include <vector4.h>

namespace NGin
{
	namespace GUI
	{
		class IGUIManager;

		//! GUI Renderer Interface
		class IGUIRenderer
		{
		public:

			virtual ~IGUIRenderer() {}

			//! Prepare for rendering, called before all GUI is drawn.
			virtual bool PreRender() = 0;

			//! Finnish rendering, called after GUI is drawn.
			virtual bool PostRender() = 0;

			//! Set a constructed manager (Added for the d3d11 impl).
			virtual void SetManager(IGUIManager* manager) = 0;

			//! Get the number of triangles drawn in the previous frame.
			virtual unsigned int GetTrianglesDrawn() const = 0;
			
			//! Create a buffer to GUI data
			virtual IGUIMeshBuffer* CreateMeshBuffer(const SGUIVertex* vertices, unsigned int vertexCount, const unsigned short* indices, unsigned int indexCount) = 0;

			//! Free a mesh buffer
			virtual void FreeMeshBuffer(IGUIMeshBuffer* buffer) = 0;

			//! Create a basic texture and pre-fill with data.
			/*!
			* Data must be width*height*4 in size
			*/
			virtual IGUITexture* CreateFontTexture(int width, int height, const char* data) = 0;

			//! 'Grab' a texture.. Used for internal reference counting
			virtual IGUITexture* GetTexture(IGUITexture* handle) = 0;

			//! Get/Load a texture (useful for cache implementations)
			virtual IGUITexture* GetTexture(const char* path, unsigned int mask = 0, bool ismask = false) = 0;

			//! Create a texture using an internal device pointer as the source handle
			virtual IGUITexture* CreateTextureFromBase(void* base, Math::Vector2 &size) = 0;

			//! Free/Drop a texture
			virtual void FreeTexture(IGUITexture* texture) = 0;

			//! Commit buffer data (usually to a GPU)
			virtual bool SetMeshBuffer(const IGUIMeshBuffer* buffer) = 0;

			//! Set a specific texture for the next rendered object
			virtual bool SetTexture(const char* name, const IGUITexture* texture) = 0;

			//! Draw a mesh buffer
			virtual void DrawMeshBuffer(const IGUIMeshBuffer* buffer) = 0;

			//! Draw a mesh buffer, but specify which primitives to draw.
			virtual void DrawMeshBuffer(const IGUIMeshBuffer* buffer, int triangleOffset, int triangleCount) = 0;

			//! Set a shader or rendering parameter
			virtual bool SetParameter(const char* name, const IParameter &parameter) = 0;

			//! Get the current scissor region
			virtual Math::Vector4 GetScissorRect() = 0;

			//! Push a scissor region
			virtual void PushScissorRect(Math::Vector4 &rect) = 0;

			//! Pop the topmost region
			virtual void PopScissorRect() = 0;

			//! Release internal data in renderer instances where a lost device is possible (D3D9!)
			virtual void OnDeviceLost() = 0;

			//! Restore internal data (see OnDeviceLost)
			virtual void OnDeviceReset() = 0;

			//! Output a debug text message
			virtual void DebugOut(const char* message) = 0;

		};
	}
}
