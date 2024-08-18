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
#include "CD3D9GUITexture.h"

namespace NGin
{
	namespace GUI
	{
		CD3D9GUITexture::CD3D9GUITexture(IDirect3DBaseTexture9* textureHandle, const std::string& name, Math::Vector2 &size, bool managed)
			: TextureHandle(textureHandle),
			  Name(name),
			  ReferenceCount(1),
			  Size(size),
			  Managed(managed)
		{
		}
		
		CD3D9GUITexture::~CD3D9GUITexture()
		{
			if(Managed && TextureHandle != 0)
				TextureHandle->Release();
		}

		void CD3D9GUITexture::AddRef()
		{
			++ReferenceCount;
		}
		
		bool CD3D9GUITexture::Release()
		{
			--ReferenceCount;

			if(ReferenceCount == 0)
			{
				delete this;
				return true;
			}

			return false;
		}

		std::string CD3D9GUITexture::GetName() const
		{
			return Name;
		}

		Math::Vector2 CD3D9GUITexture::GetSize() const
		{
			return Size;
		}
			
		const IDirect3DBaseTexture9* CD3D9GUITexture::GetTextureHandle() const
		{
			return TextureHandle;
		}
	}
}
