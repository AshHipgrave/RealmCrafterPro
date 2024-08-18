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
#include <algorithm>
#include <locale>
#include <d3dx9.h>

#include <vector2.h>
#include <vector3.h>
#include <vector4.h>
#include <matrix.h>

#include "CD3D9GUIRenderer.h"
#include "CD3D9GUIMeshBuffer.h"
#include "CD3D9GUITexture.h"

namespace NGin
{
	namespace GUI
	{
		CD3D9GUIRenderer::CD3D9GUIRenderer(IDirect3DDevice9* device, ID3DXEffect* guiEffect,
			IDirect3DVertexDeclaration9* guiDeclaration)
			: Device(device), TrianglesDrawn(0), 
			GUIDeclaration(guiDeclaration), LastDeclaration(0), GUIEffect(guiEffect)
		{
			if(device)
				device->AddRef();
		}

		CD3D9GUIRenderer::~CD3D9GUIRenderer()
		{
			if(Device)
				Device->Release();
		}

		void CD3D9GUIRenderer::SetManager(IGUIManager* manager)
		{
		}

		void CD3D9GUIRenderer::OnDeviceLost()
		{
			if(GUIEffect != 0)
				GUIEffect->OnLostDevice();
		}

		void CD3D9GUIRenderer::OnDeviceReset()
		{
			if(GUIEffect != 0)
				GUIEffect->OnResetDevice();
		}

		bool CD3D9GUIRenderer::PreRender()
		{
			LastFVF = 0;
			LastDeclaration = NULL;

			// Store vertex settings
			//Device->GetVertexDeclaration(&LastDeclaration);

			Device->GetRenderState(D3DRS_SCISSORTESTENABLE, &LastScissorTestState);
			Device->GetRenderState(D3DRS_MULTISAMPLEANTIALIAS, &LastAntiAlias);

			// Set our required states
			//if(UsingPerfHUD)
			//	Device->SetRenderState(D3DRS_SCISSORTESTENABLE, FALSE);
			//else
				Device->SetRenderState(D3DRS_SCISSORTESTENABLE, TRUE);
			Device->SetRenderState(D3DRS_MULTISAMPLEANTIALIAS, FALSE);

			// Reset triangle count
			TrianglesDrawn = 0;

			// Set declaration
			Device->SetVertexDeclaration(GUIDeclaration);

			// Get VP and make a default scissor region
			D3DVIEWPORT9 VP;
			Device->GetViewport(&VP);
			Math::Vector4 Rect(0, 0, (float)VP.Width, (float)VP.Height);
			PushScissorRect(Rect);

			// Start Effect
			D3DXHANDLE Technique = NULL;
			GUIEffect->FindNextValidTechnique(NULL, &Technique);
			if(Technique == NULL)
				return false;

			GUIEffect->SetTechnique(Technique);

			UINT Passes = 0;
			GUIEffect->Begin(&Passes, 0);
			GUIEffect->BeginPass(0);

			return true;
		}

		bool CD3D9GUIRenderer::PostRender()
		{
			PopScissorRect();

			if(GUIEffect != NULL)
			{
				GUIEffect->EndPass();
				GUIEffect->End();
			}

			// Reset vertex settings
// 			if(LastFVF != 0)
// 				Device->SetFVF(LastFVF);
			LastFVF = 0;

			if(LastDeclaration != NULL)
			{
				Device->SetVertexDeclaration(LastDeclaration);
				LastDeclaration->Release();
				LastDeclaration = NULL;
			}

			Device->SetRenderState(D3DRS_SCISSORTESTENABLE, LastScissorTestState);
			Device->SetRenderState(D3DRS_MULTISAMPLEANTIALIAS, LastAntiAlias);


			return true;
		}
		
		IGUIMeshBuffer* CD3D9GUIRenderer::CreateMeshBuffer(
			const SGUIVertex* vertices, unsigned int vertexCount,
			const unsigned short* indices, unsigned int indexCount)
		{
			return CD3D9GUIMeshBuffer::CreateD3D9GUIMeshBuffer(
				Device, vertices, vertexCount,
				indices, indexCount);
		}

