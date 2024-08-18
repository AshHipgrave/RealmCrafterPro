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

#include "CTreeZone.h"
#include "CTreeType.h"

namespace LT
{
	CTreeZone::CTreeZone(CTreeManager* manager)
		: MinX(0.0f), MinZ(0.0f), MaxX(128.0f), MaxZ(128.0f),
			GridX(0), GridZ(0),
			Manager(manager)
	{
		std::vector<STreeSector*> Default;
		Default.push_back(0);
		SectorMap.push_back(Default);
	}

	CTreeZone::~CTreeZone()
	{
		// Destroy everything.
		int SectorWidth = SectorMap.size();
		int SectorHeight = SectorMap[0].size();

		for(int x = 0; x < SectorWidth; ++x)
		{
			std::vector<STreeSector* >* ZMap = &(SectorMap[x]);
			for(int z = 0; z < SectorHeight; ++z)
			{
				STreeSector* Sector = ZMap->at(z);

				if(Sector != 0)
				{
					for(std::map<ITreeType*, std::vector<ITreeInstance*> >::iterator It = Sector->Trees.begin();
						It != Sector->Trees.end(); It++)
					{
						std::vector<ITreeInstance*>* Instances = &((*It).second);
						int TreeCnt = Instances->size();

						for(int TreeIdx = 0; TreeIdx < TreeCnt; ++TreeIdx)
						{
							ITreeInstance* Instance = Instances->at(TreeIdx);

							if(Instance != 0)
								delete Instance;
						}
						Instances->clear();
					}

					for(std::map<ITreeType*, std::vector<ITreeMeshBuffer*> >::iterator It = Sector->LODBuffers.begin();
						It != Sector->LODBuffers.end(); It++)
					{
						std::vector<ITreeMeshBuffer*>* Buffers = &((*It).second);
						int BufferCnt = Buffers->size();

						for(int BufferIdx = 0; BufferIdx < BufferCnt; ++BufferIdx)
						{
							ITreeMeshBuffer* Buffer = Buffers->at(BufferIdx);

							Manager->GetRenderer()->FreeMeshBuffer(Buffer);
						}
						Buffers->clear();
					}

					delete Sector;
				}
			}

			ZMap->clear();
		}

		SectorMap.clear();
	}

	STreeSector* CTreeZone::GetZoneSector( float x, float z, bool recursing /*= false*/ )
	{
		// Its inside the existing sector grid
		if(x >= MinX && z >= MinZ && x < MaxX && z < MaxZ)
		{
			// Get position as an integer to convert to grid space
			int tx = static_cast<int>(x);
			int tz = static_cast<int>(z);

			// Use modulus and division to find the grid offset
			tx = (tx - (tx % 128)) / 128;
			tz = (tz - (tz % 128)) / 128;

			// The grid array can start from negative values, so add in an offset
			if(tx < 0)
				tx -= 1;
			if(tz < 0)
				tz -= 1;

			// Translate the 'current' position into a world position incase we make
			// a new sector.
			NGin::Math::Vector2 SectorMin(static_cast<float>(tx) * 128.0f, static_cast<float>(tz) * 128.0f);
			NGin::Math::Vector2 SectorMax(SectorMin + NGin::Math::Vector2(128.0f, 128.0f));

			// Convert to usable offsets
			tx += GridX;
			tz += GridZ;

			// Now obtain the sector
			STreeSector* Sector = 0;
			if(tx < SectorMap.size() && tz < SectorMap[0].size())
				Sector = SectorMap[tx][tz];

			// The sector might not actually exist, since we don't create what we don't need
			if(Sector == 0)
			{
				Sector = new STreeSector(SectorMin, SectorMax, this);
				SectorMap[tx][tz] = Sector;
			}

			// Done, return it
			return Sector;
		}

		// If we're recursing then die a terrible death!
		if(recursing)
			return 0;

		// The position is outside the grid that we currently represent.
		// We need to expand the grid to store the extra zones.

		// First, we need to add more Z instances in the positive direction
		if(z >= MaxZ)
		{
			int CurrentZ = SectorMap[0].size() - 1;
			int NewZ = static_cast<int>(z);
			NewZ = (NewZ - (NewZ % 128)) / 128;
			if(NewZ < 0)
				NewZ -= 1;
			NewZ += GridZ;

			for(int i = CurrentZ; i < NewZ; ++i)
			{
				for(int tx = 0; tx < SectorMap.size(); ++tx)
				{
					SectorMap[tx].push_back(0);
				}

				MaxZ += 128.0f;
			}
		}

		// Now, we need to add more Z instances in the negative direction
		if(z < MinZ)
		{
			int NewZ = static_cast<int>(z);
			NewZ = (NewZ - (NewZ % 128)) / 128;
			if(NewZ < 0)
				NewZ -= 1;
			NewZ += GridZ;

			for(int i = 0; i > NewZ; --i)
			{
				for(int tx = 0; tx < SectorMap.size(); ++tx)
				{
					SectorMap[tx].push_back(0);
					for(int tz = SectorMap[tx].size() - 1; tz > 0; --tz)
						SectorMap[tx][tz] = SectorMap[tx][tz - 1];
					SectorMap[tx][0] = 0;
				}

				MinZ -= 128.0f;
				GridZ += 1;
			}
		}

		// Add X columns
		if(x >= MaxX)
		{
			int CurrentX = SectorMap.size() - 1;
			int NewX = static_cast<int>(x);
			NewX = (NewX - (NewX % 128)) / 128;
			if(NewX < 0)
				NewX -= 1;
			NewX += GridX;

			for(int i = CurrentX; i < NewX; ++i)
			{
				std::vector<STreeSector*> NewCollumn;
				for(int n = 0; n < SectorMap[0].size(); ++n)
					NewCollumn.push_back(0);

				SectorMap.push_back(NewCollumn);
				MaxX += 128.0f;
			}
		}

		// Add -X
		if(x < MinX)
		{
			int NewX = static_cast<int>(x);
			NewX = (NewX - (NewX % 128)) / 128;
			if(NewX < 0)
				NewX -= 1;
			NewX += GridX;

			for(int i = 0; i > NewX; --i)
			{
				std::vector<STreeSector*> NewCollumn;
				for(int n = 0; n < SectorMap[0].size(); ++n)
					NewCollumn.push_back(0);

				std::vector<STreeSector*> Empty;
				SectorMap.push_back(Empty);

				for(int tx = SectorMap.size() - 1; tx > 0; --tx)
					SectorMap[tx] = SectorMap[tx - 1];

				SectorMap[0] = NewCollumn;
				MaxX -= 128.0f;
				GridZ += 1;
			}
		}

		return GetZoneSector(x, z, true);
	}

