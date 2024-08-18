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
#include "CTerrain.h"
#include "Frustum.h"

namespace RealmCrafter
{
	namespace RCT
	{

		// Initialize static members
		IndexBuffer*** CTerrain::Buffers = 0;
		IDirect3DTexture9* CTerrain::DefaultTextures[5] = {0, 0, 0, 0, 0};
		IDirect3DTexture9* CTerrain::NoHoleTexture = 0;

		// Cleanup terrain when manager dies
		void CTerrain::CleanUp()
		{
			// Destroy all the index buffers
			if(Buffers != 0)
				for(int i = 0; i < 5; ++i)
					for(int s = 0; s < 16; ++s)
						delete Buffers[i][s];

			// Destroy the detail textures
			for(int i = 0; i < 5; ++i)
				if(DefaultTextures[i] != 0)
					DefaultTextures[i]->Release();

			// Destroy nohole texture
			if(NoHoleTexture != 0)
				NoHoleTexture->Release();
		}

		// Terrain constructor
		CTerrain::CTerrain(CTerrainManager* manager, IDirect3DDevice9* device, int segments)
			: QuadTree(segments, 128.0f), PositionChanged(false)
		{
			// Copy device to local
			Device = device;
			Manager = manager;
			Tag = 0;
			for(int i = 0; i < 5; ++i)
				TextureScales[i] = 1.0f;
			Grasses[0] = 0;
			Grasses[1] = 0;
			Grasses[2] = 0;
			Grasses[3] = 0;
			Grasses[4] = 0;
			Grasses[5] = 0;
			Grasses[6] = 0;
			Grasses[7] = 0;

			LOD4_ = new LOD(device, 4);
			LOD3_ = new LOD(device, 8);

			// Static member is not created
			if(Buffers == 0)
			{
				// First Array
				Buffers = new IndexBuffer**[5];

				for(int i = 0; i < 5; ++i)
				{
					// Second Array
					Buffers[i] = new IndexBuffer*[16];

					for(int s = 0; s < 16; ++s)
					{
						// Build buffer
						Buffers[i][s] = new IndexBuffer();
						BuildIndices(i, s, Buffers[i][s]);
					}
				}
			}

			// Static default textures
			for(int i = 0; i < 5; ++i)
			{
				if(DefaultTextures[i] == 0)
				{
					// Create texture
					if(FAILED(Device->CreateTexture(2, 2, 0, D3DUSAGE_AUTOGENMIPMAP, D3DFMT_A8R8G8B8, D3DPOOL_MANAGED, &DefaultTextures[i], NULL)))
					{
						MessageBox(0, "Failed to create a default texture!", "CTerrain::CTerrain()", MB_OK | MB_ICONERROR);
					}else
					{
						// Assign different colors for each channel
						unsigned int PCol = 0xffff0000;
						if(i == 1)
							PCol = 0xff00ff00;
						if(i == 2)
							PCol = 0xff0000ff;
						if(i == 3)
							PCol = 0xffffff00;
						if(i == 4)
							PCol = 0xffff00ff;

						unsigned int* Bits;
						D3DLOCKED_RECT LockInfo;
						DefaultTextures[i]->LockRect(0, &LockInfo, NULL, NULL);

						Bits = (unsigned int*)LockInfo.pBits;
						Bits[0] = 0xffffffff;
						Bits[1] = PCol;
						Bits[2] = PCol;
						Bits[3] = 0xffffffff;

						DefaultTextures[i]->UnlockRect(0);
					}
				}

				// Setup the default texture
				Textures[i] = new TerrainTexture();
				Textures[i]->Handle = DefaultTextures[i];
				if(Textures[i]->Handle != 0)
					Textures[i]->Handle->AddRef();
				char Str[24];sprintf(Str, "%i", i);
				Textures[i]->Path = std::string("Default") + Str;
			}

			if(NoHoleTexture == 0)
			{
				// Create texture
				if(FAILED(Device->CreateTexture(64, 64, 0, D3DUSAGE_AUTOGENMIPMAP, D3DFMT_A8, D3DPOOL_MANAGED, &NoHoleTexture, NULL)))
				{
					MessageBox(0, "Failed to create nohole texture!", "CTerrain::CTerrain()", MB_OK | MB_ICONERROR);
				}else
				{
					unsigned char* Bits;
					D3DLOCKED_RECT LockInfo;
					NoHoleTexture->LockRect(0, &LockInfo, NULL, NULL);

					Bits = (unsigned char*)LockInfo.pBits;

					int i = 0;
					for(int z = 0; z < 64; ++z)
					{
						for(int x = 0; x < 64; ++x)
						{
							Bits[i] = 0xff;
							++i;
						}
					}

					NoHoleTexture->UnlockRect(0);
				}
			}

			// Create the first array dimensions
			Chunks.SetUsed(segments);
			for(int x = 0; x < segments; ++x)
			{
				// Second (Y) dimensions
				Chunks[x].SetUsed(segments);
				for(int y = 0; y < segments; ++y)
				{
					// Allocate a bunch of data for this chunk
					T1Data* Data = new T1Data[65 * 65 + 1];
					for(int i = 0; i < 65 * 65 + 1; ++i)
						Data[i].Height = 0.0f;

					// Create this chunk
					Chunks[x][y] = new CChunk(this, Device, NGin::Math::Vector2(x * 64, y * 64), Data, LOD4_, LOD3_);
				}
			}
			QuadTree.Rebuild(Position, Chunks);
		}

