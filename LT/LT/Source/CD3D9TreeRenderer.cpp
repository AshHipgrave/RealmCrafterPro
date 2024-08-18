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
//COPYRIGHTNOTICE

#include <algorithm>
#include <locale>
#include <d3dx9.h>

#include <vector2.h>
#include <vector3.h>
#include <vector4.h>
#include <matrix.h>

#include "CD3D9TreeRenderer.h"
#include "CD3D9TreeTrunkMeshBuffer.h"
#include "CD3D9TreeLeafMeshBuffer.h"
#include "CD3D9TreeLODMeshBuffer.h"

namespace LT
{
	// Vertex Declarations
	// These could be in other files.. but we only use them once
	const D3DVERTEXELEMENT9 SD3D9TreeTrunkVertexDeclaration[] =
	{
		{0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
		{0, 12, D3DDECLTYPE_FLOAT16_2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
		{0, 16, D3DDECLTYPE_UBYTE4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 1},
		{0, 20, D3DDECLTYPE_UBYTE4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 2},
		{0, 24, D3DDECLTYPE_UBYTE4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 3},
		D3DDECL_END()
	};

	const D3DVERTEXELEMENT9 SD3D9TreeLeafVertexDeclaration[] =
	{
		{0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
		{0, 12, D3DDECLTYPE_FLOAT16_2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
		D3DDECL_END()
	};

	const D3DVERTEXELEMENT9 SD3D9TreeLODVertexDeclaration[] =
	{
		{0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
		{0, 12, D3DDECLTYPE_FLOAT16_2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
		{0, 16, D3DDECLTYPE_FLOAT16_2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 1},
		D3DDECL_END()
	};

	CD3D9TreeRenderer::CD3D9TreeRenderer(IDirect3DDevice9* device)
		: Device(device), TrianglesDrawn(0), SwayDirection(0, 0), SwayAmount(0),
		QualityLevel(0)
	{
		if(device)
			device->AddRef();
	}

	CD3D9TreeRenderer::~CD3D9TreeRenderer()
	{
		if(Device)
			Device->Release();
	}

	void CD3D9TreeRenderer::OnDeviceLost()
	{
		if(TrunkEffect != 0)
			TrunkEffect->OnLostDevice();

		if(LeafEffect != 0)
			LeafEffect->OnLostDevice();

		if(LODEffect != 0)
			LODEffect->OnLostDevice();
	}

	void CD3D9TreeRenderer::OnDeviceReset()
	{
		if(TrunkEffect != 0)
			TrunkEffect->OnResetDevice();

		if(LeafEffect != 0)
			LeafEffect->OnResetDevice();

		if(LODEffect != 0)
			LODEffect->OnResetDevice();
	}

	bool CD3D9TreeRenderer::PreRender()
	{
		LastFVF = 0;
		LastDeclaration = NULL;

		// Store vertex settings
		Device->GetVertexDeclaration(&LastDeclaration);
		//Device->GetFVF(&LastFVF);

		// Reset triangle count
		TrianglesDrawn = 0;

		// Set 'global' rendering data
		#define SETARRAY(Prefix, Name, Variable, Count)\
			if(Prefix##Name != NULL)\
				Prefix##Effect->SetFloatArray(Prefix##Name, reinterpret_cast<const FLOAT*>(&Variable), Count);

		SETARRAY(Trunk, LightAmbient, AmbientLight, 3);
		SETARRAY(Trunk, DirectionalNormal, DirectionalNormals, 9);
		SETARRAY(Trunk, DirectionalColor, DirectionalColors, 9);
		SETARRAY(Trunk, FogColor, FogColor, 4);
		SETARRAY(Trunk, FogData, FogData, 2);
		SETARRAY(Trunk, SwayDirection, SwayDirection, 2);
		SETARRAY(Trunk, SwayAmount, SwayAmount, 1);

		SETARRAY(Leaf, LightAmbient, AmbientLight, 3);
		SETARRAY(Leaf, DirectionalNormal, DirectionalNormals, 9);
		SETARRAY(Leaf, DirectionalColor, DirectionalColors, 9);
		SETARRAY(Leaf, FogColor, FogColor, 4);
		SETARRAY(Leaf, FogData, FogData, 2);
		SETARRAY(Leaf, SwayDirection, SwayDirection, 2);
		SETARRAY(Leaf, SwayAmount, SwayAmount, 1);

		SETARRAY(LOD, LightAmbient, AmbientLight, 3);
		SETARRAY(LOD, DirectionalNormal, DirectionalNormals, 9);
		SETARRAY(LOD, DirectionalColor, DirectionalColors, 9);
		SETARRAY(LOD, FogColor, FogColor, 4);
		SETARRAY(LOD, FogData, FogData, 2);
		SETARRAY(LOD, SwayDirection, SwayDirection, 2);
		SETARRAY(LOD, SwayAmount, SwayAmount, 1);

		float Time = static_cast<float>(timeGetTime());
		SETARRAY(Trunk, Time, Time, 1);
		SETARRAY(Leaf, Time, Time, 1);
		SETARRAY(LOD, Time, Time, 1);

		return true;
	}

	bool CD3D9TreeRenderer::PostRender()
	{
		// Reset vertex settings
// 		if(LastFVF != 0)
// 			Device->SetFVF(LastFVF);
		LastFVF = 0;

		if(LastDeclaration != NULL)
		{
			Device->SetVertexDeclaration(LastDeclaration);
			LastDeclaration->Release();
			LastDeclaration = NULL;
		}

		return true;
	}
	
	ITreeMeshBuffer* CD3D9TreeRenderer::CreateTrunkBuffer(
		const STreeTrunkVertex* vertices, unsigned int vertexCount,
		const unsigned short* indices, unsigned int indexCount)
	{
		return CD3D9TreeTrunkMeshBuffer::CreateD3D9TreeTrunkMeshBuffer(
			Device, vertices, vertexCount,
			indices, indexCount);
	}
	
	ITreeMeshBuffer* CD3D9TreeRenderer::CreateLeafBuffer(
		const STreeLeafVertex* vertices, unsigned int vertexCount,
		const unsigned short* indices, unsigned int indexCount)
	{
		return CD3D9TreeLeafMeshBuffer::CreateD3D9TreeLeafMeshBuffer(
			Device, vertices, vertexCount,
			indices, indexCount);
	}
	
	ITreeMeshBuffer* CD3D9TreeRenderer::CreateLODBuffer(
		const STreeLODVertex* vertices, unsigned int vertexCount,
		const unsigned short* indices, unsigned int indexCount)
	{
		return CD3D9TreeLODMeshBuffer::CreateD3D9TreeLODMeshBuffer(
			Device, vertices, vertexCount,
			indices, indexCount);
	}

	void CD3D9TreeRenderer::FreeMeshBuffer(ITreeMeshBuffer* buffer)
	{
		delete buffer;
	}

	ITreeTexture* CD3D9TreeRenderer::GetTexture(const char* path, bool ismask)
	{
		std::string PathLower = path;
		std::transform(PathLower.begin(), PathLower.end(), PathLower.begin(), ::tolower);

		// Check cache for existing texture
		for(int i = 0; i < TextureCache.size(); ++i)
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
			Hr = D3DXCreateTextureFromFileExA(Device, PathLower.c_str(), D3DX_DEFAULT, D3DX_DEFAULT, 1, NULL, D3DFMT_UNKNOWN, D3DPOOL_MANAGED, D3DX_DEFAULT, D3DX_DEFAULT, NULL, NULL, NULL, &Texture);

		if(Hr != D3D_OK)
		{
			std::string ErrorOutput = "Failed to load texture file: ";
			ErrorOutput += PathLower;
			ErrorOutput += "\n";
			OutputDebugStringA(ErrorOutput.c_str());

			return 0;
		}

		// Create instance
		CD3D9TreeTexture* TreeTexture = new CD3D9TreeTexture(Texture, PathLower);
		TextureCache.push_back(TreeTexture);

		return TreeTexture;
	}
	
	void CD3D9TreeRenderer::FreeTexture(ITreeTexture* texture)
	{
		// Simple error check
		if(texture != 0)
		{
			// Drop Texture
			CD3D9TreeTexture* TreeTexture = dynamic_cast<CD3D9TreeTexture*>(texture);
			bool Free = TreeTexture->Release();

			// Texture isn't referenced, remove from cache
			if(Free)
			{
				int Index = -1;
				for(int i = 0; i < TextureCache.size(); ++i)
				{
					if(TextureCache[i] == TreeTexture)
					{
						Index = i;
						break;
					}
				}

				if(Index > -1)
					TextureCache.erase(TextureCache.begin() + Index);
			}
		}
	}

	bool CD3D9TreeRenderer::PreRenderTrunk(int rtIndex)
	{
		if(FAILED(Device->SetVertexDeclaration(TrunkDeclaration)))
			return false;

		char ProfileLow[128];
		char ProfileHigh[128];
		sprintf(ProfileLow, "ProfileLow%i", rtIndex);
		sprintf(ProfileHigh, "ProfileHigh%i", rtIndex);

		if(QualityLevel == 0)
		{
			if(FAILED(TrunkEffect->SetTechnique(ProfileLow)))
			{
				D3DXHANDLE ValidTechnique = NULL;
				if(FAILED(TrunkEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
					return false;
				if(FAILED(TrunkEffect->SetTechnique(ValidTechnique)))
					return false;
			}
		}else
		{
			if(FAILED(TrunkEffect->SetTechnique(ProfileHigh)))
			{
				D3DXHANDLE ValidTechnique = NULL;
				if(FAILED(TrunkEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
					return false;
				if(FAILED(TrunkEffect->SetTechnique(ValidTechnique)))
					return false;
			}
		}

		UINT TPass = 0;
		if(FAILED(TrunkEffect->Begin(&TPass, 0)))
			return false;

		if(FAILED(TrunkEffect->BeginPass(0)))
		{
			TrunkEffect->End();
			return false;
		}

		SetEffect = TrunkEffect;

		return true;
	}

	bool CD3D9TreeRenderer::PreRenderTrunkDepth()
	{
		if(FAILED(Device->SetVertexDeclaration(TrunkDeclaration)))
			return false;

		if(FAILED(TrunkEffect->SetTechnique("ProfileDepth")))
		{
			D3DXHANDLE ValidTechnique = NULL;
			if(FAILED(TrunkEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
				return false;
			if(FAILED(TrunkEffect->SetTechnique(ValidTechnique)))
				return false;
		}

		UINT TPass = 0;
		if(FAILED(TrunkEffect->Begin(&TPass, 0)))
			return false;

		if(FAILED(TrunkEffect->BeginPass(0)))
		{
			TrunkEffect->End();
			return false;
		}

		SetEffect = TrunkEffect;

		return true;
	}
	
	bool CD3D9TreeRenderer::PreRenderLeaf(int rtIndex)
	{
		if(FAILED(Device->SetVertexDeclaration(LeafDeclaration)))
			return false;

		char ProfileLow[128];
		char ProfileHigh[128];
		sprintf(ProfileLow, "ProfileLow%i", rtIndex);
		sprintf(ProfileHigh, "ProfileHigh%i", rtIndex);

		if(QualityLevel == 0)
		{
			if(FAILED(LeafEffect->SetTechnique(ProfileLow)))
			{
				D3DXHANDLE ValidTechnique = NULL;
				if(FAILED(LeafEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
					return false;
				if(FAILED(LeafEffect->SetTechnique(ValidTechnique)))
					return false;
			}
		}else
		{
			if(FAILED(LeafEffect->SetTechnique(ProfileHigh)))
			{
				D3DXHANDLE ValidTechnique = NULL;
				if(FAILED(LeafEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
					return false;
				if(FAILED(LeafEffect->SetTechnique(ValidTechnique)))
					return false;
			}
		}

		UINT TPass = 0;
		if(FAILED(LeafEffect->Begin(&TPass, 0)))
			return false;

		if(FAILED(LeafEffect->BeginPass(0)))
		{
			LeafEffect->End();
			return false;
		}

		SetEffect = LeafEffect;

		return true;
	}

	bool CD3D9TreeRenderer::PreRenderLeafDepth()
	{
		if(FAILED(Device->SetVertexDeclaration(LeafDeclaration)))
			return false;

		if(FAILED(LeafEffect->SetTechnique("ProfileDepth")))
		{
			D3DXHANDLE ValidTechnique = NULL;
			if(FAILED(LeafEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
				return false;
			if(FAILED(LeafEffect->SetTechnique(ValidTechnique)))
				return false;
		}

		UINT TPass = 0;
		if(FAILED(LeafEffect->Begin(&TPass, 0)))
			return false;

		if(FAILED(LeafEffect->BeginPass(0)))
		{
			LeafEffect->End();
			return false;
		}

		SetEffect = LeafEffect;

		return true;
	}
	
	bool CD3D9TreeRenderer::PreRenderLOD(int rtIndex)
	{
		if(FAILED(Device->SetVertexDeclaration(LODDeclaration)))
			return false;

		char ProfileLow[128];
		char ProfileHigh[128];
		sprintf(ProfileLow, "ProfileLow%i", rtIndex);
		sprintf(ProfileHigh, "ProfileHigh%i", rtIndex);

		if(QualityLevel == 0)
		{
			if(FAILED(LODEffect->SetTechnique(ProfileLow)))
			{
				D3DXHANDLE ValidTechnique = NULL;
				if(FAILED(LODEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
					return false;
				if(FAILED(LODEffect->SetTechnique(ValidTechnique)))
					return false;
			}
		}else
		{
			if(FAILED(LODEffect->SetTechnique(ProfileHigh)))
			{
				D3DXHANDLE ValidTechnique = NULL;
				if(FAILED(LODEffect->FindNextValidTechnique(NULL, &ValidTechnique)))
					return false;
				if(FAILED(LODEffect->SetTechnique(ValidTechnique)))
					return false;
			}
		}

		UINT TPass = 0;
		if(FAILED(LODEffect->Begin(&TPass, 0)))
			return false;

		if(FAILED(LODEffect->BeginPass(0)))
		{
			LODEffect->End();
			return false;
		}

		SetEffect = LODEffect;

		return true;
	}

	void CD3D9TreeRenderer::PostRenderTrunk()
	{
		TrunkEffect->EndPass();
		TrunkEffect->End();
		SetEffect = NULL;
	}

	void CD3D9TreeRenderer::PostRenderLeaf()
	{
		LeafEffect->EndPass();
		LeafEffect->End();
		SetEffect = NULL;
	}
	
	void CD3D9TreeRenderer::PostRenderLOD()
	{
		LODEffect->EndPass();
		LODEffect->End();
		SetEffect = NULL;
	}

	void CD3D9TreeRenderer::PostRenderTrunkDepth()
	{
		TrunkEffect->EndPass();
		TrunkEffect->End();
		SetEffect = NULL;
	}

	void CD3D9TreeRenderer::PostRenderLeafDepth()
	{
		LeafEffect->EndPass();
		LeafEffect->End();
		SetEffect = NULL;
	}
	
	bool CD3D9TreeRenderer::SetMeshBuffer(const ITreeMeshBuffer* buffer)
	{
		return buffer->Set();
	}
	
	bool CD3D9TreeRenderer::SetTexture(const char* name, const ITreeTexture* texture)
	{
		if(texture == 0)
			return false;
		if(SetEffect == NULL)
			return false;

		D3DXHANDLE Handle = SetEffect->GetParameterBySemantic(NULL, name);
		if(Handle == NULL)
			Handle = SetEffect->GetParameterByName(NULL, name);
		if(Handle == NULL)
			return false;

		const IDirect3DBaseTexture9* TextureHandle = reinterpret_cast<const CD3D9TreeTexture*>(texture)->GetTextureHandle();
		if(FAILED(SetEffect->SetTexture(Handle, const_cast<IDirect3DBaseTexture9*>(TextureHandle))))
			return false;
		SetEffect->CommitChanges();

		return true;
	}

	bool CD3D9TreeRenderer::SetParameter(const char* name, const IParameter &parameter, bool applyToAll)
	{
		if(!applyToAll)
		{
			return SetParameter(name, parameter);
		}

		ID3DXEffect* TempEffect = SetEffect;

		SetEffect = TrunkEffect;
		SetParameter(name, parameter);
		SetEffect = LeafEffect;
		SetParameter(name, parameter);
		SetEffect = LODEffect;
		SetParameter(name, parameter);

		SetEffect = TempEffect;
		return true;
	}
	
	bool CD3D9TreeRenderer::SetParameter(const char* name, const IParameter &parameter)
	{
		if(SetEffect == NULL)
			return false;

		D3DXHANDLE Handle = SetEffect->GetParameterBySemantic(NULL, name);
		if(Handle == NULL)
			Handle = SetEffect->GetParameterByName(NULL, name);
		if(Handle == NULL)
			return false;

		if(parameter.GetType() == typeid(float))
		{
			float Val = (*((const TParameter<float>*)&parameter)).GetData();
			SetEffect->SetFloat(Handle, Val);
			SetEffect->CommitChanges();
			return true;
		}

		if(parameter.GetType() == typeid(NGin::Math::Vector2))
		{
			NGin::Math::Vector2 Val = (*((const TParameter<NGin::Math::Vector2>*)&parameter)).GetData();
			SetEffect->SetFloatArray(Handle, reinterpret_cast<const FLOAT*>(&Val), 2);
			SetEffect->CommitChanges();
			return true;
		}

		if(parameter.GetType() == typeid(NGin::Math::Vector3))
		{
			NGin::Math::Vector3 Val = (*((const TParameter<NGin::Math::Vector3>*)&parameter)).GetData();
			SetEffect->SetFloatArray(Handle, reinterpret_cast<const FLOAT*>(&Val), 3);
			SetEffect->CommitChanges();
			return true;
		}

		if(parameter.GetType() == typeid(NGin::Math::Vector4))
		{
			NGin::Math::Vector4 Val = (*((const TParameter<NGin::Math::Vector4>*)&parameter)).GetData();
			SetEffect->SetFloatArray(Handle, reinterpret_cast<const FLOAT*>(&Val), 4);
			SetEffect->CommitChanges();
			return true;
		}

		if(parameter.GetType() == typeid(NGin::Math::Matrix))
		{
			NGin::Math::Matrix Val = (*((const TParameter<NGin::Math::Matrix>*)&parameter)).GetData();
			SetEffect->SetMatrix(Handle, reinterpret_cast<const D3DXMATRIX*>(&Val));
			SetEffect->CommitChanges();
			return true;
		}

		return false;
	}
	
	void CD3D9TreeRenderer::DrawMeshBuffer(const ITreeMeshBuffer* buffer)
	{
		buffer->Draw(TrianglesDrawn);
	}

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
		ITreeRenderer* CreateDirect3D9TreeRenderer(
		IDirect3DDevice9* device,
		const char* trunkEffect, const char* leafEffect, const char* lodEffect)
	{
		IDirect3DVertexDeclaration9* LocalTrunkDeclaration = 0;
		IDirect3DVertexDeclaration9* LocalLeafDeclaration = 0;
		IDirect3DVertexDeclaration9* LocalLODDeclaration = 0;
		ID3DXEffect* LocalTrunkEffect = 0;
		ID3DXEffect* LocalLeafEffect = 0;
		ID3DXEffect* LocalLODEffect = 0;

		// Create all used declarations
		if(device->CreateVertexDeclaration(SD3D9TreeTrunkVertexDeclaration, &LocalTrunkDeclaration) != D3D_OK)
		{
			return 0;
		}

		if(device->CreateVertexDeclaration(SD3D9TreeLeafVertexDeclaration, &LocalLeafDeclaration) != D3D_OK)
		{
			LocalTrunkDeclaration->Release();
			return 0;
		}

		if(device->CreateVertexDeclaration(SD3D9TreeLODVertexDeclaration, &LocalLODDeclaration) != D3D_OK)
		{
			LocalTrunkDeclaration->Release();
			LocalLeafDeclaration->Release();
			return 0;
		}

		// Create Effects
		ID3DXBuffer* ErrorBuffer = 0;
		D3DXCreateBuffer(2048, &ErrorBuffer);

		if(!CheckFile(trunkEffect))
			return 0;
		if(!CheckFile(leafEffect))
			return 0;
		if(!CheckFile(lodEffect))
			return 0;

		if(FAILED(D3DXCreateEffectFromFileA(device, trunkEffect, NULL, NULL, 0, NULL, &LocalTrunkEffect, &ErrorBuffer)))
		{
			// Loading failed
			MessageBox(0, (LPCSTR)ErrorBuffer->GetBufferPointer(), 0, 0);
			return 0;
		}

		if(FAILED(D3DXCreateEffectFromFileA(device, leafEffect, NULL, NULL, 0, NULL, &LocalLeafEffect, &ErrorBuffer)))
		{
			// Loading failed
			MessageBox(0, (LPCSTR)ErrorBuffer->GetBufferPointer(), 0, 0);
			return 0;
		}

		if(FAILED(D3DXCreateEffectFromFileA(device, lodEffect, NULL, NULL, 0, NULL, &LocalLODEffect, &ErrorBuffer)))
		{
			// Loading failed
			MessageBox(0, (LPCSTR)ErrorBuffer->GetBufferPointer(), 0, 0);
			return 0;
		}


		// Setup renderer
		CD3D9TreeRenderer* TreeRenderer = new CD3D9TreeRenderer(device);
		TreeRenderer->TrunkDeclaration = LocalTrunkDeclaration;
		TreeRenderer->LeafDeclaration = LocalLeafDeclaration;
		TreeRenderer->LODDeclaration = LocalLODDeclaration;
		TreeRenderer->TrunkEffect = LocalTrunkEffect;
		TreeRenderer->LeafEffect = LocalLeafEffect;
		TreeRenderer->LODEffect = LocalLODEffect;
		TreeRenderer->GetHandles();

		// Done
		return TreeRenderer;
	}

	unsigned int CD3D9TreeRenderer::GetTrianglesDrawn() const
	{
		return TrianglesDrawn;
	}

	void CD3D9TreeRenderer::SetFog(NGin::Math::Color& color, NGin::Math::Vector2& data)
	{
		FogColor = color;
		if(FogData.X >= FogData.Y)
			FogData.X = FogData.Y - 1.0f;
		FogData = data;
	}

	void CD3D9TreeRenderer::GetClosestLights(NGin::Math::Vector3** colors, NGin::Math::Vector4** positions, NGin::Math::Vector3 &position)
	{
		PointLight* Ordered[5] = {0, 0, 0, 0, 0};

		float DiffX, DiffY, DiffZ;

		for(unsigned int l = 0; l < PointLights.Size(); ++l)
		{
			PointLight* L = PointLights[l];
			if(L->Active)
			{

				DiffX = (position.X - L->Position.X);
				DiffY = (position.Y - L->Position.Y);
				DiffZ = (position.Z - L->Position.Z);
				DiffX *= DiffX;
				DiffY *= DiffY;
				DiffZ *= DiffZ;

				L->Distance = DiffX + DiffY + DiffZ;

				for(int i = 0; i < 5; ++i)
				{
					if(Ordered[i] != 0)
					{
						if(L->Distance < Ordered[i]->Distance)
						{
							if(i == 4)
							{
								Ordered[4] = L;
							}else
							{
								for(int f = 4; f > i; --f)
									Ordered[f] = Ordered[f - 1];
								Ordered[i] = L;
							}

							break;
						}
					}
					else
					{
						Ordered[i] = L;
						break;
					}
				}

			}
		}

		for(unsigned int i = 0; i < 5; ++i)
		{
			if(Ordered[i] != 0)
			{
				(*colors)[i].X = Ordered[i]->Color.R;
				(*colors)[i].Y = Ordered[i]->Color.G;
				(*colors)[i].Z = Ordered[i]->Color.B;
				(*positions)[i].X = Ordered[i]->Position.X;
				(*positions)[i].Y = Ordered[i]->Position.Y;
				(*positions)[i].Z = Ordered[i]->Position.Z;
				(*positions)[i].W = Ordered[i]->Radius;
			}else
			{
				(*colors)[i].Reset();
				(*positions)[i].Reset();
			}
		}
	}

	void CD3D9TreeRenderer::SetShadowMap(void* handle)
	{
		D3DXHANDLE H = TrunkEffect->GetParameterBySemantic(NULL, "ShadowMap");
		if(H == NULL)
			H = TrunkEffect->GetParameterByName(NULL, "ShadowMap");
		if(H != NULL)
			TrunkEffect->SetTexture(H, (IDirect3DTexture9*)handle);

		H = LeafEffect->GetParameterBySemantic(NULL, "ShadowMap");
		if(H == NULL)
			H = LeafEffect->GetParameterByName(NULL, "ShadowMap");
		if(H != NULL)
			LeafEffect->SetTexture(H, (IDirect3DTexture9*)handle);

		H = LODEffect->GetParameterBySemantic(NULL, "ShadowMap");
		if(H == NULL)
			H = LODEffect->GetParameterByName(NULL, "ShadowMap");
		if(H != NULL)
			LODEffect->SetTexture(H, (IDirect3DTexture9*)handle);
	}

	void CD3D9TreeRenderer::SetPointLights(void** lights, int count, int stride)
	{
		if(stride != sizeof(CD3D9TreeRenderer::PointLight))
		{
			char OO[2048];
			sprintf(OO, "CTerrainManager::SetPointLights() stride did not match internal stride (%i != %u)\n", stride, sizeof(PointLight));
			OutputDebugString(OO);
			return;
		}

		for(int i = 0; i < PointLights.Size(); ++i)
			delete PointLights[i];

		PointLights.Empty();
		PointLights.SetUsed(count);

		PointLight** Lights = (PointLight**)lights;

		for(int i = 0; i < count; ++i)
		{
			PointLight* L = new PointLight();
			L->Position = Lights[i]->Position;
			L->Active = Lights[i]->Active;
			L->Color = Lights[i]->Color;
			L->Radius = Lights[i]->Radius;
			L->Distance = 0.0f;
			PointLights[i] = L;

			//char OO[1024];
			//sprintf(OO, "L: %f, %f, %f: %i: %f, %f, %f: %s\n", L->Position.X, L->Position.Y, L->Position.Z, L->Radius, L->Color.R, L->Color.G, L->Color.B, L->Active ? "Active" : "Disabled");
			//OutputDebugString(OO);
		}

		//char OO[1024];
		//sprintf(OO, "L: %i\n", count);
		//OutputDebugString(OO);
	}

	void CD3D9TreeRenderer::SetLightNormal(int index, NGin::Math::Vector3 &normal)
	{
		if(index < 0 || index > 2)
			return;

		DirectionalNormals[index] = normal;
		DirectionalNormals[index].Normalize();
	}

	void CD3D9TreeRenderer::SetLightColor(int index, NGin::Math::Color &color)
	{
		if(index < 0 || index > 2)
			return;

		DirectionalColors[index].X = color.R;
		DirectionalColors[index].Y = color.G;
		DirectionalColors[index].Z = color.B;
	}

	void CD3D9TreeRenderer::SetAmbientLight(NGin::Math::Color &color)
	{
		AmbientLight.X = color.R;
		AmbientLight.Y = color.G;
		AmbientLight.Z = color.B;
	}

	void CD3D9TreeRenderer::GetHandles()
	{
		#define GETHANDLE(Prefix, Name, Str) \
			Prefix##Name = Prefix##Effect->GetParameterBySemantic(NULL, Str);\
			if(Prefix##Name == NULL)\
				Prefix##Name = Prefix##Effect->GetParameterByName(NULL, Str);

		GETHANDLE(Trunk, LightAmbient, "LightAmbient");
		GETHANDLE(Trunk, PointColor, "PointColor");
		GETHANDLE(Trunk, PointPosition, "PointPosition");
		GETHANDLE(Trunk, DirectionalNormal, "DirectionalNormal");
		GETHANDLE(Trunk, DirectionalColor, "DirectionalColor");
		GETHANDLE(Trunk, FogColor, "FogColor");
		GETHANDLE(Trunk, FogData, "FogData");
		GETHANDLE(Trunk, Time, "Time");
		GETHANDLE(Trunk, SwayDirection, "SwayDirection");
		GETHANDLE(Trunk, SwayAmount, "SwayAmount");

		GETHANDLE(Leaf, LightAmbient, "LightAmbient");
		GETHANDLE(Leaf, PointColor, "PointColor");
		GETHANDLE(Leaf, PointPosition, "PointPosition");
		GETHANDLE(Leaf, DirectionalNormal, "DirectionalNormal");
		GETHANDLE(Leaf, DirectionalColor, "DirectionalColor");
		GETHANDLE(Leaf, FogColor, "FogColor");
		GETHANDLE(Leaf, FogData, "FogData");
		GETHANDLE(Leaf, Time, "Time");
		GETHANDLE(Leaf, SwayDirection, "SwayDirection");
		GETHANDLE(Leaf, SwayAmount, "SwayAmount");

		GETHANDLE(LOD, LightAmbient, "LightAmbient");
		GETHANDLE(LOD, PointColor, "PointColor");
		GETHANDLE(LOD, PointPosition, "PointPosition");
		GETHANDLE(LOD, DirectionalNormal, "DirectionalNormal");
		GETHANDLE(LOD, DirectionalColor, "DirectionalColor");
		GETHANDLE(LOD, FogColor, "FogColor");
		GETHANDLE(LOD, FogData, "FogData");
		GETHANDLE(LOD, Time, "Time");
		GETHANDLE(LOD, SwayDirection, "SwayDirection");
		GETHANDLE(LOD, SwayAmount, "SwayAmount");

		#undef GETHANDLE
	}

	void CD3D9TreeRenderer::SetPointLightParameters( NGin::Math::Vector3 &position )
	{
		NGin::Math::Vector4* LightPositions = new NGin::Math::Vector4[5];
		NGin::Math::Vector3* LightColors = new NGin::Math::Vector3[5];

		GetClosestLights((NGin::Math::Vector3**)&LightColors, (NGin::Math::Vector4**)&LightPositions, position);
		SetEffect->SetFloatArray("PointLightColor", (float*)LightColors, 15);
		SetEffect->SetFloatArray("PointLightPosition", (float*)LightPositions, 20);
		SetEffect->CommitChanges();

		// Cleanup
		delete[] LightPositions;
		delete[] LightColors;
	}

	void CD3D9TreeRenderer::SetSwayDirection(NGin::Math::Vector2 &direction)
	{
		SwayDirection = direction;
	}

	void CD3D9TreeRenderer::SetSwayPower(float power)
	{
		SwayAmount = power;
	}

	void CD3D9TreeRenderer::SetQualityLevel(int qualityLevel)
	{
		QualityLevel = qualityLevel;
	}
}