	void CTreeZone::Remove(ITreeInstance* instance)
	{
		CTreeInstance* Instance = dynamic_cast<CTreeInstance*>(instance);

		if(Instance == 0)
			return;

		STreeSector* Sector = Instance->GetParentSector();
		ITreeType* Type = Instance->GetType();

		for(int i = 0; i < TreeTypes.size(); ++i)
		{
			if(TreeTypes[i]->TreeType == Type)
			{
				--TreeTypes[i]->RefCount;
				break;
			}
		}

		if(Sector == 0)
			return;

		std::vector<ITreeInstance* >* Instances = &(Sector->Trees[Type]);
		int DelIndex = -1;
		for(int i = 0; i < Instances->size(); ++i)
		{
			if(Instances->at(i) == Instance)
			{
				DelIndex = i;
				break;
			}
		}

		if(DelIndex > -1)
			Instances->erase(Instances->begin() + DelIndex);

		RequestReBuild(Sector);

		// Only once we've removed the instance from a zone will we delete it (this prevents using Remove on an incorrect zone).
		delete Instance;
	}

	ITreeInstance* CTreeZone::AddTreeInstance( ITreeType* type, NGin::Math::Vector3 &position )
	{
		// Get a sector to place this instance
		STreeSector* Sector = GetZoneSector(position.X, position.Z);
		if(Sector == 0)
			return 0;

		bool Found = false;
		for(int i = 0; i < TreeTypes.size(); ++i)
		{
			if(TreeTypes[i]->TreeType == type)
			{
				++TreeTypes[i]->RefCount;
				Found = true;
				break;
			}
		}

		if(!Found)
		{
			TreeTypeRef* Ref = new TreeTypeRef();
			Ref->TreeType = type;
			Ref->RefCount = 1;
			TreeTypes.push_back(Ref);
		}

		CTreeInstance* Instance = new CTreeInstance(type, Sector);
		Sector->Trees[type].push_back(Instance);
		Instance->SetPosition(position);
		Instance->UpdateCollisions(true);

		if(Locked)
			UniqueTreeInsert(Sector, LockedSectors);
		else
			Sector->ReBuild(Manager->GetRenderer());

		return Instance;
	}