		// Destructor
		CTerrain::~CTerrain()
		{
			int MyIndex = -1;
			for(int i = 0; i < Manager->T1List.Size(); ++i)
			{
				if(Manager->T1List[i] == this)
					MyIndex = i;
			}

			if(MyIndex > -1)
				Manager->T1List.Remove(MyIndex);

			// Delete terrain textures
			for(int i = 0; i < 5; ++i)
				if(Textures[i] != 0)
					delete Textures[i];

			// Free every chunk
			for(int i = 0; i < Chunks.Size(); ++i)
			{
				for(int f = 0; f < Chunks[i].Size(); ++f)
					delete Chunks[i][f];
				Chunks[i].Empty();
			}
			Chunks.Empty();

			if(LOD4_)
			{
				delete LOD4_;
				LOD4_ = NULL;
			}

			if(LOD3_)
			{
				delete LOD3_;
				LOD3_ = NULL;
			}
		}

		// Set the position of the terrain
		void CTerrain::SetPosition(NGin::Math::Vector3 &position)
		{
			for(int i = 0; i < Chunks.Size(); ++i)
			{
				for(int f = 0; f < Chunks[i].Size(); ++f)
				{
					if(Chunks[i][f]->CollisionsActive)
						Chunks[i][f]->ClearCollisions();
				}
			}

			Position = position;
			PositionChanged = true;
		}

		// Get the position of the terrain
		NGin::Math::Vector3 CTerrain::GetPosition()
		{
			return Position;
		}

		ITerrainManager* CTerrain::GetManager()
		{
			return Manager;
		}

		// Device was lost
		void CTerrain::OnDeviceLost()
		{
			for(int i = 0; i < Chunks.Size(); ++i)
				for(int f = 0; f < Chunks[i].Size(); ++f)
					Chunks[i][f]->OnDeviceLost();

			LOD3_->OnDeviceLost();
			LOD4_->OnDeviceLost();
		}

		// Device was reset
		void CTerrain::OnDeviceReset()
		{
			for(int i = 0; i < Chunks.Size(); ++i)
				for(int f = 0; f < Chunks[i].Size(); ++f)
					Chunks[i][f]->OnDeviceReset();

			LOD3_->OnDeviceReset();
			LOD4_->OnDeviceReset();
		}

