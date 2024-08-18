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
#include "CGrassType.h"
#include <list.h>

namespace RealmCrafter
{
	namespace RCT
	{
		//class CGrassType : public IGrassType
		//{
		//protected:

		//	IDirect3DTexture9* Texture;
		//	IDirect3DDevice9* Device;

		//	std::string Name, Path;
		//	NGin::Math::Vector2 Scale;
		//	float Offset, Coverage;

		//public:

		CGrassType::CGrassType(IDirect3DDevice9* device)
		{
			Device = device;
			Texture = 0;
			Name = "";
			Path = "";
			Offset = 0;
			Coverage = 0;
			HeightVariance = 0;
		}

		CGrassType::~CGrassType()
		{
			if(Texture != 0)
				Texture->Release();
			Texture = 0;
		}

		void CGrassType::CreateGrass(CChunk* chunk)
		{
			SGrassBuffer* B = 0;
			for(int i = 0; i < Buffers.Size(); ++i)
			{
				if(Buffers[i]->Chunk == chunk)
				{
					B = Buffers[i];
					break;
				}
			}

			if(B != 0)
				return;

			srand(GetTickCount());
			Buffers.Add(new SGrassBuffer(rand(), chunk));
		}

		void CGrassType::RemoveGrass(CChunk* chunk)
		{
			SGrassBuffer* B = 0;
			int ID = -1;
			for(int i = 0; i < Buffers.Size(); ++i)
			{
				if(Buffers[i]->Chunk == chunk)
				{
					B = Buffers[i];
					ID = i;
					break;
				}
			}

			if(B == 0)
				return;

			delete B;

			if(ID != -1)
				Buffers.Remove(ID);
		}

		void CGrassType::Invalidate()
		{
			for(int i = 0; i < Buffers.Size(); ++i)
			{
				Buffers[i]->Invalidate();
			}
			
		}

		void CGrassType::Invalidate(CChunk* chunk)
		{
			SGrassBuffer* B = 0;
			for(int i = 0; i < Buffers.Size(); ++i)
			{
				if(Buffers[i]->Chunk == chunk)
				{
					B = Buffers[i];
					break;
				}
			}

			if(B == 0)
				return;

			B->Invalidate();
		}

		void CGrassType::Update()
		{
			for(int i = 0; i < Buffers.Size(); ++i)
			{
				if(Buffers[i]->Valid == false)
					ReBuild(Buffers[i]->Chunk, Buffers[i]);
			}
			
		}

		bool ListFind(std::list<CChunk*> &renderChunks, CChunk* chunk)
		{
			for(std::list<CChunk*>::iterator It = renderChunks.begin(); It != renderChunks.end(); ++It)
				if(*It == chunk)
					return true;

			return false;
		}

		void CGrassType::Render(ID3DXEffect* effect, std::list<CChunk*> &renderChunks, NGin::Math::Vector3 &cameraPosition, float drawDistance, NGin::Math::Vector3 &offset)
		{
			if(Buffers.Size() == 0)
				return;

			D3DXHANDLE Hr = effect->GetParameterBySemantic(NULL, "TextureStage0");
			effect->SetTexture(Hr, Texture);
			effect->CommitChanges();

			for(int i = 0; i < Buffers.Size(); ++i)
			{
				CChunk* Chunk = Buffers[i]->Chunk;
				
				float XDist = cameraPosition.X - (Chunk->GetBoundingBox()._Min.X);// + Chunk->GetTerrain()->GetPosition().X);
				float ZDist = cameraPosition.Z - (Chunk->GetBoundingBox()._Min.Z);// + Chunk->GetTerrain()->GetPosition().Z);
				float DistSq = (XDist * XDist) + (ZDist * ZDist);

				

				if(ListFind(renderChunks, Chunk) && DistSq < drawDistance)
				{
					effect->SetFloatArray("Offset", reinterpret_cast<const FLOAT*>(&(Chunk->GetTerrain()->GetPosition() + offset)), 3);
					effect->CommitChanges();

					SGrassBuffer* B = Buffers[i];

					Device->SetStreamSource(0, B->Vertices, 0, sizeof(SGrassVertex));
					Device->SetIndices(B->Indices);

					Device->DrawIndexedPrimitive(D3DPT_TRIANGLELIST, 0, 0, B->VertexCount, 0, B->IndexCount / 3);
				}
			}
		}