		void CD3D9GUIRenderer::FreeMeshBuffer(IGUIMeshBuffer* buffer)
		{
			delete buffer;
		}

		IGUITexture* CD3D9GUIRenderer::CreateTextureFromBase(void* base, Math::Vector2 &size)
		{
			CD3D9GUITexture* TTexture = new CD3D9GUITexture(reinterpret_cast<IDirect3DBaseTexture9*>(base), std::string("USERTEXTURE"), size, false);
			return TTexture;
		}

		IGUITexture* CD3D9GUIRenderer::CreateFontTexture(int width, int height, const char* data)
		{
			IDirect3DTexture9* Texture = NULL;
			if(FAILED(Device->CreateTexture(width, height, 1, 0, D3DFMT_A8R8G8B8, D3DPOOL_MANAGED, &Texture, NULL)))
			{
				MessageBox(0, "Font texture could not be made!", "NGUI-D3D9 Error", MB_OK);
				return 0;
			}

			D3DLOCKED_RECT LockedRect;
			Texture->LockRect(0, &LockedRect, NULL, 0);

			memcpy(LockedRect.pBits, data, width * height * 4);

			Texture->UnlockRect(0);

			return new CD3D9GUITexture(Texture, std::string("FONTTEXTURE"), Math::Vector2((float)width, (float)height), true);
		}

		IGUITexture* CD3D9GUIRenderer::GetTexture(IGUITexture* handle)
		{
			if(handle == 0)
				return 0;

			for(unsigned int i = 0; i < TextureCache.size(); ++i)
			{
				if(TextureCache[i] == handle)
				{
					TextureCache[i]->AddRef();
					return handle;
				}
			}

			return handle;
		}

		IGUITexture* CD3D9GUIRenderer::GetTexture(const char* path, unsigned int mask, bool ismask)
		{
			std::string PathLower = path;
			std::transform(PathLower.begin(), PathLower.end(), PathLower.begin(), ::tolower);

			// Check cache for existing texture
			for(unsigned int i = 0; i < TextureCache.size(); ++i)
			{
				if(TextureCache[i]->GetName().compare(PathLower) == 0)
				{
					TextureCache[i]->AddRef();
					return TextureCache[i];
				}
			}

			// Could find one, load it!
			IDirect3DTexture9* Texture = 0;
			HRESULT Hr;

			if(!ismask)
				Hr = D3DXCreateTextureFromFile(Device, PathLower.c_str(), &Texture);
			else
				Hr = D3DXCreateTextureFromFileExA(Device, PathLower.c_str(), D3DX_DEFAULT, D3DX_DEFAULT, 1, NULL, D3DFMT_UNKNOWN, D3DPOOL_MANAGED, D3DX_DEFAULT, D3DX_DEFAULT, mask, NULL, NULL, &Texture);

			if(Hr != D3D_OK || Texture == NULL)
			{
				std::string ErrorOutput = "Failed to load texture file: ";
				ErrorOutput += PathLower;
				ErrorOutput += "\n";
				OutputDebugStringA(ErrorOutput.c_str());

				return 0;
			}


			D3DSURFACE_DESC Desc;
			Texture->GetLevelDesc(0, &Desc);

			Math::Vector2 Size((float)Desc.Width, (float)Desc.Height);

			// Create instance
			CD3D9GUITexture* TTexture = new CD3D9GUITexture(Texture, PathLower, Size, true);
			TextureCache.push_back(TTexture);

			return TTexture;
		}
		
		void CD3D9GUIRenderer::FreeTexture(IGUITexture* texture)
		{
			// Simple error check
			if(texture != 0)
			{
				// Drop Texture
				CD3D9GUITexture* TreeTexture = dynamic_cast<CD3D9GUITexture*>(texture);
				bool Free = TreeTexture->Release();

				// Texture isn't referenced, remove from cache
				if(Free)
				{
					unsigned int Index = -1;
					for(unsigned int i = 0; i < TextureCache.size(); ++i)
					{
						if(TextureCache[i] == TreeTexture)
						{
							TextureCache.erase(TextureCache.begin() + i);
							return;
						}
					}
					
				}
			}
		}
		