		// Update this terrain
		void CTerrain::Update(NGin::Math::Vector3& position, bool updateCollisions)
		{
			int UpdateCount = 0;

			// Update the LOD levels on every chunk
			for(int i = 0; i < Chunks.Size(); ++i)
				for(int f = 0; f < Chunks[i].Size(); ++f)
					Chunks[i][f]->UpdateLOD(position, updateCollisions, UpdateCount);

			// Update Chunk meshes with side LOD
			for(int i = 0; i < Chunks.Size(); ++i)
			{
				for(int f = 0; f < Chunks[i].Size(); ++f)
				{
					unsigned char Sides = 0;
					unsigned char LOD = Chunks[i][f]->GetLODLevel();

					// Modify bitmask for adjacent chunks
					if(f > 0)
						Sides |= (LOD < Chunks[i][f - 1]->GetLODLevel()) ? 1 : 0;
					if(f < Chunks[i].Size() - 1)
						Sides |= (LOD < Chunks[i][f + 1]->GetLODLevel()) ? 2 : 0;
					if(i > 0)
						Sides |= (LOD < Chunks[i - 1][f]->GetLODLevel()) ? 8 : 0;
					if(i < Chunks.Size() - 1)
						Sides |= (LOD < Chunks[i + 1][f]->GetLODLevel()) ? 4 : 0;

					// Update
					Chunks[i][f]->UpdateSides(Sides);
				}
			}

			if(PositionChanged)
			{
				QuadTree.Rebuild(Position, Chunks);
				PositionChanged = false;
			}

			if( LOD4_ )
				LOD4_->Update();
			if( LOD3_ )
				LOD3_->Update();
		}

		// Render every chunk
		void CTerrain::Render(int rtIndex, ID3DXEffect* effect, FrameStatistics &stats, D3DXMATRIX* viewProjection, float* renderBox, std::list<CChunk*>* renderChunks, bool isDepth, NGin::Math::Vector3 &offset )
		{
			// Create the Frustum used for culling
			Frustum ViewFrustum;
			ViewFrustum.FromViewProjection(*viewProjection);
			
			if(!isDepth)
			{
				// Set all of the effect textures and constants
				effect->SetTexture("Texture0", Textures[0]->Handle);
				effect->SetTexture("Texture1", Textures[1]->Handle);
				effect->SetTexture("Texture2", Textures[2]->Handle);
				effect->SetTexture("Texture3", Textures[3]->Handle);
				effect->SetTexture("Texture4", Textures[4]->Handle);
				effect->SetFloatArray("CoordScales", (float*)TextureScales, 5);

				D3DXMATRIX World;
				D3DXMatrixIdentity(&World);
				effect->SetMatrix("World", &World);
			}

			NGin::Math::Vector4* LightPositions = new NGin::Math::Vector4[5];
			NGin::Math::Vector3* LightColors = new NGin::Math::Vector3[5];

			std::list<CChunk*> RenderChunks, LodChunks;
			QuadTree.GetVisibleChunks(ViewFrustum, RenderChunks, LodChunks, offset);

			D3DXHANDLE RCTHandle = effect->GetTechniqueByName("RCT");
			D3DXHANDLE RCTLowHandle = effect->GetTechniqueByName("RCTLow");

			if(rtIndex == 1)
			{
				D3DXHANDLE TH = effect->GetTechniqueByName("RCT1");
				if(TH == NULL)
				{
					TH = effect->GetTechniqueByName("RCTLow1");
					if(TH == NULL)
						return;
					RCTHandle = TH;
				}
				RCTHandle = TH;

				TH = effect->GetTechniqueByName("RCTLow1");
				RCTLowHandle = TH;
			}

			if(!isDepth)
			{
				if(RCTHandle == NULL || effect->SetTechnique(RCTHandle) != S_OK)
					if(RCTLowHandle == NULL || effect->SetTechnique(RCTLowHandle) != S_OK)
						return;
			}

			UINT EPass = 0;
			effect->Begin(&EPass, 0);
			effect->BeginPass(0);

			int RCounts[5] = {0, 0, 0, 0, 0};

			bool LastWas = false;

			//foreachf(CIt, CChunk, RenderChunks)
			for(std::list<CChunk*>::iterator CIt = RenderChunks.begin(); CIt != RenderChunks.end(); ++CIt)
			{
				// Get Chunk
				CChunk* Chunk = *CIt;

				NGin::Math::AABB ChunkBox = Chunk->GetBoundingBox();
				if(renderBox != NULL)
				{
					if(ChunkBox._Min.X < renderBox[0] || ChunkBox._Min.Z < renderBox[1]
					|| ChunkBox._Max.X > renderBox[2] || ChunkBox._Max.Z > renderBox[3])
					{
						continue;
					}
				}

				if(Manager->GetCollisionRender() && Chunk->CollisionsActive != LastWas)
				{
					float CollisionColor[3] = {1.0f, 1.0f, 1.0f};
					if(Chunk->CollisionsActive)
					{
						CollisionColor[0] = 0.0f;
						CollisionColor[2] = 0.0f;
					}
					effect->SetFloatArray("CollisionColor", (float*)CollisionColor, 3);
					LastWas = Chunk->CollisionsActive;
				}

				if(!isDepth)
				{
					if(Chunk->GetLODLevel() <= 1)
					{
						Manager->GetClosestLights((NGin::Math::Vector3**)&LightColors, (NGin::Math::Vector4**)&LightPositions, ChunkBox._Min + NGin::Math::Vector3(32, 0, 32));
						effect->SetFloatArray("PointLightColor", (float*)LightColors, 15);
						effect->SetFloatArray("PointLightPosition", (float*)LightPositions, 20);
					}
				}

				RCounts[Chunk->GetLODLevel()]++;
				Chunk->Render(&Buffers, effect, stats, isDepth, offset);

				if(renderChunks != NULL)
					renderChunks->push_back(Chunk);

				if(!isDepth)
					++stats.ChunksDrawn;
			}

			effect->EndPass();
			effect->End();

			if(!isDepth)
			{
				if(RCTLowHandle == NULL || effect->SetTechnique(RCTLowHandle) != S_OK)
					return;
			}

			effect->Begin(&EPass, 0);
			effect->BeginPass(0);

			for(std::list<CChunk*>::iterator LIt = LodChunks.begin(); LIt != LodChunks.end(); ++LIt)
			{
				// Get Chunk
				CChunk* Chunk = *LIt;

				NGin::Math::AABB ChunkBox = Chunk->GetBoundingBox();
				if(renderBox != NULL)
				{
					if(ChunkBox._Min.X < renderBox[0] || ChunkBox._Min.Z < renderBox[1]
					|| ChunkBox._Max.X > renderBox[2] || ChunkBox._Max.Z > renderBox[3])
					{
						continue;
					}
				}

				if(Manager->GetCollisionRender() && Chunk->CollisionsActive != LastWas)
				{
					float CollisionColor[3] = {1.0f, 1.0f, 1.0f};
					if(Chunk->CollisionsActive)
					{
						CollisionColor[0] = 0.0f;
						CollisionColor[2] = 0.0f;
					}
					effect->SetFloatArray("CollisionColor", (float*)CollisionColor, 3);
					LastWas = Chunk->CollisionsActive;
				}

				RCounts[Chunk->GetLODLevel()]++;
				Chunk->Render(&Buffers, effect, stats, isDepth, offset);

				if(renderChunks != NULL)
					renderChunks->push_back(Chunk);

				if(!isDepth)
					++stats.ChunksDrawn;
			}

			effect->EndPass();
			effect->End();

			// Cleanup
			delete LightPositions;
			delete LightColors;
		}