		void CGrassType::ReBuild(CChunk* chunk, SGrassBuffer* buffer)
		{
			int Index = chunk->GetTerrain()->GetGrassIndex(this);

			if(Index == -1)
				return;

			float PX = chunk->GetBoundingBox()._Min.X - chunk->GetTerrain()->GetPosition().X;
			float PZ = chunk->GetBoundingBox()._Min.Z - chunk->GetTerrain()->GetPosition().Z;

			PX *= 0.5f;
			PZ *= 0.5f;

			Index = (int)pow(2.0f, (float)Index);

			srand(buffer->Seed);

			float Incremement = (1.0f / Coverage);
			int MaxLen = ((int)64.0f / Incremement) + 1;

			int GrassesCount = MaxLen * MaxLen;
			int GrassIt = 0;
			NGin::Math::Vector4* Grasses = new NGin::Math::Vector4[GrassesCount];
			
			for(float z = 0.0f; z < 64.0f; z += (1.0f / Coverage))
			{
				for(float x = 0.0f; x < 64.0f; x += (1.0f / Coverage))
				{
					float cx = x + (((float)rand()) / ((float)RAND_MAX));
					float cz = z + (((float)rand()) / ((float)RAND_MAX));
					float YO = (((float)rand()) / ((float)RAND_MAX));

					if((chunk->GetTerrain()->GetGrass(cx + PX, cz + PZ) & Index) > 0)
					{
						float XMin = floor(cx);
						float XMax = ceil(cx);
						float ZMin = floor(cz);
						float ZMax = ceil(cz);

						float Height = 0.0f;
						Height += chunk->GetTerrain()->GetHeight(XMin + PX, ZMin + PZ);
						Height += chunk->GetTerrain()->GetHeight(XMax + PX, ZMin + PZ);
						Height += chunk->GetTerrain()->GetHeight(XMin + PX, ZMax + PZ);
						Height += chunk->GetTerrain()->GetHeight(XMax + PX, ZMax + PZ);
						Height /= 4.0f;

						Grasses[GrassIt++].Reset(cx, Height, cz, YO);

						//Grasses.Add(new NGin::Math::Vector3(cx, chunk->GetTerrain()->GetHeight(cx + PX, cz + PZ), cz));
					}
				}
			}

			int BladeCount = GrassesCount;
			if(BladeCount == 0)
				return;

			if(Device->CreateVertexBuffer(sizeof(SGrassVertex) * 4 * (3 * BladeCount), D3DUSAGE_WRITEONLY, NULL, D3DPOOL_MANAGED, &(buffer->Vertices), NULL) != D3D_OK)
			{
				OutputDebugString("GrassVertexBuffer Generation Failed!\n");
				return;
			}

			if(Device->CreateIndexBuffer(sizeof(unsigned short) * 6 * (3 * BladeCount), D3DUSAGE_WRITEONLY, D3DFMT_INDEX16, D3DPOOL_MANAGED, &(buffer->Indices), NULL) != D3D_OK)
			{
				OutputDebugString("GrassIndexBuffer Generation Failed!\n");
				return;
			}

			buffer->VertexCount = 4 * 3 * BladeCount;
			buffer->IndexCount = 6 * 3 * BladeCount;

			SGrassVertex* Vertices = 0;
			unsigned short* Indices = 0;

			buffer->Vertices->Lock(0, 0, (void**)&Vertices, 0);
			buffer->Indices->Lock(0, 0, (void**)&Indices, 0);

			PX *= 2.0f;
			PZ *= 2.0f;

			int i = 0;
			for(i = 0; i < GrassIt; ++i)
			{
				float cx = Grasses[i].X * 2.0f;
				float cy = Grasses[i].Y;
				float cz = Grasses[i].Z * 2.0f;
				float YO = (Grasses[i].W * 2.0f) - 1.0f;

				float ScaleY = Scale.Y + (YO * HeightVariance);

				// Create Quad 1
				Vertices[(i * 12) + 0].Set((cx - (Scale.X * 0.5f)) + PX, (ScaleY + Offset) + cy, cz + PZ, 0.0f, 0.0f, ScaleY);
				Vertices[(i * 12) + 1].Set((cx + (Scale.X * 0.5f)) + PX, (ScaleY + Offset) + cy, cz + PZ, 1.0f, 0.0f, ScaleY);
				Vertices[(i * 12) + 2].Set((cx - (Scale.X * 0.5f)) + PX, (Offset) + cy, cz + PZ, 0.0f, 1.0f, 0.0f);
				Vertices[(i * 12) + 3].Set((cx + (Scale.X * 0.5f)) + PX, (Offset) + cy, cz + PZ, 1.0f, 1.0f, 0.0f);
				Indices[(i * 18) + 0] = (i * 12) + 0;
				Indices[(i * 18) + 1] = (i * 12) + 1;
				Indices[(i * 18) + 2] = (i * 12) + 2;
				Indices[(i * 18) + 3] = (i * 12) + 1;
				Indices[(i * 18) + 4] = (i * 12) + 3;
				Indices[(i * 18) + 5] = (i * 12) + 2;

				// Create Quad 2
				Vertices[(i * 12) + 4].Set((cx - (Scale.X * 0.25f)) + PX, (ScaleY + Offset) + cy, (cz + (Scale.X * 0.43f)) + PZ, 0.0f, 0.0f, ScaleY);
				Vertices[(i * 12) + 5].Set((cx + (Scale.X * 0.25f)) + PX, (ScaleY + Offset) + cy, (cz - (Scale.X * 0.43f)) + PZ, 1.0f, 0.0f, ScaleY);
				Vertices[(i * 12) + 6].Set((cx - (Scale.X * 0.25f)) + PX, (Offset) + cy, (cz + (Scale.X * 0.43f)) + PZ, 0.0f, 1.0f, 0.0f);
				Vertices[(i * 12) + 7].Set((cx + (Scale.X * 0.25f)) + PX, (Offset) + cy, (cz - (Scale.X * 0.43f)) + PZ, 1.0f, 1.0f, 0.0f);
				Indices[(i * 18) + 6] = (i * 12) + 4;
				Indices[(i * 18) + 7] = (i * 12) + 5;
				Indices[(i * 18) + 8] = (i * 12) + 6;
				Indices[(i * 18) + 9] = (i * 12) + 5;
				Indices[(i * 18) + 10] = (i * 12) + 7;
				Indices[(i * 18) + 11] = (i * 12) + 6;

				// Create Quad 3
				Vertices[(i * 12) + 8 ].Set((cx - (Scale.X * 0.25f)) + PX, (ScaleY + Offset) + cy, (cz - (Scale.X * 0.43f)) + PZ, 0.0f, 0.0f, ScaleY);
				Vertices[(i * 12) + 9 ].Set((cx + (Scale.X * 0.25f)) + PX, (ScaleY + Offset) + cy, (cz + (Scale.X * 0.43f)) + PZ, 1.0f, 0.0f, ScaleY);
				Vertices[(i * 12) + 10].Set((cx - (Scale.X * 0.25f)) + PX, (Offset) + cy, (cz - (Scale.X * 0.43f)) + PZ, 0.0f, 1.0f, 0.0f);
				Vertices[(i * 12) + 11].Set((cx + (Scale.X * 0.25f)) + PX, (Offset) + cy, (cz + (Scale.X * 0.43f)) + PZ, 1.0f, 1.0f, 0.0f);
				Indices[(i * 18) + 12] = (i * 12) + 8;
				Indices[(i * 18) + 13] = (i * 12) + 9;
				Indices[(i * 18) + 14] = (i * 12) + 10;
				Indices[(i * 18) + 15] = (i * 12) + 9;
				Indices[(i * 18) + 16] = (i * 12) + 11;
				Indices[(i * 18) + 17] = (i * 12) + 10;
			}

			delete[] Grasses;

			buffer->Vertices->Unlock();
			buffer->Indices->Unlock();
			buffer->Valid = true;
		}


