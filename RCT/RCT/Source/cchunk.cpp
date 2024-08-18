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
#include "CChunk.h"
#include <vector3.h>

namespace RealmCrafter
{
	namespace RCT
	{

		// Static declarations
		int CChunk::PatchSize[5] = {64, 32, 16, 8, 4};
		int CChunk::PatchSkip[5] = {1, 2, 4, 8, 16};
		float CChunk::PatchScale[5] = {1.0f, 2.0f, 4.0f, 8.0f, 16.0f};
		//float CChunk::LODDistances[5] = {128.0f, 256.0f, 512.0f, 1024.0f, 200000000.0f};
		float CChunk::LODDistances[5] = {64.0f, 128.0f, 256.0f, 512.0f, 200000000.0f};

		// Constructor
		CChunk::CChunk(CTerrain* terrain, IDirect3DDevice9* device, NGin::Math::Vector2 &position, T1Data* data, RCT::LOD* lod4, RCT::LOD* lod3)
			: LOD4_(lod4), LOD3_(lod3)
		{
			// Defaults
			LOD = 0;
			Sides = 1;
			State = ECS_Dead;
			Vertices = 0;
			SplatTex = 0;
			HoleTex = 0;
			TexLOD = 254;
			VertexCount = 0;
			DELSENT = false;
			CollisionsActive = false;
			GrassActive = false;
			GrassInvalidation = 0;

			// Copies
			Terrain = terrain;
			Data = data;
			ChunkPosition = position;
			Device = device;

			// Rebuild box
			RecalculateBoundingBox();
		}

		// Destructor
		CChunk::~CChunk()
		{
			// Free LOD instance
			if( LOD4_ )
				LOD4_->RemoveChunk(this);
			if( LOD3_ )
				LOD3_->RemoveChunk(this);

			// Free collision instance
			ClearCollisions();

			for(int i = 0; i < 8; ++i)
			{
				CGrassType* T = (CGrassType*)this->Terrain->Grasses[i];

				if(T != 0)
					T->RemoveGrass(this);
			}

			// Free the splat texture
			if(SplatTex != 0)
				SplatTex->Release();
			SplatTex = 0;

			// Free vertices
			if(Vertices != 0)
				Vertices->Release();
			Vertices = 0;

			// Free data pointer
			delete Data;
			Data = 0;
		}

		// Device was lost
		void CChunk::OnDeviceLost()
		{
		}

		// Device was reset
		void CChunk::OnDeviceReset()
		{
		}

		// Get terrain
		CTerrain* CChunk::GetTerrain()
		{
			return Terrain;
		}

		// Rebuild bounding box
		void CChunk::RecalculateBoundingBox()
		{
			// Min/Max height
			float MinY, MaxY;
			MinY = MaxY = 0.0f;

			// Go through every height
			for(int i = 0; i < 65 * 65 + 1; ++i)
			{
				// If it is above/below what we have, then use it
				if(Data[i].Height < MinY)
					MinY = Data[i].Height;
				if(Data[i].Height > MaxY)
					MaxY = Data[i].Height;
			}

			// Make box
			BoundingBox = NGin::Math::AABB(NGin::Math::Vector3(ChunkPosition.X * 2.0f, MinY, ChunkPosition.Y * 2.0f), NGin::Math::Vector3((ChunkPosition.X + 64.0f)  * 2.0f, MaxY, (ChunkPosition.Y + 64.0f) * 2.0f));
		}

		// Get the bounding box
		NGin::Math::AABB CChunk::GetBoundingBox()
		{
			NGin::Math::AABB TAABB(BoundingBox.Min() + Terrain->GetPosition(), BoundingBox.Max() + Terrain->GetPosition());
			return TAABB;
		}

		// Get vertex exclusion value
		unsigned char CChunk::GetExclusion(int x, int y)
		{
			if(x < 0 || y < 0)
				return 0xff;

			for(unsigned int i = 0; i < ExclusionData.Size(); ++i)
				if(ExclusionData[i] == (y * 65) + x)
					return 0x00;
			
			return 0xff;
		}

		// Set exclusion of a position
		void CChunk::SetExclusion(int x, int y, bool erase)
		{
			if(x < 0 || y < 0)
				return;

			int Found = 0;

			for(unsigned int i = 0; i < ExclusionData.Size(); ++i)
				if(ExclusionData[i] == (y * 65) + x)
					if(!erase) // We aren't erasing data, we are "creating" it
					{
						ExclusionData.Remove(i);
						Found = 1;
					}else
						Found = 2;

			if(Found == 1)
			{
				if(HoleTex != 0)
					HoleTex->Release();

				// Kill chunk for next update
				if(State == ECS_Alive)
					State = ECS_ReBuild;
				CollisionsActive = false;

				HoleTex = 0;
				return;

			}else if(Found == 2)
			{
				return;
			}

			if(!erase)
				return;

			// Not found
			ExclusionData.Add(y * 65 + x);

			// Kill chunk for next update
			if(State == ECS_Alive)
				State = ECS_ReBuild;
			CollisionsActive = false;

			if(HoleTex != 0)
				HoleTex->Release();

			HoleTex = 0;
			return;
		}