		void CTerrain::RenderLOD(ID3DXEffect* effect)
		{
			D3DXHANDLE RCTLODHandle = effect->GetTechniqueByName("RCTLOD");

			// LOD Meshes
			if( LOD4_ && LOD3_ && RCTLODHandle != NULL )
			{
				effect->SetTechnique( RCTLODHandle );

				// Set all of the effect textures and constants
				effect->SetTexture("Texture0", Textures[0]->Handle);
				effect->SetTexture("Texture1", Textures[1]->Handle);
				effect->SetTexture("Texture2", Textures[2]->Handle);
				effect->SetTexture("Texture3", Textures[3]->Handle);
				effect->SetTexture("Texture4", Textures[4]->Handle);
				effect->SetFloatArray("CoordScales", (float*)TextureScales, 5);

				UINT nPasses = 0;
				effect->Begin(&nPasses, 0);
				effect->BeginPass( 0 );

				LOD4_->Draw(effect);
				LOD3_->Draw(effect);

				effect->EndPass();
				effect->End();
			}

		}

		void CTerrain::SetGrassType(int index, std::string type)
		{
			if(index < 0 || index > 7)
				return;

			CGrassType* T = (CGrassType*)Grasses[index];
			if(T != 0)
			{
				for(int i = 0; i < Chunks.Size(); ++i)
				{
					for(int f = 0; f < Chunks[i].Size(); ++f)
					{
						T->RemoveGrass(Chunks[i][f]);
					}
				}
			}

			Grasses[index] = Manager->FindGrassType(type);
			//RebuildGrass();?
		}
		
