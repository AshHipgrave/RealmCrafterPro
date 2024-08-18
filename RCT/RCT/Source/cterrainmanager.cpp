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
// Includes
#include <STerrainVertex.h>
#include "CTerrainManager.h"
#include "SaveLoad.h"

namespace std
{
	// Trim values using a bit of work (grr STL)
	inline std::string trim(const std::string &str)
	{
		std::string OutStr = "";
		size_t TrimStart = str.find_first_not_of(" \t");
		size_t TrimEnd = str.find_last_not_of(" \t");
		if(TrimStart == std::string::npos || TrimEnd == std::string::npos)
			OutStr = "";
		else
			OutStr = str.substr(TrimStart, TrimEnd - TrimStart + 1);

		return OutStr;
	}

	// Convert a string to a float
	inline float toFloat(const std::string &str)
	{
		return atof(str.c_str());
	}

	// Convert a string to an integer
	inline int toInt(const std::string &str)
	{
		return atoi(str.c_str());
	}

	// Mini converter class
	class miniConverter
	{
		std::string Output;
	public:

		miniConverter(std::string &str) { Output = str; }
		miniConverter(int i) { char T[32]; sprintf(T, "%i", i); Output = T; }
		miniConverter(unsigned int i) { char T[32]; sprintf(T, "%u", i); Output = T; }
		miniConverter(float i) { char T[32]; sprintf(T, "%f", i); Output = T; }

		std::string str() { return Output; }
	};

	// Convert an integer to a string
	template <typename T>
	inline std::string toString(T in)
	{
		return miniConverter(in).str();
	}

	// Convert a string to lower case
	inline std::string stringToLower(const std::string &str)
	{
		std::string Out = str;
		std::transform(Out.begin(), Out.end(), Out.begin(), ::tolower);
		return Out;
	}

	// Convert a string to upper case
	inline std::string stringToUpper(const std::string &str)
	{
		std::string Out = str;
		std::transform(Out.begin(), Out.end(), Out.begin(), ::toupper);
		return Out;
	}
}

namespace RealmCrafter
{
	namespace RCT
	{