		// Get a color
		T1TexCol CChunk::GetColor(int x, int y)
		{
			if(x < 0 || y < 0)
				return T1TexCol();

			return Data[(y * 65) + x].Splat;
		}

		// Set a color
		void CChunk::SetColor(int x, int y, T1TexCol color)
		{
			if(x < 0 || y < 0)
				return;

			Data[(y * 65) + x].Splat = color;
			TexLOD = 254;

			// Kill chunk for next update
			if(State == ECS_Alive)
				State = ECS_ReBuild;
		}

		// Sets the grass bitmask
		void CChunk::SetGrass(int x, int y, unsigned char grass)
		{
			if(x < 0 || y < 0)
				return;

			int Offset = (y * 65) + x;

			Data[Offset].GrassTypes = grass;

			GrassInvalidation = 0xff;
		}

		// Gets the grass
		unsigned char CChunk::GetGrass(int x, int y)
		{
			if(x < 0 || y < 0)
				return 0;

			return Data[(y * 65) + x].GrassTypes;
		}

		// Get a height
		float CChunk::GetHeight(int x, int y)
		{
			if(x < 0 || y < 0)
				return 0.0f;

			return Data[(y * 65) + x].Height;
		}

		// Set a height
		void CChunk::SetHeight(int x, int y, float height)
		{
			if(x < 0 || y < 0)
				return;

			Data[(y * 65) + x].Height = height;
			
			// Kill chunk for next update
			if(State == ECS_Alive)
				State = ECS_ReBuild;
			CollisionsActive = false;
		}

		// Get detail
		unsigned char CChunk::GetLODLevel()
		{
			return LOD;
		}

		// Secondary Update
		void CChunk::UpdateSides(unsigned char sides)
		{
			// If there was a change
			if(sides != Sides || State == ECS_ReBuild)
			{
				

				// Remake box
				RecalculateBoundingBox();

				// Reset data and remake vertices
				Sides = sides;
				State = ECS_Alive;
				ReBuild();
			}
		}

		// Update level of detail
		void CChunk::UpdateLOD(NGin::Math::Vector3& position, bool updateCollisions, int &updateCount)
		{
			// Copy position
			LastPosition = position;

			// Get squared distance to chunk center
			float XDist = (((ChunkPosition.X * 2.0f) + Terrain->GetPosition().X) - position.X) + (32.0f * 2.0f);
			float YDist = (((ChunkPosition.Y * 2.0f) + Terrain->GetPosition().Z) - position.Z) + (32.0f * 2.0f); // JB the 32 here was doubled from 16
			float DistanceSq = XDist * XDist + YDist * YDist;
			
			// Find next LOD level
			int tLOD = 0;
			for(int i = 3; i >= 0; --i)
			{
				if(DistanceSq > (LODDistances[i] * 2.0f) * (LODDistances[i] * 2.0f))
				{
					tLOD = i + 1;
					break;
				}
			}

			// If there was a change, set state data for next update
			if(tLOD != LOD || State == ECS_Dead)
			{
				LOD = tLOD;
				State = ECS_ReBuild;
			}

			// If collisions need updating
			if(updateCollisions && updateCount < 10)
			{
				bool EditorMode = Terrain->GetManager()->GetEditorMode();
				if((EditorMode && DistanceSq < 1500.0f * 1500.0f) ||
					(EditorMode == false && DistanceSq < 500.0f * 500.0f))
				{
					if(DistanceSq < 500.0f * 500.0f)
					{
						if(CollisionsActive == false && Terrain->GetManager()->GetEditorLock() == false)
						{
							ReBuildCollisions(true);
							CollisionsActive = true;
							++updateCount;
						}
					}else if(EditorMode)
					{
						if(CollisionsActive == false && Terrain->GetManager()->GetEditorLock() == false)
						{
							ReBuildCollisions(false);
							CollisionsActive = true;
							++updateCount;
						}
					}
				}else
				{
					if(CollisionsActive == true)
					{
						ClearCollisions();
						CollisionsActive = false;
						++updateCount;
					}
				}
			}


			float GrassDrawDistance = Terrain->GetManager()->GetGrassDrawDistance() + 70.0f;
			GrassDrawDistance *= GrassDrawDistance;

			// If grass needs updating
			if(DistanceSq < GrassDrawDistance)
			{
				if(GrassActive == false)
				{
					GrassInvalidation = 0xff;
					GrassActive = true;
				}

				for(int i = 0; i < 8; ++i)
				{
					CGrassType* T = (CGrassType*)this->Terrain->Grasses[i];

					if(T != 0)
					{
						T->CreateGrass(this);

						unsigned char Pwr = (unsigned char)(int)pow(2.0f, (float)i);
						if((GrassInvalidation & Pwr) > 0)
							T->Invalidate(this);
					}
				}
				GrassInvalidation = 0;
			}else
			{
				if(GrassActive == true)
				{
					for(int i = 0; i < 8; ++i)
					{
						CGrassType* T = (CGrassType*)this->Terrain->Grasses[i];

						if(T != 0)
							T->RemoveGrass(this);
					}
				}

				GrassActive = false;
			}



		}