		std::string CTerrain::GetGrassType(int index)
		{
			if(index < 0 || index > 7)
				return "";

			if(Grasses[index] == 0)
				return "";

			return Grasses[index]->GetName();
		}
		
		int CTerrain::GetGrassIndex(IGrassType* type)
		{
			for(int i = 0; i < 8; ++i)
				if(Grasses[i] == type)
					return i;
			return -1;
		}

		// Get the size of the terrain (used on its creation)
		int CTerrain::GetSize()
		{
			return Chunks.Size() * 64;
		}

		// Set a void* tag for this terrain
		void CTerrain::SetTag(void* tag)
		{
			Tag = tag;
		}

		// Get a void* tag for this terrain
		void* CTerrain::GetTag()
		{
			return Tag;
		}

		// Set the scale of the terrain texture
		void CTerrain::SetTextureScale(int index, float scale)
		{
			if(index < 0 || index > 4)
				return;

			TextureScales[index] = scale;
		}

		// Get the scale of the terrain texture
		float CTerrain::GetTextureScale(int index)
		{
			if(index < 0 || index > 4)
				return 1.0f;

			return TextureScales[index];
		}

		// Set a texture index
		bool CTerrain::SetTexture(int index, std::string& path)
		{
			// Check bounds, above all
			if(index < 0 || index > 4)
				return false;

			// Create the new texture container
			TerrainTexture* T = new TerrainTexture();
			T->Path = path;

			// Load it
			if(FAILED(D3DXCreateTextureFromFileA(Device, (LPCSTR)path.c_str(), &T->Handle)))
			{
				// Loading failed; free our resource
				std::string Err = std::string("Failed to load terrain texture: ") + path + std::string("\n");
				OutputDebugString(Err.c_str());
				delete T;
				return false;
			}

			// Delete old texture
			delete Textures[index];

			// Copy in new texture
			Textures[index] = T;

			// Done
			return true;
		}

		// Get a texture
		std::string CTerrain::GetTexture(int index)
		{
			// Check bounds, above all
			if(index < 0 || index > 4)
				return "INVALIDINDEX";

			return Textures[index]->Path;
		}

		// Set the grass mask of a position
		void CTerrain::SetGrass(int x, int y, unsigned int grass)
		{
			// Chunk X/Y
			int cx = 0;
			int cy = 0;

			// Bring global positions into local positions
			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// Verify chunk bounds
			if(cx >= Chunks.Size())
				return;
			if(cy >= Chunks[cx].Size())
				return;

			// Set height
			Chunks[cx][cy]->SetGrass(x, y, grass);
		}

		// Get the grass mask of a position
		unsigned char CTerrain::GetGrass(int x, int y)
		{
			// Chunk X/Y
			int cx = 0;
			int cy = 0;

			// Bring global positions into local positions
			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// Verify chunk bounds
			if(cx >= Chunks.Size())
				return false;
			if(cy >= Chunks[cx].Size())
				return false;

			// Set height
			return Chunks[cx][cy]->GetGrass(x, y);
		}

		// Set exclusion of a position
		void CTerrain::SetExclusion(int x, int y, bool exclude)
		{
			// Chunk X/Y
			int cx = 0;
			int cy = 0;

			// Bring global positions into local positions
			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// Verify chunk bounds
			if(cx >= Chunks.Size())
				return;
			if(cy >= Chunks[cx].Size())
				return;

			// Set height
			Chunks[cx][cy]->SetExclusion(x, y, exclude);
		}

		// Get exclusion of a position
		bool CTerrain::GetExclusion(int x, int y)
		{
			// Chunk X/Y
			int cx = 0;
			int cy = 0;

			// Bring global positions into local positions
			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// Verify chunk bounds
			if(cx >= Chunks.Size())
				return false;
			if(cy >= Chunks[cx].Size())
				return false;

			// Set height
			return Chunks[cx][cy]->GetExclusion(x, y) > 0;
		}

		// Set the height globally
		void CTerrain::SetHeight(int x, int y, float height)
		{
			// Chunk X/Y
			int cx = 0;
			int cy = 0;

			// Bring global positions into local positions
			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// SetHeight locally
			return SetHeight(cx, cy, x, y, height);
		}