		/*void CGrassType::ReBuild(CChunk* chunk, SGrassBuffer* buffer)
		{

			// Get total number of blades
			float fBladeCount = 32.0f * Coverage;
			fBladeCount *= fBladeCount;
			int BladeCount = (int)fBladeCount;

			if(Device->CreateVertexBuffer(sizeof(SGrassVertex) * 4 * (3 * BladeCount), D3DUSAGE_WRITEONLY, NULL, D3DPOOL_MANAGED, &(buffer->Vertices), NULL) != D3D_OK)
			{
				OutputDebugString("GrassVertexBuffer Generation Failed!\n");
				return;
			}

			if(Device->CreateIndexBuffer(sizeof(unsigned short) * 6 * (3 * BladeCount), D3DUSAGE_WRITEONLY, D3DFMT_INDEX16, D3DPOOL_MANAGED, &(buffer->Indices), NULL) != D3D_OK)
			{
				OutputDebugString("GrassIndexBuffer Generation Failed!\n");
				return;
			}

			buffer->VertexCount = 4 * 3 * BladeCount;
			buffer->IndexCount = 6 * 3 * BladeCount;

			SGrassVertex* Vertices = 0;
			unsigned short* Indices = 0;

			buffer->Vertices->Lock(0, 0, (void**)&Vertices, 0);
			buffer->Indices->Lock(0, 0, (void**)&Indices, 0);

			float PX = chunk->GetBoundingBox()._Min.X;
			float PZ = chunk->GetBoundingBox()._Min.Z;

			

			int i = 0;
			for(float z = 0.0f; z < 32.0f; z += (1.0f / Coverage))
			{
				for(float x = 0.0f; x < 32.0f; x += (1.0f / Coverage))
				{
					if(i > BladeCount)
						continue;

					float cx = x + (((float)rand()) / ((float)RAND_MAX));
					float cz = z + (((float)rand()) / ((float)RAND_MAX));

					// Create Quad 1
					Vertices[(i * 12) + 0].Set((cx - (Scale.X * 0.5f)) + PX, Scale.Y + Offset, cz + PZ, 0.0f, 0.0f);
					Vertices[(i * 12) + 1].Set((cx + (Scale.X * 0.5f)) + PX, Scale.Y + Offset, cz + PZ, 1.0f, 0.0f);
					Vertices[(i * 12) + 2].Set((cx - (Scale.X * 0.5f)) + PX, Offset, cz + PZ, 0.0f, 1.0f);
					Vertices[(i * 12) + 3].Set((cx + (Scale.X * 0.5f)) + PX, Offset, cz + PZ, 1.0f, 1.0f);
					Indices[(i * 18) + 0] = (i * 12) + 0;
					Indices[(i * 18) + 1] = (i * 12) + 1;
					Indices[(i * 18) + 2] = (i * 12) + 2;
					Indices[(i * 18) + 3] = (i * 12) + 1;
					Indices[(i * 18) + 4] = (i * 12) + 3;
					Indices[(i * 18) + 5] = (i * 12) + 2;

					// Create Quad 2
					Vertices[(i * 12) + 4].Set((cx - (Scale.X * 0.25f)) + PX, Scale.Y + Offset, (cz + (Scale.X * 0.43f)) + PZ, 0.0f, 0.0f);
					Vertices[(i * 12) + 5].Set((cx + (Scale.X * 0.25f)) + PX, Scale.Y + Offset, (cz - (Scale.X * 0.43f)) + PZ, 1.0f, 0.0f);
					Vertices[(i * 12) + 6].Set((cx - (Scale.X * 0.25f)) + PX, Offset, (cz + (Scale.X * 0.43f)) + PZ, 0.0f, 1.0f);
					Vertices[(i * 12) + 7].Set((cx + (Scale.X * 0.25f)) + PX, Offset, (cz - (Scale.X * 0.43f)) + PZ, 1.0f, 1.0f);
					Indices[(i * 18) + 6] = (i * 12) + 4;
					Indices[(i * 18) + 7] = (i * 12) + 5;
					Indices[(i * 18) + 8] = (i * 12) + 6;
					Indices[(i * 18) + 9] = (i * 12) + 5;
					Indices[(i * 18) + 10] = (i * 12) + 7;
					Indices[(i * 18) + 11] = (i * 12) + 6;

					// Create Quad 3
					Vertices[(i * 12) + 8 ].Set((cx - (Scale.X * 0.25f)) + PX, Scale.Y + Offset, (cz - (Scale.X * 0.43f)) + PZ, 0.0f, 0.0f);
					Vertices[(i * 12) + 9 ].Set((cx + (Scale.X * 0.25f)) + PX, Scale.Y + Offset, (cz + (Scale.X * 0.43f)) + PZ, 1.0f, 0.0f);
					Vertices[(i * 12) + 10].Set((cx - (Scale.X * 0.25f)) + PX, Offset, (cz - (Scale.X * 0.43f)) + PZ, 0.0f, 1.0f);
					Vertices[(i * 12) + 11].Set((cx + (Scale.X * 0.25f)) + PX, Offset, (cz + (Scale.X * 0.43f)) + PZ, 1.0f, 1.0f);
					Indices[(i * 18) + 12] = (i * 12) + 8;
					Indices[(i * 18) + 13] = (i * 12) + 9;
					Indices[(i * 18) + 14] = (i * 12) + 10;
					Indices[(i * 18) + 15] = (i * 12) + 9;
					Indices[(i * 18) + 16] = (i * 12) + 11;
					Indices[(i * 18) + 17] = (i * 12) + 10;

					++i;
				}
			}

			buffer->Vertices->Unlock();
			buffer->Indices->Unlock();
			buffer->Valid = true;
		}*/