		// Render this chunk
		void CChunk::Render( IndexBuffer**** buffers, ID3DXEffect* effect, FrameStatistics &stats, bool isDepth, NGin::Math::Vector3 &offset )
		{
			if( LOD >= 3 )
			{
				return;
			}

			// Cast pointer to out usable array
			IndexBuffer*** rBuffers = *buffers;

			// If we can render
			if(Vertices != 0)
			{
				// Get chunk distance and LOD distance
				float XDist = (((ChunkPosition.X * 2.0f) + Terrain->GetPosition().X) - LastPosition.X) + (32.0f * 2.0f);
				float YDist = (((ChunkPosition.Y * 2.0f) + Terrain->GetPosition().Z) - LastPosition.Y) + (32.0f * 2.0f); //JB The 32 from here was 16*2
				float DistanceSq = XDist * XDist + YDist * YDist;
				float LODSq = (LODDistances[LOD] * 2.0f) * (LODDistances[LOD] * 2.0f);

				if(LOD > 10)
					return;

				// Put into 0-60 range
				DistanceSq -= LODSq - (3600.0f);
				DistanceSq /= 3600.0f;

				// Clamp bounds
				if(DistanceSq < 0.0f)
					DistanceSq = 0.0f;
				if(DistanceSq > 1.0f)
					DistanceSq = 1.0f;

				NGin::Math::Vector3 vChunkPosition = NGin::Math::Vector3((ChunkPosition.X * 2.0f), 0.0f, (ChunkPosition.Y * 2.0f)) + Terrain->GetPosition();

				// Set constants and commit
				if(!isDepth)
				{
					effect->SetTexture("Splat", SplatTex);
					effect->SetTexture("Holes", HoleTex == 0 ? CTerrain::NoHoleTexture : HoleTex);
				}

				effect->SetFloatArray("Offset", (float*)&vChunkPosition, 3);
				effect->SetFloatArray("Offset2", (float*)&offset, 3);
				effect->SetFloat("ChunkInterp", DistanceSq);
				effect->CommitChanges();

				// Set indices
				Device->SetIndices(rBuffers[LOD][Sides]->Buffer);

				// Set vertex buffer
				Device->SetStreamSource(0, Vertices, 0, sizeof(T1Vertex));

				// Draw chunk
				Device->DrawIndexedPrimitive(D3DPT_TRIANGLELIST, 0, 0, VertexCount, 0, rBuffers[LOD][Sides]->IndexCount / 3);
				
				// Update stats
				stats.TrianglesDrawn += rBuffers[LOD][Sides]->IndexCount;
			}
		}

		void CChunk::CalculateNormal(int x, int y, T1Vertex* vertex)
		{
			D3DXPLANE T0, T1, T2, T3;
			D3DXVECTOR3 N, S, E, W, NE, SW, X, nT0, nT1, nT2, nT3;

			// Tesselation of terrain vertices
			//     N---NE
			//    /|  /|
			//   / |2/3|
			//  /  |/  |
			// W---X---E
			// |  /|  /
			// |0/1| /
			// |/  |/
			// SW--S

			x += ChunkPosition.X;
			y += ChunkPosition.Y;

			float fx = x;
			float fy = y;

			// Get positions of local vertices (based upon tesselation)
			// Use D3DX vectors as they are superior
			N.x = fx;
			N.y = Terrain->GetHeight(x, y + 1);
			N.z = fy + 1;

			S.x = fx;
			S.y = Terrain->GetHeight(x, y - 1);
			S.z = fy - 1;
			
			E.x = fx + 1;
			E.y = Terrain->GetHeight(x + 1, y);
			E.z = fy;
			
			W.x = fx - 1;
			W.y = Terrain->GetHeight(x - 1, y);
			W.z = fy;
			
			NE.x = fx + 1;
			NE.y = Terrain->GetHeight(x + 1, y + 1);
			NE.z = fy + 1;
			
			SW.x = fx - 1;
			SW.y = Terrain->GetHeight(x - 1, y - 1);
			SW.z = fy - 1;

			X.x = fx;
			X.y = Terrain->GetHeight(x, y);
			X.z = fy;
			
			D3DXPlaneFromPoints(&T0, &SW, &W, &X);
			D3DXPlaneFromPoints(&T1, &X, &S, &SW);
			D3DXPlaneFromPoints(&T2, &X, &N, &NE);
			D3DXPlaneFromPoints(&T3, &NE, &E, &X);

			nT0.x = T0.a;
			nT0.y = T0.b;
			nT0.z = T0.c;

			nT1.x = T1.a;
			nT1.y = T1.b;
			nT1.z = T1.c;
			
			nT2.x = T2.a;
			nT2.y = T2.b;
			nT2.z = T2.c;
			
			nT3.x = T3.a;
			nT3.y = T3.b;
			nT3.z = T3.c;

			D3DXVec3Add(&nT0, &nT0, &nT1);
			D3DXVec3Add(&nT0, &nT0, &nT2);
			D3DXVec3Add(&nT0, &nT0, &nT3);
			D3DXVec3Normalize(&nT0, &nT0);

			/*nT0.x += 1.0f;
			nT0.z += 1.0f;
			nT0.x /= 2.0f;
			nT0.z /= 2.0f;
			nT0.x *= 255.0f;
			nT0.z *= 255.0f;
			vertex->NX = (unsigned char)(int)nT0.x;
			vertex->NZ = (unsigned char)(int)nT0.z;
			*/

			vertex->NX = nT0.x;
			vertex->NY = nT0.y;
			vertex->NZ = nT0.z;
		}