		// Declarations
		const D3DVERTEXELEMENT9 T1VertexDecl[] =
		{
			{0, 0,  D3DDECLTYPE_FLOAT4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
			{0, 16,  D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
			/*{0, 0,  D3DDECLTYPE_UBYTE4, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
			{0, 4,  D3DDECLTYPE_FLOAT16_2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 1},*/
			D3DDECL_END()	
		};

		const D3DVERTEXELEMENT9 T1VertexLODDecl[] = 
		{
			{0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
			{0, 12, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_NORMAL, 0},
			{0, 24, D3DDECLTYPE_FLOAT2, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
			{0, 32, D3DDECLTYPE_D3DCOLOR, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_COLOR, 0},
			D3DDECL_END()
		};

		const D3DVERTEXELEMENT9 GrassVertexDecl[] =
		{
			{0, 0, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_POSITION, 0},
			{0, 12, D3DDECLTYPE_FLOAT3, D3DDECLMETHOD_DEFAULT, D3DDECLUSAGE_TEXCOORD, 0},
			D3DDECL_END()
		};


		// Constructor
		CTerrainManager::CTerrainManager(IDirect3DDevice9* device, ID3DXEffect* t1Effect, ID3DXEffect* t1Editor, ID3DXEffect* grassEffect, ID3DXEffect* t1Depth)
			: RenderTerrain(true), RenderGrass(true), RenderCollision(false)
		{
			// Copy to locals
			Device = device;
			T1Effect = t1Effect;
			T1Editor = t1Editor;
			T1DepthEffect = t1Depth;
			GrassEffect = grassEffect;
			CollisionChangedEvent = new CollisionEventHandler();

			// Create declarations
			Device->CreateVertexDeclaration(T1VertexDecl, &T1Declaration);
			Device->CreateVertexDeclaration(T1VertexLODDecl, &T1LODDeclaration);
			Device->CreateVertexDeclaration(GrassVertexDecl, &GrassDeclaration);

			// AddRef to the device
			Device->AddRef();

			EditorMode = false;
			CircleBrush[0] = true;
			CircleBrush[1] = true;
			BrushPosition[0] = NGin::Math::Vector2(50, 64);
			BrushPosition[1] = NGin::Math::Vector2(0, 0);
			BrushRadius[0] = 5.0f;
			BrushRadius[1] = 0.0f;
			BrushHardness[0] = 0.5f;
			BrushHardness[1] = 0.0f;
			EditorLock = false;
			GrassDrawDistance = 192.0f;
			GrassSwayPower = 0.0f;

			FogColor = NGin::Math::Color(255, 255, 255);
			FogData = NGin::Math::Vector2(4096, 4096);
		}

		// Destructor
		CTerrainManager::~CTerrainManager()
		{
			// Free T1 Resources
			if(T1Effect != NULL)
				T1Effect->Release();

			if(T1Declaration != NULL)
				T1Declaration->Release();
			if(T1LODDeclaration != NULL)
				T1LODDeclaration->Release();

			// Free up T1 Terrains
			for(int i = 0; i < T1List.Size(); ++i)
				delete T1List[i];

			// Clean up the index buffers that terrain allocated
			CTerrain::CleanUp();

			// Release our handle on the device
			Device->Release();
		}

		// Device was lost
		void CTerrainManager::OnDeviceLost()
		{
			if(T1Effect != 0)
				T1Effect->OnLostDevice();

			if(T1Editor != 0)
				T1Editor->OnLostDevice();

			if(GrassEffect != 0)
				GrassEffect->OnLostDevice();

			if(T1DepthEffect != 0)
				T1DepthEffect->OnLostDevice();

			for(int i = 0; i < T1List.Size(); ++i)
				T1List[i]->OnDeviceLost();
		}

		// Device was reset
		void CTerrainManager::OnDeviceReset()
		{
			if(T1Effect != 0)
				T1Effect->OnResetDevice();

			if(T1Editor != 0)
				T1Editor->OnResetDevice();

			if(T1DepthEffect != 0)
				T1DepthEffect->OnResetDevice();

			if(GrassEffect != 0)
				GrassEffect->OnResetDevice();
			for(int i = 0; i < T1List.Size(); ++i)
				T1List[i]->OnDeviceReset();
		}

		// Create a T1 Terrain
		ITerrain* CTerrainManager::CreateT1(int size)
		{
			// If its not a multiple of 32
			if((size % 32) != 0)
				return 0;

			// Create terrain
			CTerrain* T1 = new CTerrain(this, Device, size / 64);
			T1->SetTexture(0, std::string("Default"));
			T1->SetTexture(1, std::string("Default"));
			T1->SetTexture(2, std::string("Default"));
			T1->SetTexture(3, std::string("Default"));
			T1->SetTexture(4, std::string("Default"));

			// Add to local list
			T1List.Add(T1);

			// Done
			return T1;
		}

		// Save a T1
		bool CTerrainManager::SaveT1(ITerrain* terrain, std::string path)
		{
			FILE* F = fopen(path.c_str(), "wb");
			if(F == 0)
				return false;

			Save10(terrain, F);

			fclose(F);
			return true;
		}

		// Load a T1
		ITerrain* CTerrainManager::LoadT1(std::string path)
		{
			FILE* F = fopen(path.c_str(), "rb");
			if(F == 0)
				return 0;

			unsigned char VersionMajor = 0, VersionMinor = 0;
			fread(&VersionMajor, sizeof(unsigned char), 1, F);
			fread(&VersionMinor, sizeof(unsigned char), 1, F);

			if(VersionMajor == 1 && VersionMinor == 0)
				return Load10(this, F);

			
			
			OutputDebugString("Terrain version not found!\n");

			return 0;
		}

		// Update all terrains
		void CTerrainManager::Update(NGin::Math::Vector3 &position, bool updateCollisions)
		{
			CameraPosition = position;

			// Run through T1 List
			for(int i = 0; i < T1List.Size(); ++i)
				T1List[i]->Update(position, updateCollisions);

			// Run through grasses
 			for(int i = 0; i < GrassTypes.Size(); ++i)
 				GrassTypes[i]->Update();
		}

		void CTerrainManager::RenderDepth(const float* fViewProjection)
		{
			RenderDepth(fViewProjection, NGin::Math::Vector3(0,0,0));
		}

		// Render depth
		void CTerrainManager::RenderDepth(const float* fViewProjection, NGin::Math::Vector3 &offset)
		{
			// Convert float array to matrix
			D3DXMATRIX* ViewProjection = (D3DXMATRIX*)fViewProjection;

			// FrameStats to return
			FrameStatistics Stats;

			// List of chunks rendered for the grass system
			std::list<CChunk*> RenderedChunks;

			// Grab the current vertex declaration and set our own.
			// This will prevent other 'cache' code from failing
			IDirect3DVertexDeclaration9* TempDecl = NULL;
			//Device->GetVertexDeclaration(&TempDecl);
			Device->SetVertexDeclaration(T1Declaration);

			// Set the WVP of the terrain shader
			T1DepthEffect->SetMatrix("ViewProjection", ViewProjection);

			// We're only going to render one pass
			UINT EPass = 0;
			T1DepthEffect->Begin(&EPass, 0);
			T1DepthEffect->BeginPass(0);

			// Render every terrain
			for(int i = 0; i < T1List.Size(); ++i)
				T1List[i]->Render(0, T1DepthEffect, Stats, ViewProjection, NULL, &RenderedChunks, true, offset);

			// End shader
			T1DepthEffect->EndPass();
			T1DepthEffect->End();

			// If the original declaration existed, then reset it
			if(TempDecl != NULL)
			{
				Device->SetVertexDeclaration(TempDecl);

				// GetVertexDeclaration called AddRef when the Decl came
				// into our scope
				TempDecl->Release();
			}

			return;
		}

		FrameStatistics CTerrainManager::Render(int rtIndex, float* fViewProjection, const float* fLightProjection, void* iShadowMap)
		{
			return Render(rtIndex, fViewProjection, fLightProjection, iShadowMap, NGin::Math::Vector3(0, 0, 0));
		}

		// Render all terrains
		FrameStatistics CTerrainManager::Render(int rtIndex, float* fViewProjection, const float* fLightProjection, void* iShadowMap, NGin::Math::Vector3 offset)
		{
			// Convert float array to matrix
			D3DXMATRIX* ViewProjection = (D3DXMATRIX*)fViewProjection;
			D3DXMATRIX* LightProjection = (D3DXMATRIX*)fLightProjection;
			IDirect3DTexture9* ShadowMap = (IDirect3DTexture9*)iShadowMap;

			// FrameStats to return
			FrameStatistics Stats;
			if( rtIndex != 0 )
				return Stats;

			// List of chunks rendered for the grass system
			std::list<CChunk*> RenderedChunks;

			// Grab the current vertex declaration and set our own.
			// This will prevent other 'cache' code from failing
			IDirect3DVertexDeclaration9* TempDecl = NULL;
			//Device->GetVertexDeclaration(&TempDecl);
			Device->SetVertexDeclaration(T1Declaration);

			// Set the WVP of the terrain shader
			T1Effect->SetMatrix("WorldViewProjection", ViewProjection);
			T1Effect->SetFloatArray("AmbientLight", (float*)&AmbientLight, 3);
			T1Effect->SetFloatArray("DirectionalNormal", (float*)&DirectionalNormals, 9);
			T1Effect->SetFloatArray("DirectionalColor", (float*)&DirectionalColors, 9);
			T1Effect->SetFloatArray("FogColor", (float*)&FogColor, 4);
			T1Effect->SetFloatArray("FogData", (float*)&FogData, 2);
			T1Effect->SetMatrix("LightProjection", LightProjection);
			T1Effect->SetTexture("ShadowMapTex", ShadowMap);

			GrassEffect->SetMatrix("LightProjection", LightProjection);
			GrassEffect->SetTexture("ShadowMapTex", ShadowMap);

			float CollisionColor[3] = {1.0f, 1.0f, 1.0f};
			T1Effect->SetFloatArray("CollisionColor", (float*)CollisionColor, 3);

			// Render every terrain
			if(RenderTerrain)
				for(int i = 0; i < T1List.Size(); ++i)
					T1List[i]->Render(rtIndex, T1Effect, Stats, ViewProjection, NULL, &RenderedChunks, false, offset);

			if(RenderTerrain && rtIndex == 0)
			{
				Device->SetVertexDeclaration( T1LODDeclaration );

				T1Effect->SetFloatArray("Offset2", (float*)&offset, 3);

				for(int i = 0; i < T1List.Size(); ++i)
					T1List[i]->RenderLOD(T1Effect);

				Device->SetVertexDeclaration( T1Declaration );
			}

			if(T1Editor != 0 && EditorMode && RenderTerrain)
			{
				for(int i = 1; i >= 0; --i)
				{
					FrameStatistics NullStats;

					T1Editor->SetMatrix("WorldViewProjection", ViewProjection);
					T1Editor->SetMatrix("ViewProjection", ViewProjection);

					//float TRadius[2] = {BrushRadius, 10.0f};
					//float THardness[2] = {BrushHardness, 0.0f};
					//NGin::Math::Vector2 TPosition[2] = {BrushPosition, NGin::Math::Vector2(16, 16)};
					//BOOL TCircle[2] = {CircleBrush ? TRUE : FALSE, true ? TRUE : FALSE};

					//T1Editor->SetFloatArray("Radius", (float*)TRadius, 2);
					//T1Editor->SetFloatArray("Hardness", (float*)THardness, 2);
					//T1Editor->SetFloatArray("BrushPosition", (float*)TPosition, 4);
					//T1Editor->SetBoolArray("Circular", (BOOL*)TCircle, 2);

					T1Editor->SetFloat("Radius", BrushRadius[i] * 2.0f);
					T1Editor->SetFloat("Hardness", BrushHardness[i]);
					T1Editor->SetFloatArray("BrushPosition", (float*)&(BrushPosition[i]), 2);
					T1Editor->SetBool("Circular", this->CircleBrush[i]);

					float RenderRadius = (BrushRadius[i] * 2.0f) + 140.0f;
					//RenderRadius *= RenderRadius;

					float M[4];
					M[0] = BrushPosition[i].X - RenderRadius;
					M[1] = BrushPosition[i].Y - RenderRadius;
					M[2] = BrushPosition[i].X + RenderRadius;
					M[3] = BrushPosition[i].Y + RenderRadius;

					for(int j = 0; j < T1List.Size(); ++j)
						T1List[j]->Render(0, T1Editor, NullStats, ViewProjection, M, NULL, true, offset);
				}
			}

			// Clear mapping
			T1Effect->SetTexture("ShadowMapTex", NULL);
			T1Effect->CommitChanges();

			if(rtIndex == 0)
			{
				float GrassDistSq = GrassDrawDistance + 140.0f;
				GrassDistSq *= GrassDistSq;

				float OuterDistSq = GrassDrawDistance * GrassDrawDistance;
				float InnerDistSq = GrassDrawDistance - 20.0f;
				if(InnerDistSq < 0.0f)
					InnerDistSq = 0.0f;
				InnerDistSq *= InnerDistSq;

				Device->SetVertexDeclaration(GrassDeclaration);

				GrassEffect->SetMatrix("ViewProjection", ViewProjection);
				GrassEffect->SetFloatArray("CameraPosition", (FLOAT*)&CameraPosition, 3);
				GrassEffect->SetFloat("InnerDistance", InnerDistSq);
				GrassEffect->SetFloat("OuterDistance", OuterDistSq);
				GrassEffect->SetFloat("Timer", (float)((GetTickCount() % 100000)));
				GrassEffect->SetFloat("GrassSwayPower", GrassSwayPower);
				GrassEffect->SetFloatArray("GrassSwayDirection", (const FLOAT*)&GrassSwayDirection, 2);
				GrassEffect->SetFloatArray("DirectionalNormal", (float*)&DirectionalNormals, 9);
				GrassEffect->SetFloatArray("DirectionalColor", (float*)&DirectionalColors, 9);
				GrassEffect->SetFloatArray("AmbientLight", (float*)&AmbientLight, 3);

				UINT EPass = 0;
				GrassEffect->Begin(&EPass, 0);
				GrassEffect->BeginPass(0);

				if(RenderGrass)
				{
					for(int i = 0; i < GrassTypes.Size(); ++i)
					{
						GrassTypes[i]->Render(GrassEffect, RenderedChunks, CameraPosition, GrassDistSq, offset);
					}
				}

				GrassEffect->EndPass();
				GrassEffect->End();
			}
			
			// If the original declaration existed, then reset it
			if(TempDecl != NULL)
			{
				Device->SetVertexDeclaration(TempDecl);

				// GetVertexDeclaration called AddRef when the Decl came
				// into our scope
				TempDecl->Release();
			}

			// Return vital statistics
			return Stats;
		}

		CollisionEventHandler* CTerrainManager::CollisionChanged()
		{
			return CollisionChangedEvent;
		}

		void CTerrainManager::SetGrassDrawDistance(float distance)
		{
			GrassDrawDistance = distance;
		}

		float CTerrainManager::GetGrassDrawDistance()
		{
			return GrassDrawDistance;
		}

		void CTerrainManager::SetFog(NGin::Math::Color& color, NGin::Math::Vector2& data)
		{
			FogColor = color;
			if(FogData.X >= FogData.Y)
				FogData.X = FogData.Y - 1.0f;
			FogData = data;
		}

		void CTerrainManager::GetClosestLights(NGin::Math::Vector3** colors, NGin::Math::Vector4** positions, NGin::Math::Vector3 &position)
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

		void CTerrainManager::SetPointLights(void** lights, int count, int stride)
		{
			if(stride != sizeof(CTerrainManager::PointLight))
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

		void CTerrainManager::SetLightNormal(int index, NGin::Math::Vector3 &normal)
		{
			if(index < 0 || index > 2)
				return;

			DirectionalNormals[index] = normal;
			DirectionalNormals[index].Normalize();
		}

		void CTerrainManager::SetLightColor(int index, NGin::Math::Color &color)
		{
			if(index < 0 || index > 2)
				return;

			DirectionalColors[index].X = color.R;
			DirectionalColors[index].Y = color.G;
			DirectionalColors[index].Z = color.B;
		}

		void CTerrainManager::SetAmbientLight(NGin::Math::Color &color)
		{
			AmbientLight.X = color.R;
			AmbientLight.Y = color.G;
			AmbientLight.Z = color.B;
		}

		bool CTerrainManager::LoadGrassTypes(std::string path)
		{
			NGin::XMLReader* X = NGin::ReadXMLFile(path);
			if(X == 0)
				return false;

			// Read
			while(X->Read())
			{
				std::string ElementName = X->GetNodeName();
				std::transform(ElementName.begin(), ElementName.end(), ElementName.begin(), ::tolower);

				// If its a <grass> element
				if(X->GetNodeType() == NGin::XNT_Element && ElementName.compare("grass") == 0)
				{
					// Read config
					CGrassType* T = new CGrassType(this->Device);
					GrassTypes.Add(T);

					T->SetName(X->GetAttributeString("name"));
					T->SetTexture(X->GetAttributeString("path"));
					T->SetScale(NGin::Math::Vector2(X->GetAttributeFloat("sizex"), X->GetAttributeFloat("sizey")));
					T->SetOffset(X->GetAttributeFloat("offset"));
					T->SetCoverage(X->GetAttributeFloat("coverage"));
					T->SetHeightVariance(X->GetAttributeFloat("heightvariance"));
				}
			}

			delete X;

			return true;
		}

		bool CTerrainManager::SaveGrassTypes(std::string path)
		{
			FILE* F = fopen(path.c_str(), "w");
			if(F == 0)
				return false;

			std::string Base = "<?xml version=\"1.0\" ?>\r\n<grasstypes>\r\n";
			fwrite(Base.c_str(), 1, Base.length(), F);

			for(int i = 0; i < GrassTypes.Size(); ++i)
			{
				CGrassType* T = GrassTypes[i];

				std::string SizeX, SizeY, Offset, Coverage, Variance;
				SizeX = std::toString(T->GetScale().X);
				SizeY = std::toString(T->GetScale().Y);
				Offset = std::toString(T->GetOffset());
				Coverage = std::toString(T->GetCoverage());
				Variance = std::toString(T->GetHeightVariance());

				// Serialize
				std::string S = std::string("<grass name=\"") + T->GetName()
					+ "\" path=\"" + T->GetTexture()
					+ "\" sizex=\"" + SizeX
					+ "\" sizey=\"" + SizeY
					+ "\" offset=\"" + Offset
					+ "\" coverage=\"" + Coverage
					+ "\" heightvariance=\"" + Variance
					+ "\" />\r\n";

				fwrite(S.c_str(), 1, S.length(), F);
			}

			Base = "</grasstypes>";
			fwrite(Base.c_str(), 1, Base.length(), F);

			fclose(F);

			return true;
		}

		IGrassType* CTerrainManager::CreateGrassType()
		{
			CGrassType* T = new CGrassType(this->Device);
			GrassTypes.Add(T);
			return T;
		}

		IGrassType* CTerrainManager::FindGrassType(std::string name)
		{
			std::transform(name.begin(), name.end(), name.begin(), ::tolower);

			for(int i = 0; i < GrassTypes.Size(); ++i)
			{
				std::string NameLower = GrassTypes[i]->GetName();
				std::transform(NameLower.begin(), NameLower.end(), NameLower.begin(), ::tolower);

				if(NameLower.compare(name) == 0)
					return GrassTypes[i];
			}
			return 0;
		}

		void CTerrainManager::RemoveGrassType(IGrassType* type)
		{
			int Rem = -1;
			for(int i = 0; i < GrassTypes.Size(); ++i)
				if(GrassTypes[i] == ((CGrassType*)type))
					Rem = i;

			if(Rem == -1)
				return;

			delete type;
			GrassTypes.Remove(Rem);
		}

		void CTerrainManager::FetchGrassTypes(NGin::ArrayList<IGrassType*> &outTypesList)
		{
			for(int i = 0; i < GrassTypes.Size(); ++i)
				outTypesList.Add(GrassTypes[i]);
		}

		void CTerrainManager::SetBrushRadius(float radius)
		{
			BrushRadius[0] = radius;
		}

		void CTerrainManager::SetBrushHardness(float hardness)
		{
			BrushHardness[0] = hardness;
		}

		void CTerrainManager::SetBrushPosition(NGin::Math::Vector2 position)
		{
			BrushPosition[0] = position;
		}

		void CTerrainManager::SetBrushIsCircle(bool circle)
		{
			CircleBrush[0] = circle;
		}

		void CTerrainManager::SetSecondBrush(float radius, float hardness, NGin::Math::Vector2 position, bool circle)
		{
			BrushRadius[1] = radius;
			BrushHardness[1] = hardness;
			BrushPosition[1] = position;
			CircleBrush[1] = circle;
		}

		void CTerrainManager::SetEditorMode(bool editorMode)
		{
			EditorMode = editorMode;
		}

		float CTerrainManager::GetBrushRadius()
		{
			return BrushRadius[0];
		}

		float CTerrainManager::GetBrushHardness()
		{
			return BrushHardness[0];
		}

		NGin::Math::Vector2 CTerrainManager::GetBrushPosition()
		{
			return BrushPosition[0];
		}

		bool CTerrainManager::GetBrushIsCircle()
		{
			return CircleBrush[0];
		}

		bool CTerrainManager::GetEditorMode()
		{
			return EditorMode;
		}

		void CTerrainManager::SetEditorLock(bool lock)
		{
			EditorLock = lock;
		}

		bool CTerrainManager::GetEditorLock()
		{
			return EditorLock;
		}

		void CTerrainManager::SetGrassSwayPower(float power)
		{
			GrassSwayPower = power;
		}

		void CTerrainManager::SetGrassSwayDirection(NGin::Math::Vector2 direction)
		{
			GrassSwayDirection = direction;
		}

		float CTerrainManager::GetGrassSwayPower()
		{
			return GrassSwayPower;
		}

		NGin::Math::Vector2 CTerrainManager::GetGrassSwayDirection()
		{
			return GrassSwayDirection;
		}

		void CTerrainManager::SetTerrainRender( bool render )
		{
			RenderTerrain = render;
		}

		void CTerrainManager::SetGrassRender( bool render )
		{
			RenderGrass = render;
		}

		void CTerrainManager::SetCollisionRender( bool render )
		{
			RenderCollision = render;
		}

		bool CTerrainManager::GetTerrainRender() const
		{
			return RenderTerrain;
		}

		bool CTerrainManager::GetGrassRender() const
		{
			return RenderGrass;
		}

		bool CTerrainManager::GetCollisionRender() const
		{
			return RenderCollision;
		}

		// Create the terrain manager
		ITerrainManager* CreateTerrainManager(IDirect3DDevice9* device, std::string t1EffectPath, std::string t1EditorPath, std::string grassEffectPath, std::string t1DepthPath)
		{
			// Create a D3DXBuffer to catch effect errors
			ID3DXEffect* Effect, *Editor, *GrassEffect, *DepthEffect;
			ID3DXBuffer* Buffer;
			D3DXCreateBuffer(2048, &Buffer);

			// Check file
			FILE* Fp = fopen(t1EffectPath.c_str(), "r");
			if(Fp == 0)
			{
				MessageBox(0, (std::string("Could not open effect: ") + t1EffectPath).c_str(), "CreateTerrainManager", MB_ICONERROR);
				return 0;
			}
			fclose(Fp);

			// Load effect
			if(FAILED(D3DXCreateEffectFromFile(device, t1EffectPath.c_str(), NULL, NULL, 0, NULL, &Effect, &Buffer)))
			{
				// Loading failed, inform the world
				MessageBox(0, (LPCSTR)Buffer->GetBufferPointer(), 0, 0);
				return 0;
			}

			D3DXCreateBuffer(2048, &Buffer);

			// Check file
			Fp = fopen(t1EditorPath.c_str(), "r");
			if(Fp != 0)
			{
				fclose(Fp);

				// Load effect
				if(FAILED(D3DXCreateEffectFromFile(device, t1EditorPath.c_str(), NULL, NULL, 0, NULL, &Editor, &Buffer)))
				{
					// Loading failed, inform the world
					MessageBox(0, (LPCSTR)Buffer->GetBufferPointer(), 0, 0);
					return 0;
				}
			}else
			{
				Editor = 0;
			}

			// Check file
			Fp = fopen(grassEffectPath.c_str(), "r");
			if(Fp == 0)
			{
				MessageBox(0, (std::string("Could not open effect: ") + grassEffectPath).c_str(), "CreateTerrainManager", MB_ICONERROR);
				return 0;
			}
			fclose(Fp);

			// Load effect
			if(FAILED(D3DXCreateEffectFromFile(device, grassEffectPath.c_str(), NULL, NULL, 0, NULL, &GrassEffect, &Buffer)))
			{
				// Loading failed, inform the world
				MessageBox(0, (LPCSTR)Buffer->GetBufferPointer(), 0, 0);
				return 0;
			}

			// Check file
			Fp = fopen(t1DepthPath.c_str(), "r");
			if(Fp == 0)
			{
				//MessageBox(0, (std::string("Could not open effect: ") + t1DepthPath).c_str(), "CreateTerrainManager", MB_ICONERROR);
				DepthEffect = 0;
			}else
			{
				fclose(Fp);

				// Load effect
				if(FAILED(D3DXCreateEffectFromFile(device, t1DepthPath.c_str(), NULL, NULL, 0, NULL, &DepthEffect, &Buffer)))
				{
					// Loading failed, inform the world
					MessageBox(0, (LPCSTR)Buffer->GetBufferPointer(), 0, 0);
					return 0;
				}
			}

			// Clean
			//Buffer->Release();

			// Create the manager
			CTerrainManager* Mgr = new CTerrainManager(device, Effect, Editor, GrassEffect, DepthEffect);

			// Return manager
			return Mgr;
		}
	}
}