		// Set the height locally
		void CTerrain::SetHeight(int chunkX, int chunkY, int x, int y, float height)
		{
			// Verify chunk bounds
			if(chunkX >= Chunks.Size())
				return;
			if(chunkY >= Chunks[chunkX].Size())
				return;

			// Set height
			Chunks[chunkX][chunkY]->SetHeight(x, y, height);
		}

		// Get the color of a vertex
		T1TexCol CTerrain::GetColorChunk(NGin::Math::Vector2 &position, int x, int y)
		{
			// Modify the position into global space
			x += (int)position.X;
			y += (int)position.Y;

			// Convert position back to local
			int cx = 0;
			int cy = 0;

			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// Check bounds
			if(cx >= Chunks.Size())
				return T1TexCol();
			if(cy >= Chunks[cx].Size())
				return T1TexCol();

			// Get color
			return Chunks[cx][cy]->GetColor(x, y);
		}

		// Get the color of a vertex
		void CTerrain::SetColorChunk(NGin::Math::Vector2 &position, int x, int y, T1TexCol color)
		{
			// Modify the position into global space
			x += (int)position.X;
			y += (int)position.Y;

			// Convert position back to local
			int cx = 0;
			int cy = 0;

			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// Check bounds
			if(cx >= Chunks.Size())
				return;
			if(cy >= Chunks[cx].Size())
				return;

			// Get color
			Chunks[cx][cy]->SetColor(x, y, color);
		}

		// Get the 'half' height of a chunk
		float CTerrain::GetHeightChunk( NGin::Math::Vector2 &position, int x, int y )
		{
			// Convert position to global intpos
			x += (int)position.X;
			y += (int)position.Y;

			// Get height float
			return GetHeight(x, y);
		}

		// Get a global height
		float CTerrain::GetHeight(int x, int y)
		{
			// Convert position to local
			int cx = 0;
			int cy = 0;

			while(x > 64)
			{
				++cx;
				x -= 64;
			}

			while(y > 64)
			{
				++cy;
				y -= 64;
			}

			// Get local height
			return GetHeight(cx, cy, x, y);
		}

		// Get a local height
		float CTerrain::GetHeight(int chunkX, int chunkY, int x, int y)
		{
			// Check bounds
			if(chunkX >= Chunks.Size())
				return 0.0f;
			if(chunkY >= Chunks[chunkX].Size())
				return 0.0f;

			// Return height
			return Chunks[chunkX][chunkY]->GetHeight(x, y);
		}

		// Create a triangle
		inline void Triangle(unsigned short* Indices, int* Index, int V0, int V1, int V2)
		{
			int I = Index[0];
			Indices[I+0] = V2;
			Indices[I+1] = V1;
			Indices[I+2] = V0;
			Index[0] = I + 3;
		}