	void CTreeZone::RenderDepth( RealmCrafter::RCT::Frustum &viewFrustum, NGin::Math::Vector3 &cameraPosition, unsigned int &treesDrawn, unsigned int &lodsDrawn, NGin::Math::Vector3 &offset)
	{
		// Locals
		std::list<STreeSector*> FullSectors;
		std::list<ITreeType*> FullTreeTypes;
		std::vector<std::list<ITreeInstance*> > FullRenderInstances;

		// We need to translate the camera position into 'renderable' positions
		float RenderDistance = Manager->GetRenderDistance();
		float LODDistance = Manager->GetLODDistance();
// 		float PointLightDistance = Manager->GetPointLightDistance();
// 		PointLightDistance *= PointLightDistance;

		float CamMinX = cameraPosition.X - (RenderDistance + 128.0f);
		float CamMinZ = cameraPosition.Z - (RenderDistance + 128.0f);
		float CamMaxX = cameraPosition.X + RenderDistance;
		float CamMaxZ = cameraPosition.Z + RenderDistance;
		float CamLODMinX = cameraPosition.X - (LODDistance + 128.0f);
		float CamLODMinZ = cameraPosition.Z - (LODDistance + 128.0f);
		float CamLODMaxX = cameraPosition.X + LODDistance;
		float CamLODMaxZ = cameraPosition.Z + LODDistance;

		// Min/Max need to be converted into grid positions
		int LookupLODMinX = static_cast<int>(CamLODMinX);
		int LookupLODMinZ = static_cast<int>(CamLODMinZ);
		int LookupLODMaxX = static_cast<int>(CamLODMaxX);
		int LookupLODMaxZ = static_cast<int>(CamLODMaxZ);

		// Use modulus and division to find the grid offset
		LookupLODMinX = (LookupLODMinX - (LookupLODMinX % 128)) / 128;
		LookupLODMinZ = (LookupLODMinZ - (LookupLODMinZ % 128)) / 128;
		LookupLODMaxX = (LookupLODMaxX - (LookupLODMaxX % 128)) / 128;
		LookupLODMaxZ = (LookupLODMaxZ - (LookupLODMaxZ % 128)) / 128;

		// The grid array can start from negative values, so add in an offset
		if(LookupLODMinX < 0)
			LookupLODMinX -= 1;
		if(LookupLODMinZ < 0)
			LookupLODMinZ -= 1;
		if(LookupLODMaxX < 0)
			LookupLODMaxX -= 1;
		if(LookupLODMaxZ < 0)
			LookupLODMaxZ -= 1;

		// Add the zone grid offsets to get the correct array indices
		LookupLODMinX += GridX;
		LookupLODMinZ += GridZ;
		LookupLODMaxX += GridX;
		LookupLODMaxZ += GridZ;

		// Finally, cap all values to their array bounds.
		if(LookupLODMinX < 0)
			LookupLODMinX = 0;
		if(LookupLODMinZ < 0)
			LookupLODMinZ = 0;
		if(LookupLODMinX >= SectorMap.size())
			LookupLODMinX = SectorMap.size() - 1;
		if(LookupLODMinZ >= SectorMap[0].size())
			LookupLODMinZ = SectorMap[0].size() - 1;

		if(LookupLODMaxX < 0)
			LookupLODMaxX = 0;
		if(LookupLODMaxZ < 0)
			LookupLODMaxZ = 0;
		if(LookupLODMaxX >= SectorMap.size())
			LookupLODMaxX = SectorMap.size() - 1;
		if(LookupLODMaxZ >= SectorMap[0].size())
			LookupLODMaxZ = SectorMap[0].size() - 1;


		// Now we have array bounds to search when rendering with (rather than testing the entire world).
		// We now have to populate the lists
		for(int x = LookupLODMinX; x <= LookupLODMaxX; ++x)
		{
			for(int z = LookupLODMinZ; z <= LookupLODMaxZ; ++z)
			{
				// Get the sector
				STreeSector* Sector = SectorMap[x][z];
				if(Sector == 0)
					continue;

				// Build an AABB out of the sector
				NGin::Math::AABB BoundingBox(
					NGin::Math::Vector3(Sector->Min.X, -20000, Sector->Min.Y),
					NGin::Math::Vector3(Sector->Max.X, 20000, Sector->Max.Y));
				BoundingBox._Min += offset;
				BoundingBox._Max += offset;

				// Check frustum
				if(viewFrustum.BoxInFrustum(BoundingBox))
				{
					// Push sector
					FullSectors.push_back(Sector);

					// Push tree types
					for(std::map<ITreeType*, std::vector<ITreeInstance*> >::iterator It = Sector->Trees.begin();
						It != Sector->Trees.end(); It++)
						UniqueTreeInsert(It->first, FullTreeTypes);
				}
			}
		}

		// Start rendering trunks
		if(!Manager->GetRenderer()->PreRenderTrunkDepth())
			return;

		// Loop through types first
		int TypeIdx = 0;
		for(std::list<ITreeType*>::iterator TIt = FullTreeTypes.begin(); TIt != FullTreeTypes.end(); TIt++)
		{
			// Get implementation type and trunk count
			CTreeType* Type = dynamic_cast<CTreeType*>(*TIt);
			int TrunkCount = Type->GetTrunkSurfaceCount();

			std::list<ITreeInstance*> TempList;
			FullRenderInstances.push_back(TempList);

			// Render in trunk over
			for(int Trunk = 0; Trunk < TrunkCount; ++Trunk)
			{
				// Setup for rendering
				Type->BeginTrunk(Trunk);

				// Run through each sector
				for(std::list<STreeSector*>::iterator SIt = FullSectors.begin(); SIt != FullSectors.end(); SIt++)
				{
					// Grab instance list
					std::vector<ITreeInstance*> Instances = (*SIt)->Trees[Type];

					// See if the sector is entirely in view.. this will save some processing time in some cases
					NGin::Math::AABB SectorBox;
					SectorBox.AddPoint((*SIt)->Min.X, -50000.0f, (*SIt)->Min.Y);
					SectorBox.AddPoint((*SIt)->Max.X, 50000.0f, (*SIt)->Max.Y);

					SectorBox._Min += offset;
					SectorBox._Max += offset;

					if(viewFrustum.BoxInFrustum(SectorBox))
					{
						for(int TreeInstance = 0; TreeInstance < Instances.size(); ++TreeInstance)
						{
							NGin::Math::AABB Box = Type->GetBoundingBox();
							NGin::Math::Matrix World = Instances[TreeInstance]->GetTransform();
							//World.TransformVector(&(Box._Min));
							//World.TransformVector(&(Box._Max));
							Box.Transform(World);

							Box._Min += offset;
							Box._Max += offset;

							if(viewFrustum.BoxInFrustum(Box))
							{

								FullRenderInstances[TypeIdx].push_back(Instances[TreeInstance]);

								LT::TParameter<NGin::Math::Matrix> WorldParam;
								World.Translation(World.Translation() + offset);
								WorldParam.SetData(World);
								Manager->GetRenderer()->SetParameter("World", WorldParam);

								Type->DrawTrunk(Trunk);
								++treesDrawn;
							}
						}
					} // Sector frustum test
				} // Sectors
			} // Trunks

			++TypeIdx;
		} // Types

		// Finnish trunks
		Manager->GetRenderer()->PostRenderTrunkDepth();

		// Start rendering leaves
		if(!Manager->GetRenderer()->PreRenderLeafDepth())
			return;

		// Loop through types first
		TypeIdx = 0;
		for(std::list<ITreeType*>::iterator TIt = FullTreeTypes.begin(); TIt != FullTreeTypes.end(); TIt++)
		{
			// Get implementation type and trunk count
			CTreeType* Type = dynamic_cast<CTreeType*>(*TIt);
			int LeafCount = Type->GetLeafSurfaceCount();

			// Render in trunk over
			for(int Leaf = 0; Leaf < LeafCount; ++Leaf)
			{
				// Setup for rendering
				Type->BeginLeaf(Leaf);

				for(std::list<ITreeInstance*>::iterator IIt = FullRenderInstances[TypeIdx].begin(); IIt != FullRenderInstances[TypeIdx].end(); IIt++)
				{
					LT::TParameter<NGin::Math::Matrix> WorldParam;
					NGin::Math::Matrix WorldMat = (*IIt)->GetTransform();
					WorldMat.Translation(WorldMat.Translation() + offset);
					WorldParam.SetData(WorldMat);
					Manager->GetRenderer()->SetParameter("World", WorldParam);

					Type->DrawLeaf(Leaf);
				}

			} // Leafs

			++TypeIdx;
		} // Types

		// Finnish trunks
		Manager->GetRenderer()->PostRenderLeafDepth();

	}