		void CChunk::ClearCollisions()
		{
			CollisionsActive = false;
			for(float my = 0.0f; my < 4.0f; my += 1.0f)
			{
				for(float mx = 0.0f; mx < 4.0f; mx += 1.0f)
				{
					float mmx = mx * 16.0f;
					float mmy = my * 16.0f;

					CollisionEventArgs E(0, NGin::Math::Vector3(((ChunkPosition.X + mmx) * 2.0f), 0.0f, ((ChunkPosition.Y + mmy) * 2.0f)), 0, Terrain->GetPosition(), true);
					Terrain->Manager->CollisionChanged()->Execute(Terrain, &E);
				}
			}
		}

		// Build a collision mesh
		void CChunk::ReBuildCollisions(bool highDetail)
		{
			float SizeF = 16.0f;
			float SizeF1 = 17.0f;
			int Size = 16;
			int Size1 = 17;
			float It = 4.0f;
			int VertIt = 1;

			if(!highDetail)
			{
				It = 2.0f;
				VertIt = 2;
			}

			for(float my = 0.0f; my < It; my += 1.0f)
			{
				for(float mx = 0.0f; mx < It; mx += 1.0f)
				{
					float mmx = mx * SizeF * VertIt;
					float mmy = my * SizeF * VertIt;

 					NGin::Math::Vector3* tVertices = new NGin::Math::Vector3[Size1*Size1];
 					unsigned int* Indices = new unsigned int[Size*Size*6];

 					int Idx = 0;
 					for(float y = 0.0f; y < SizeF1; y += 1.0f)
 					{
 						for(float x = 0.0f; x < SizeF1; x += 1.0f)
 						{
 							tVertices[Idx].Reset(x * VertIt, Terrain->GetHeight(ChunkPosition.X + (x * VertIt) + mmx, ChunkPosition.Y + (y * VertIt) + mmy), y * VertIt);

							++Idx;
 						}
 					}
		 
 					Idx = 0;
 					for(int y = 0; y < Size; ++y)
 					{
 						for(int x = 0; x < Size; ++x)
 						{
 							int Base = (y * Size1);
 							int Next = ((y + 1) * Size1);
		 
 							Indices[Idx + 2] = Base + x;
 							Indices[Idx + 1] = Base + x + 1;
 							Indices[Idx + 0] = Next + x + 1;
 							Indices[Idx + 5] = Next + x + 1;
 							Indices[Idx + 4] = Next + x;
 							Indices[Idx + 3] = Base + x;
		 
 							Idx += 6;
 						}
 					}

					bool UseExclusion = !this->Terrain->GetManager()->GetEditorMode();
					
					int Iter = 0;
					NGin::Math::Vector3* TriList = new NGin::Math::Vector3[Size*Size*6];

					for(unsigned int i = 0; i < Size*Size*6; i+=3)
					{
					 	if(UseExclusion)
					 	{
					 		bool Excl1 = this->GetExclusion((int)(tVertices[Indices[i+0]].X + mmx), (int)(tVertices[Indices[i+0]].Z + mmy)) == 0;
					 		bool Excl2 = this->GetExclusion((int)(tVertices[Indices[i+1]].X + mmx), (int)(tVertices[Indices[i+1]].Z + mmy)) == 0;
					 		bool Excl3 = this->GetExclusion((int)(tVertices[Indices[i+2]].X + mmx), (int)(tVertices[Indices[i+2]].Z + mmy)) == 0;
					 					
					 		if(Excl1 || Excl2 || Excl3)
					 			continue;
					 	}
					 
					 	TriList[Iter+0] = NGin::Math::Vector3(tVertices[Indices[i+0]].X * 2.0f, tVertices[Indices[i+0]].Y, tVertices[Indices[i+0]].Z * 2.0f);
					 	TriList[Iter+1] = NGin::Math::Vector3(tVertices[Indices[i+1]].X * 2.0f, tVertices[Indices[i+1]].Y, tVertices[Indices[i+1]].Z * 2.0f);
					 	TriList[Iter+2] = NGin::Math::Vector3(tVertices[Indices[i+2]].X * 2.0f, tVertices[Indices[i+2]].Y, tVertices[Indices[i+2]].Z * 2.0f);
					 	Iter += 3;
					}
		 
 					
		 
//  					for(int i = 0; i < Size*Size*6; ++i)
//  					{
//  						TriList[i].Reset(tVertices[Indices[i]].X * 2.0f, tVertices[Indices[i]].Y, tVertices[Indices[i]].Z * 2.0f);
//  					}

					CollisionEventArgs E((float*)TriList, NGin::Math::Vector3(((ChunkPosition.X + (mmx)) * 2.0f), 0.0f, ((ChunkPosition.Y + (mmy)) * 2.0f)), /*Size*Size*6*/Iter, Terrain->GetPosition(), highDetail);
					Terrain->Manager->CollisionChanged()->Execute(Terrain, &E);

					delete[] TriList;
					delete[] tVertices;
					delete[] Indices;
				}
			}


// 			int MainPatchSegs = PatchSize[0] - 2;
// 			int MainPatchVerts = ((MainPatchSegs + 1) * (MainPatchSegs + 1));
// 			int LODPatchVerts = 0;
// 			int LODPatchTris = 0;
// 			int LODPatchMin = PatchSize[0] / 2;
// 			int LODPatchMax = PatchSize[0];
// 
// 			int MainPatchTris = MainPatchSegs * MainPatchSegs * 2;
// 			int LODTrisMin = (LODPatchMin - 1) * 3 + 1;
// 			int LODTrisMax = MainPatchSegs * 2 + 2;
// 
// 			int LODScale = PatchSkip[0];
// 
// 			int LODScaleLOD = PatchSkip[1];
// 
// 
// 			LODPatchVerts += LODPatchMax;
// 			LODPatchTris += LODTrisMax;
// 
// 			LODPatchVerts += LODPatchMax;
// 			LODPatchTris += LODTrisMax;
// 
// 			LODPatchVerts += LODPatchMax;
// 			LODPatchTris += LODTrisMax;
// 
// 			LODPatchVerts += LODPatchMax;
// 			LODPatchTris += LODTrisMax;
// 
// 			int VCount = LODPatchVerts + MainPatchVerts;
// 			int ICount = LODPatchTris + MainPatchTris;
// 
// 			NGin::Math::Vector3* tVertices = new NGin::Math::Vector3[(LODPatchVerts + MainPatchVerts)];
// 
// 			int Index = 0;
// 			int TopIndex = 0, LeftIndex = 0, RightIndex = 0, BottomIndex = 0, MiddleIndex = 0;
// 
// 			TopIndex = Index;
// 			for(int i = 0; i < LODPatchMax; ++i)
// 			{
// 				tVertices[Index].Reset(i, Terrain->GetHeight(ChunkPosition.X + i, ChunkPosition.Y), 0);
// 				++Index;
// 			}
// 
// 			RightIndex = Index;
// 			for(int i = 0; i < LODPatchMax; ++i)
// 			{
// 				tVertices[Index].Reset(LODPatchMax, Terrain->GetHeight(ChunkPosition.X + 64, ChunkPosition.Y + i), i);
// 				++Index;
// 			}	
// 
// 
// 			BottomIndex = Index;
// 			for(int i = LODPatchMax; i > 0; --i)
// 			{
// 				tVertices[Index].Reset(i, Terrain->GetHeight(ChunkPosition.X + i, ChunkPosition.Y + 64), LODPatchMax);
// 				++Index;
// 			}	
// 
// 			LeftIndex = Index;
// 			for(int i = LODPatchMax; i > 0; --i)
// 			{
// 				tVertices[Index].Reset(0, Terrain->GetHeight(ChunkPosition.X, ChunkPosition.Y + i), i);
// 				++Index;
// 			}	
// 
// 
// 			MiddleIndex = Index;
// 			
// 			for(int z = 0; z < MainPatchSegs + 1; ++z)
// 			{
// 				for(int x = 0; x < MainPatchSegs + 1; ++x)
// 				{
// 					int Lx = (x + 1);
// 					int Lz = (z + 1);
// 					float H = Terrain->GetHeight(ChunkPosition.X + Lx, ChunkPosition.Y + Lz);
// 
// 					tVertices[Index].Reset((x + 1), H, (z + 1));
// 					++Index;
// 				}
// 			}

			//for(int ii = 0; ii < Index; ++ii)
			//	tVertices[ii] *= NGin::Math::Vector3(2.0f, 1.0f, 2.0f);

// 			unsigned int CBSize = CTerrain::Buffers[0][0]->IndexCount;
// 			unsigned short* CBBuffer = CTerrain::Buffers[0][0]->Indices;
// 
// 			NGin::Math::Vector3* TriList = new NGin::Math::Vector3[CBSize];
// 
// 			unsigned int IterSize = CBSize;
// 			unsigned int Iter = 0;
// 
// 			bool UseExclusion = !this->Terrain->GetManager()->GetEditorMode();
// 
// 			for(unsigned int i = 0; i < IterSize; i+=3)
// 			{
// 				if(UseExclusion)
// 				{
// 					bool Excl1 = this->GetExclusion((int)tVertices[CBBuffer[i+0]].X, (int)tVertices[CBBuffer[i+0]].Z) == 0;
// 					bool Excl2 = this->GetExclusion((int)tVertices[CBBuffer[i+1]].X, (int)tVertices[CBBuffer[i+1]].Z) == 0;
// 					bool Excl3 = this->GetExclusion((int)tVertices[CBBuffer[i+2]].X, (int)tVertices[CBBuffer[i+2]].Z) == 0;
// 					
// 					if(Excl1 || Excl2 || Excl3)
// 						continue;
// 				}
// 
// 				TriList[Iter+0] = NGin::Math::Vector3(tVertices[CBBuffer[i+0]].X * 2.0f, tVertices[CBBuffer[i+0]].Y, tVertices[CBBuffer[i+0]].Z * 2.0f);
// 				TriList[Iter+1] = NGin::Math::Vector3(tVertices[CBBuffer[i+1]].X * 2.0f, tVertices[CBBuffer[i+1]].Y, tVertices[CBBuffer[i+1]].Z * 2.0f);
// 				TriList[Iter+2] = NGin::Math::Vector3(tVertices[CBBuffer[i+2]].X * 2.0f, tVertices[CBBuffer[i+2]].Y, tVertices[CBBuffer[i+2]].Z * 2.0f);
// 				Iter += 3;
// 			}
// 
// 
// 			CollisionEventArgs E((float*)TriList, NGin::Math::Vector3((ChunkPosition.X * 2.0f), 0.0f, (ChunkPosition.Y * 2.0f)), Iter, Terrain->GetPosition());
// 			Terrain->Manager->CollisionChanged()->Execute(Terrain, &E);
// 			
// 			delete[] TriList;
// 			delete[] tVertices;

		}