		// Build an index buffer
		void CTerrain::BuildIndices(int lod, char sides, IndexBuffer* masterIndexBuffer)
		{
			IDirect3DIndexBuffer9** indexBuffer = &(masterIndexBuffer->Buffer);
			int* indexCount = &(masterIndexBuffer->IndexCount);

			bool Top, Right, Bottom, Left;
			Top = Right = Bottom = Left = false;

			Top = sides & 1;
			Bottom = sides & 2;
			Right = sides & 4;
			Left = sides & 8;

			int MainPatchSegs = CChunk::PatchSize[lod] - 2;
			int MainPatchVerts = ((MainPatchSegs + 1) * (MainPatchSegs + 1));
			int LODPatchVerts = 0;
			int LODPatchTris = 0;
			int LODPatchMin = CChunk::PatchSize[lod] / 2;
			int LODPatchMax = CChunk::PatchSize[lod];

			int MainPatchTris = MainPatchSegs * MainPatchSegs * 2;
			int LODTrisMin = (LODPatchMin - 1) * 3 + 1;
			int LODTrisMax = MainPatchSegs * 2 + 2;

			int LODScale = CChunk::PatchSkip[lod];

			if(Top)
			{
				LODPatchVerts += LODPatchMin;
				LODPatchTris += LODTrisMin;
			}else
			{
				LODPatchVerts += LODPatchMax;
				LODPatchTris += LODTrisMax;
			}

			if(Right)
			{
				LODPatchVerts += LODPatchMin;
				LODPatchTris += LODTrisMin;
			}else
			{
				LODPatchVerts += LODPatchMax;
				LODPatchTris += LODTrisMax;
			}

			if(Bottom)
			{
				LODPatchVerts += LODPatchMin;
				LODPatchTris += LODTrisMin;
			}else
			{
				LODPatchVerts += LODPatchMax;
				LODPatchTris += LODTrisMax;
			}

			if(Left)
			{
				LODPatchVerts += LODPatchMin;
				LODPatchTris += LODTrisMin;
			}else
			{
				LODPatchVerts += LODPatchMax;
				LODPatchTris += LODTrisMax;
			}

			Device->CreateIndexBuffer(sizeof(short) * (LODPatchTris + MainPatchTris) * 3, D3DUSAGE_WRITEONLY, D3DFMT_INDEX16, D3DPOOL_MANAGED, indexBuffer, NULL);
			int VCount = LODPatchVerts + MainPatchVerts;
			int ICount = LODPatchTris + MainPatchTris;

			int Index = 0;
			int TopIndex = 0, LeftIndex = 0, RightIndex = 0, BottomIndex = 0, MiddleIndex = 0;

			TopIndex = Index;
			if(Top)
				for(int i = 0; i < LODPatchMax; i+=2)
					++Index;
			else
				for(int i = 0; i < LODPatchMax; ++i)
					++Index;

			RightIndex = Index;
			if(Right)
				for(int i = 0; i < LODPatchMax; i += 2)
					++Index;
			else
				for(int i = 0; i < LODPatchMax; ++i)
					++Index;

			BottomIndex = Index;
			if(Bottom)
				for(int i = LODPatchMax; i > 0; i-=2)
					++Index;
			else
				for(int i = LODPatchMax; i > 0; --i)
					++Index;

			LeftIndex = Index;
			if(Left)
				for(int i = LODPatchMax; i > 0; i-=2)
					++Index;
			else
				for(int i = LODPatchMax; i > 0; --i)
					++Index;

			MiddleIndex = Index;
			for(int z = 0; z < MainPatchSegs + 1; ++z)
				for(int x = 0; x < MainPatchSegs + 1; ++x)
					++Index;

			unsigned short* Indices;
			Index = 0;
			(*indexBuffer)->Lock(0, 0, (void**)&Indices, 0);

			if(Top)
			{
				for(int i = 0; i < LODPatchMin - 1; ++i)
				{
					int I = TopIndex + i;
					int M = MiddleIndex + (i * 2);
					Triangle(Indices, &Index, I, I + 1, M);
					Triangle(Indices, &Index, I + 1, M + 1, M);
					Triangle(Indices, &Index, I + 1, M + 2, M + 1);
				}
				Triangle(Indices, &Index, MiddleIndex + LODPatchMax - 2, TopIndex + LODPatchMin - 1, RightIndex);
			}else
			{
				Triangle(Indices, &Index, TopIndex, TopIndex + 1, MiddleIndex);
				for(int i = 1; i < LODPatchMax - 1; ++i)
				{
					int I = TopIndex + (i);
					int M = MiddleIndex + i - 1;
					Triangle(Indices, &Index, I, I + 1, M + 1);
					Triangle(Indices, &Index, I, M + 1, M);
				}
				Triangle(Indices, &Index, TopIndex + LODPatchMax - 1, TopIndex + LODPatchMax, MiddleIndex + LODPatchMax - 2);
			}

			if(Right)
			{
				for(int i = 0; i < LODPatchMin - 1; ++i)
				{
					int I = RightIndex + i;
					int M0 = MiddleIndex + ((LODPatchMax - 1) * ((i * 2)+0)) + (LODPatchMax - 2);
					int M1 = MiddleIndex + ((LODPatchMax - 1) * ((i * 2)+1)) + (LODPatchMax - 2);
					int M2 = MiddleIndex + ((LODPatchMax - 1) * ((i * 2)+2)) + (LODPatchMax - 2);
					Triangle(Indices, &Index, I, I + 1, M0);
					Triangle(Indices, &Index, I + 1, M1, M0);
					Triangle(Indices, &Index, I + 1, M2, M1);
				}
				Triangle(Indices, &Index, MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - 1, RightIndex + LODPatchMin - 1, BottomIndex);
			}else
			{
				Triangle(Indices, &Index, RightIndex, RightIndex + 1, MiddleIndex + LODPatchMax - 2);
				for(int i = 1; i < LODPatchMax - 1; ++i)
				{
					int I = RightIndex + (i);
					int M = MiddleIndex + ((LODPatchMax - 1) * (i)) - 1;
					Triangle(Indices, &Index, I, I + 1, M);
					Triangle(Indices, &Index, M, I + 1, M + (LODPatchMax - 1));
				}
				Triangle(Indices, &Index, MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - 1, RightIndex + LODPatchMax - 1, RightIndex + LODPatchMax);
			}

			if(Bottom)
			{
				for(int i = 0; i < LODPatchMin - 1; ++i)
				{
					int I = BottomIndex + i;
					int M0 = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - (i*2) - 1;
					int M1 = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - (i*2) - 2;
					int M2 = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - (i*2) - 3;
					Triangle(Indices, &Index, I, I + 1, M0);
					Triangle(Indices, &Index, I + 1, M1, M0);
					Triangle(Indices, &Index, I + 1, M2, M1);
				}
				Triangle(Indices, &Index, MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - LODPatchMax + 1, BottomIndex + LODPatchMin - 1, LeftIndex);
			}else
			{
				Triangle(Indices, &Index, MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - 1, BottomIndex, BottomIndex + 1);
				for(int i = 1; i < LODPatchMax - 1; ++i)
				{
					int I = BottomIndex + (i);
					int M = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - i;
					Triangle(Indices, &Index, I, I + 1, M - 1);
					Triangle(Indices, &Index, I, M - 1, M);
				}
				Triangle(Indices, &Index, MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 2)), BottomIndex + LODPatchMax - 1, BottomIndex + LODPatchMax);
			}