	void CTreeZone::Render(int rtIndex, RealmCrafter::RCT::Frustum &viewFrustum, NGin::Math::Vector3 &cameraPosition, unsigned int &treesDrawn, unsigned int &lodsDrawn, NGin::Math::Vector3 &offset)
	{
		// Locals
		std::list<STreeSector*> FullSectors;
		std::list<STreeSector*> LODSectors;
		std::list<ITreeType*> FullTreeTypes;
		std::vector<std::list<ITreeInstance*> > FullRenderInstances;
		std::list<ITreeType*> LODTreeTypes;

		// We need to translate the camera position into 'renderable' positions
		float RenderDistance = Manager->GetRenderDistance();
		float LODDistance = Manager->GetLODDistance();
		float PointLightDistance = Manager->GetPointLightDistance();
		PointLightDistance *= PointLightDistance;

		float CamMinX = cameraPosition.X - (RenderDistance + 128.0f);
		float CamMinZ = cameraPosition.Z - (RenderDistance + 128.0f);
		float CamMaxX = cameraPosition.X + RenderDistance;
		float CamMaxZ = cameraPosition.Z + RenderDistance;
		float CamLODMinX = cameraPosition.X - (LODDistance + 128.0f);
		float CamLODMinZ = cameraPosition.Z - (LODDistance + 128.0f);
		float CamLODMaxX = cameraPosition.X + LODDistance;
		float CamLODMaxZ = cameraPosition.Z + LODDistance;

		// Min/Max need to be converted into grid positions
		int LookupMinX = static_cast<int>(CamMinX);
		int LookupMinZ = static_cast<int>(CamMinZ);
		int LookupMaxX = static_cast<int>(CamMaxX);
		int LookupMaxZ = static_cast<int>(CamMaxZ);
		int LookupLODMinX = static_cast<int>(CamLODMinX);
		int LookupLODMinZ = static_cast<int>(CamLODMinZ);
		int LookupLODMaxX = static_cast<int>(CamLODMaxX);
		int LookupLODMaxZ = static_cast<int>(CamLODMaxZ);

		// Use modulus and division to find the grid offset
		LookupMinX = (LookupMinX - (LookupMinX % 128)) / 128;
		LookupMinZ = (LookupMinZ - (LookupMinZ % 128)) / 128;
		LookupMaxX = (LookupMaxX - (LookupMaxX % 128)) / 128;
		LookupMaxZ = (LookupMaxZ - (LookupMaxZ % 128)) / 128;
		LookupLODMinX = (LookupLODMinX - (LookupLODMinX % 128)) / 128;
		LookupLODMinZ = (LookupLODMinZ - (LookupLODMinZ % 128)) / 128;
		LookupLODMaxX = (LookupLODMaxX - (LookupLODMaxX % 128)) / 128;
		LookupLODMaxZ = (LookupLODMaxZ - (LookupLODMaxZ % 128)) / 128;

		// The grid array can start from negative values, so add in an offset
		if(LookupMinX < 0)
			LookupMinX -= 1;
		if(LookupMinZ < 0)
			LookupMinZ -= 1;
		if(LookupMaxX < 0)
			LookupMaxX -= 1;
		if(LookupMaxZ < 0)
			LookupMaxZ -= 1;
		if(LookupLODMinX < 0)
			LookupLODMinX -= 1;
		if(LookupLODMinZ < 0)
			LookupLODMinZ -= 1;
		if(LookupLODMaxX < 0)
			LookupLODMaxX -= 1;
		if(LookupLODMaxZ < 0)
			LookupLODMaxZ -= 1;

		// Add the zone grid offsets to get the correct array indices
		LookupMinX += GridX;
		LookupMinZ += GridZ;
		LookupMaxX += GridX;
		LookupMaxZ += GridZ;
		LookupLODMinX += GridX;
		LookupLODMinZ += GridZ;
		LookupLODMaxX += GridX;
		LookupLODMaxZ += GridZ;

		// Finally, cap all values to their array bounds.
		if(LookupMinX < 0)
			LookupMinX = 0;
		if(LookupMinZ < 0)
			LookupMinZ = 0;
		if(LookupMinX >= SectorMap.size())
			LookupMinX = SectorMap.size() - 1;
		if(LookupMinZ >= SectorMap[0].size())
			LookupMinZ = SectorMap[0].size() - 1;

		if(LookupMaxX < 0)
			LookupMaxX = 0;
		if(LookupMaxZ < 0)
			LookupMaxZ = 0;
		if(LookupMaxX >= SectorMap.size())
			LookupMaxX = SectorMap.size() - 1;
		if(LookupMaxZ >= SectorMap[0].size())
			LookupMaxZ = SectorMap[0].size() - 1;

		if(LookupLODMinX < 0)
			LookupLODMinX = 0;
		if(LookupLODMinZ < 0)
			LookupLODMinZ = 0;
		if(LookupLODMinX >= SectorMap.size())
			LookupLODMinX = SectorMap.size() - 1;
		if(LookupLODMinZ >= SectorMap[0].size())
			LookupLODMinZ = SectorMap[0].size() - 1;

		if(LookupLODMaxX < 0)
			LookupLODMaxX = 0;
		if(LookupLODMaxZ < 0)
			LookupLODMaxZ = 0;
		if(LookupLODMaxX >= SectorMap.size())
			LookupLODMaxX = SectorMap.size() - 1;
		if(LookupLODMaxZ >= SectorMap[0].size())
			LookupLODMaxZ = SectorMap[0].size() - 1;

// 		HWND Wnd = GetActiveWindow();
// 		char OO[2048];
// 		sprintf(OO, "(%i, %i) -> (%i, %i); (%i, %i) -> (%i, %i)",
// 			LookupMinX, LookupMinZ, LookupMaxX, LookupMaxZ,
// 			LookupLODMinX, LookupLODMinZ, LookupLODMaxX, LookupLODMaxZ);
// 		SetWindowText(Wnd, OO);

		// Now we have array bounds to search when rendering with (rather than testing the entire world).
		// We now have to populate the lists
		for(int x = LookupMinX; x <= LookupMaxX; ++x)
		{
			for(int z = LookupMinZ; z <= LookupMaxZ; ++z)
			{
				// Get the sector
				STreeSector* Sector = SectorMap[x][z];
				if(Sector == 0)
					continue;

				// Build an AABB out of the sector
				NGin::Math::AABB BoundingBox(
					NGin::Math::Vector3(Sector->Min.X, -20000, Sector->Min.Y),
					NGin::Math::Vector3(Sector->Max.X, 20000, Sector->Max.Y));

				BoundingBox._Min += offset;
				BoundingBox._Max += offset;

				// Check frustum
				if(viewFrustum.BoxInFrustum(BoundingBox))
				{
					// Check LOD/Full status
					if(x >= LookupLODMinX
						&& z >= LookupLODMinZ
						&& x <= LookupLODMaxX + 1
						&& z <= LookupLODMaxZ + 1)
					{
						// Push sector
						FullSectors.push_back(Sector);

						// Push tree types
						for(std::map<ITreeType*, std::vector<ITreeInstance*> >::iterator It = Sector->Trees.begin();
							It != Sector->Trees.end(); It++)
							UniqueTreeInsert(It->first, FullTreeTypes);
					}else
					{
						// Push sector
						LODSectors.push_back(Sector);

						// Push tree types
						for(std::map<ITreeType*, std::vector<ITreeInstance*> >::iterator It = Sector->Trees.begin();
							It != Sector->Trees.end(); It++)
							UniqueTreeInsert(It->first, LODTreeTypes);
					}
				}
			}
		}

		// Start rendering trunks
		if(!Manager->GetRenderer()->PreRenderTrunk(rtIndex))
			return;

		// Loop through types first
		int TypeIdx = 0;
		for(std::list<ITreeType*>::iterator TIt = FullTreeTypes.begin(); TIt != FullTreeTypes.end(); TIt++)
		{
			// Get implementation type and trunk count
			CTreeType* Type = dynamic_cast<CTreeType*>(*TIt);
			int TrunkCount = Type->GetTrunkSurfaceCount();

			std::list<ITreeInstance*> TempList;
			FullRenderInstances.push_back(TempList);

			// Render in trunk over
			for(int Trunk = 0; Trunk < TrunkCount; ++Trunk)
			{
				// Setup for rendering
				Type->BeginTrunk(Trunk);

				// Run through each sector
				for(std::list<STreeSector*>::iterator SIt = FullSectors.begin(); SIt != FullSectors.end(); SIt++)
				{
					// Grab instance list
					std::vector<ITreeInstance*> Instances = (*SIt)->Trees[Type];
					
					// See if the sector is entirely in view.. this will save some processing time in some cases
					NGin::Math::AABB SectorBox;
					SectorBox.AddPoint((*SIt)->Min.X, -50000.0f, (*SIt)->Min.Y);
					SectorBox.AddPoint((*SIt)->Max.X, 50000.0f, (*SIt)->Max.Y);

					SectorBox._Min += offset;
					SectorBox._Max += offset;

					if(viewFrustum.BoxInFrustum(SectorBox))
					{
						for(int TreeInstance = 0; TreeInstance < Instances.size(); ++TreeInstance)
						{
							NGin::Math::AABB Box = Type->GetBoundingBox();
							NGin::Math::Matrix World = Instances[TreeInstance]->GetTransform();
							//World.TransformVector(&(Box._Min));
							//World.TransformVector(&(Box._Max));
							Box.Transform(World);

							Box._Min += offset;
							Box._Max += offset;

							if(viewFrustum.BoxInFrustum(Box))
							{
								if(World.Translation().DistanceSq(cameraPosition) < PointLightDistance)
									Manager->GetRenderer()->SetPointLightParameters(World.Translation());

								FullRenderInstances[TypeIdx].push_back(Instances[TreeInstance]);

								LT::TParameter<NGin::Math::Matrix> WorldParam;
								World.Translation(World.Translation() + offset);
								WorldParam.SetData(World);
								Manager->GetRenderer()->SetParameter("World", WorldParam);

								Type->DrawTrunk(Trunk);
								++treesDrawn;
							}
						}
					} // Sector frustum test
				} // Sectors
			} // Trunks

			++TypeIdx;
		} // Types

		// Finnish trunks
		Manager->GetRenderer()->PostRenderTrunk();

		// Start rendering leaves
		if(!Manager->GetRenderer()->PreRenderLeaf(rtIndex))
			return;

		// Loop through types first
		TypeIdx = 0;
		for(std::list<ITreeType*>::iterator TIt = FullTreeTypes.begin(); TIt != FullTreeTypes.end(); TIt++)
		{
			// Get implementation type and trunk count
			CTreeType* Type = dynamic_cast<CTreeType*>(*TIt);
			int LeafCount = Type->GetLeafSurfaceCount();

			// Render in trunk over
			for(int Leaf = 0; Leaf < LeafCount; ++Leaf)
			{
				// Setup for rendering
				Type->BeginLeaf(Leaf);

				for(std::list<ITreeInstance*>::iterator IIt = FullRenderInstances[TypeIdx].begin(); IIt != FullRenderInstances[TypeIdx].end(); IIt++)
				{
					LT::TParameter<NGin::Math::Matrix> WorldParam;
					NGin::Math::Matrix WorldMat = (*IIt)->GetTransform();
					WorldMat.Translation(WorldMat.Translation() + offset);
					WorldParam.SetData(WorldMat);
					Manager->GetRenderer()->SetParameter("World", WorldParam);

					Type->DrawLeaf(Leaf);
				}

			} // Leafs

			++TypeIdx;
		} // Types

		// Finnish trunks
		Manager->GetRenderer()->PostRenderLeaf();

		// Start rendering LODS
		if(!Manager->GetRenderer()->PreRenderLOD(rtIndex))
			return;

		LT::TParameter<NGin::Math::Matrix> LODWorldParam;
		NGin::Math::Matrix Identity;
		Identity.Translation(offset);
		LODWorldParam.SetData(Identity);
		Manager->GetRenderer()->SetParameter("World", LODWorldParam);

		for(std::list<ITreeType*>::iterator It = LODTreeTypes.begin(); It != LODTreeTypes.end(); ++It)
		{
			// Real Type
			CTreeType* Type = dynamic_cast<CTreeType*>(*It);
			int LODCount = Type->GetLODTexturesCount();

			for(int i = 0; i < LODCount; ++i)
			{
				Type->BeginLOD(i);

				for(std::list<STreeSector*>::iterator SIt = LODSectors.begin(); SIt != LODSectors.end(); ++SIt)
				{
					STreeSector* Sector = *SIt;
					
					if(Sector != 0 && Sector->LODBuffers[Type].size() > i)
					{
						ITreeMeshBuffer* Buffer = Sector->LODBuffers[Type][i];
						if(Buffer != 0)
						{
							Type->DrawLOD(Buffer);
							lodsDrawn += (Buffer->GetTriangleCount() / 2);
						}
					}

				}
			}
		} // Tree Types

		// Finnished LODS
		Manager->GetRenderer()->PostRenderLOD();
	}

