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

#include <IGUIRenderer.h>
#include <vector>

namespace NGin
{
	namespace GUI
	{
		//! GUI Mesh Builder
		struct SGUIMeshBuilder
		{
		private:

			IGUIRenderer* Renderer;
			std::vector<int> Indices;
			std::vector<SGUIVertex> Vertices;

		public:

			// Constructor
			SGUIMeshBuilder(IGUIRenderer* renderer)
				: Renderer(renderer)
			{
				Indices.reserve(600);
				Vertices.reserve(100);
			}

			//! Build a device buffer
			IGUIMeshBuffer* Build()
			{
				SGUIVertex* Vrts = new SGUIVertex[Vertices.size()];
				unsigned short* Inds = new unsigned short[Indices.size()];

				for(int Index = 0; Index < (int)Vertices.size(); ++Index)
				{
					Vrts[Index] = Vertices[Index];
				}

				for(int Index = 0; Index < (int)Indices.size(); ++Index)
				{
					Inds[Index] = Indices[Index];
				}

				IGUIMeshBuffer* Buffer = Renderer->CreateMeshBuffer(Vrts, Vertices.size(), Inds, Indices.size());

				delete[] Vrts;
				delete[] Inds;

				return Buffer;
			}

			//! Add a single triangle
			void AddTriangle(Math::Vector2 &position0, Math::Vector2 &position1, Math::Vector2 &position2, Math::Vector2 &texcoord0, Math::Vector2 &texcoord1, Math::Vector2 &texcoord2, Math::Color &color)
			{
				SGUIVertex V0, V1, V2;
				V0.Set(position0.X, position0.Y, 0.0f, texcoord0.X, texcoord0.Y);
				V1.Set(position1.X, position1.Y, 0.0f, texcoord1.X, texcoord1.Y);
				V2.Set(position2.X, position2.Y, 0.0f, texcoord2.X, texcoord2.Y);

				V0.Color.Reset(color.R, color.G, color.B, color.A);
				V1.Color.Reset(color.R, color.G, color.B, color.A);
				V2.Color.Reset(color.R, color.G, color.B, color.A);

				V0.Position.X *= 2.0f;
				V0.Position.X -= 1.0f;
				V1.Position.X *= 2.0f;
				V1.Position.X -= 1.0f;
				V2.Position.X *= 2.0f;
				V2.Position.X -= 1.0f;

				V0.Position.Y *= 2.0f;
				V0.Position.Y -= 1.0f;
				V1.Position.Y *= 2.0f;
				V1.Position.Y -= 1.0f;
				V2.Position.Y *= 2.0f;
				V2.Position.Y -= 1.0f;

				V0.Position.Y = -V0.Position.Y;
				V1.Position.Y = -V1.Position.Y;
				V2.Position.Y = -V2.Position.Y;


				int Index = Vertices.size();
				Vertices.push_back(V0);
				Vertices.push_back(V1);
				Vertices.push_back(V2);

				Indices.push_back(Index + 2);
				Indices.push_back(Index + 1);
				Indices.push_back(Index + 0);
			}

			//! Add a quad(Two Triangles)
			void AddQuad(Math::Vector2 &position, Math::Vector2 &size, Math::Vector2 &texCoordMin, Math::Vector2 &texCoordMax, Math::Color &color)
			{
				SGUIVertex V0, V1, V2, V3;

				V0.Set(position.X, position.Y, 0, texCoordMin.X, texCoordMin.Y);
				V1.Set(position.X + size.X, position.Y, 0, texCoordMin.X + texCoordMax.X, texCoordMin.Y);
				V2.Set(position.X, position.Y + size.Y, 0, texCoordMin.X, texCoordMin.Y + texCoordMax.Y);
				V3.Set(position.X + size.X, position.Y + size.Y, 0, texCoordMin.X + texCoordMax.X, texCoordMin.Y + texCoordMax.Y);

				V0.Color.Reset(color.R, color.G, color.B, color.A);
				V1.Color.Reset(color.R, color.G, color.B, color.A);
				V2.Color.Reset(color.R, color.G, color.B, color.A);
				V3.Color.Reset(color.R, color.G, color.B, color.A);

				V0.Position.X *= 2.0f;
				V0.Position.X -= 1.0f;
				V1.Position.X *= 2.0f;
				V1.Position.X -= 1.0f;
				V2.Position.X *= 2.0f;
				V2.Position.X -= 1.0f;
				V3.Position.X *= 2.0f;
				V3.Position.X -= 1.0f;

				V0.Position.Y *= 2.0f;
				V0.Position.Y -= 1.0f;
				V1.Position.Y *= 2.0f;
				V1.Position.Y -= 1.0f;
				V2.Position.Y *= 2.0f;
				V2.Position.Y -= 1.0f;
				V3.Position.Y *= 2.0f;
				V3.Position.Y -= 1.0f;

				V0.Position.Y = -V0.Position.Y;
				V1.Position.Y = -V1.Position.Y;
				V2.Position.Y = -V2.Position.Y;
				V3.Position.Y = -V3.Position.Y;


				int Index = Vertices.size();
 				Vertices.push_back(V0);
 				Vertices.push_back(V1);
 				Vertices.push_back(V2);
 				Vertices.push_back(V3);

				Indices.push_back(Index + 2);
				Indices.push_back(Index + 1);
				Indices.push_back(Index + 0);
				Indices.push_back(Index + 2);
				Indices.push_back(Index + 3);
				Indices.push_back(Index + 1);
			}

			//! Remove a number of quads from the end of the builder
			void RemoveQuads(int count)
			{
				for(int i = 0; i < count * 4; ++i)
					Vertices.pop_back();
				for(int i = 0; i < count * 6; ++i)
					Indices.pop_back();

			}

		};
	}
}