		// Rebuild Vertices
		void CChunk::ReBuild()
		{
			if( LOD >= 3 )
			{
				LOD3_->AddChunk( this );
				return;
			}
			else
			{
				LOD3_->RemoveChunk( this );
			}

			// If vertices are already here, then destroy them
			if(Vertices != 0)
				Vertices->Release();

			// Texture of completely dead
			if(TexLOD > 250)
			{
				// Free up if necessary
				if(SplatTex != 0)
					SplatTex->Release();
				SplatTex = 0;
			}else if(LOD <= 1 && TexLOD > 0) // Lod Changed
			{
				if(SplatTex != 0)
					SplatTex->Release();
				SplatTex = 0;
			}else if(LOD > 1 && TexLOD < 1) // Lod Changes
			{
				if(SplatTex != 0)
					SplatTex->Release();
				SplatTex = 0;
			}

			// Texture is dead
			if(SplatTex == 0)
			{
				// Calculate LOD
				if(LOD > 1)
					TexLOD = 1;
				else
					TexLOD = 0;

				// Find width that we will use
				int TexWidth = 64;
				int PosMul = 1;
				if(TexLOD > 0)
				{
					TexWidth = 32;
					PosMul = 2;
				}

				// Create texture
				if(FAILED(Device->CreateTexture((UINT)TexWidth, (UINT)TexWidth, 0, D3DUSAGE_AUTOGENMIPMAP, D3DFMT_A8R8G8B8, D3DPOOL_MANAGED, &SplatTex, NULL)))
				{
					// Device is dead or GPU is screwed
					MessageBox(0, "Fatal Error: Could not create splat texture on terrain chunk. I cannot go on!", "CChunk::ReBuild()", MB_OK | MB_ICONERROR);
					exit(0);
				}

				// Lock texture
				T1TexCol* MapData;
				D3DLOCKED_RECT LockArea;
				SplatTex->LockRect(0, &LockArea, NULL, NULL);
				MapData = (T1TexCol*)LockArea.pBits;

				// Loop through available pixels
				int i = 0;
				for(int y = 0; y < TexWidth; ++y)
				{
					for(int x = 0; x < TexWidth; ++x)
					{
						// Set color
						MapData[i] = Terrain->GetColorChunk(ChunkPosition, x * PosMul, y * PosMul);
						++i;
					}
				}

				// Unlock
				SplatTex->UnlockRect(0);

			}

			if(HoleTex == 0 && ExclusionData.Size() > 0)
			{
				// Create texture
				if(FAILED(Device->CreateTexture(64, 64, 0, D3DUSAGE_AUTOGENMIPMAP, D3DFMT_A8, D3DPOOL_MANAGED, &HoleTex, NULL)))
				{
					// Device is dead or GPU is screwed
					MessageBox(0, "Fatal Error: Could not create hole texture on terrain chunk. I cannot go on!", "CChunk::ReBuild()", MB_OK | MB_ICONERROR);
					exit(0);
				}

				// Lock texture
				unsigned char* MapData;
				D3DLOCKED_RECT LockArea;
				HoleTex->LockRect(0, &LockArea, NULL, NULL);
				MapData = (unsigned char*)LockArea.pBits;

				int cx = (int)ChunkPosition.X;
				int cy = (int)ChunkPosition.Y;

				// Loop through available pixels
				int i = 0;
				for(int y = 0; y < 64; ++y)
				{
					for(int x = 0; x < 64; ++x)
					{
						// Set color
						MapData[i] = Terrain->GetExclusion(cx + x, cy + y) ? 0xff : 0x00;//Terrain->GetColorChunk(ChunkPosition, x * PosMul, y * PosMul);
						//MapData[i] = Terrain->GetGrass(cx + x, cy + y);
						++i;
					}
				}

				// Unlock
				HoleTex->UnlockRect(0);
			}

			bool Top, Right, Bottom, Left;
			Top = Right = Bottom = Left = false;

			Top = Sides & 1;
			Bottom = Sides & 2;
			Right = Sides & 4;
			Left = Sides & 8;

			int MainPatchSegs = PatchSize[LOD] - 2;
			int MainPatchVerts = ((MainPatchSegs + 1) * (MainPatchSegs + 1));
			int LODPatchVerts = 0;
			int LODPatchTris = 0;
			int LODPatchMin = PatchSize[LOD] / 2;
			int LODPatchMax = PatchSize[LOD];

			int MainPatchTris = MainPatchSegs * MainPatchSegs * 2;
			int LODTrisMin = (LODPatchMin - 1) * 3 + 1;
			int LODTrisMax = MainPatchSegs * 2 + 2;

			int LODScale = PatchSkip[LOD];

			int LODScaleLOD = LOD <= 4 ? PatchSkip[LOD + 1] : PatchSkip[LOD];

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

			Device->CreateVertexBuffer(sizeof(T1Vertex) * (LODPatchVerts + MainPatchVerts), D3DUSAGE_WRITEONLY, 0, D3DPOOL_MANAGED, &Vertices, NULL);
			int VCount = LODPatchVerts + MainPatchVerts;
			int ICount = LODPatchTris + MainPatchTris;

			T1Vertex* tVertices;
			Vertices->Lock(0, 0, (void**)&tVertices, 0);

			int Index = 0;
			int TopIndex = 0, LeftIndex = 0, RightIndex = 0, BottomIndex = 0, MiddleIndex = 0;

			TopIndex = Index;
			if(Top)
			{
				for(int i = 0; i < LODPatchMax; i+=2)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, i * PatchSkip[LOD], 0);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(i * LODScale, sH, 0);
					CalculateNormal(i * PatchSkip[LOD], 0, &(tVertices[Index]));
					++Index;
				}
			}else
			{
				for(int i = 0; i < LODPatchMax; ++i)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, i * PatchSkip[LOD], 0);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(i * LODScale, sH, 0);
					CalculateNormal(i * PatchSkip[LOD], 0, &(tVertices[Index]));
					++Index;
				}
			}

			RightIndex = Index;
			if(Right)
			{
				for(int i = 0; i < LODPatchMax; i += 2)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, 64, i * PatchSkip[LOD]);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(LODPatchMax * LODScale, sH, i * LODScale);
					CalculateNormal(64, i * PatchSkip[LOD], &(tVertices[Index]));
					++Index;
				}
			}else
			{
				for(int i = 0; i < LODPatchMax; ++i)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, 64, i * PatchSkip[LOD]);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(LODPatchMax * LODScale, sH, i * LODScale);
					CalculateNormal(64, i * PatchSkip[LOD], &(tVertices[Index]));
					++Index;
				}	
			}

			BottomIndex = Index;
			if(Bottom)
			{
				for(int i = LODPatchMax; i > 0; i-=2)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, i * PatchSkip[LOD], 64);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(i * LODScale, sH, LODPatchMax * LODScale);
					CalculateNormal(i * PatchSkip[LOD], 64, &(tVertices[Index]));
					++Index;
				}
			}else
			{
				for(int i = LODPatchMax; i > 0; --i)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, i * PatchSkip[LOD], 64);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(i * LODScale, sH, LODPatchMax * LODScale);
					CalculateNormal(i * PatchSkip[LOD], 64, &(tVertices[Index]));
					++Index;
				}	
			}

			LeftIndex = Index;
			if(Left)
			{
				for(int i = LODPatchMax; i > 0; i-=2)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, 0, i * PatchSkip[LOD]);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(0, sH, i * LODScale);
					CalculateNormal(0, i * PatchSkip[LOD], &(tVertices[Index]));
					++Index;
				}
			}else
			{
				for(int i = LODPatchMax; i > 0; --i)
				{
					float sH = Terrain->GetHeightChunk(ChunkPosition, 0, i * PatchSkip[LOD]);
					tVertices[Index].nY = sH;
					tVertices[Index].Set(0, sH, i * LODScale);
					CalculateNormal(0, i * PatchSkip[LOD], &(tVertices[Index]));
					++Index;
				}	
			}

			MiddleIndex = Index;
			
			for(int z = 0; z < MainPatchSegs + 1; ++z)
			{
				for(int x = 0; x < MainPatchSegs + 1; ++x)
				{
					bool ZFound = false;
					bool XFound = false;
					for(int qz = 0; qz < MainPatchSegs + 1; ++qz)
					{
						if((qz + 1) * LODScaleLOD == (z + 1) * LODScale)
							ZFound = true;

						for(int qx = 0; qx < MainPatchSegs + 1; ++qx)
						{
							if((qx + 1) * LODScaleLOD == (x + 1) * LODScale)
								XFound = true;
						}
					}

					int Lx = (x + 1) * LODScale;
					int Lz = (z + 1) * LODScale;
					float sH = Terrain->GetHeightChunk(ChunkPosition, Lx, Lz);
					float H = Terrain->GetHeight(ChunkPosition.X + Lx, ChunkPosition.Y + Lz);

					// X Aligned, but not ZAligned
					//if(XFound && !ZFound)
					if(!XFound && ZFound)
					{
						//unsigned int S1 = half_to_float(_Terrain->GetHeightChunk(_ChunkPosition, Lx, Lz, 1));
						float S2 = Terrain->GetHeightChunk(ChunkPosition, Lx - 1, Lz);
						float S3 = Terrain->GetHeightChunk(ChunkPosition, Lx + 1, Lz);

						H = S2 + S3;
						H /= 2.0f;

						//H = -1.0f;
					}

					// Z Aligned, but not XAligned
					//if(!XFound && ZFound)
					if(XFound && !ZFound)
					{
						//unsigned int S1 = half_to_float(_Terrain->GetHeightChunk(_ChunkPosition, Lx, Lz, 1));
						float S2 = Terrain->GetHeightChunk(ChunkPosition, Lx, Lz - 1);
						float S3 = Terrain->GetHeightChunk(ChunkPosition, Lx, Lz + 1);

						H = S2 + S3;
						H /= 2.0f;

						//H = -1.0f;
					}

					// Neither X/Z aligned
					if(!XFound && !ZFound)
					{
						//unsigned int S1 = half_to_float(_Terrain->GetHeightChunk(_ChunkPosition, Lx, Lz, 1));
						float S2 = Terrain->GetHeightChunk(ChunkPosition, Lx - 1, Lz - 1);
						float S3 = Terrain->GetHeightChunk(ChunkPosition, Lx + 1, Lz + 1);

						H = S2 + S3;
						H /= 2.0f;
					}

					tVertices[Index].nY = H;
					tVertices[Index].Set((x + 1) * LODScale, sH, (z + 1) * LODScale);
					CalculateNormal((x + 1) * LODScale, (z + 1) * LODScale, &(tVertices[Index]));
					++Index;
				}
			}

			//if(DELSENT == false)
			//{
			//	unsigned int CBSize = CTerrain::Buffers[LOD][Sides]->IndexCount;// - (this->ExclusionData.Size() * 3);
			//	unsigned short* CBBuffer = CTerrain::Buffers[LOD][Sides]->Indices;

			//	NGin::Math::Vector3* TriList = new NGin::Math::Vector3[CBSize];

			//	unsigned int IterSize = CBSize;// + (this->ExclusionData.Size() * 3);
			//	unsigned int Iter = 0;

			//	char DBO[2048];
			//	sprintf(DBO, "Exclusions: %u; IterSize: %u; CBSize: %u;", this->ExclusionData.Size() * 3, IterSize, CBSize);


			//	for(unsigned int i = 0; i < IterSize; i+=3)
			//	{
			//		bool Excl1 = this->GetExclusion((int)tVertices[CBBuffer[i+0]].X, (int)tVertices[CBBuffer[i+0]].Z) == 0;
			//		bool Excl2 = this->GetExclusion((int)tVertices[CBBuffer[i+1]].X, (int)tVertices[CBBuffer[i+1]].Z) == 0;
			//		bool Excl3 = this->GetExclusion((int)tVertices[CBBuffer[i+2]].X, (int)tVertices[CBBuffer[i+2]].Z) == 0;
			//		//if(this->GetExclusion((int)tVertices[CBBuffer[i]].X, (int)tVertices[CBBuffer[i]].Z) == 0)
			//		//	continue;
			//		
			//		if(Excl1 || Excl2 || Excl3)
			//			continue;

			//		uint32_t y = half_to_float(tVertices[CBBuffer[i+0]].nY);
			//		TriList[Iter+0] = NGin::Math::Vector3(tVertices[CBBuffer[i+0]].X, *((float*)&y), tVertices[CBBuffer[i+0]].Z);

			//		y = half_to_float(tVertices[CBBuffer[i+1]].nY);
			//		TriList[Iter+1] = NGin::Math::Vector3(tVertices[CBBuffer[i+1]].X, *((float*)&y), tVertices[CBBuffer[i+1]].Z);

			//		y = half_to_float(tVertices[CBBuffer[i+2]].nY);
			//		TriList[Iter+2] = NGin::Math::Vector3(tVertices[CBBuffer[i+2]].X, *((float*)&y), tVertices[CBBuffer[i+2]].Z);
			//		Iter += 3;
			//	}

			//	sprintf(DBO, "%s Iter: %u;\n", DBO, Iter);
			//	OutputDebugString(DBO);


			//	CollisionEventArgs E((float*)TriList, NGin::Math::Vector3(ChunkPosition.X, 0.0f, ChunkPosition.Y), Iter);//CBSize);
			//	Terrain->Manager->CollisionChanged()->Execute(Terrain, &E);
			//	
			//	delete[] TriList;

			//	//DELSENT = true;
			//}

			Vertices->Unlock();
			VertexCount = VCount;
		}
	}
}