	void CTreeZone::Lock()
	{
		Locked = true;
	}

	void CTreeZone::Unlock()
	{
		Locked = false;
		for(std::list<STreeSector*>::iterator It = LockedSectors.begin(); It != LockedSectors.end(); ++It)
			(*It)->ReBuild(Manager->GetRenderer());
		LockedSectors.clear();
	}

	bool CTreeZone::AllowReBuild( STreeSector* sector )
	{
		if(Locked)
		{
			UniqueTreeInsert(sector, LockedSectors);
			return false;
		}
		return true;
	}

	void CTreeZone::RequestReBuild( STreeSector* sector )
	{
		sector->ReBuild(Manager->GetRenderer());
	}
	void STreeSector::ReBuild( ITreeRenderer* renderer )
	{
		if(!Zone->AllowReBuild(this))
			return;

		// Count tree types
		std::list<ITreeType*> LODTreeTypes;
		for(std::map<ITreeType*, std::vector<ITreeInstance*> >::iterator It = Trees.begin();
			It != Trees.end(); It++)
			UniqueTreeInsert(It->first, LODTreeTypes);

		// Destroy existing buffers
		for(std::map<ITreeType*, std::vector<ITreeMeshBuffer*> >::iterator It = LODBuffers.begin();
			It != LODBuffers.end(); It++)
		{
			for (int i = 0; i < It->second.size(); ++i)
			{
				renderer->FreeMeshBuffer(It->second[i]);
			}
			It->second.clear();
		}
		LODBuffers.clear();

		for(std::list<ITreeType*>::iterator It = LODTreeTypes.begin(); It != LODTreeTypes.end(); ++It)
		{
			CTreeType* Type = dynamic_cast<CTreeType*>(*It);
			std::vector<ITreeInstance*>* Instances = &(Trees[(*It)]);

			// Get scale constants
			NGin::Math::AABB BoundingBox = Type->GetBoundingBox();
			NGin::Math::Vector3 BoxSize = ((BoundingBox._Max - BoundingBox._Min) * 0.5f);
			NGin::Math::Vector3 Center = BoxSize + BoundingBox._Min;
			if(BoxSize.Z > BoxSize.X)
				BoxSize.X = BoxSize.Z;
// 			if(BoxSize.Y > BoxSize.X)
// 				BoxSize.X = BoxSize.Y;
// 			else
// 				BoxSize.Y = BoxSize.X;
			NGin::Math::Vector2 V0(-BoxSize.X, -BoxSize.Y);
			NGin::Math::Vector2 V1(BoxSize.X, -BoxSize.Y);
			NGin::Math::Vector2 V2(-BoxSize.X, BoxSize.Y);
			NGin::Math::Vector2 V3(BoxSize.X, BoxSize.Y);

			// Setup our counter (determines how big to make the buffers)
			std::vector<LODSet*> LODCounts;
			int LODTextures = Type->GetLODTexturesCount();
			for(int i = 0; i < LODTextures; ++i)
				LODCounts.push_back(new LODSet());

			// Run through instances to work out which instance belongs where
			int CurrentTexture = 0;
			for(int i = 0; i < Instances->size(); ++i)
			{
				LODCounts[CurrentTexture]->Instances.push_back((*Instances)[i]);
				++CurrentTexture;
				if(CurrentTexture >= LODTextures)
					CurrentTexture = 0;
			}

			// Now run through the sets and create the buffers
			for(int i = 0; i < LODCounts.size(); ++i)
			{
				LODSet* Set = LODCounts[i];

				STreeLODVertex* Vertices = new STreeLODVertex[Set->Instances.size() * 4];
				unsigned short* Indices = new unsigned short[Set->Instances.size() * 6];

				int VCnt = 0;
				int ICnt = 0;

				for(std::list<ITreeInstance*>::iterator It = Set->Instances.begin(); It != Set->Instances.end(); ++It)
				{
					NGin::Math::Matrix Transform = (*It)->GetTransform();
					//NGin::Math::Vector2 Scale(Transform.Scale().X, Transform.Scale().Y);
					NGin::Math::Vector3 TransScale = (*It)->GetScale();;
					NGin::Math::Vector3 ScaledCenter = (Center * TransScale);

					Vertices[VCnt + 0].Set(Transform.Translation() + ScaledCenter, 0, 0, (V0.X * 2.0f) * TransScale.X, V0.Y * TransScale.Y);
					Vertices[VCnt + 1].Set(Transform.Translation() + ScaledCenter, 1, 0, (V1.X * 2.0f) * TransScale.X, V1.Y * TransScale.Y);
					Vertices[VCnt + 2].Set(Transform.Translation() + ScaledCenter, 0, 1, (V2.X * 2.0f) * TransScale.X, V2.Y * TransScale.Y);
					Vertices[VCnt + 3].Set(Transform.Translation() + ScaledCenter, 1, 1, (V3.X * 2.0f) * TransScale.X, V3.Y * TransScale.Y);

					Indices[ICnt + 0] = VCnt + 2;
					Indices[ICnt + 1] = VCnt + 1;
					Indices[ICnt + 2] = VCnt + 0;
					Indices[ICnt + 3] = VCnt + 2;
					Indices[ICnt + 4] = VCnt + 3;
					Indices[ICnt + 5] = VCnt + 1;

					VCnt += 4;
					ICnt += 6;
				}

				ITreeMeshBuffer* Buffer = renderer->CreateLODBuffer(Vertices, Set->Instances.size() * 4, Indices, Set->Instances.size() * 6);
				if(Buffer != 0)
					LODBuffers[Type].push_back(Buffer);


				delete[] Vertices;
				delete[] Indices;
				delete Set;
			}

		}
	}