		bool CD3D9GUIRenderer::SetMeshBuffer(const IGUIMeshBuffer* buffer)
		{
			return buffer->Set();
		}
		
		bool CD3D9GUIRenderer::SetTexture(const char* name, const IGUITexture* texture)
		{
			if(texture == 0)
				return false;
			if(GUIEffect == NULL)
				return false;

			D3DXHANDLE Handle = GUIEffect->GetParameterBySemantic(NULL, name);
			if(Handle == NULL)
				Handle = GUIEffect->GetParameterByName(NULL, name);
			if(Handle == NULL)
				return false;

			const IDirect3DBaseTexture9* TextureHandle = reinterpret_cast<const CD3D9GUITexture*>(texture)->GetTextureHandle();
			if(FAILED(GUIEffect->SetTexture(Handle, const_cast<IDirect3DBaseTexture9*>(TextureHandle))))
				return false;
			GUIEffect->CommitChanges();

			return true;
		}
		
		void CD3D9GUIRenderer::DrawMeshBuffer(const IGUIMeshBuffer* buffer)
		{
			dynamic_cast<const CD3D9GUIMeshBuffer*>(buffer)->Draw(TrianglesDrawn);
		}

		void CD3D9GUIRenderer::DrawMeshBuffer(const IGUIMeshBuffer* buffer, int triangleOffset, int triangleCount)
		{
			dynamic_cast<const CD3D9GUIMeshBuffer*>(buffer)->Draw(TrianglesDrawn, triangleOffset, triangleCount);
		}

		unsigned int CD3D9GUIRenderer::GetTrianglesDrawn() const
		{
			return TrianglesDrawn;
		}

		Math::Vector4 CD3D9GUIRenderer::GetScissorRect()
		{
			if(ScissorRegions.size() > 0)
				return ScissorRegions.top();

			return Math::Vector4();
		}

		void CD3D9GUIRenderer::PushScissorRect(Math::Vector4 &rect)
		{
			ScissorRegions.push(rect);

			RECT Rgn;
			Rgn.left = (LONG)rect.X;
			Rgn.top = (LONG)rect.Y;
			Rgn.right = (LONG)rect.Z;
			Rgn.bottom = (LONG)rect.W;

			D3DVIEWPORT9 VP;
			Device->GetViewport(&VP);
			if(Rgn.right > (LONG)VP.Width)
				Rgn.right = (LONG)VP.Width;
			if(Rgn.bottom > (LONG)VP.Height)
				Rgn.bottom = (LONG)VP.Height;
			if(Rgn.left < 0)
				Rgn.left = 0;
			if(Rgn.top < 0)
				Rgn.top = 0;

			Device->SetScissorRect(&Rgn);
		}

		void CD3D9GUIRenderer::PopScissorRect()
		{
			if(ScissorRegions.size() == 0)
				return;
			ScissorRegions.pop();
			if(ScissorRegions.size() == 0)
				return;

			Math::Vector4 Rect = ScissorRegions.top();

			RECT Rgn;
			Rgn.left = (LONG)Rect.X;
			Rgn.top = (LONG)Rect.Y;
			Rgn.right = (LONG)Rect.Z;
			Rgn.bottom = (LONG)Rect.W;

			D3DVIEWPORT9 VP;
			Device->GetViewport(&VP);
			if(Rgn.right > (LONG)VP.Width)
				Rgn.right = (LONG)VP.Width;
			if(Rgn.bottom > (LONG)VP.Height)
				Rgn.bottom = (LONG)VP.Height;
			if(Rgn.left < 0)
				Rgn.left = 0;
			if(Rgn.top < 0)
				Rgn.top = 0;

			Device->SetScissorRect(&Rgn);
		}

