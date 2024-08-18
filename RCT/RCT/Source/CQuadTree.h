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


#include <AABB.h>
#include <list>
#include "Frustum.h"
#include <ArrayList.h>

namespace RealmCrafter
{
	namespace RCT
	{
		class CChunk;
		struct SQuadTreeNode
		{
			NGin::Math::AABB BoundingBox;
			SQuadTreeNode* Nodes[4];
			CChunk* Chunk;
			int X, Y;

			SQuadTreeNode();
			~SQuadTreeNode();
		};

		class CQuadTree
		{
			int TileCount;
			float TileWidth;
			NGin::Math::Vector3 Position;

			SQuadTreeNode* Tree;

			void Populate(SQuadTreeNode* node, int coverage, NGin::ArrayList<NGin::ArrayList<CChunk*> > &chunks);
			void TestQuad(SQuadTreeNode* node, Frustum &viewFrustum, std::list<CChunk*> &renderChunks, std::list<CChunk*> &lodChunks, NGin::Math::Vector3 &offset);

		public:

			CQuadTree(int tileCount, float tileWidth);
			~CQuadTree();

			void Rebuild(NGin::Math::Vector3& position, NGin::ArrayList<NGin::ArrayList<CChunk*> > &chunks);
			void GetVisibleChunks(Frustum &viewFrustum, std::list<CChunk*> &renderChunks, std::list<CChunk*> &lodChunks, NGin::Math::Vector3 &offset);

		};
	}
}