	inline int CTreeZone::GetInstanceIndex(ITreeType* instance) const
	{
		for(int i = 0; i < TreeTypes.size(); ++i)
			if(TreeTypes[i]->TreeType == instance)
				return i;
		return -1;
	}

	bool CTreeZone::Save(const std::string &path) const
	{
		// Write file
		FILE* F = fopen(path.c_str(), "wb");
		if(F == 0)
			return false;

		char VMaj = 1, VMin = 0;
		fwrite(&VMaj, 1, 1, F);
		fwrite(&VMin, 1, 1, F);

		fwrite(&MinX, 4, 1, F);
		fwrite(&MinZ, 4, 1, F);
		fwrite(&MaxX, 4, 1, F);
		fwrite(&MaxZ, 4, 1, F);
		fwrite(&GridX, 4, 1, F);
		fwrite(&GridZ, 4, 1, F);

		int TypeCount = TreeTypes.size();
		fwrite(&TypeCount, 4, 1, F);
		for(int i = 0; i < TreeTypes.size(); ++i)
		{
			std::string Name = TreeTypes[i]->TreeType->GetName();
			int NameLength = Name.length();
			const char* NameChars = Name.c_str();

			fwrite(&NameLength, 4, 1, F);
			fwrite(NameChars, 1, NameLength, F);
		}

		int SectorWidth = SectorMap.size();
		int SectorHeight = SectorMap[0].size();

		fwrite(&SectorWidth, 4, 1, F);
		fwrite(&SectorHeight, 4, 1, F);

		for(int x = 0; x < SectorWidth; ++x)
		{
			const std::vector<STreeSector* >* ZMap = &(SectorMap[x]);
			for(int z = 0; z < SectorHeight; ++z)
			{
				STreeSector* Sector = ZMap->at(z);

				char WriteSector = 0;
				if(Sector != 0)
					WriteSector = 1;
				fwrite(&WriteSector, 1, 1, F);

				if(Sector != 0)
				{
					fwrite(&(Sector->Min), sizeof(NGin::Math::Vector2), 1, F);
					fwrite(&(Sector->Max), sizeof(NGin::Math::Vector2), 1, F);

					int InstanceCnt = 0;
					for(std::map<ITreeType*, std::vector<ITreeInstance*> >::iterator It = Sector->Trees.begin();
						It != Sector->Trees.end(); It++)
					{
						++InstanceCnt;
					}
					fwrite(&InstanceCnt, 4, 1, F);

					for(std::map<ITreeType*, std::vector<ITreeInstance*> >::iterator It = Sector->Trees.begin();
						It != Sector->Trees.end(); It++)
					{
						int InstanceIdx = GetInstanceIndex((*It).first);
						std::vector<ITreeInstance*>* Instances = &((*It).second);
						unsigned int TreeCnt = Instances->size();

						fwrite(&InstanceIdx, 4, 1, F);
						fwrite(&TreeCnt, 4, 1, F);

						for(int TreeIdx = 0; TreeIdx < TreeCnt; ++TreeIdx)
						{
							ITreeInstance* Instance = Instances->at(TreeIdx);
							
							NGin::Math::Matrix Transform = Instance->GetTransform();
							NGin::Math::Vector3 Position = Transform.Translation();
							NGin::Math::Vector3 Scale = Instance->GetScale();
							//NGin::Math::Quaternion Rotation = Transform.Rotation();
							NGin::Math::Vector3 Rotation = Instance->GetRotation();

							fwrite(&Position, sizeof(NGin::Math::Vector3), 1, F);
							fwrite(&Scale, sizeof(NGin::Math::Vector3), 1, F);
							fwrite(&Rotation, sizeof(NGin::Math::Vector3), 1, F);
							fwrite(&Rotation, sizeof(float), 1, F);

						}
						
					}
				}
			}
		}

		fclose(F);
		return true;

	}

