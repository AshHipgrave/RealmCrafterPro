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
#include "CQuadTree.h"
#include <Vector3.h>
#include "CChunk.h"

namespace RealmCrafter
{
	namespace RCT
	{
		SQuadTreeNode::SQuadTreeNode()
			: Chunk(0), X(0), Y(0)
		{
			Nodes[0] = 0;
			Nodes[1] = 0;
			Nodes[2] = 0;
			Nodes[3] = 0;
		}

		SQuadTreeNode::~SQuadTreeNode()
		{
			if(Nodes[0] != NULL)
				delete Nodes[0];
			if(Nodes[1] != NULL)
				delete Nodes[1];
			if(Nodes[2] != NULL)
				delete Nodes[2];
			if(Nodes[3] != NULL)
				delete Nodes[3];

			Nodes[0] = Nodes[1] = Nodes[2] = Nodes[3] = 0;

			Chunk = 0;
		}

		void CQuadTree::Populate(SQuadTreeNode* node, int coverage, NGin::ArrayList<NGin::ArrayList<CChunk*> > &chunks)
		{
			float PosY = Position.Y;

			int BoxSize = coverage / 2;

			if(coverage > 1)
			{
				node->Nodes[0] = new SQuadTreeNode();
				node->Nodes[1] = new SQuadTreeNode();
				node->Nodes[2] = new SQuadTreeNode();
				node->Nodes[3] = new SQuadTreeNode();

 				node->Nodes[0]->X = node->X;
 				node->Nodes[1]->X = node->X + BoxSize;
 				node->Nodes[2]->X = node->X;
 				node->Nodes[3]->X = node->X + BoxSize;
 				node->Nodes[0]->Y = node->Y;
 				node->Nodes[1]->Y = node->Y;
 				node->Nodes[2]->Y = node->Y + BoxSize;
 				node->Nodes[3]->Y = node->Y + BoxSize;

				Populate(node->Nodes[0], BoxSize, chunks);
				Populate(node->Nodes[1], BoxSize, chunks);
				Populate(node->Nodes[2], BoxSize, chunks);
				Populate(node->Nodes[3], BoxSize, chunks);

				NGin::Math::Vector3 Min = node->Nodes[0]->BoundingBox._Min;
				NGin::Math::Vector3 Max = node->Nodes[0]->BoundingBox._Max;

				for(int i = 1; i < 4; ++i)
				{
					if(node->Nodes[i]->BoundingBox._Min.X < Min.X)
						Min.X = node->Nodes[i]->BoundingBox._Min.X;
					if(node->Nodes[i]->BoundingBox._Min.Y < Min.Y)
						Min.Y = node->Nodes[i]->BoundingBox._Min.Y;
					if(node->Nodes[i]->BoundingBox._Min.Z < Min.Z)
						Min.Z = node->Nodes[i]->BoundingBox._Min.Z;

					if(node->Nodes[i]->BoundingBox._Max.X > Max.X)
						Max.X = node->Nodes[i]->BoundingBox._Max.X;
					if(node->Nodes[i]->BoundingBox._Max.Y > Max.Y)
						Max.Y = node->Nodes[i]->BoundingBox._Max.Y;
					if(node->Nodes[i]->BoundingBox._Max.Z > Max.Z)
						Max.Z = node->Nodes[i]->BoundingBox._Max.Z;
				}

				node->BoundingBox._Min = Min;
				node->BoundingBox._Max = Max;
				node->BoundingBox._SetFirst = true;

			}else if(coverage == 1) // Its the lowest level!
			{
				node->Chunk = chunks[node->X][node->Y];

				// Copy bounding box back up
				if(node->Chunk != 0)
				{
					node->BoundingBox = node->Chunk->GetBoundingBox();
					node->BoundingBox._Min.X = ((float)node->X * (64 * 2)) + Position.X;
					node->BoundingBox._Min.Z = ((float)node->Y * (64 * 2)) + Position.Z;
					node->BoundingBox._Max.X = node->BoundingBox._Min.X + (64.0f * 2.0f);
					node->BoundingBox._Max.Z = node->BoundingBox._Min.Z + (64.0f * 2.0f);
				}
			}

		}

		CQuadTree::CQuadTree(int tileCount, float tileWidth)
			: TileCount(tileCount), TileWidth(tileWidth), Tree(0)
		{
		}

		CQuadTree::~CQuadTree()
		{
			if(Tree != 0)
				delete Tree;
			Tree = 0;
		}

		void CQuadTree::Rebuild(NGin::Math::Vector3& position, NGin::ArrayList<NGin::ArrayList<CChunk*> > &chunks)
		{
			Position = position;

			if(Tree != NULL)
				delete Tree;
			Tree = new SQuadTreeNode();

			int Coverage = TileCount;
			Tree->X = 0;
			Tree->Y = 0;
			Tree->BoundingBox = NGin::Math::AABB(NGin::Math::Vector3(Position.X, -10000.0f, Position.Z), NGin::Math::Vector3(Position.X + (((float)TileCount * TileWidth) * 2), 10000.0f, Position.Z + (((float)TileCount * TileWidth) * 2)));

			Populate(Tree, Coverage, chunks);
		}

		void CQuadTree::TestQuad(SQuadTreeNode* node, Frustum &viewFrustum, std::list<CChunk*> &renderChunks, std::list<CChunk*> &lodChunks, NGin::Math::Vector3 &offset)
		{
			NGin::Math::AABB Box = node->BoundingBox;
			Box._Min += offset;
			Box._Max += offset;

			if(viewFrustum.BoxInFrustum(Box))
			{
				if(node->Chunk != 0)
					if(node->Chunk->GetLODLevel() > 1)
						lodChunks.push_back(node->Chunk);
					else
						renderChunks.push_back(node->Chunk);

				if(node->Nodes[0] != 0)
					TestQuad(node->Nodes[0], viewFrustum, renderChunks, lodChunks, offset);
				if(node->Nodes[1] != 0)
					TestQuad(node->Nodes[1], viewFrustum, renderChunks, lodChunks, offset);
				if(node->Nodes[2] != 0)
					TestQuad(node->Nodes[2], viewFrustum, renderChunks, lodChunks, offset);
				if(node->Nodes[3] != 0)
					TestQuad(node->Nodes[3], viewFrustum, renderChunks, lodChunks, offset);
			}
		}

		void CQuadTree::GetVisibleChunks(Frustum &viewFrustum, std::list<CChunk*> &renderChunks, std::list<CChunk*> &lodChunks, NGin::Math::Vector3 &offset)
		{
			if(Tree != 0)
			{
				TestQuad(Tree, viewFrustum, renderChunks, lodChunks, offset);
			}
			
		}

	}
}