		bool CD3D9GUIRenderer::SetParameter(const char* name, const IParameter &parameter)
		{
			if(GUIEffect == NULL)
				return false;

			D3DXHANDLE Handle = GUIEffect->GetParameterBySemantic(NULL, name);
			if(Handle == NULL)
				Handle = GUIEffect->GetParameterByName(NULL, name);
			if(Handle == NULL)
				return false;

			if(parameter.GetType() == typeid(float))
			{
				float Val = (*((const TParameter<float>*)&parameter)).GetData();
				GUIEffect->SetFloat(Handle, Val);
				GUIEffect->CommitChanges();
				return true;
			}

			if(parameter.GetType() == typeid(NGin::Math::Vector2))
			{
				NGin::Math::Vector2 Val = (*((const TParameter<NGin::Math::Vector2>*)&parameter)).GetData();
				GUIEffect->SetFloatArray(Handle, reinterpret_cast<const FLOAT*>(&Val), 2);
				GUIEffect->CommitChanges();
				return true;
			}

			if(parameter.GetType() == typeid(NGin::Math::Vector3))
			{
				NGin::Math::Vector3 Val = (*((const TParameter<NGin::Math::Vector3>*)&parameter)).GetData();
				GUIEffect->SetFloatArray(Handle, reinterpret_cast<const FLOAT*>(&Val), 3);
				GUIEffect->CommitChanges();
				return true;
			}

			if(parameter.GetType() == typeid(NGin::Math::Vector4))
			{
				NGin::Math::Vector4 Val = (*((const TParameter<NGin::Math::Vector4>*)&parameter)).GetData();
				GUIEffect->SetFloatArray(Handle, reinterpret_cast<const FLOAT*>(&Val), 4);
				GUIEffect->CommitChanges();
				return true;
			}

			if(parameter.GetType() == typeid(NGin::Math::Matrix))
			{
				NGin::Math::Matrix Val = (*((const TParameter<NGin::Math::Matrix>*)&parameter)).GetData();
				GUIEffect->SetMatrix(Handle, reinterpret_cast<const D3DXMATRIX*>(&Val));
				GUIEffect->CommitChanges();
				return true;
			}

			return false;
		}

		void CD3D9GUIRenderer::DebugOut(const char* message)
		{
			OutputDebugStringA(message);
		}

	} // Namespace GUI
} // Namespace NGin

inline bool CheckFile(const std::string &path)
{
	FILE* F = fopen(path.c_str(), "r");
	if(F == 0)
		return false;
	fclose(F);
	return true;
}

#ifdef EXPORTS
	__declspec(dllexport)
#else
	__declspec(dllimport)
#endif
	NGin::GUI::IGUIRenderer* CreateDirect3D9GUIRenderer(
		void* device,
		const char* effectPath)
	{
		IDirect3DDevice9* Device = reinterpret_cast<IDirect3DDevice9*>(device);
		IDirect3DVertexDeclaration9* LocalGUIDeclaration = 0;
		ID3DXEffect* LocalGUIEffect = 0;

		const D3DVERTEXELEMENT9 SD3D9GUIVertexDeclaration[] =
		{
			{0, 0,  D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
			{0, 12, D3DDECLTYPE_D3DCOLOR, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_COLOR, 0},
			{0, 16, D3DDECLTYPE_FLOAT2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
			D3DDECL_END()
		};

		// Create all used declarations
		if(Device->CreateVertexDeclaration(SD3D9GUIVertexDeclaration, &LocalGUIDeclaration) != D3D_OK)
		{
			return 0;
		}

		// Create Effects
		ID3DXBuffer* ErrorBuffer = 0;
		D3DXCreateBuffer(2048, &ErrorBuffer);

		if(!CheckFile(effectPath))
			return 0;

		if(FAILED(D3DXCreateEffectFromFileA(Device, effectPath, NULL, NULL, 0, NULL, &LocalGUIEffect, &ErrorBuffer)))
		{
			// Loading failed
			MessageBox(0, (LPCSTR)ErrorBuffer->GetBufferPointer(), 0, 0);
			return 0;
		}

		// Setup renderer
		NGin::GUI::CD3D9GUIRenderer* GUIRenderer = new NGin::GUI::CD3D9GUIRenderer(
			Device, LocalGUIEffect, LocalGUIDeclaration);

		// Done
		return GUIRenderer;
	}