	bool CTreeZone::Load(const std::string &path)
	{
		// Open file
		FILE* F = fopen(path.c_str(), "rb");
		if(F == 0)
			return false;

		char VMaj = 0, VMin = 0;
		int TypesCount = 0;
		int SectorWidth = 0, SectorHeight = 0;

		fread(&VMaj, 1, 1, F);
		fread(&VMin, 1, 1, F);

		if(VMaj != 1 && VMin != 0)
		{
			MessageBox(NULL, "Incorrect LTZ Version!", "LT Error", MB_ICONERROR | MB_OK);
			fclose(F);
			return false;
		}

		fread(&MinX, 4, 1, F);
		fread(&MinZ, 4, 1, F);
		fread(&MaxX, 4, 1, F);
		fread(&MaxZ, 4, 1, F);
		fread(&GridX, 4, 1, F);
		fread(&GridZ, 4, 1, F);

		fread(&TypesCount, 4, 1, F);

		for(int i = 0; i < TypesCount; ++i)
		{
			int Length = 0;
			char* Buffer = 0;
			std::string Name = "UNKNOWN";

			fread(&Length, 4, 1, F);
			if(Length > 0)
			{
				Buffer = new char[Length + 1];
				Buffer[Length] = 0;

				fread(Buffer, 1, Length, F);

				Name = Buffer;

				delete[] Buffer;
			}

			TreeTypeRef* Ref = new TreeTypeRef();
			Ref->TreeType = Manager->FindTreeType(Name.c_str());
			Ref->RefCount = 0;
			TreeTypes.push_back(Ref);
		}

		fread(&SectorWidth, 4, 1, F);
		fread(&SectorHeight, 4, 1, F);

		SectorMap.clear();
		Lock();

		for(int x = 0; x < SectorWidth; ++x)
		{
			std::vector<STreeSector*> TArr;
			for(int z = 0; z < SectorHeight; ++z)
			{
				NGin::Math::Vector2 Min, Max;

				char ReadSector = 0;
				fread(&ReadSector, 1, 1, F);
				if(ReadSector == 0)
				{
					TArr.push_back(0);
					continue;
				}

				if(feof(F))
					MessageBox(NULL, "Came too soon!", "", MB_OK);

				fread(&Min, sizeof(NGin::Math::Vector2), 1, F);
				fread(&Max, sizeof(NGin::Math::Vector2), 1, F);

				STreeSector* Sector = new STreeSector(Min, Max, this);

				int TypeCount = 0;
				fread(&TypeCount, 4, 1, F);

				for(int TypeIdx = 0; TypeIdx < TypeCount; ++TypeIdx)
				{
					int InstanceIdx = 0, InstanceCount = 0;
					fread(&InstanceIdx, 4, 1, F);
					fread(&InstanceCount, 4, 1, F);

					for(int Tree = 0; Tree < InstanceCount; ++Tree)
					{
						NGin::Math::Vector3 Position;
						NGin::Math::Vector3 Scale;
						NGin::Math::Quaternion Rotation;

						fread(&Position, sizeof(NGin::Math::Vector3), 1, F);
						fread(&Scale, sizeof(NGin::Math::Vector3), 1, F);
						fread(&Rotation, sizeof(NGin::Math::Quaternion), 1, F);

						NGin::Math::Vector3 RotationDeg(Rotation.X, Rotation.Y, Rotation.Z);

						CTreeInstance* Instance = new CTreeInstance(TreeTypes[InstanceIdx]->TreeType, Sector);
						Sector->Trees[TreeTypes[InstanceIdx]->TreeType].push_back(Instance);
						Instance->SetPosition(Position);
						Instance->SetScale(Scale);
						Instance->SetRotation(RotationDeg);
						OutputDebugString(RotationDeg.ToString().c_str());
						OutputDebugString("\n");
						Instance->UpdateCollisions(true);
					}
				}

				TArr.push_back(Sector);
			}
			SectorMap.push_back(TArr);
		}

		fclose(F);

		Unlock();

		return true;
	}

	void CTreeZone::Destroy()
	{
		Manager->UnloadZone(this);
	}
	STreeSector::STreeSector( NGin::Math::Vector2 &min, NGin::Math::Vector2 &max, CTreeZone* zone ) : Min(min), Max(max), Zone(zone)
	{

	}

	void STreeSector::InstanceReBuild()
	{
		Zone->RequestReBuild(this);
	}
}