		std::string CGrassType::GetName()
		{
			return Name;
		}

		void CGrassType::SetName(std::string& name)
		{
			Name = name;
		}

		std::string CGrassType::GetTexture()
		{
			return Path;
		}

		void CGrassType::SetTexture(std::string& path)
		{
			Path = path;

			if(Texture != 0)
				Texture->Release();
			Texture = 0;

			if(FAILED(D3DXCreateTextureFromFileExA(
				Device,
				path.c_str(),
				D3DX_DEFAULT, D3DX_DEFAULT,
				1, // One mip level
				0,
				D3DFMT_UNKNOWN,
				D3DPOOL_MANAGED,
				D3DX_DEFAULT,
				D3DX_DEFAULT,
				0xff000000,
				NULL, NULL,
				&Texture)))
			{
				OutputDebugStringA("Failed to load grass texture: ");
				OutputDebugStringA(path.c_str());
				OutputDebugStringA("\n");
				Texture = 0;
			}
		}

		NGin::Math::Vector2 CGrassType::GetScale()
		{
			return Scale;
		}

		void CGrassType::SetScale(NGin::Math::Vector2& scale)
		{
			Scale = scale;
			Invalidate();
		}

		float CGrassType::GetOffset()
		{
			return Offset;
		}

		void CGrassType::SetOffset(float offset)
		{
			Offset = offset;
			Invalidate();
		}

		float CGrassType::GetCoverage()
		{
			return Coverage;
		}

		void CGrassType::SetCoverage(float coverage)
		{
			Coverage = coverage;
			Invalidate();
		}

		float CGrassType::GetHeightVariance()
		{
			return HeightVariance;
		}

		void CGrassType::SetHeightVariance(float heightVariance)
		{
			HeightVariance = heightVariance;
			Invalidate();
		}
	}
}