			if(Left)
			{
				for(int i = 0; i < LODPatchMin - 1; ++i)
				{
					int I = LeftIndex + i;
					int M0 = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - (LODPatchMax - 1) - ((LODPatchMax - 1) * (i*2));
					int M1 = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - (LODPatchMax - 1) - ((LODPatchMax - 1) * ((i*2)+1));
					int M2 = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 1)) - (LODPatchMax - 1) - ((LODPatchMax - 1) * ((i*2)+2));
					Triangle(Indices, &Index, I, I + 1, M0);
					Triangle(Indices, &Index, I + 1, M1, M0);
					Triangle(Indices, &Index, I + 1, M2, M1);
				}
				Triangle(Indices, &Index, MiddleIndex, LeftIndex + LODPatchMin - 1, TopIndex);
			}else
			{
				Triangle(Indices, &Index, MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - 2)), LeftIndex, LeftIndex + 1);
				for(int i = 1; i < LODPatchMax - 1; ++i)
				{
					int I = LeftIndex + (i);
					int M = MiddleIndex + ((LODPatchMax - 1) * (LODPatchMax - i - 1));
					Triangle(Indices, &Index, I, I + 1, M);
					Triangle(Indices, &Index, M, I + 1, M - (LODPatchMax - 1));
				}
				Triangle(Indices, &Index, MiddleIndex, LeftIndex + LODPatchMax - 1, TopIndex);
			}


			int ZJump = (LODPatchMax -1);
			for(int z = 0; z < MainPatchSegs; ++z)
			{
				for(int x = 0; x < MainPatchSegs; ++x)
				{
					int XOffset = MiddleIndex + x + (ZJump * z);
					int ZOffset = MiddleIndex + x + (ZJump * (z + 1));
					Triangle(Indices, &Index, XOffset, XOffset + 1, ZOffset + 1);
					Triangle(Indices, &Index, XOffset, ZOffset + 1, ZOffset);
				}
			}

			if(masterIndexBuffer->Indices != 0)
				delete masterIndexBuffer->Indices;

			masterIndexBuffer->Indices = new unsigned short[(LODPatchTris + MainPatchTris) * 3];

			memcpy(masterIndexBuffer->Indices, Indices, sizeof(short) * (LODPatchTris + MainPatchTris) * 3);

			(*indexBuffer)->Unlock();
			*indexCount = ICount * 3;
		}
	}
}

